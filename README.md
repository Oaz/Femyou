![](logo.svg)

# Femyou
loading and running [fmi-standard](https://fmi-standard.org/) FMUs in .NET

![CI](https://github.com/Oaz/Femyou/workflows/CI/badge.svg)

[The binary package is available from NuGet](https://www.nuget.org/packages/Femyou/) as a .NETStandard2.1 lib.


## Features

This is a limited implementation of the fmi-standards.

Available features :
* Load FMU model encoded in either FMI 2.0 or 3.0 standard
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

These GIFs were created with
- the included demo program
- the [Bouncing Ball](https://github.com/modelica/Reference-FMUs/tree/master/BouncingBall) reference FMU compiled in FMI 2.0 and 3.0 standards

| FMI 2.0 | FMI 3.0 |
|---------|---------|
| ![](BouncingBall2.gif?raw=true) | ![](BouncingBall3.gif?raw=true) |
