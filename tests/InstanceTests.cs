using System;
using NUnit.Framework;
using Femyou;
using System.Linq;

namespace Femyou.Tests
{
  public class InstanceTests
  {
    [TestCaseSource("DefaultValuesTestCases")]
    public void InstanceHasDefaultValues((string filename, string variableName, Func<IInstance, IVariable, object> reader, object expectedDefaultValue) t)
    {
      using var model = Model.Load(TestTools.GetFmuPath(t.filename));
      using var instance = model.CreateCoSimulationInstance("example");
      var variable = model.Variables[t.variableName];
      var actualValue = t.reader(instance, variable);
      Assert.That(actualValue, Is.EqualTo(t.expectedDefaultValue));
    }
    static readonly (string, string, Func<IInstance, IVariable, object>, object)[] DefaultValuesTestCases =
    {
      ("Feedthrough.fmu", "bool_in", (IInstance i, IVariable v) => i.ReadBoolean(v), false),
      ("Feedthrough.fmu", "string_param", (IInstance i, IVariable v) => i.ReadString(v), "Set me!"),
      ("BouncingBall.fmu", "g", (IInstance i, IVariable v) => i.ReadReal(v), -9.81),
      ("Stair.fmu", "counter", (IInstance i, IVariable v) => i.ReadInteger(v), 1)
    };
  }
}