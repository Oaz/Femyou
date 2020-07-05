using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;

namespace Femyou
{
  class Callbacks : IDisposable
  {
    public Callbacks()
    {
      var functions = new FMI2.fmi2CallbackFunctions
      {
        logger = Marshalling.Logger,
        allocateMemory = Marshalling.AllocateMemory,
        freeMemory = Marshalling.FreeMemory,
        stepFinished = Marshalling.StepFinished,
        componentEnvironment = IntPtr.Zero
      };
      Structure = Marshalling.AllocateMemory(1, (ulong)Marshal.SizeOf(functions));
    }
    public readonly IntPtr Structure;

    public void Dispose()
    {
      Marshalling.FreeMemory(Structure);
    }
  }
}