#pragma warning disable IDE1006 // Styles d'affectation de noms
namespace SolAR
{
    public partial class IComponentIntrospect
    {
        [global::System.Runtime.InteropServices.DllImport("SolARWrapper", EntryPoint = "CSharp_SolAR_IComponentIntrospect_queryInterfaceIComponent1")]
        public static extern global::System.IntPtr IComponentIntrospect_queryInterfaceIComponent1(global::System.Runtime.InteropServices.HandleRef jarg1, global::System.Runtime.InteropServices.HandleRef jarg2);

        public IComponentIntrospect queryInterfaceIComponent1(SWIGTYPE_p_org__bcom__xpcf__uuids__uuid interfaceUUID)
        {
            global::System.IntPtr cPtr = IComponentIntrospect_queryInterfaceIComponent1(swigCPtr, SWIGTYPE_p_org__bcom__xpcf__uuids__uuid.getCPtr(interfaceUUID));
            IComponentIntrospect ret = (cPtr == global::System.IntPtr.Zero) ? null : new IComponentIntrospect(cPtr, true);
            if (SolARWrapperPINVOKE.SWIGPendingException.Pending) throw SolARWrapperPINVOKE.SWIGPendingException.Retrieve();
            return ret;
        }
    }
}
#pragma warning restore IDE1006 // Styles d'affectation de noms
