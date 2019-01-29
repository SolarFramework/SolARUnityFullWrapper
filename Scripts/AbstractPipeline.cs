using System;
using System.Collections.Generic;
using SolAR.Core;
using SolAR.Datastructure;
using UnityEngine;
using XPCF.Api;

public abstract class AbstractPipeline : IPipeline
{
    protected readonly IComponentManager xpcfComponentManager;

    protected readonly IList<IDisposable> subscriptions = new List<IDisposable>();

    protected AbstractPipeline(IComponentManager xpcfComponentManager)
    {
        this.xpcfComponentManager = xpcfComponentManager;
    }

    public void Dispose()
    {
        foreach (var d in subscriptions) d.Dispose();
        subscriptions.Clear();
    }

    protected void LOG_ERROR(string message, params object[] objects) { Debug.LogErrorFormat(message, objects); }
    protected void LOG_INFO(string message, params object[] objects) { Debug.LogWarningFormat(message, objects); }
    protected void LOG_DEBUG(string message, params object[] objects) { Debug.LogFormat(message, objects); }

    public abstract Sizef GetMarkerSize();
    public abstract void SetCameraParameters(Matrix3x3f intrinsic, Vector5Df distorsion);
    public abstract FrameworkReturnCode Proceed(Image inputImage, Transform3Df pose);
}
