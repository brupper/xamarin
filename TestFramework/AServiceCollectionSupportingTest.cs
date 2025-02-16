using System;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Brupper.TestFramework;

/// <summary> Implementation is based on https://github.com/MvvmCross/MvvmCross/tree/develop/MvvmCross.Tests </summary>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public abstract class AServiceCollectionSupportingTest
{
    private ILogger logger;

    public AServiceCollectionSupportingTest()
    {
        Setup();
    }

    public IServiceProvider ServiceProvider
        => HostEnvironment?.Services ?? throw new InvalidOperationException("Varj, amig nincs build hivva ClearAll() utolso utasitasa kent!");

    public void Setup()
    {
        ClearAll();
    }

    public void Reset()
    {
        HostEnvironment = null;
    }

    public virtual void ClearAll()
    {
        // fake set up of the IoC
        Reset();

        HostEnvironment = GetHostEnvironment();
    }

    public IHost? HostEnvironment { get; private set; }

    public IHost GetHostEnvironment()
    {
        SetCulture();

        var builder = Host.CreateDefaultBuilder()
            .ConfigureServices(ConfigureServices)
            .ConfigureLogging(ConfigureLogging)
            .ConfigureAppConfiguration(ConfigureAppConfiguration)
            ;

        HostEnvironment = builder.Build();

        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            var loggerFactory = HostEnvironment.Services.GetService<ILoggerFactory>();
            var logger = loggerFactory!.CreateLogger<Ref>();
            logger.LogError(args.ExceptionObject as Exception, "An error happened");
        };

        return HostEnvironment;
    }

    protected abstract void ConfigureAppConfiguration(HostBuilderContext context, IConfigurationBuilder builder);

    protected abstract void ConfigureLogging(HostBuilderContext context, ILoggingBuilder builder);

    protected abstract void ConfigureServices(HostBuilderContext context, IServiceCollection collection);

    public virtual void SetCulture()
    {
        var invariantCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentCulture = invariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = invariantCulture;
    }
}

internal class Ref { }