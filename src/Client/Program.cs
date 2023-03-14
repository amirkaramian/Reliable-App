using FluentValidation.AspNetCore;
using MyReliableSite.Application.DependencyInjection;
using MyReliableSite.Client.API.Extensions;
using MyReliableSite.Infrastructure.DependencyInjection;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
Log.Information("Server Booting Up...");
try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.AddConfigurations();
    builder.Host.UseSerilog((_, config) => config.WriteTo.Console().ReadFrom.Configuration(builder.Configuration));

    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration, "Client");
    builder.Services.AddControllers().AddFluentValidation();

    var app = builder.Build();

    app.UseInfrastructure(builder.Configuration, "Client");

    app.Run();
}
catch (Exception ex) when (!ex.GetType().Name.Equals("StopTheHostException", StringComparison.Ordinal))
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Server Shutting down...");
    Log.CloseAndFlush();
}