using System.Text.Json.Serialization;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using NLog.Extensions.Logging;
using WorldRank.Application;
using WorldRank.Application.Interfaces;
using WorldRank.Application.Services;
using WorldRank.Infrastructure;
using WorldRank.Infrastructure.Caching;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(container =>
{
    container.RegisterModule(new ApplicationModule());
    container.RegisterModule(new InfrastructureModule());
});

builder.Logging.ClearProviders();
builder.Logging.AddNLog("nlog.config");

builder.Services.AddInfrastructure(useDatabase: false);

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICache, MemoryCacheStore>();

builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IWalletService, WalletService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapGet("/", () => Results.Redirect("/swagger"));
}

app.MapControllers();

app.Run();
