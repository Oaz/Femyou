using System;
using System.Xml.Linq;

namespace Femyou.Internal
{
  class Variable : IVariable
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

    public string Name { get; }
    public string Description { get; }
    public uint ValueReference { get; }
  }
}