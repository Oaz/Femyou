using System;
using NUnit.Framework;
using System.Linq;

namespace Femyou.Tests
{
  [TestFixture(2)]
  [TestFixture(3)]
  public class ModelTests
  {
    public ModelTests(int version)
    {
      _getFmuPath = name => TestTools.GetFmuPath(version, name);
      var isDarwin = System.Runtime.InteropServices.RuntimeInformation.OSDescription.Contains("Darwin");
      var isArm64 = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture == System.Runtime.InteropServices.Architecture.Arm64;
      Assume.That(version == 3 && isDarwin && isArm64, Is.False);
    }
    private readonly Func<string, string> _getFmuPath;
    
    [TestCase("BouncingBall.fmu", "bouncingball", "This model calculates the trajectory, over time, of a ball dropped from a height of 1 m.")]
    [TestCase("VanDerPol.fmu", "van der pol oscillator", "This model implements the van der Pol oscillator")]
    public void ModelHasNameAndDescription(string filename, string expectedName, string expectedDescription)
    {
      var fmuPath = _getFmuPath(filename);
      using var model = Model.Load(fmuPath);
      Assert.That(model.Name.ToLower(), Is.EqualTo(expectedName));
      Assert.That(model.Description, Is.Null.Or.EqualTo(expectedDescription));
    }

    [Test]
    public void ModelHasVariables()
    {
      var fmuPath = _getFmuPath("Feedthrough.fmu");
      using var model = Model.Load(fmuPath);
      Assert.That(model.Variables.Count(), Is.GreaterThanOrEqualTo(11));
      Assert.That(model.Variables["string_param"].Description, Is.EqualTo("String parameter"));
      Assert.That(model.Variables["bool_out"].Description, Is.EqualTo("boolean output"));
    }
  }
}