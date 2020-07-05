using System;

namespace Femyou
{
  public interface IInstance : IDisposable
  {
    string Name { get; }
    double ReadReal(IVariable variable);
    int ReadInteger(IVariable variable);
    bool ReadBoolean(IVariable variable);
    string ReadString(IVariable variable);

  }
}