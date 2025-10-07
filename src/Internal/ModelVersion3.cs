using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Femyou.Internal
{
  public class ModelVersion3 : IModelVersion
  {
    public string CoSimulationElementName { get; } = "CoSimulation";
    public string GuidAttributeName { get; } = "instantiationToken";
    public Library Load(string path) => new Library3(path);
    
    public string RelativePath(string name, Architecture architecture, PlatformID platform) =>
      platform switch
      {
        PlatformID.Unix => Path.Combine("binaries", MapArchitecture(architecture)+"-linux", name + ".so"),
        PlatformID.Win32NT => Path.Combine("binaries", MapArchitecture(architecture)+"-windows", name + ".dll"),
        _ => throw new FmuException($"Unsupported operating system {platform}"),
      };

    private string MapArchitecture(Architecture architecture) =>
      architecture switch
      {
        Architecture.X86 => "x86",
        Architecture.X64 => "x86_64",
        Architecture.Arm64 => "aarch64",
        _ => throw new FmuException($"Unsupported architecture {architecture}"),
      };
  }
}