using System;
using NUnit.Framework;
using Femyou;

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
  }
}