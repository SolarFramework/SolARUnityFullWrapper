using System;
using UnityEngine;

public partial class RuntimeEditor
{
    public static class GUITools
    {
        public static IDisposable ChangeCheckScope
        {
            get
            {
                GUI.changed = false;
                return changeCheckScope;
            }
        }
        static IDisposable changeCheckScope = Disposable.Create(() =>
        {
            GUI.changed = false;
        });
    }
}
