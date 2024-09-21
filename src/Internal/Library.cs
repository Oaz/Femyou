using System;
using NativeLibraryLoader;

namespace Femyou.Internal
{
  class Library : IDisposable
  {
    public Library(string baseFolder, string name)
      : this(Platform.GetLibraryPath(
        Environment.OSVersion.Platform,
        Environment.Is64BitProcess,
        baseFolder,
        name))
    {
    }

    public Library(string path)
    {
      _fmuLibrary = new NativeLibrary(path);

      fmi2Instantiate = _fmuLibrary.LoadFunction<FMI2.fmi2InstantiateTYPE>(nameof(fmi2Instantiate));
      fmi2FreeInstance = _fmuLibrary.LoadFunction<FMI2.fmi2FreeInstanceTYPE>(nameof(fmi2FreeInstance));
      fmi2SetupExperiment = _fmuLibrary.LoadFunction<FMI2.fmi2SetupExperimentTYPE>(nameof(fmi2SetupExperiment));
      fmi2EnterInitializationMode = _fmuLibrary.LoadFunction<FMI2.fmi2EnterInitializationModeTYPE>(nameof(fmi2EnterInitializationMode));
      fmi2ExitInitializationMode = _fmuLibrary.LoadFunction<FMI2.fmi2ExitInitializationModeTYPE>(nameof(fmi2ExitInitializationMode));
      fmi2Terminate = _fmuLibrary.LoadFunction<FMI2.fmi2TerminateTYPE>(nameof(fmi2Terminate));
      fmi2Reset = _fmuLibrary.LoadFunction<FMI2.fmi2ResetTYPE>(nameof(fmi2Reset));

      fmi2GetReal = _fmuLibrary.LoadFunction<FMI2.fmi2GetRealTYPE>(nameof(fmi2GetReal));
      fmi2GetInteger = _fmuLibrary.LoadFunction<FMI2.fmi2GetIntegerTYPE>(nameof(fmi2GetInteger));
      fmi2GetBoolean = _fmuLibrary.LoadFunction<FMI2.fmi2GetBooleanTYPE>(nameof(fmi2GetBoolean));
      fmi2GetString = _fmuLibrary.LoadFunction<FMI2.fmi2GetStringTYPE>(nameof(fmi2GetString));
      fmi2SetReal = _fmuLibrary.LoadFunction<FMI2.fmi2SetRealTYPE>(nameof(fmi2SetReal));
      fmi2SetInteger = _fmuLibrary.LoadFunction<FMI2.fmi2SetIntegerTYPE>(nameof(fmi2SetInteger));
      fmi2SetBoolean = _fmuLibrary.LoadFunction<FMI2.fmi2SetBooleanTYPE>(nameof(fmi2SetBoolean));
      fmi2SetString = _fmuLibrary.LoadFunction<FMI2.fmi2SetStringTYPE>(nameof(fmi2SetString));

      fmi2DoStep = _fmuLibrary.LoadFunction<FMI2.fmi2DoStepTYPE>(nameof(fmi2DoStep));
    }

    // ReSharper disable InconsistentNaming -- must use fmi standard names to load function in library
    public readonly FMI2.fmi2InstantiateTYPE fmi2Instantiate;
    public readonly FMI2.fmi2FreeInstanceTYPE fmi2FreeInstance;
    public readonly FMI2.fmi2SetupExperimentTYPE fmi2SetupExperiment;
    public readonly FMI2.fmi2EnterInitializationModeTYPE fmi2EnterInitializationMode;
    public readonly FMI2.fmi2ExitInitializationModeTYPE fmi2ExitInitializationMode;
    public readonly FMI2.fmi2TerminateTYPE fmi2Terminate;
    public readonly FMI2.fmi2ResetTYPE fmi2Reset;
    public readonly FMI2.fmi2GetRealTYPE fmi2GetReal;
    public readonly FMI2.fmi2GetIntegerTYPE fmi2GetInteger;
    public readonly FMI2.fmi2GetBooleanTYPE fmi2GetBoolean;
    public readonly FMI2.fmi2GetStringTYPE fmi2GetString;
    public readonly FMI2.fmi2SetRealTYPE fmi2SetReal;
    public readonly FMI2.fmi2SetIntegerTYPE fmi2SetInteger;
    public readonly FMI2.fmi2SetBooleanTYPE fmi2SetBoolean;
    public readonly FMI2.fmi2SetStringTYPE fmi2SetString;
    public readonly FMI2.fmi2DoStepTYPE fmi2DoStep;

    private readonly NativeLibrary _fmuLibrary;
    public void Dispose()
    {
      _fmuLibrary.Dispose();
    }
  }
}