using System;
using System.IO;
using System.IO.Compression;
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
    }

    public string Name => ModelDescription.Root.Attribute("modelName").Value;
    public string Description => ModelDescription.Root.Attribute("description").Value;

    public void Dispose()
    {
      Directory.Delete(TmpFolder,true);
    }
  }
}