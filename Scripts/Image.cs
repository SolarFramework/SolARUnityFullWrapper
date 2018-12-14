#pragma warning disable IDE1006 // Styles d'affectation de noms
namespace SolAR
{
    public partial class Image
    {
        [global::System.Runtime.InteropServices.DllImport("SolARWrapper", EntryPoint = "CSharp_SolAR_new_Image__SWIG_120")]
        public static extern global::System.IntPtr new_Image__SWIG_120();

        public Image() : this(new_Image__SWIG_120(), true) { }
    }
}
#pragma warning restore IDE1006 // Styles d'affectation de noms
