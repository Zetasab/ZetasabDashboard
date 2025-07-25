using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MudBlazor.Services;
using ZetaCommon.Auth;
using ZetaDashboard.Common.Mongo.Config;
using ZetaDashboard.Common.Mongo.DataModels;
using ZetaDashboard.Common.ZDB.Services;
using ZetaDashboard.Components;
using ZetaDashboard.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

//Auth
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddAuthenticationCore();

//mudlbazor
builder.Services.AddMudServices();

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

var app = builder.Build();


//Mongo check
if (!await CheckMongoConnectionAsync(config))
{
    Console.WriteLine("?? La aplicación no se iniciará sin conexión a MongoDB.");
    return; // Evita que la app arranque si no hay conexión
}

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


