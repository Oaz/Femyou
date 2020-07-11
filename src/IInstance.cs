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
    void WriteReal(IEnumerable<(IVariable, double)> variables);
    void WriteInteger(IEnumerable<(IVariable, int)> variables);
    void WriteBoolean(IEnumerable<(IVariable, bool)> variables);
    void WriteString(IEnumerable<(IVariable, string)> variables);
    double CurrentTime { get; }
    void StartTime(double time);
    void AdvanceTime(double time);
  }

  public static class ExtensionsIInstance
  {
    public static IEnumerable<double> ReadReal(this IInstance instance, params IVariable[] variables) => instance.ReadReal(variables);
    public static IEnumerable<int> ReadInteger(this IInstance instance, params IVariable[] variables) => instance.ReadInteger(variables);
    public static IEnumerable<bool> ReadBoolean(this IInstance instance, params IVariable[] variables) => instance.ReadBoolean(variables);
    public static IEnumerable<string> ReadString(this IInstance instance, params IVariable[] variables) => instance.ReadString(variables);
    public static void WriteReal(this IInstance instance, params (IVariable, double)[] variables) => instance.WriteReal(variables);
    public static void WriteInteger(this IInstance instance, params (IVariable, int)[] variables) => instance.WriteInteger(variables);
    public static void WriteBoolean(this IInstance instance, params (IVariable, bool)[] variables) => instance.WriteBoolean(variables);
    public static void WriteString(this IInstance instance, params (IVariable, string)[] variables) => instance.WriteString(variables);
  }
}