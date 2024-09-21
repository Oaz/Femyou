using System;
using NUnit.Framework;
using Femyou.Internal;

namespace Femyou.Tests
{
  public class PlatformTests
  {
    [TestCase(PlatformID.Unix,true,"/home/foo","bar","/home/foo/binaries/linux64/bar.so")]
    [TestCase(PlatformID.Unix,false,"/home/foo","bar","/home/foo/binaries/linux32/bar.so")]
    [TestCase(PlatformID.Win32NT,true,"C:\\foo","bar", "C:\\foo/binaries/win64/bar.dll")]
    [TestCase(PlatformID.Win32NT,false,"C:\\foo","bar", "C:\\foo/binaries/win32/bar.dll")]
    public void GetLibraryPath(PlatformID platformId, bool is64Bits, string baseFolder, string fmuName, string expectedLibraryPath)
    {
      Assert.That(
        Platform.GetLibraryPath(platformId, is64Bits, baseFolder, fmuName),
        Is.EqualTo(expectedLibraryPath)
        );
    }

    [TestCase(PlatformID.MacOSX,"/home/foo","bar")]
    [TestCase(PlatformID.WinCE,"C:\\foo","bar")]
    public void Unsupported(PlatformID platformId, string baseFolder, string fmuName)
    {
      Assert.Throws<ArgumentException>(() => Platform.GetLibraryPath(platformId,true,baseFolder,fmuName));
    }
  }
}