using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;

namespace Femyou.Internal
{
  public class ModelImpl : IModel
  {
    public ModelImpl(string fmuPath)
    {
      var res = System.Runtime.InteropServices.RuntimeInformation.OSArchitecture;
      var res2 = System.Runtime.InteropServices.RuntimeInformation.OSDescription;
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
        var fmiVersion = root!.Attribute("fmiVersion")?.Value;
        ModelVersion = fmiVersion!.StartsWith("2.") ? (IModelVersion)new ModelVersion2() : new ModelVersion3();
        var coSimulationId = root
          !.Element(ModelVersion.CoSimulationElementName)
          !.Attribute("modelIdentifier")
          !.Value;
        var libPath = GetLibPath(ModelVersion, coSimulationId);
        _library = ModelVersion.Load(libPath);
        Name = root!.Attribute("modelName")!.Value;
        Description = root?.Attribute("description")?.Value;
        Guid = root!.Attribute(ModelVersion.GuidAttributeName)!.Value;
      }
      catch (Exception e)
      {
        throw new FmuException("Failed to load model description", e);
      }
    }

    private string GetLibPath(IModelVersion version, string coSimulationId) =>
      Path.Combine(
        TmpFolder,
        version.RelativePath(
          coSimulationId,
          System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture,
          Environment.OSVersion.Platform
        )
      );

    public readonly string TmpFolder;
    public string Name { get; }
    public string Description { get; }
    public string Guid { get; }
    public IModelVersion ModelVersion { get; }
    public IReadOnlyDictionary<string,IVariable> Variables { get; }

    public IInstance CreateCoSimulationInstance(string name, ICallbacks callbacks) =>
      new Instance(name, this, _library, callbacks);

    private readonly Library _library;
    
    public void Dispose()
    {
      _library.Dispose();
      Directory.Delete(TmpFolder,true);
    }
  }
}
