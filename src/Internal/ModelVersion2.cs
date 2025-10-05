using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Femyou.Internal
{
  public class ModelVersion2 : IModelVersion
  {
    public string CoSimulationElementName { get; } = "CoSimulation";
    public string GuidAttributeName { get; } = "guid";
    public string RelativePath(string name, Architecture architecture, PlatformID platform)
    {
      var isDarwin = System.Runtime.InteropServices.RuntimeInformation.OSDescription.Contains("Darwin");
      return platform switch
      {
        PlatformID.Unix => Path.Combine("binaries", (isDarwin ? "darwin" : "linux") + MapArchitecture(architecture), name + (isDarwin ? ".dylib" : ".so")),
        PlatformID.Win32NT => Path.Combine("binaries", "win" + MapArchitecture(architecture), name + ".dll"),
        _ => throw new FmuException($"Unsupported operating system {platform}"),
      };
    }

    public Library Load(string path) => new Library2(path);

    private string MapArchitecture(Architecture architecture) =>
      architecture switch
      {
        Architecture.X86 => "32",
        Architecture.X64 => "64",
        _ => throw new FmuException($"Unsupported architecture {architecture}"),
      };
  }
}