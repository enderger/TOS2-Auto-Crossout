// Shared utilities for AutoCrossout
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System;

namespace AutoCrossout
{
  class Utils
  {
    public static void WriteLog(string message)
    {
      Console.WriteLine("[" + AutoCrossout.ModInfo.PLUGIN_GUID + "] " + message);
    }

    public static void crossOut(Game.Interface.RoleListItem it)
    {
      it.isCrossedOut = true;
      it.ValidateCrossOut();
    }
  }
}
