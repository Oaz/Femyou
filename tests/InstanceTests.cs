using System;
using NUnit.Framework;
using Femyou;
using System.Linq;

namespace Femyou.Tests
{
  public class InstanceTests
  {
    [Test]
    public void InstanceHasDefaultValues()
    {
      using var model = Model.Load(TestTools.GetFmuPath("Feedthrough.fmu"));
      using var instance = model.CreateCoSimulationInstance("example");
      Assert.That(instance.ReadBoolean(model.Variables["bool_in"]), Is.EqualTo(false));
      Assert.That(instance.ReadString(model.Variables["string_param"]), Is.EqualTo("Set me!"));
    }
    [Test]
    public void OtherInstanceHasDefaultValues()
    {
      using var model = Model.Load(TestTools.GetFmuPath("BouncingBall.fmu"));
      using var instance = model.CreateCoSimulationInstance("example");
      Assert.That(instance.ReadReal(model.Variables["g"]), Is.EqualTo(-9.81));
    }
  }
}