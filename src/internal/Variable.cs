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
    }

    public string Name { get; }
    public string Description { get; }
  }
}