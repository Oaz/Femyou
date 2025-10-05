using System;
using Femyou.Internal;
using NUnit.Framework;

namespace Femyou.Tests;

[TestFixture(2)]
[TestFixture(3)]
public class CallbacksTests
{
  public CallbacksTests(int version)
  {
    _getFmuPath = name => TestTools.GetFmuPath(version, name);
    var isDarwin = System.Runtime.InteropServices.RuntimeInformation.OSDescription.Contains("Darwin");
    var isArm64 = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture == System.Runtime.InteropServices.Architecture.Arm64;
    Assume.That(version == 3 && isDarwin && isArm64, Is.False);
  }
  private Func<string, string> _getFmuPath;

  [Test]
  public void LogWhenVariableCannotBeSet()
  {
    var spyCallback = new SpyCallback();
    using var model = Model.Load(_getFmuPath("BouncingBall.fmu"));
    using var instance = model.CreateCoSimulationInstance("my name", spyCallback);
    instance.StartTime(0);
    try
    {
      instance.WriteReal((new Variable("foo","",42), 1));
    }
    catch(FmuException)
    {
      Assert.That(spyCallback.Instance.Name, Is.EqualTo("my name"));
      Assert.That(spyCallback.Status, Is.EqualTo(Status.Error));
      Assert.That(spyCallback.Category, Is.EqualTo("logStatusError"));
      return;
    }

    Assert.Fail("Exception shall have been raised");
  }

  [Test]
  public void CallbacksMustSurviveGarbageCollection()
  {
    var spyCallback = new SpyCallback();
    using var model = Model.Load(_getFmuPath("BouncingBall.fmu"));
    using var instance = model.CreateCoSimulationInstance("my name", spyCallback);
    instance.StartTime(0);
    GC.Collect();
    Assert.Throws<FmuException>(() =>
    {
      instance.WriteReal((new Variable("foo","",42), 1));
    });
  }

  class SpyCallback : ICallbacks
  {
    public IInstance Instance { get; private set; }
    public Status Status { get; private set; }
    public string Category { get; private set; }
    public string Message { get; private set; }

    public void Logger(IInstance instance, Status status, string category, string message)
    {
      Instance = instance;
      Status = status;
      Category = category;
      Message = message;
    }
  }
}