using System;
using System.Runtime.InteropServices;

namespace Femyou.Internal
{
  public interface IModelVersion
  {
    string CoSimulationElementName { get; }
    string GuidAttributeName { get; }
    string RelativePath(string name, Architecture architecture, PlatformID platform);
    Library Load(string path);
  }
}