using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MudBlazor;
using MudBlazor.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using System.Net;
using System.Net.Http.Headers;
using ZetaCommon.Auth;
using ZetaDashboard.Common.Http.Config;
using ZetaDashboard.Common.Mongo.Config;
using ZetaDashboard.Common.Mongo.DataModels;
using ZetaDashboard.Common.Services;
using ZetaDashboard.Common.ZDB.Services;
using ZetaDashboard.Components;
using ZetaDashboard.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddHubOptions(options =>
    {
        options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
        options.KeepAliveInterval = TimeSpan.FromSeconds(15);
        options.HandshakeTimeout = TimeSpan.FromSeconds(15);
        options.MaximumReceiveMessageSize = 64 * 1024;
    });

//Auth
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddAuthenticationCore();

//mudlbazor
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;
    config.SnackbarConfiguration.HideTransitionDuration = 100; 
    config.SnackbarConfiguration.ShowTransitionDuration = 100; 
    config.SnackbarConfiguration.VisibleStateDuration = 1000;  
});

//mongo
builder.Services.Configure<MongoConfig>(builder.Configuration.GetSection("Mongo"));

builder.Services.AddSingleton(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoConfig>>().Value;
    return new MongoContext(settings);
});


builder.Services.AddScoped<BaseService>();


var configMDB = builder.Configuration.GetSection("MovieDataBaseConfig").Get<MovieDataBaseConfig>();
builder.Services.AddHttpClient<HttpService>(client =>
{
    client.BaseAddress = new Uri("https://api.themoviedb.org/3/");
    client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", configMDB.Mdb_token); // muévelo a appsettings/secret
});

var config = builder.Configuration.GetSection("Mongo").Get<MongoConfig>();

builder.Services.AddSingleton<MongoInfoService>(provider =>
{
    return new MongoInfoService(config);
});


//services
builder.Services.AddScoped<CommonServices>();
builder.Services.AddScoped<DataController>();


//SignalR
builder.Services.AddServerSideBlazor()
    .AddHubOptions(options =>
    {
        options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);     // Tiempo antes de marcar desconexión
        options.KeepAliveInterval = TimeSpan.FromSeconds(15);         // Ping del servidor al cliente
        options.MaximumReceiveMessageSize = 64 * 1024;                // 64 KB por mensaje
        options.HandshakeTimeout = TimeSpan.FromSeconds(15);          // Negociación inicial
    });

builder.Services.AddSingleton<CircuitHandler, UserCountCircuitHandler>();


builder.Services.AddHttpClient("img").ConfigureHttpClient(c => c.Timeout = TimeSpan.FromSeconds(10));
builder.Services.AddMemoryCache();



var app = builder.Build();


app.MapGet("/img", async (
    string url, int? w, int? h, int? q, string? format,
    IHttpClientFactory http, IMemoryCache cache) =>
{
    if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
        (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        return Results.BadRequest("Invalid url");

    var key = $"img:{url}:w={w}:h={h}:q={q}:f={format}";
    if (cache.TryGetValue(key, out byte[] bytes) && cache.TryGetValue(key + ":ct", out string ct))
        return Results.File(bytes, ct, enableRangeProcessing: true);

    var client = http.CreateClient("img");
    using var resp = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
    if (!resp.IsSuccessStatusCode) return Results.StatusCode((int)resp.StatusCode);

    await using var src = await resp.Content.ReadAsStreamAsync();
    using var image = await Image.LoadAsync(src);

    if (w is not null || h is not null)
    {
        var size = new SixLabors.ImageSharp.Size(w ?? 0, h ?? 0);
        image.Mutate(x => x.Resize(new SixLabors.ImageSharp.Processing.ResizeOptions
        {
            Mode = SixLabors.ImageSharp.Processing.ResizeMode.Crop, // Recorta para llenar el tamaño exacto
            Size = new SixLabors.ImageSharp.Size(w ?? 0, h ?? 0)
        }));
    }

    var quality = Math.Clamp(q ?? 70, 30, 95);
    await using var ms = new MemoryStream();
    string outCt;

    if ((format ?? "webp").ToLowerInvariant() == "webp")
    {
        await image.SaveAsync(ms, new WebpEncoder { Quality = quality });
        outCt = "image/webp";
    }
    else
    {
        await image.SaveAsync(ms, new JpegEncoder { Quality = quality });
        outCt = "image/jpeg";
    }

    bytes = ms.ToArray();
    var opts = new MemoryCacheEntryOptions
    {
        SlidingExpiration = TimeSpan.FromHours(6),
        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(2)
    };
    cache.Set(key, bytes, opts);
    cache.Set(key + ":ct", outCt, opts);

    return Results.File(bytes, outCt, enableRangeProcessing: true, lastModified: DateTimeOffset.UtcNow);
});

//Mongo check
//if (!await CheckMongoConnectionAsync(config))
//{
//    Console.WriteLine("?? La aplicación no se iniciará sin conexión a MongoDB.");
//    return; // Evita que la app arranque si no hay conexión
//}

async Task<bool> CheckMongoConnectionAsync(MongoConfig? config)
{
    try
    {
        var client = new MongoClient(config.ConnectionString);
        var database = client.GetDatabase(config.DatabaseName);

        // Comando simple para verificar conexión
        var result = await database.RunCommandAsync((Command<BsonDocument>)"{ ping: 1 }");

        Console.WriteLine("? Conexión a MongoDB exitosa.");
        return true;
    }
    catch (Exception ex)
    {
        Console.WriteLine("? Error conectando a MongoDB:");
        Console.WriteLine(ex.Message);
        return false;
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();


