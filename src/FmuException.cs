using System;

namespace Femyou
{
  public class FmuException : Exception
  {
    public FmuException(string text) : base(text)
    {
    }

    public FmuException(string text, Exception innerException) : base(text, innerException)
    {
    }
  }
}