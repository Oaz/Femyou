using System;
using System.IO;

namespace Femyou.Internal
{
  public class Platform
  {
    public static string GetLibraryPath(PlatformID platformId, bool is64Bits, string baseFolder, string name) =>
      platformId switch
      {
        PlatformID.Unix => Path.Combine(baseFolder, "binaries", "linux" + Bits(is64Bits), name + ".so"),
        PlatformID.Win32NT => Path.Combine(baseFolder, "binaries", "win" + Bits(is64Bits), name + ".dll"),
        _ => throw new ArgumentException($"Unsupported platform {platformId}"),
      };

    private static string Bits(bool is64Bits) => is64Bits ? "64" : "32";
  }
}