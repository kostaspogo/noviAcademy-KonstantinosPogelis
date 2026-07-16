using System.Text.Json.Serialization;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using NLog.Extensions.Logging;
using Quartz;
using WorldRank.API.Jobs;
using WorldRank.Application;
using WorldRank.Application.Interfaces;
using WorldRank.Application.Services;
using WorldRank.Gateway;
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

builder.Services.AddInfrastructure(useDatabase: true);

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICache, MemoryCacheStore>();

builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IWalletService, WalletService>();

// Gateway: typed EcbHttpClient για τις ισοτιμίες της ΕΚΤ.
builder.Services.AddEcbGateway();

// Quartz: περιοδικό job που τραβάει τις ισοτιμίες και τις αποθηκεύει.
builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey(nameof(EcbRatesSyncJob));
    q.AddJob<EcbRatesSyncJob>(opts => opts.WithIdentity(jobKey));
    q.AddTrigger(t => t
        .ForJob(jobKey)
        .WithIdentity($"{nameof(EcbRatesSyncJob)}-trigger")
        .StartNow()
        .WithSimpleSchedule(s => s.WithIntervalInHours(1).RepeatForever()));
});
builder.Services.AddQuartzHostedService(opts => opts.WaitForJobsToComplete = true);

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
