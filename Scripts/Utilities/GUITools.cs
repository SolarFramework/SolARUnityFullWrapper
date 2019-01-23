using System;
using UnityEngine;

public static class GUITools
{
    public static ChangeCheckScope changeCheckScope
    {
        get
        {
            GUI.changed = false;
            return ChangeCheckScope.Instance;
        }
    }

    public class ChangeCheckScope : IDisposable
    {
        public static readonly ChangeCheckScope Instance = new ChangeCheckScope();

        private ChangeCheckScope() { }

        public bool Changed { get { return GUI.changed; } }

        public void Dispose()
        {
            GUI.changed = false;
        }
    }
}
