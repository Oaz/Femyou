// ReSharper disable InconsistentNaming

using System;
using System.Runtime.InteropServices;
using size_t = System.UInt64;
using fmi3Instance = System.IntPtr; /* Pointer to FMU instance       */
using fmi3InstanceEnvironment = System.IntPtr; /* Pointer to FMU environment    */
using fmi3ValueReference = System.UInt32;
using fmi3Float64 = System.Double;
using fmi3Int32 = System.Int32;
using fmi3Integer = System.Int32;
using fmi3String = System.String;
using fmi3Status = System.Int32;

namespace Femyou.Interop
{
  static class FMI3
  {
    public enum fmi3Boolean
    {
      fmi3False = 0,
      fmi3True = 1
    }
    
    public delegate void fmi3CallbackLogMessage(fmi3InstanceEnvironment componentEnvironment,
      fmi3String instanceName,
      fmi3Status status,
      fmi3String category,
      fmi3String message);

    public delegate fmi3Instance fmi3InstantiateCoSimulationTYPE(
      fmi3String instanceName,
      fmi3String instantiationToken,
      fmi3String resourceLocation,
      fmi3Boolean visible,
      fmi3Boolean loggingOn,
      fmi3Boolean eventModeUsed,
      fmi3Boolean earlyReturnAllowed,
      IntPtr requiredIntermediateVariables, /* FAKE, pointer */
      fmi3Integer nRequiredIntermediateVariables, /* FAKE, size_t */
      fmi3InstanceEnvironment instanceEnvironment,
      fmi3CallbackLogMessage logMessage,
      IntPtr intermediateUpdate);

    public delegate void fmi3FreeInstanceTYPE(fmi3Instance c);

    /* Enter and exit initialization mode, terminate and reset */

    public delegate fmi3Status fmi3EnterInitializationModeTYPE(fmi3Instance instance,
      fmi3Boolean toleranceDefined,
      fmi3Float64 tolerance,
      fmi3Float64 startTime,
      fmi3Boolean stopTimeDefined,
      fmi3Float64 stopTime);

    public delegate fmi3Status fmi3ExitInitializationModeTYPE(fmi3Instance instance);

    public delegate fmi3Status fmi3EnterStepModeTYPE(fmi3Instance instance);


    public delegate fmi3Status fmi3TerminateTYPE(fmi3Instance c);

    
    public delegate fmi3Status fmi3DoStepTYPE(fmi3Instance instance,
      fmi3Float64 currentCommunicationPoint,
      fmi3Float64 communicationStepSize,
      fmi3Boolean noSetFMUStatePriorToCurrentPoint,
      IntPtr eventHandlingNeeded,
      IntPtr terminateSimulation,
      IntPtr earlyReturn,
      IntPtr lastSuccessfulTime);

    /* Getting and setting variable values */
    public delegate fmi3Status fmi3GetFloat64TYPE(fmi3Instance c, fmi3ValueReference[] vr, size_t nvr,
      fmi3Float64[] value, size_t nv);

    public delegate fmi3Status fmi3GetInt32TYPE(fmi3Instance c, fmi3ValueReference[] vr, size_t nvr,
      fmi3Integer[] value, size_t nv);

    public delegate fmi3Status fmi3GetBooleanTYPE(fmi3Instance c, fmi3ValueReference[] vr, size_t nvr,
      fmi3Boolean[] value, size_t nv);

    public delegate fmi3Status fmi3GetStringTYPE(fmi3Instance c, fmi3ValueReference[] vr, size_t nvr, IntPtr[] value,
      size_t nv);


    public delegate fmi3Status fmi3SetFloat64TYPE(fmi3Instance c, fmi3ValueReference[] vr, size_t nvr,
      fmi3Float64[] value, size_t nv);

    public delegate fmi3Status fmi3SetInt32TYPE(fmi3Instance c, fmi3ValueReference[] vr, size_t nvr,
      fmi3Integer[] value, size_t nv);

    public delegate fmi3Status fmi3SetBooleanTYPE(fmi3Instance c, fmi3ValueReference[] vr, size_t nvr,
      fmi3Boolean[] value, size_t nv);

    public delegate fmi3Status fmi3SetStringTYPE(fmi3Instance c, fmi3ValueReference[] vr, size_t nvr,
      [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] fmi3String[] value, size_t nv);
  }
}