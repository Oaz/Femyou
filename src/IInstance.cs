using System;
using System.Collections.Generic;

namespace Femyou
{
  public interface IInstance : IDisposable
  {
    string Name { get; }
    IEnumerable<double> ReadReal(IEnumerable<IVariable> variables);
    IEnumerable<int> ReadInteger(IEnumerable<IVariable> variables);
    IEnumerable<bool> ReadBoolean(IEnumerable<IVariable> variables);
    IEnumerable<string> ReadString(IEnumerable<IVariable> variables);
  }

  public static class ExtensionsIInstance
  {
    public static IEnumerable<double> ReadReal(this IInstance instance, params IVariable[] variables) => instance.ReadReal(variables);
    public static IEnumerable<int> ReadInteger(this IInstance instance, params IVariable[] variables) => instance.ReadInteger(variables);
    public static IEnumerable<bool> ReadBoolean(this IInstance instance, params IVariable[] variables) => instance.ReadBoolean(variables);
    public static IEnumerable<string> ReadString(this IInstance instance, params IVariable[] variables) => instance.ReadString(variables);
  }
}