using System;
using System.IO;
using System.Reflection;

namespace Femyou.Tests
{
  public static class TestTools
  {
    public static string GetFmuPath(string filename) => Path.Combine(FmuFolder, filename);
    public static string FmuFolder => Path.Combine(BaseFolder, "FMU", "bin", "dist");
    public static string BaseFolder => Tools
      .GetBaseFolder(new Uri(Assembly.GetExecutingAssembly().Location).AbsolutePath, nameof(Femyou));
  }
}