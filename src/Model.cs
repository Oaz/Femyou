﻿using Femyou.Internal;

namespace Femyou
{
  public class Model
  {
    public static IModel Load(string fmuPath)
    {
      return new ModelImpl(fmuPath);
    }
  }
}
