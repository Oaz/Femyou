using System;
using System.Xml.Linq;

namespace Femyou
{
  class Variable : IVariable
  {
    public Variable(XElement xElement)
    {
      Name = xElement.Attribute("name").Value;
      Description = xElement.Attribute("description")?.Value;
      ValueReference = uint.Parse(xElement.Attribute("valueReference").Value);
    }

    public string Name { get; }
    public string Description { get; }
    public uint ValueReference { get; }
  }
}