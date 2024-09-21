using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Femyou
{
  static class Marshalling
  {
    public static IntPtr[] CreateArray(int size) => Enumerable.Repeat(IntPtr.Zero, size).ToArray();
    public static string GetString(IntPtr stringPtr) => Marshal.PtrToStringAnsi(stringPtr);
    public static IntPtr AllocateMemory(UInt64 nobj, UInt64 size) => Marshal.AllocHGlobal((int)(nobj * size));
    public static void FreeMemory(IntPtr obj) => Marshal.FreeHGlobal(obj);
  }
}