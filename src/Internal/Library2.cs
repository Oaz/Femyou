using System;
using System.Collections.Generic;
using System.Linq;
using Femyou.Interop;

namespace Femyou.Internal
{
  class Library2 : Library
  {
    public Library2(string path) : base(path)
    {
      fmi2Instantiate = FmuLibrary.LoadFunction<FMI2.fmi2InstantiateTYPE>(nameof(fmi2Instantiate));
      fmi2FreeInstance = FmuLibrary.LoadFunction<FMI2.fmi2FreeInstanceTYPE>(nameof(fmi2FreeInstance));
      fmi2SetupExperiment = FmuLibrary.LoadFunction<FMI2.fmi2SetupExperimentTYPE>(nameof(fmi2SetupExperiment));
      fmi2EnterInitializationMode = FmuLibrary.LoadFunction<FMI2.fmi2EnterInitializationModeTYPE>(nameof(fmi2EnterInitializationMode));
      fmi2ExitInitializationMode = FmuLibrary.LoadFunction<FMI2.fmi2ExitInitializationModeTYPE>(nameof(fmi2ExitInitializationMode));
      fmi2Terminate = FmuLibrary.LoadFunction<FMI2.fmi2TerminateTYPE>(nameof(fmi2Terminate));
      fmi2DoStep = FmuLibrary.LoadFunction<FMI2.fmi2DoStepTYPE>(nameof(fmi2DoStep));

      fmi2GetReal = FmuLibrary.LoadFunction<FMI2.fmi2GetRealTYPE>(nameof(fmi2GetReal));
      fmi2GetInteger = FmuLibrary.LoadFunction<FMI2.fmi2GetIntegerTYPE>(nameof(fmi2GetInteger));
      fmi2GetBoolean = FmuLibrary.LoadFunction<FMI2.fmi2GetBooleanTYPE>(nameof(fmi2GetBoolean));
      fmi2GetString = FmuLibrary.LoadFunction<FMI2.fmi2GetStringTYPE>(nameof(fmi2GetString));
      fmi2SetReal = FmuLibrary.LoadFunction<FMI2.fmi2SetRealTYPE>(nameof(fmi2SetReal));
      fmi2SetInteger = FmuLibrary.LoadFunction<FMI2.fmi2SetIntegerTYPE>(nameof(fmi2SetInteger));
      fmi2SetBoolean = FmuLibrary.LoadFunction<FMI2.fmi2SetBooleanTYPE>(nameof(fmi2SetBoolean));
      fmi2SetString = FmuLibrary.LoadFunction<FMI2.fmi2SetStringTYPE>(nameof(fmi2SetString));
    }

    // ReSharper disable InconsistentNaming -- must use fmi standard names to load function in library
    private readonly FMI2.fmi2InstantiateTYPE fmi2Instantiate;
    private readonly FMI2.fmi2FreeInstanceTYPE fmi2FreeInstance;
    private readonly FMI2.fmi2SetupExperimentTYPE fmi2SetupExperiment;
    private readonly FMI2.fmi2EnterInitializationModeTYPE fmi2EnterInitializationMode;
    private readonly FMI2.fmi2ExitInitializationModeTYPE fmi2ExitInitializationMode;
    private readonly FMI2.fmi2TerminateTYPE fmi2Terminate;
    private readonly FMI2.fmi2GetRealTYPE fmi2GetReal;
    private readonly FMI2.fmi2GetIntegerTYPE fmi2GetInteger;
    private readonly FMI2.fmi2GetBooleanTYPE fmi2GetBoolean;
    private readonly FMI2.fmi2GetStringTYPE fmi2GetString;
    private readonly FMI2.fmi2SetRealTYPE fmi2SetReal;
    private readonly FMI2.fmi2SetIntegerTYPE fmi2SetInteger;
    private readonly FMI2.fmi2SetBooleanTYPE fmi2SetBoolean;
    private readonly FMI2.fmi2SetStringTYPE fmi2SetString;
    private readonly FMI2.fmi2DoStepTYPE fmi2DoStep;

    public override Callbacks CreateCallbacks(Instance instance, ICallbacks cb) => 
      new Callbacks2(instance, cb);

    public override IntPtr Instantiate(string name, string guid, string tmpFolder, Callbacks callbacks) =>
      fmi2Instantiate(
        name,
        FMI2.fmi2Type.fmi2CoSimulation,
        guid,
        tmpFolder,
        callbacks.Custom,
        FMI2.fmi2Boolean.fmi2False,
        FMI2.fmi2Boolean.fmi2False
      );

    public override void Setup(IntPtr handle, double currentTime)
    {
      fmi2SetupExperiment(
        handle,
        FMI2.fmi2Boolean.fmi2False,
        0.0,
        currentTime,
        FMI2.fmi2Boolean.fmi2False,
        0.0
      );
      fmi2EnterInitializationMode(handle);
      fmi2ExitInitializationMode(handle);
    }

    public override void Step(IntPtr handle, double currentTime, double step) =>
      fmi2DoStep(
        handle,
        currentTime,
        step,
        FMI2.fmi2Boolean.fmi2True
      );

    public override void Shutdown(IntPtr handle, bool started)
    {
      if (started)
        fmi2Terminate(handle);
      fmi2FreeInstance(handle);
    }

    public override IEnumerable<double> ReadReal(IntPtr handle, IEnumerable<IVariable> variables) =>
      Read(
        handle,
        variables,
        l => new double[l],
        (a, b, c, d) => fmi2GetReal(a, b, c, d)
      );

    public override IEnumerable<int> ReadInteger(IntPtr handle, IEnumerable<IVariable> variables) =>
      Read(
        handle,
        variables,
        l => new int[l],
        (a, b, c, d) => fmi2GetInteger(a, b, c, d)
      );

    public override IEnumerable<bool> ReadBoolean(IntPtr handle, IEnumerable<IVariable> variables) =>
      Read(
        handle,
        variables,
        l => new FMI2.fmi2Boolean[l],
        (a, b, c, d) => fmi2GetBoolean(a, b, c, d)
      ).Select(r => r == FMI2.fmi2Boolean.fmi2True);

    public override IEnumerable<string> ReadString(IntPtr handle, IEnumerable<IVariable> variables) =>
      Read(
        handle,
        variables,
        Marshalling.CreateArray,
        (a, b, c, d) => fmi2GetString(a, b, c, d)
      ).Select(Marshalling.GetString);

    private T[] Read<T>(
      IntPtr handle,
      IEnumerable<IVariable> variables,
      Func<int,T[]> createArray,
      Func<IntPtr, UInt32[], ulong, T[], int> call
      )
    {
      var inputArray = variables as IVariable[] ?? variables.ToArray();
      var valueReferences = inputArray.Cast<Variable>().Select(vs => vs.ValueReference).ToArray();
      var values = createArray(inputArray.Length);
      var status = call(handle, valueReferences, (ulong)valueReferences.Length, values);
      if (status != 0)
        throw new FmuException("Failed to read");
      return values;
    }

    public override void WriteReal(IntPtr handle, IEnumerable<(IVariable, double)> variables) => Write(
      handle,
      variables,
      (a, b, c, d) => fmi2SetReal(a, b, c, d)
    );
    public override void WriteInteger(IntPtr handle, IEnumerable<(IVariable, int)> variables) => Write(
      handle,
      variables,
      (a, b, c, d) => fmi2SetInteger(a, b, c, d)
    );
    public override void WriteBoolean(IntPtr handle, IEnumerable<(IVariable, bool)> variables) => Write(
      handle,
      variables.Select(v => (v.Item1, v.Item2 ? FMI2.fmi2Boolean.fmi2True : FMI2.fmi2Boolean.fmi2False)),
      (a, b, c, d) => fmi2SetBoolean(a, b, c, d)
    );
    public override void WriteString(IntPtr handle, IEnumerable<(IVariable, string)> variables) => Write(
      handle,
      variables,
      (a, b, c, d) => fmi2SetString(a, b, c, d)
    );

    private void Write<T>(
      IntPtr handle,
      IEnumerable<(IVariable, T)> variables,
      Func<IntPtr, UInt32[], ulong, T[], int> call
      )
    {
      var valueTuples = variables as (IVariable, T)[] ?? variables.ToArray();
      var valueReferences = valueTuples
        .Select(vs => vs.Item1)
        .Cast<Variable>()
        .Select(vs => vs.ValueReference)
        .ToArray();
      var values = valueTuples.Select(vs => vs.Item2).ToArray();
      var status = call(handle, valueReferences, (ulong)valueReferences.Length, values);
      if (status != 0)
        throw new FmuException("Failed to write");
    }

  }
}