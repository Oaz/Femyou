using System;
using System.Collections.Generic;
using System.Linq;

namespace Femyou
{
  class Instance : IInstance
  {
    public Instance(string name, ModelImpl model, Library library, FMI2.fmi2Type instanceType, ICallbacks cb)
    {
      Name = name;
      this.library = library;
      callbacks = new Callbacks(this,cb);
      handle = library.fmi2Instantiate(
        name,
        instanceType,
        model.GUID,
        model.TmpFolder,
        callbacks.Structure,
        FMI2.fmi2Boolean.fmi2False,
        FMI2.fmi2Boolean.fmi2False
      );
      if (handle == IntPtr.Zero)
        throw new FmuException("Cannot instanciate model");
    }
    public string Name { get; }
    public double CurrentTime { get; private set; }

    public void StartTime(double time)
    {
      CurrentTime = time;
      library.fmi2SetupExperiment(handle, FMI2.fmi2Boolean.fmi2False, 0.0, CurrentTime, FMI2.fmi2Boolean.fmi2False, 0.0);
      library.fmi2EnterInitializationMode(handle);
      library.fmi2ExitInitializationMode(handle);
      started = true;
    }
    public void AdvanceTime(double step)
    {
      if(step == 0.0)
        return;
      library.fmi2DoStep(handle,CurrentTime,step,FMI2.fmi2Boolean.fmi2True);
      CurrentTime += step;
    }
    public IEnumerable<double> ReadReal(IEnumerable<IVariable> variables) => Read(
      variables,
      new double[variables.Count()],
      (a, b, c, d) => library.fmi2GetReal(a, b, c, d)
    );
    public IEnumerable<int> ReadInteger(IEnumerable<IVariable> variables) => Read(
      variables,
      new int[variables.Count()],
      (a, b, c, d) => library.fmi2GetInteger(a, b, c, d)
    );
    public IEnumerable<bool> ReadBoolean(IEnumerable<IVariable> variables) => Read(
      variables,
      new FMI2.fmi2Boolean[variables.Count()],
      (a, b, c, d) => library.fmi2GetBoolean(a, b, c, d)
    ).Select(r => r == FMI2.fmi2Boolean.fmi2True);
    public IEnumerable<string> ReadString(IEnumerable<IVariable> variables) => Read(
      variables,
      Marshalling.CreateArray(variables.Count()),
      (a, b, c, d) => library.fmi2GetString(a, b, c, d)
    ).Select(r => Marshalling.GetString(r));

    private T[] Read<T>(IEnumerable<IVariable> variables, T[] values, Func<IntPtr, UInt32[], ulong, T[], int> call)
    {
      var valueReferences = variables.Cast<Variable>().Select(variables => variables.ValueReference).ToArray();
      var status = call(handle, valueReferences, (ulong)valueReferences.Length, values);
      if (status != 0)
        throw new FmuException("Failed to read");
      return values;
    }

    public void WriteReal(IEnumerable<(IVariable, double)> variables) => Write(
      variables,
      (a, b, c, d) => library.fmi2SetReal(a, b, c, d)
    );
    public void WriteInteger(IEnumerable<(IVariable, int)> variables) => Write(
      variables,
      (a, b, c, d) => library.fmi2SetInteger(a, b, c, d)
    );
    public void WriteBoolean(IEnumerable<(IVariable, bool)> variables) => Write(
      variables.Select(v => (v.Item1, v.Item2 ? FMI2.fmi2Boolean.fmi2True : FMI2.fmi2Boolean.fmi2False)),
      (a, b, c, d) => library.fmi2SetBoolean(a, b, c, d)
    );
    public void WriteString(IEnumerable<(IVariable, string)> variables) => Write(
      variables,
      (a, b, c, d) => library.fmi2SetString(a, b, c, d)
    );

    private void Write<T>(IEnumerable<(IVariable, T)> variables, Func<IntPtr, UInt32[], ulong, T[], int> call)
    {
      var valueReferences = variables.Select(variables => variables.Item1).Cast<Variable>().Select(variables => variables.ValueReference).ToArray();
      var values = variables.Select(variables => variables.Item2).ToArray();
      var status = call(handle, valueReferences, (ulong)valueReferences.Length, values);
      if (status != 0)
        throw new FmuException("Failed to write");
    }

    private readonly Library library;
    private readonly IntPtr handle;
    private readonly Callbacks callbacks;
    private bool started;

    public void Dispose()
    {
      if(started)
        library.fmi2Terminate(handle);
      library.fmi2FreeInstance(handle);
      callbacks.Dispose();
    }
  }
}