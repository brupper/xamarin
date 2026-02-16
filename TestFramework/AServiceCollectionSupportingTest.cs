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
    public AServiceCollectionSupportingTest()
    {
        Setup();
    }

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

        // HostEnvironment = CreateHostEnvironment();
    }

    public IHost? HostEnvironment { get; protected set; }
}

internal class Ref { }