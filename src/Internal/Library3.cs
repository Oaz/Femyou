using System;
using System.Collections.Generic;
using System.Linq;
using Femyou.Interop;

namespace Femyou.Internal
{
  class Library3 : Library
  {
    public Library3(string path) : base(path)
    {
      fmi3InstantiateCoSimulation = FmuLibrary.LoadFunction<FMI3.fmi3InstantiateCoSimulationTYPE>(nameof(fmi3InstantiateCoSimulation));
      fmi3FreeInstance = FmuLibrary.LoadFunction<FMI3.fmi3FreeInstanceTYPE>(nameof(fmi3FreeInstance));
      fmi3EnterInitializationMode = FmuLibrary.LoadFunction<FMI3.fmi3EnterInitializationModeTYPE>(nameof(fmi3EnterInitializationMode));
      fmi3ExitInitializationMode = FmuLibrary.LoadFunction<FMI3.fmi3ExitInitializationModeTYPE>(nameof(fmi3ExitInitializationMode));
      fmi3EnterStepMode = FmuLibrary.LoadFunction<FMI3.fmi3EnterStepModeTYPE>(nameof(fmi3EnterStepMode));
      fmi3Terminate = FmuLibrary.LoadFunction<FMI3.fmi3TerminateTYPE>(nameof(fmi3Terminate));
      fmi3DoStep = FmuLibrary.LoadFunction<FMI3.fmi3DoStepTYPE>(nameof(fmi3DoStep));

      fmi3GetFloat64 = FmuLibrary.LoadFunction<FMI3.fmi3GetFloat64TYPE>(nameof(fmi3GetFloat64));
      fmi3GetInt32 = FmuLibrary.LoadFunction<FMI3.fmi3GetInt32TYPE>(nameof(fmi3GetInt32));
      fmi3GetBoolean = FmuLibrary.LoadFunction<FMI3.fmi3GetBooleanTYPE>(nameof(fmi3GetBoolean));
      fmi3GetString = FmuLibrary.LoadFunction<FMI3.fmi3GetStringTYPE>(nameof(fmi3GetString));
      fmi3SetFloat64 = FmuLibrary.LoadFunction<FMI3.fmi3SetFloat64TYPE>(nameof(fmi3SetFloat64));
      fmi3SetInt32 = FmuLibrary.LoadFunction<FMI3.fmi3SetInt32TYPE>(nameof(fmi3SetInt32));
      fmi3SetBoolean = FmuLibrary.LoadFunction<FMI3.fmi3SetBooleanTYPE>(nameof(fmi3SetBoolean));
      fmi3SetString = FmuLibrary.LoadFunction<FMI3.fmi3SetStringTYPE>(nameof(fmi3SetString));
    }

    // ReSharper disable InconsistentNaming -- must use fmi standard names to load function in library
    private readonly FMI3.fmi3InstantiateCoSimulationTYPE fmi3InstantiateCoSimulation;
    private readonly FMI3.fmi3FreeInstanceTYPE fmi3FreeInstance;
    private readonly FMI3.fmi3EnterInitializationModeTYPE fmi3EnterInitializationMode;
    private readonly FMI3.fmi3ExitInitializationModeTYPE fmi3ExitInitializationMode;
    private readonly FMI3.fmi3EnterStepModeTYPE fmi3EnterStepMode;
    private readonly FMI3.fmi3TerminateTYPE fmi3Terminate;
    private readonly FMI3.fmi3DoStepTYPE fmi3DoStep;

    private readonly FMI3.fmi3GetFloat64TYPE fmi3GetFloat64;
    private readonly FMI3.fmi3GetInt32TYPE fmi3GetInt32;
    private readonly FMI3.fmi3GetBooleanTYPE fmi3GetBoolean;
    private readonly FMI3.fmi3GetStringTYPE fmi3GetString;
    private readonly FMI3.fmi3SetFloat64TYPE fmi3SetFloat64;
    private readonly FMI3.fmi3SetInt32TYPE fmi3SetInt32;
    private readonly FMI3.fmi3SetBooleanTYPE fmi3SetBoolean;
    private readonly FMI3.fmi3SetStringTYPE fmi3SetString;
    
    public override Callbacks CreateCallbacks(Instance instance, ICallbacks cb) => 
      new Callbacks3(instance, cb);
    
    public override IntPtr Instantiate(string name, string guid, string tmpFolder, Callbacks callbacks)
    {
      return fmi3InstantiateCoSimulation(
        name,
        guid,
        tmpFolder,
        FMI3.fmi3Boolean.fmi3False,
        FMI3.fmi3Boolean.fmi3True,
        FMI3.fmi3Boolean.fmi3False,
        FMI3.fmi3Boolean.fmi3False,
        IntPtr.Zero,
        0,
        callbacks.Custom,
        ((Callbacks3)callbacks).LogMessageDelegate,
        IntPtr.Zero
      );
    }

    public override void Setup(IntPtr handle, double currentTime)
    {
      fmi3EnterInitializationMode(
        handle,
        FMI3.fmi3Boolean.fmi3False,
        0.0,
        currentTime,
        FMI3.fmi3Boolean.fmi3False,
        0.0
      );
      fmi3ExitInitializationMode(handle);
    }

    public override void Step(IntPtr handle, double currentTime, double step)
    {
      var buf = Marshalling.AllocateMemory(4, sizeof(bool));
      var res = fmi3DoStep(
        handle,
        currentTime,
        step,
        FMI3.fmi3Boolean.fmi3True,
        buf,
        buf+sizeof(bool),
        buf+2*sizeof(bool),
        buf+3*sizeof(bool)
      );
      if (res != 0) { throw new FmuException("Status not OK: " + res); }
    }

    public override void Shutdown(IntPtr handle, bool started)
    {
      if (started)
        fmi3Terminate(handle);
      fmi3FreeInstance(handle);
    }

    public override IEnumerable<double> ReadReal(IntPtr handle, IEnumerable<IVariable> variables) =>
      Read(
        handle,
        variables,
        l => new double[l],
        (a, b, c, d, e) => fmi3GetFloat64(a, b, c, d, e)
      );

    public override IEnumerable<int> ReadInteger(IntPtr handle, IEnumerable<IVariable> variables) =>
      Read(
        handle,
        variables,
        l => new int[l],
        (a, b, c, d, e) => fmi3GetInt32(a, b, c, d, e)
      );

    public override IEnumerable<bool> ReadBoolean(IntPtr handle, IEnumerable<IVariable> variables) =>
      Read(
        handle,
        variables,
        l => new FMI3.fmi3Boolean[l],
        (a, b, c, d, e) => fmi3GetBoolean(a, b, c, d, e)
      ).Select(r => r == FMI3.fmi3Boolean.fmi3True);

    public override IEnumerable<string> ReadString(IntPtr handle, IEnumerable<IVariable> variables) =>
      Read(
        handle,
        variables,
        Marshalling.CreateArray,
        (a, b, c, d, e) => fmi3GetString(a, b, c, d, e)
      ).Select(Marshalling.GetString);

    private T[] Read<T>(
      IntPtr handle,
      IEnumerable<IVariable> variables,
      Func<int,T[]> createArray,
      Func<IntPtr, UInt32[], ulong, T[], ulong, int> call
      )
    {
      var inputArray = variables as IVariable[] ?? variables.ToArray();
      var valueReferences = inputArray.Cast<Variable>().Select(vs => vs.ValueReference).ToArray();
      var values = createArray(inputArray.Length);
      var status = call(handle, valueReferences, (ulong)valueReferences.Length, values, (ulong)values.Length);
      if (status != 0)
        throw new FmuException("Failed to read");
      return values;
    }

    public override void WriteReal(IntPtr handle, IEnumerable<(IVariable, double)> variables) => Write(
      handle,
      variables,
      (a, b, c, d, e) => fmi3SetFloat64(a, b, c, d, e)
    );
    public override void WriteInteger(IntPtr handle, IEnumerable<(IVariable, int)> variables) => Write(
      handle,
      variables,
      (a, b, c, d, e) => fmi3SetInt32(a, b, c, d, e)
    );
    public override void WriteBoolean(IntPtr handle, IEnumerable<(IVariable, bool)> variables) => Write(
      handle,
      variables.Select(v => (v.Item1, v.Item2 ? FMI3.fmi3Boolean.fmi3True : FMI3.fmi3Boolean.fmi3False)),
      (a, b, c, d, e) => fmi3SetBoolean(a, b, c, d, e)
    );
    public override void WriteString(IntPtr handle, IEnumerable<(IVariable, string)> variables) => Write(
      handle,
      variables,
      (a, b, c, d, e) => fmi3SetString(a, b, c, d, e)
    );

    private void Write<T>(
      IntPtr handle,
      IEnumerable<(IVariable, T)> variables,
      Func<IntPtr, UInt32[], ulong, T[], ulong, int> call
      )
    {
      var valueTuples = variables as (IVariable, T)[] ?? variables.ToArray();
      var valueReferences = valueTuples
        .Select(vs => vs.Item1)
        .Cast<Variable>()
        .Select(vs => vs.ValueReference)
        .ToArray();
      var values = valueTuples.Select(vs => vs.Item2).ToArray();
      var status = call(handle, valueReferences, (ulong)valueReferences.Length, values, (ulong)values.Length);
      if (status != 0)
        throw new FmuException("Failed to write");
    }
  }
}
