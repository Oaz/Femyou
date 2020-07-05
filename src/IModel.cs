using System;

namespace Femyou
{
  public interface IModel : IDisposable
  {
    string Name { get; }
    string Description { get; }
  }
}