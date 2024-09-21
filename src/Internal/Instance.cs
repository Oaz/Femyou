using System;
using System.Collections.Generic;
using System.Linq;

namespace Femyou.Internal
{
  class Instance : IInstance
  {
    public Instance(string name, ModelImpl model, Library library, FMI2.fmi2Type instanceType, ICallbacks cb)
    {
      Name = name;
      this._library = library;
      _callbacks = new Callbacks(this,cb);
      _handle = library.fmi2Instantiate(
        name,
        instanceType,
        model.Guid,
        model.TmpFolder,
        _callbacks.Structure,
        FMI2.fmi2Boolean.fmi2False,
        FMI2.fmi2Boolean.fmi2False
      );
      if (_handle == IntPtr.Zero)
        throw new FmuException("Cannot instanciate model");
    }
    public string Name { get; }
    public double CurrentTime { get; private set; }

    public void StartTime(double time)
    {
      CurrentTime = time;
      _library.fmi2SetupExperiment(
        _handle,
        FMI2.fmi2Boolean.fmi2False,
        0.0,
        CurrentTime,
        FMI2.fmi2Boolean.fmi2False,
        0.0
        );
      _library.fmi2EnterInitializationMode(_handle);
      _library.fmi2ExitInitializationMode(_handle);
      _started = true;
    }
    public void AdvanceTime(double step)
    {
      if(step == 0.0)
        return;
      _library.fmi2DoStep(
        _handle,
        CurrentTime,
        step,
        FMI2.fmi2Boolean.fmi2True
        );
      CurrentTime += step;
    }
    public IEnumerable<double> ReadReal(IEnumerable<IVariable> variables) =>
      Read(
        variables,
        l => new double[l],
        (a, b, c, d) => _library.fmi2GetReal(a, b, c, d)
      );

    public IEnumerable<int> ReadInteger(IEnumerable<IVariable> variables) =>
      Read(
        variables,
        l => new int[l],
        (a, b, c, d) => _library.fmi2GetInteger(a, b, c, d)
      );

    public IEnumerable<bool> ReadBoolean(IEnumerable<IVariable> variables) =>
      Read(
        variables,
        l => new FMI2.fmi2Boolean[l],
        (a, b, c, d) => _library.fmi2GetBoolean(a, b, c, d)
      ).Select(r => r == FMI2.fmi2Boolean.fmi2True);

    public IEnumerable<string> ReadString(IEnumerable<IVariable> variables) =>
      Read(
        variables,
        Marshalling.CreateArray,
        (a, b, c, d) => _library.fmi2GetString(a, b, c, d)
      ).Select(Marshalling.GetString);

    private T[] Read<T>(
      IEnumerable<IVariable> variables,
      Func<int,T[]> createArray,
      Func<IntPtr, UInt32[], ulong, T[], int> call
      )
    {
      var inputArray = variables as IVariable[] ?? variables.ToArray();
      var valueReferences = inputArray.Cast<Variable>().Select(vs => vs.ValueReference).ToArray();
      var values = createArray(inputArray.Length);
      var status = call(_handle, valueReferences, (ulong)valueReferences.Length, values);
      if (status != 0)
        throw new FmuException("Failed to read");
      return values;
    }

    public void WriteReal(IEnumerable<(IVariable, double)> variables) => Write(
      variables,
      (a, b, c, d) => _library.fmi2SetReal(a, b, c, d)
    );
    public void WriteInteger(IEnumerable<(IVariable, int)> variables) => Write(
      variables,
      (a, b, c, d) => _library.fmi2SetInteger(a, b, c, d)
    );
    public void WriteBoolean(IEnumerable<(IVariable, bool)> variables) => Write(
      variables.Select(v => (v.Item1, v.Item2 ? FMI2.fmi2Boolean.fmi2True : FMI2.fmi2Boolean.fmi2False)),
      (a, b, c, d) => _library.fmi2SetBoolean(a, b, c, d)
    );
    public void WriteString(IEnumerable<(IVariable, string)> variables) => Write(
      variables,
      (a, b, c, d) => _library.fmi2SetString(a, b, c, d)
    );

    private void Write<T>(IEnumerable<(IVariable, T)> variables, Func<IntPtr, UInt32[], ulong, T[], int> call)
    {
      var valueTuples = variables as (IVariable, T)[] ?? variables.ToArray();
      var valueReferences = valueTuples
        .Select(vs => vs.Item1)
        .Cast<Variable>()
        .Select(vs => vs.ValueReference)
        .ToArray();
      var values = valueTuples.Select(vs => vs.Item2).ToArray();
      var status = call(_handle, valueReferences, (ulong)valueReferences.Length, values);
      if (status != 0)
        throw new FmuException("Failed to write");
    }

    private readonly Library _library;
    private readonly IntPtr _handle;
    private readonly Callbacks _callbacks;
    private bool _started;

    public void Dispose()
    {
      if(_started)
        _library.fmi2Terminate(_handle);
      _library.fmi2FreeInstance(_handle);
      _callbacks.Dispose();
    }
  }
}