using System;
using System.IO;
using System.Reflection;

namespace Femyou.Tests
{
  public static class TestTools
  {
    public static string GetFmuPath(string filename) => Path.Combine(FmuFolder,filename);
    public static string FmuFolder => Path.Combine(BaseFolder,"FMU","bin","dist");
    public static string BaseFolder => GetBaseFolder(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath, nameof(Femyou));

    public static string GetBaseFolder(string folder, string name)
    {
      var directory = Path.GetDirectoryName(folder);
      if(Path.GetFileName(directory) == name)
        return directory;
      return GetBaseFolder(directory,name);
    }

  }
}