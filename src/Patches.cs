// Patch which crosses out known roles in the list
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using HarmonyLib;
using Server.Shared.Extensions;
using Server.Shared.State;

namespace AutoCrossout.Patches
{

  [HarmonyPatch(typeof(Game.Interface.HudRoleListAndGraveyardController), "Start")]
  public class RoleListAccessor
  {
    public static Game.Interface.HudRoleListAndGraveyardController hrg_controller = null;

    [HarmonyPostfix]
    public static void Postfix(Game.Interface.HudRoleListAndGraveyardController __instance)
    {
      hrg_controller = __instance;
    }
  }

  [HarmonyPatch(typeof(Game.Interface.HudGraveyardPanel), "CreateGraveyardItem")]
  public class CrossOutRoles
  {
    static void crossOut(Game.Interface.RoleListItem it)
    {
      it.isCrossedOut = true;
      it.ValidateCrossOut();
    }

    [HarmonyPostfix]
    public static void Postfix(Server.Shared.State.KillRecord killRecord)
    {
      Server.Shared.State.Role roleKilled = killRecord.playerRole;

      foreach (var item in RoleListAccessor.hrg_controller.roleListPanel.roleListItems)
      {
        if (!item.isCrossedOut)
        {
          if (item.role == roleKilled)
          {
            crossOut(item);
            return;
          }
          else if (item.role.IsBucket())
          {
            SharedRoleData.roleBucketLookup.TryGetValue(item.role, out var bucket);
            if (bucket.roles.Contains(roleKilled))
            {
              crossOut(item);
              return;
            }
          }
        }
      }
    }
  }
}

