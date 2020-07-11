
using System;
using size_t = System.UInt64;

/* Type definitions of variables passed as arguments
   Version "default" means:
   fmi2Component           : an opaque object pointer
   fmi2ComponentEnvironment: an opaque object pointer
   fmi2FMUstate            : an opaque object pointer
   fmi2ValueReference      : handle to the value of a variable
   fmi2Real                : double precision floating-point data type
   fmi2Integer             : basic signed integer data type
   fmi2Boolean             : basic signed integer data type
   fmi2Char                : character data type
   fmi2String              : a pointer to a vector of fmi2Char characters
                             ('\0' terminated, UTF8 encoded)
   fmi2Byte                : smallest addressable unit of the machine, typically one byte.
*/

using fmi2Component = System.IntPtr;               /* Pointer to FMU instance       */
using fmi2ComponentEnvironment = System.IntPtr;    /* Pointer to FMU environment    */
using fmi2FMUstate = System.IntPtr;                /* Pointer to internal FMU state */
using fmi2ValueReference = System.UInt32;
using fmi2Real = System.Double;
using fmi2Integer = System.Int32;
using fmi2Boolean = System.Int32;
using fmi2String = System.String;

/* Type definitions */
using fmi2Status = System.Int32;
using fmi2StatusKind = System.Int32;

using System.Runtime.InteropServices;
using System.Linq;

namespace Femyou
{
  static class FMI2
  {
    /* Values for fmi2Boolean  */
    public enum fmi2Boolean
    {
      fmi2False = 0,
      fmi2True = 1
    }

    /* Type definitions */
    public enum fmi2Type
    {
      fmi2ModelExchange = 0,
      fmi2CoSimulation = 1
    }

    public delegate void fmi2CallbackLogger(fmi2ComponentEnvironment componentEnvironment,
                                                fmi2String instanceName,
                                                fmi2Status status,
                                                fmi2String category,
                                                fmi2String message);
    public delegate IntPtr fmi2CallbackAllocateMemory(size_t nobj, size_t size);
    public delegate void fmi2CallbackFreeMemory(IntPtr obj);
    public delegate void fmi2StepFinished(fmi2ComponentEnvironment componentEnvironment, fmi2Status status);

    public struct fmi2CallbackFunctions
    {
      public fmi2CallbackLogger logger;
      public fmi2CallbackAllocateMemory allocateMemory;
      public fmi2CallbackFreeMemory freeMemory;
      public fmi2StepFinished stepFinished;
      public fmi2ComponentEnvironment componentEnvironment;
    }

    /* Creation and destruction of FMU instances and setting debug status */
    public delegate fmi2Component fmi2InstantiateTYPE(
      fmi2String instanceName,
      fmi2Type fmuType,
      fmi2String fmuGUID,
      fmi2String fmuResourceLocation,
      IntPtr functions,
      fmi2Boolean visible,
      fmi2Boolean loggingOn
    );

    public delegate void fmi2FreeInstanceTYPE(fmi2Component c);

    /* Enter and exit initialization mode, terminate and reset */
    public delegate fmi2Status fmi2SetupExperimentTYPE(fmi2Component c,
                                                      fmi2Boolean toleranceDefined,
                                                      fmi2Real tolerance,
                                                      fmi2Real startTime,
                                                      fmi2Boolean stopTimeDefined,
                                                      fmi2Real stopTime);
    public delegate fmi2Status fmi2EnterInitializationModeTYPE(fmi2Component c);
    public delegate fmi2Status fmi2ExitInitializationModeTYPE(fmi2Component c);
    public delegate fmi2Status fmi2TerminateTYPE(fmi2Component c);
    public delegate fmi2Status fmi2ResetTYPE(fmi2Component c);

    /* Getting and setting variable values */
    public delegate fmi2Status fmi2GetRealTYPE(fmi2Component c, fmi2ValueReference[] vr, size_t nvr, fmi2Real[] value);
    public delegate fmi2Status fmi2GetIntegerTYPE(fmi2Component c, fmi2ValueReference[] vr, size_t nvr, fmi2Integer[] value);
    public delegate fmi2Status fmi2GetBooleanTYPE(fmi2Component c, fmi2ValueReference[] vr, size_t nvr, fmi2Boolean[] value);
    public delegate fmi2Status fmi2GetStringTYPE(fmi2Component c, fmi2ValueReference[] vr, size_t nvr, IntPtr[] value);


    public delegate fmi2Status fmi2SetRealTYPE(fmi2Component c, fmi2ValueReference[] vr, size_t nvr, fmi2Real[] value);
    public delegate fmi2Status fmi2SetIntegerTYPE(fmi2Component c, fmi2ValueReference[] vr, size_t nvr, fmi2Integer[] value);
    public delegate fmi2Status fmi2SetBooleanTYPE(fmi2Component c, fmi2ValueReference[] vr, size_t nvr, fmi2Boolean[] value);
    public delegate fmi2Status fmi2SetStringTYPE(fmi2Component c, fmi2ValueReference[] vr, size_t nvr, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] fmi2String[] value);


    /***************************************************
      Types for Functions for FMI2 for Co-Simulation
    ****************************************************/

    /* Simulating the slave */
    public delegate fmi2Status fmi2DoStepTYPE(fmi2Component c,
                                         fmi2Real currentCommunicationPoint,
                                         fmi2Real communicationStepSize,
                                         fmi2Boolean noSetFMUStatePriorToCurrentPoint);
  }

  static class Marshalling
  {
    public static IntPtr[] CreateArray(int size) => Enumerable.Repeat(IntPtr.Zero, size).ToArray();
    public static string GetString(IntPtr stringPtr) => Marshal.PtrToStringAnsi(stringPtr);
    public static IntPtr AllocateMemory(size_t nobj, size_t size) => Marshal.AllocHGlobal((int)(nobj * size));
    public static void FreeMemory(IntPtr obj) => Marshal.FreeHGlobal(obj);
  }
}

