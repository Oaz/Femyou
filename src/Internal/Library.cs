using System;
using System.Collections.Generic;
using NativeLibraryLoader;

namespace Femyou.Internal
{
  public abstract class Library : IDisposable
  {
    public Library(string path)
    {
      FmuLibrary = new NativeLibrary(path);
    }

    protected readonly NativeLibrary FmuLibrary;
    public void Dispose()
    {
      FmuLibrary.Dispose();
    }
    public abstract Callbacks CreateCallbacks(Instance instance, ICallbacks cb);

    public abstract IntPtr Instantiate(string name, string modelGuid, string modelTmpFolder, Callbacks callbacks);
    public abstract void Setup(IntPtr handle, double currentTime);
    public abstract void Step(IntPtr handle, double currentTime, double step);
    public abstract void Shutdown(IntPtr handle, bool started);
    
    public abstract IEnumerable<double> ReadReal(IntPtr handle, IEnumerable<IVariable> variables);
    public abstract IEnumerable<int> ReadInteger(IntPtr handle, IEnumerable<IVariable> variables);
    public abstract IEnumerable<bool> ReadBoolean(IntPtr handle, IEnumerable<IVariable> variables);
    public abstract IEnumerable<string> ReadString(IntPtr handle, IEnumerable<IVariable> variables);
    public abstract void WriteReal(IntPtr handle, IEnumerable<(IVariable, double)> variables);
    public abstract void WriteInteger(IntPtr handle, IEnumerable<(IVariable, int)> variables);
    public abstract void WriteBoolean(IntPtr handle, IEnumerable<(IVariable, bool)> variables);
    public abstract void WriteString(IntPtr handle, IEnumerable<(IVariable, string)> variables);

  }
}
