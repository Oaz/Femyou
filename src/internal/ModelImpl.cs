using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;

namespace Femyou
{
  class ModelImpl : IModel
  {
    public readonly string FmuPath;
    public readonly string TmpFolder;
    public readonly XDocument ModelDescription;

    public ModelImpl(string fmuPath)
    {
      FmuPath = fmuPath;
      TmpFolder = Path.Combine(Path.GetTempPath(),nameof(Femyou),Path.GetFileName(FmuPath));
      ZipFile.ExtractToDirectory(fmuPath, TmpFolder, true);
      ModelDescription = XDocument.Load(Path.Combine(TmpFolder,"modelDescription.xml"));
      Variables = ModelDescription.Root.Element("ModelVariables").Elements().Select(sv => new Variable(sv) as IVariable).ToDictionary(sv => sv.Name, sv => sv);
      var coSimulationID = ModelDescription.Root.Element("CoSimulation").Attribute("modelIdentifier").Value;
      CoSimulation = new Library(TmpFolder,coSimulationID);
    }

    public string Name => ModelDescription.Root.Attribute("modelName").Value;
    public string Description => ModelDescription.Root.Attribute("description").Value;
    public string GUID => ModelDescription.Root.Attribute("guid").Value;
    public IReadOnlyDictionary<string,IVariable> Variables { get; }
    public IInstance CreateCoSimulationInstance(string name, ICallbacks callbacks) => new Instance(name,this,CoSimulation,FMI2.fmi2Type.fmi2CoSimulation,callbacks);

    private readonly Library CoSimulation;

    public void Dispose()
    {
      CoSimulation.Dispose();
      Directory.Delete(TmpFolder,true);
    }
  }
}
