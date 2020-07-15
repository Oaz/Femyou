using System;

namespace Femyou
{
  public class ConsoleCallbacks : ICallbacks
  {
    public void Logger(IInstance instance, Status status, string category, string message)
    {
      using var console = new System.IO.StreamWriter(System.Console.OpenStandardOutput());
      console.WriteLine($"[{instance.Name}] {category}({status}): {message}");
    }
  }
}