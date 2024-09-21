using System;

namespace Femyou
{
  public class FmuException : Exception
  {
    public FmuException(string text) : base(text)
    {
    }
  }
}