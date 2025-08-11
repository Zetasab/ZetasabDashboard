using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MudBlazor;
using MudBlazor.Services;
using ZetaCommon.Auth;
using ZetaDashboard.Common.Mongo.Config;
using ZetaDashboard.Common.Mongo.DataModels;
using ZetaDashboard.Common.ZDB.Models;
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
        options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);     // Tiempo antes de marcar desconexi�n
        options.KeepAliveInterval = TimeSpan.FromSeconds(15);         // Ping del servidor al cliente
        options.MaximumReceiveMessageSize = 64 * 1024;                // 64 KB por mensaje
        options.HandshakeTimeout = TimeSpan.FromSeconds(15);          // Negociaci�n inicial
    });

builder.Services.AddSingleton<CircuitHandler, UserCountCircuitHandler>();

var app = builder.Build();


//Mongo check
//if (!await CheckMongoConnectionAsync(config))
//{
//    Console.WriteLine("?? La aplicaci�n no se iniciar� sin conexi�n a MongoDB.");
//    return; // Evita que la app arranque si no hay conexi�n
//}

async Task<bool> CheckMongoConnectionAsync(MongoConfig? config)
{
    try
    {
        var client = new MongoClient(config.ConnectionString);
        var database = client.GetDatabase(config.DatabaseName);

        // Comando simple para verificar conexi�n
        var result = await database.RunCommandAsync((Command<BsonDocument>)"{ ping: 1 }");

        Console.WriteLine("? Conexi�n a MongoDB exitosa.");
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


