using System;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;

namespace Femyou.Tests;

[TestFixture(2)]
[TestFixture(3)]
public class InstanceTests
{
  public InstanceTests(int version)
  {
    _getFmuPath = name => TestTools.GetFmuPath(version, name);
    var isDarwin = System.Runtime.InteropServices.RuntimeInformation.OSDescription.Contains("Darwin");
    var isArm64 = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture == System.Runtime.InteropServices.Architecture.Arm64;
    Assume.That(version == 3 && isDarwin && isArm64, Is.False);

  }
  private Func<string, string> _getFmuPath;

  [TestCaseSource("DefaultValuesTestCases")]
  public void InstanceHasDefaultValues((string filename, string variableName, Func<IInstance, IEnumerable<IVariable>, IEnumerable<object>> reader, object expectedDefaultValue, Action<IInstance, IEnumerable<IVariable>> writer, object newValue) t)
  {
    using var model = Model.Load(_getFmuPath(t.filename));
    using var instance = Tools.CreateInstance(model, "example");
    var variables = new IVariable[] { model.Variables[t.variableName] };
    instance.StartTime(0.0);
    var actualValue = t.reader(instance, variables).First();
    Assert.That(actualValue, Is.EqualTo(t.expectedDefaultValue));
  }

  [TestCaseSource("DefaultValuesTestCases")]
  public void ChangeInstanceDefaultValues((string filename, string variableName, Func<IInstance, IEnumerable<IVariable>, IEnumerable<object>> reader, object expectedDefaultValue, Action<IInstance, IEnumerable<IVariable>> writer, object newValue) t)
  {
    using var model = Model.Load(_getFmuPath(t.filename));
    using var instance = Tools.CreateInstance(model, "example");
    var variables = new IVariable[] { model.Variables[t.variableName] };
    instance.StartTime(0.0);
    t.writer(instance, variables);
    var actualValue = t.reader(instance, variables).First();
    Assert.That(actualValue, Is.EqualTo(t.newValue));
  }

  static readonly (string, string, Func<IInstance, IEnumerable<IVariable>, IEnumerable<object>>, object, Action<IInstance, IEnumerable<IVariable>>, object)[] DefaultValuesTestCases =
  {
    ("Feedthrough.fmu", "bool_in", (i,v) => i.ReadBoolean(v).Cast<object>(), false, (i,vs) => i.WriteBoolean(vs.Select(v => (v,true))), true),
    ("Feedthrough.fmu", "string_param", (i,v) => i.ReadString(v), "Set me!", (i,vs) => i.WriteString(vs.Select(v => (v,"Foo"))), "Foo"),
    ("BouncingBall.fmu", "g", (i,v) => i.ReadReal(v).Cast<object>(), -9.81, (i,vs) => i.WriteReal(vs.Select(v => (v,3.46))), 3.46),
    ("Stair.fmu", "counter", (i,v) => i.ReadInteger(v).Cast<object>(), 1, (i,vs) => {}, 1),
    ("Feedthrough.fmu", "int_in", (i,v) => i.ReadInteger(v).Cast<object>(), 0, (i,vs) => i.WriteInteger(vs.Select(v => (v,28))), 28)
  };

  [Test]
  public void ReadMultipleValuesInOneCall()
  {
    using var model = Model.Load(_getFmuPath("VanDerPol.fmu"));
    using var instance = Tools.CreateInstance(model, "example");
    instance.StartTime(0.0);
    var actualValues = instance.ReadReal(model.Variables["x0"], model.Variables["x1"], model.Variables["mu"]);
    Assert.That(actualValues, Is.EqualTo(new double[] { 2, 0, 1 }));
  }

  // ReSharper disable InconsistentNaming

  [Test]
  public void FeedthroughReferenceScenario() // see https://github.com/modelica/Reference-FMUs/tree/master/Feedthrough
  {
    using var model = Model.Load(_getFmuPath("Feedthrough.fmu"));
    using var instance = Tools.CreateInstance(model, "reference");
    instance.WriteReal((model.Variables["real_fixed_param"], 1));
    instance.WriteString((model.Variables["string_param"], "FMI is awesome!"));
    var real_tunable_param = model.Variables["real_tunable_param"];
    var real_continuous_in = model.Variables["real_continuous_in"];
    var real_continuous_out = model.Variables["real_continuous_out"];
    var real_discrete_in = model.Variables["real_discrete_in"];
    var real_discrete_out = model.Variables["real_discrete_out"];
    var int_in = model.Variables["int_in"];
    var int_out = model.Variables["int_out"];
    var bool_in = model.Variables["bool_in"];
    var bool_out = model.Variables["bool_out"];
    instance.StartTime(0.0);
    foreach (var pt in feedthroughScenario)
    {
      instance.AdvanceTime(pt.time - instance.CurrentTime);
      instance.WriteReal((real_tunable_param, pt.real_tunable_param), (real_continuous_in, pt.input.real_continuous), (real_discrete_in, pt.input.real_discrete));
      instance.WriteInteger((int_in, pt.input.integer));
      instance.WriteBoolean((bool_in, pt.input.boolean));
      var actual_reals = instance.ReadReal(real_continuous_out, real_discrete_out).ToArray();
      var actual_int = instance.ReadInteger(int_out);
      var actual_bool = instance.ReadBoolean(bool_out);
      Assert.That(actual_reals.First(), Is.EqualTo(pt.expectedOutput.real_continuous));
      Assert.That(actual_reals.Last(), Is.EqualTo(pt.expectedOutput.real_discrete));
      Assert.That(actual_int.First(), Is.EqualTo(pt.expectedOutput.integer));
      Assert.That(actual_bool.First(), Is.EqualTo(pt.expectedOutput.boolean));
    }
  }


  readonly FTPoint[] feedthroughScenario = {
    FTPoint.At( time:0.0, real_tunable_param:0,  input:FTData.Is(0.0,0,0,false), expectedOutput: FTData.Is(1.0,0,0,false) ),
    FTPoint.At( time:0.2, real_tunable_param:0,  input:FTData.Is(0.0,0,0,false), expectedOutput: FTData.Is(1.0,0,0,false) ),
    FTPoint.At( time:0.4, real_tunable_param:0,  input:FTData.Is(0.0,0,0,false), expectedOutput: FTData.Is(1.0,0,0,false) ),
    FTPoint.At( time:0.5, real_tunable_param:0,  input:FTData.Is(0.0,0,0,false), expectedOutput: FTData.Is(1.0,0,0,false) ),
    FTPoint.At( time:0.5, real_tunable_param:0,  input:FTData.Is(2.0,0,0,false), expectedOutput: FTData.Is(3.0,0,0,false) ),
    FTPoint.At( time:0.6, real_tunable_param:0,  input:FTData.Is(1.8,0,0,false), expectedOutput: FTData.Is(2.8,0,0,false) ),
    FTPoint.At( time:0.8, real_tunable_param:0,  input:FTData.Is(1.4,0,0,false), expectedOutput: FTData.Is(2.4,0,0,false) ),
    FTPoint.At( time:1.0, real_tunable_param:0,  input:FTData.Is(1.0,0,0,false), expectedOutput: FTData.Is(2.0,0,0,false) ),
    FTPoint.At( time:1.0, real_tunable_param:0,  input:FTData.Is(1.0,1,1,true),  expectedOutput: FTData.Is(2.0,1,1,true)  ),
    FTPoint.At( time:1.2, real_tunable_param:0,  input:FTData.Is(1.0,1,1,true),  expectedOutput: FTData.Is(2.0,1,1,true)  ),
    FTPoint.At( time:1.4, real_tunable_param:0,  input:FTData.Is(1.0,1,1,true),  expectedOutput: FTData.Is(2.0,1,1,true)  ),
    FTPoint.At( time:1.5, real_tunable_param:0,  input:FTData.Is(1.0,1,1,true),  expectedOutput: FTData.Is(2.0,1,1,true)  ),
    FTPoint.At( time:1.5, real_tunable_param:-1, input:FTData.Is(1.0,1,1,true),  expectedOutput: FTData.Is(1.0,1,1,true)  ),
    FTPoint.At( time:1.6, real_tunable_param:-1, input:FTData.Is(1.0,1,1,true),  expectedOutput: FTData.Is(1.0,1,1,true)  ),
    FTPoint.At( time:1.8, real_tunable_param:-1, input:FTData.Is(1.0,1,1,true),  expectedOutput: FTData.Is(1.0,1,1,true)  ),
    FTPoint.At( time:2.0, real_tunable_param:-1, input:FTData.Is(1.0,1,1,true),  expectedOutput: FTData.Is(1.0,1,1,true)  )
  };

  struct FTData
  {
    public double real_continuous;
    public double real_discrete;
    public int integer;
    public bool boolean;
    public static FTData Is(double real_continuous, double real_discrete, int integer, bool boolean)
    {
      FTData dt;
      dt.real_continuous = real_continuous;
      dt.real_discrete = real_discrete;
      dt.integer = integer;
      dt.boolean = boolean;
      return dt;
    }
  }

  struct FTPoint
  {
    public double time;
    public double real_tunable_param;
    public FTData input;
    public FTData expectedOutput;
    public static FTPoint At(double time, double real_tunable_param, FTData input, FTData expectedOutput)
    {
      FTPoint pt;
      pt.time = time;
      pt.real_tunable_param = real_tunable_param;
      pt.input = input;
      pt.expectedOutput = expectedOutput;
      return pt;
    }
  }

}