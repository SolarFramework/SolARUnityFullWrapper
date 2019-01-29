using System;
using SolAR.Core;
using SolAR.Datastructure;

public interface IPipeline : IDisposable
{
    Sizef GetMarkerSize();
    void SetCameraParameters(Matrix3x3f intrinsic, Vector5Df distorsion);
    FrameworkReturnCode Proceed(Image inputImage, Transform3Df pose);
}
