using System;
using System.Collections.Generic;
using SolAR.Core;
using UnityEngine.Assertions;
using XPCF.Api;

public abstract class AbstractPipeline : IPipeline
{
    protected readonly IComponentManager xpcfComponentManager;

    protected readonly IList<IDisposable> subscriptions = new List<IDisposable>();

    protected FrameworkReturnCode ok { set { Assert.AreEqual(FrameworkReturnCode._SUCCESS, value); } }

    protected AbstractPipeline(IComponentManager xpcfComponentManager)
    {
        this.xpcfComponentManager = xpcfComponentManager;
    }

    public void Dispose()
    {
        foreach (var d in subscriptions) d.Dispose();
        subscriptions.Clear();
    }
}
