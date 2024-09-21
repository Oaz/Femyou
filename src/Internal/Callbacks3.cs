using System;
using System.Runtime.InteropServices;
using Femyou.Interop;

namespace Femyou.Internal
{
  class Callbacks3 : Callbacks
  {
    public Callbacks3(Instance instance, ICallbacks cb)
      : base(instance, cb)
    {
      _handle = GCHandle.Alloc(this);
      Custom = GCHandle.ToIntPtr(_handle);
      LogMessageDelegate = LoggerCallback;
    }
    
    private GCHandle _handle;
    public readonly FMI3.fmi3CallbackLogMessage LogMessageDelegate;


    private static void LoggerCallback(
      IntPtr componentEnvironment, string instanceName, 
      int status, string category, string message)
    {
      var self = (Callbacks) GCHandle.FromIntPtr(componentEnvironment).Target;
      self.Cb?.Logger(self.Instance, (Status)status, category, message);
    }

    public override IntPtr Custom { get; }

    public override void Dispose()
    {
      _handle.Free();
    }
  }
}