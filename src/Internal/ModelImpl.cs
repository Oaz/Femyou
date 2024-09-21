using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;

namespace Femyou.Internal
{
  class ModelImpl : IModel
  {
    public readonly string TmpFolder;

    public ModelImpl(string fmuPath)
    {
      try
      {
        TmpFolder = Path.Combine(Path.GetTempPath(),nameof(Femyou),Path.GetFileName(fmuPath));
        ZipFile.ExtractToDirectory(fmuPath, TmpFolder, true);
        var modelDescription = XDocument.Load(Path.Combine(TmpFolder,"modelDescription.xml"));
        var root = modelDescription.Root;
        Variables = root
          !.Element("ModelVariables")
          !.Elements()
          .Select(sv => new Variable(sv) as IVariable)
          .ToDictionary(sv => sv.Name, sv => sv);
        var coSimulationId = root
          !.Element("CoSimulation")
          !.Attribute("modelIdentifier")
          !.Value;
        _coSimulation = new Library(TmpFolder,coSimulationId);
        Name = root!.Attribute("modelName")!.Value;
        Description = root!.Attribute("description")!.Value;
        Guid = root!.Attribute("guid")!.Value;
      }
      catch (Exception e)
      {
        throw new FmuException("Failed to load model description", e);
      }
    }

    public string Name { get; }
    public string Description { get; }
    public string Guid { get; }
    public IReadOnlyDictionary<string,IVariable> Variables { get; }
    public IInstance CreateCoSimulationInstance(string name, ICallbacks callbacks) => new Instance(name,this,_coSimulation,FMI2.fmi2Type.fmi2CoSimulation,callbacks);

    private readonly Library _coSimulation;
    
    public void Dispose()
    {
      _coSimulation.Dispose();
      Directory.Delete(TmpFolder,true);
    }
  }
}
