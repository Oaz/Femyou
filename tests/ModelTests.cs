using System;
using NUnit.Framework;
using Femyou;
using System.Linq;

namespace Femyou.Tests
{
  public class ModelTests
  {
    [SetUp]
    public void Setup()
    {
    }

    [TestCase("BouncingBall.fmu", "BouncingBall", "This model calculates the trajectory, over time, of a ball dropped from a height of 1 m.")]
    [TestCase("VanDerPol.fmu", "Van der Pol oscillator", "This model implements the van der Pol oscillator")]
    public void ModelHasNameAndDescription(string filename, string expectedName, string expectedDescription)
    {
      var fmuPath = TestTools.GetFmuPath(filename);
      using var model = Model.Load(fmuPath);
      Assert.That(model.Name, Is.EqualTo(expectedName));
      Assert.That(model.Description, Is.EqualTo(expectedDescription));
    }

    [Test]
    public void ModelHasVariables()
    {
      var fmuPath = TestTools.GetFmuPath("Feedthrough.fmu");
      using var model = Model.Load(fmuPath);
      Assert.That(model.Variables.Count(), Is.EqualTo(11));
      Assert.That(model.Variables["string_param"].Description, Is.EqualTo("String parameter"));
      Assert.That(model.Variables["bool_out"].Description, Is.EqualTo("boolean output"));
    }
  }
}