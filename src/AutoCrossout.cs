// Automatically crosses out taken slots (mod for Town of Salem 2)
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using SML;

namespace AutoCrossout
{

  [Mod.SalemMod]
  class Main
  {
    public void Start()
    {
      Utils.WriteLog("Loaded " + ModInfo.PLUGIN_GUID + " Version " + ModInfo.PLUGIN_VERSION);
    }
  }

  public class ModInfo
  {
    public static string PLUGIN_GUID = "AutoCrossout";
    public static string PLUGIN_NAME = "Auto Crossout";
    public static string PLUGIN_VERSION = "0.1.0";

    public static string SETTING_CROSS_OUT_DEAD = "Cross Out Dead Roles";
    public static string SETTING_CROSS_OUT_PLAYER = "Cross Out Player Role";
  }
}

