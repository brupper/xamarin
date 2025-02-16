using NUnit.Framework;
using System;

namespace Brupper.TestFramework;

/// <summary> Implementation is based on https://github.com/MvvmCross/MvvmCross/tree/develop/MvvmCross.Tests </summary>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public abstract class ABaseTestFixtureGeneric<TSubject, TTestEnvironment> : AServiceCollectionSupportingTest, IDisposable
       where TTestEnvironment : ATestEnvironmentGeneric<TSubject>
{
    [OneTimeSetUp]
    public void TestFixtureSetUp()
    {
        Setup();
    }

    [SetUp]
    public void TestSetup()
    {
        ClearAll();
    }

    public virtual TTestEnvironment GetEnvironment()
    {
        var env = Activator.CreateInstance<TTestEnvironment>();

        env.SetupEnvironment(HostEnvironment);

        return env;
    }

    #region IDisposable

    ~ABaseTestFixtureGeneric()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Reset();
        }
    }

    #endregion
}
