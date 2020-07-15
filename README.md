# Femyou
loading and running [fmi-standard](https://fmi-standard.org/) FMUs in dotnet core

[![License][license-image]][license-url]

## Features

This is a limited implementation of the fmi-standard.

Available features :
* Load FMU model
* Get model variables
* Create co-simulation instance
* Read and write variable values from instance
* Simulate by advancing time

The features are verified with the [reference FMUs](https://github.com/modelica/Reference-FMUs).

## Example

```C#
using var model = Model.Load("BouncingBall.fmu");
var height = model.Variables["h"];
var velocity = model.Variables["v"];
double h = 60.0, v = 0.0;
using var instance = model.CreateCoSimulationInstance("demo");
instance.WriteReal((height, h), (velocity, v));
instance.StartTime(0.0);
while (h > 0 || Math.Abs(v) > 0)
{
  var values = instance.ReadReal(height, velocity).ToArray();
  h = values[0];
  v = values[1];
  instance.AdvanceTime(0.1);
}
```

## Demos

The [Bouncing Ball](https://github.com/modelica/Reference-FMUs/tree/master/BouncingBall) reference FMU visualized with GIF animation.

![BouncingBall](BouncingBall.gif?raw=true)
