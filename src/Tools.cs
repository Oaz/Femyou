using System;
using System.IO;
using System.Reflection;

namespace Femyou
{
  public static class Tools
  {
    public static string GetBaseFolder(string folder, string name)
    {
      var directory = Path.GetDirectoryName(folder);
      if (Path.GetFileName(directory) == name)
        return directory;
      return GetBaseFolder(directory, name);
    }

    public static IInstance CreateInstance(IModel model, string name) => model.CreateCoSimulationInstance(name, new ConsoleCallbacks());
  }
}