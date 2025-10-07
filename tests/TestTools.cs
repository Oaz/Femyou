using System;
using System.IO;
using System.Reflection;

namespace Femyou.Tests
{
  public static class TestTools
  {
    public static string GetFmuPath(int version, string filename) =>
      Path.Combine(FmuFolder(version), filename);
    public static string FmuFolder(int version) =>
      Path.Combine(BaseFolder, "FMU", $"bin{version}", "fmus");
    public static string BaseFolder => Tools
      .GetBaseFolder(new Uri(Assembly.GetExecutingAssembly().Location).AbsolutePath, nameof(Femyou));
  }
}