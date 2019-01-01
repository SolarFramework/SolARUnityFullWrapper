using System;

public partial class RuntimeEditor
{
    public static class Disposable
    {
        class DisposableAction : IDisposable
        {
            readonly Action action;
            public DisposableAction(Action action) { this.action = action; }
            public void Dispose() { action(); }
        }

        public static IDisposable Create(Action action) { return new DisposableAction(action); }
    }
}
