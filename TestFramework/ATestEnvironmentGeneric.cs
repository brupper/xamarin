using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

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

    protected IServiceProvider ServiceProvider { get; private set; }

    /*
    protected abstract void SetupEnvironment(IHost? hostEnvironment);
    public void SetupEnvironment(IServiceCollection serviceCollection, Func<IServiceProvider> createServiceProvider)
    {
        if (createServiceProvider == null)
        {
            throw new ArgumentNullException(nameof(createServiceProvider));
        }

        OnSetupEnvironment(serviceCollection);

        ServiceProvider = createServiceProvider.Invoke();
        OnFinalizeEnvironment(ServiceProvider);
    }
    // */

    protected virtual TSubject CreateSubjectInstance() => Activator.CreateInstance<TSubject>();

    //protected abstract void OnSetupEnvironment(IServiceCollection serviceCollection);

    protected abstract void OnFinalizeEnvironment(IServiceProvider serviceProvider);

    
}
