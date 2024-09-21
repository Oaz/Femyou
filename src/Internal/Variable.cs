using System;
using System.Xml.Linq;

namespace Femyou.Internal
{
  public class Variable : IVariable
  {
    public Variable(XElement xElement)
    {
      try
      {
        Name = xElement!.Attribute("name")!.Value;
        Description = xElement.Attribute("description")?.Value;
        ValueReference = uint.Parse(xElement!.Attribute("valueReference")!.Value);
      }
      catch (Exception e)
      {
        throw new FmuException("Failed to load variable description", e);
      }
    }

    public Variable(string name, string description, uint valueReference)
    {
      Name = name;
      Description = description;
      ValueReference = valueReference;
    }

    public string Name { get; }
    public string Description { get; }
    public uint ValueReference { get; }
  }
}