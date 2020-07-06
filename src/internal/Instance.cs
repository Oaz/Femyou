using System;
using System.Collections.Generic;
using System.Linq;

namespace Femyou
{
  class Instance : IInstance
  {
    public Instance(string name, ModelImpl model, Library library, FMI2.fmi2Type instanceType)
    {
      Name = name;
      this.library = library;
      callbacks = new Callbacks();
      handle = library.fmi2Instantiate(
        name,
        instanceType,
        model.GUID,
        model.TmpFolder,
        callbacks.Structure,
        FMI2.fmi2Boolean.fmi2False,
        FMI2.fmi2Boolean.fmi2False
      );
      if (handle==IntPtr.Zero)
        throw new Exception("Cannot instanciate model");
    }
    public string Name { get; }
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
        throw new Exception("Failed to read");
      return values;
    }

    private readonly Library library;
    private readonly IntPtr handle;
    private readonly Callbacks callbacks;

    public void Dispose()
    {
      library.fmi2FreeInstance(handle);
      callbacks.Dispose();
    }
  }
}