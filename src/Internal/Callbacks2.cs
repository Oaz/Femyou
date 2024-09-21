using System;
using System.Runtime.InteropServices;
using Femyou.Interop;

namespace Femyou.Internal
{
  class Callbacks2 : Callbacks
  {
    public Callbacks2(Instance instance, ICallbacks cb)
      : base(instance, cb)
    {
      _handle = GCHandle.Alloc(this);
      _functions = new FMI2.fmi2CallbackFunctions
      {
        logger = LoggerCallback,
        allocateMemory = Marshalling.AllocateMemory,
        freeMemory = Marshalling.FreeMemory,
        stepFinished = StepFinishedCallback,
        componentEnvironment = GCHandle.ToIntPtr(_handle)
      };
      _structure = Marshalling.AllocateMemory(1, (ulong)Marshal.SizeOf(_functions));
      Marshal.StructureToPtr(_functions, _structure, false);
    }

    private readonly IntPtr _structure;
    private GCHandle _handle;
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly FMI2.fmi2CallbackFunctions _functions;

    public override IntPtr Custom => _structure;

    public override void Dispose()
    {
      Marshalling.FreeMemory(_structure);
      _handle.Free();
    }

    private static void LoggerCallback(IntPtr componentEnvironment, string instanceName, int status, string category, string message)
    {
      var self = (Callbacks) GCHandle.FromIntPtr(componentEnvironment).Target;
      self.Cb?.Logger(self.Instance, (Status)status, category, message);
    }

    private static void StepFinishedCallback(IntPtr componentEnvironment, int status)
    {
    }
  }
  
}