using System;
using System.Runtime.InteropServices;
using NUnit.Framework;
using Femyou.Internal;

namespace Femyou.Tests;

public class PlatformTests
{
  [TestCase(PlatformID.Unix,Architecture.X64,"bar","binaries/linux64/bar.so")]
  [TestCase(PlatformID.Unix,Architecture.X86,"bar","binaries/linux32/bar.so")]
  [TestCase(PlatformID.Win32NT,Architecture.X64,"bar", "binaries/win64/bar.dll")]
  [TestCase(PlatformID.Win32NT,Architecture.X86,"bar", "binaries/win32/bar.dll")]
  public void GetRelativePath2(PlatformID platformId, Architecture architecture, string fmuName, string expectedLibraryPath)
  {
    Assert.That(
      new ModelVersion2().RelativePath( fmuName, architecture, platformId),
      Is.EqualTo(expectedLibraryPath)
    );
  }
  
  [TestCase(PlatformID.Unix,Architecture.X64,"bar","binaries/x86_64-linux/bar.so")]
  [TestCase(PlatformID.Unix,Architecture.X86,"bar","binaries/x86-linux/bar.so")]
  [TestCase(PlatformID.Win32NT,Architecture.X64,"bar", "binaries/x86_64-windows/bar.dll")]
  [TestCase(PlatformID.Win32NT,Architecture.X86,"bar", "binaries/x86-windows/bar.dll")]
  public void GetRelativePath3(PlatformID platformId, Architecture architecture, string fmuName, string expectedLibraryPath)
  {
    Assert.That(
      new ModelVersion3().RelativePath( fmuName, architecture, platformId),
      Is.EqualTo(expectedLibraryPath)
    );
  }

  [TestCase(PlatformID.MacOSX,Architecture.X64,"bar")]
  [TestCase(PlatformID.WinCE,Architecture.X86,"bar")]
  public void Unsupported2(PlatformID platformId, Architecture architecture, string fmuName)
  {
    Assert.Throws<FmuException>(() => new ModelVersion2().RelativePath(fmuName, architecture, platformId));
  }

  [TestCase(PlatformID.MacOSX,Architecture.X64,"bar")]
  [TestCase(PlatformID.WinCE,Architecture.X86,"bar")]
  public void Unsupported3(PlatformID platformId, Architecture architecture, string fmuName)
  {
    Assert.Throws<FmuException>(() => new ModelVersion3().RelativePath(fmuName, architecture, platformId));
  }
  
}