using System;
using NUnit.Framework;
using Femyou;

namespace Femyou.Tests
{
  public class PlatformTests
  {
    [TestCase(PlatformID.Unix,true,"/home/foo","bar","/home/foo/binaries/linux64/bar.so")]
    [TestCase(PlatformID.Unix,false,"/home/foo","bar","/home/foo/binaries/linux32/bar.so")]
    [TestCase(PlatformID.Win32NT,true,"C:\\foo","bar", "C:\\foo/binaries/win64/bar.dll")]
    [TestCase(PlatformID.Win32NT,false,"C:\\foo","bar", "C:\\foo/binaries/win32/bar.dll")]
    public void GetLibraryPath(PlatformID platformID, bool is64bits, string baseFolder, string fmuName, string expectedLibraryPath)
    {
      Assert.That(Platform.GetLibraryPath(platformID, is64bits, baseFolder, fmuName), Is.EqualTo(expectedLibraryPath));
    }

    [TestCase(PlatformID.MacOSX,"/home/foo","bar")]
    [TestCase(PlatformID.WinCE,"C:\\foo","bar")]
    public void Unsupported(PlatformID platformID, string baseFolder, string fmuName)
    {
      Assert.Throws<ArgumentException>(() => Platform.GetLibraryPath(platformID,true,baseFolder,fmuName));
    }
  }
}