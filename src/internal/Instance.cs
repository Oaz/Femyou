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
    }
    public string Name { get; }
    public double ReadReal(IVariable variable) => Read(
      new IVariable[] { variable },
      new double[1],
      (a, b, c, d) => library.fmi2GetReal(a, b, c, d)
    )[0];
    public bool ReadBoolean(IVariable variable) => Read(
      new IVariable[] { variable },
      new FMI2.fmi2Boolean[1],
      (a, b, c, d) => library.fmi2GetBoolean(a, b, c, d)
    )[0] == FMI2.fmi2Boolean.fmi2True;
    public string ReadString(IVariable variable) => Marshalling.GetStringArray(Read(
      new IVariable[] { variable },
      Marshalling.CreateArray(1),
      (a, b, c, d) => library.fmi2GetString(a, b, c, d)
    ))[0];

    private T[] Read<T>(IEnumerable<IVariable> variables, T[] values, Func<IntPtr, UInt32[], ulong, T[], int> call)
    {
      var valueReferences = variables.Cast<Variable>().Select(variables => variables.ValueReference).ToArray();
      var status = call(handle, valueReferences, (ulong)valueReferences.Length, values);
      if (status != 0)
        throw new Exception();
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