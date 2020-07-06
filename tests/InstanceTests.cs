using System;
using NUnit.Framework;
using Femyou;
using System.Linq;
using System.Collections.Generic;

namespace Femyou.Tests
{
  public class InstanceTests
  {
    [TestCaseSource("DefaultValuesTestCases")]
    public void InstanceHasDefaultValues((string filename, string variableName, Func<IInstance, IEnumerable<IVariable>, IEnumerable<object>> reader, object expectedDefaultValue) t)
    {
      using var model = Model.Load(TestTools.GetFmuPath(t.filename));
      using var instance = model.CreateCoSimulationInstance("example");
      var variables = new IVariable[] { model.Variables[t.variableName] };
      var actualValue = t.reader(instance, variables).First();
      Assert.That(actualValue, Is.EqualTo(t.expectedDefaultValue));
    }
    static readonly (string, string, Func<IInstance, IEnumerable<IVariable>, IEnumerable<object>>, object)[] DefaultValuesTestCases =
    {
      ("Feedthrough.fmu", "bool_in", (IInstance i, IEnumerable<IVariable> v) => i.ReadBoolean(v).Cast<object>(), false),
      ("Feedthrough.fmu", "string_param", (IInstance i, IEnumerable<IVariable> v) => i.ReadString(v), "Set me!"),
      ("BouncingBall.fmu", "g", (IInstance i, IEnumerable<IVariable> v) => i.ReadReal(v).Cast<object>(), -9.81),
      ("Stair.fmu", "counter", (IInstance i, IEnumerable<IVariable> v) => i.ReadInteger(v).Cast<object>(), 1)
    };

    [Test]
    public void ReadMultipleValuesInOneCall()
    {
      using var model = Model.Load(TestTools.GetFmuPath("VanDerPol.fmu"));
      using var instance = model.CreateCoSimulationInstance("example");
      var actualValues = instance.ReadReal(model.Variables["x0"],model.Variables["x1"],model.Variables["mu"]);
      Assert.That(actualValues, Is.EqualTo(new double[] {2,0,1}));
    }
  }
}