using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;

namespace Brupper.TestFramework;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public abstract class ATestEnvironmentGeneric<TSubject>
{
    private TSubject? subject;

    public TSubject Subject
    {
        get
        {
            if (subject == null)
            {
                subject = CreateSubjectInstance();
            }

            return subject;
        }
    }

    public IHost? HostEnvironment { get; protected set; }

    protected IServiceProvider ServiceProvider => HostEnvironment?.Services ?? throw new ArgumentNullException(nameof(HostEnvironment));

    public IHost SetupEnvironment()
    {
        SetCulture();
        HostEnvironment = CreateHostEnvironment();

        OnFinalizeEnvironment(HostEnvironment.Services);

        return HostEnvironment;
    }

    protected abstract void OnFinalizeEnvironment(IServiceProvider serviceProvider);

    protected virtual TSubject CreateSubjectInstance() => Activator.CreateInstance<TSubject>();

    public IHost CreateHostEnvironment()
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

    protected abstract void ConfigureServices(HostBuilderContext context, IServiceCollection services);

    public virtual void SetCulture()
    {
        var invariantCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentCulture = invariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = invariantCulture;
    }
}
