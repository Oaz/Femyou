using System;
using System.Collections.Generic;

namespace Femyou.Internal
{
  public class Instance : IInstance
  {
    public Instance(string name, ModelImpl model, Library library, ICallbacks cb)
    {
      Name = name;
      _library = library;
      _callbacks = library.CreateCallbacks(this, cb);

      _handle = library.Instantiate(
        name,
        model.Guid,
        model.TmpFolder,
        _callbacks
      );
      if (_handle == IntPtr.Zero)
        throw new FmuException("Cannot instantiate model");
    }

    public string Name { get; }
    public double CurrentTime { get; private set; }

    public void StartTime(double time)
    {
      CurrentTime = time;
      _library.Setup(
        _handle,
        CurrentTime
      );
      _started = true;
    }

    public void AdvanceTime(double step)
    {
      if (step == 0.0)
        return;
      _library.Step(
        _handle,
        CurrentTime,
        step
      );
      CurrentTime += step;
    }

    public IEnumerable<double> ReadReal(IEnumerable<IVariable> variables) =>
      _library.ReadReal(_handle, variables);

    public IEnumerable<int> ReadInteger(IEnumerable<IVariable> variables) =>
      _library.ReadInteger(_handle, variables);

    public IEnumerable<bool> ReadBoolean(IEnumerable<IVariable> variables) =>
      _library.ReadBoolean(_handle, variables);

    public IEnumerable<string> ReadString(IEnumerable<IVariable> variables) =>
      _library.ReadString(_handle, variables);

    public void WriteReal(IEnumerable<(IVariable, double)> variables) =>
      _library.WriteReal(_handle, variables);

    public void WriteInteger(IEnumerable<(IVariable, int)> variables) =>
      _library.WriteInteger(_handle, variables);

    public void WriteBoolean(IEnumerable<(IVariable, bool)> variables) =>
      _library.WriteBoolean(_handle, variables);

    public void WriteString(IEnumerable<(IVariable, string)> variables) =>
      _library.WriteString(_handle, variables);

    private readonly Library _library;
    private readonly IntPtr _handle;
    private readonly Callbacks _callbacks;
    private bool _started;

    public void Dispose()
    {
      _library.Shutdown(_handle, _started);
      _callbacks.Dispose();
    }
  }
}