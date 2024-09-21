using System;

namespace Femyou.Internal
{
  public abstract class Callbacks : IDisposable
  {
    public Callbacks(Instance instance, ICallbacks cb)
    {
      Instance = instance;
      Cb = cb;
    }

    public readonly Instance Instance;
    public readonly ICallbacks Cb;
    public abstract IntPtr Custom { get; }

    public abstract void Dispose();
  }
}