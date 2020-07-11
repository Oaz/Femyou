
using NUnit.Framework;

namespace Femyou.Tests
{
  public class CallbacksTests
  {
    [Test]
    public void LogWhenVariableCannotBeSet()
    {
      var spyCallback = new SpyCallback();
      using var model = Model.Load(TestTools.GetFmuPath("BouncingBall.fmu"));
      using var instance = model.CreateCoSimulationInstance("my name", spyCallback);
      try
      {
        instance.WriteReal((model.Variables["v_min"], 1));
      }
      catch
      {
        Assert.That(spyCallback.Instance.Name, Is.EqualTo("my name"));
        Assert.That(spyCallback.Status, Is.EqualTo(Status.Error));
        Assert.That(spyCallback.Category, Is.EqualTo("logStatusError"));
        Assert.That(spyCallback.Message, Is.EqualTo("Variable v_min (value reference 4) is constant and cannot be set."));
        return;
      }
      Assert.Fail("Exception shall have been raised");
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
}