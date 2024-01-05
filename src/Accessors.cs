// Patches which collect info from the game state
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using HarmonyLib;
using Server.Shared.Extensions;
using Server.Shared.Info;
using Server.Shared.State;

namespace AutoCrossout.Accessors
{
  [HarmonyPatch(typeof(Game.Interface.HudRoleListAndGraveyardController), "Start")]
  public class RoleListAccessor
  {
    public static Game.Interface.HudRoleListAndGraveyardController? hrg_controller = null;

    /// <summary>
    /// Cross out a given role list item
    /// </summary>
    public static void crossOut(Game.Interface.RoleListItem it)
    {
      it.isCrossedOut = true;
      it.ValidateCrossOut();
    }

    /// <summary>
    /// Cross out the slot for a given role in the role list
    /// </summary>
    public static void crossOutA(Role role)
    {
      foreach (var item in hrg_controller!.roleListPanel.roleListItems)
      {
        if (!item.isCrossedOut)
        {
          if (item.role == role)
          {
            crossOut(item);
            return;
          }
          else if (item.role.IsBucket())
          {
            SharedRoleData.roleBucketLookup.TryGetValue(item.role, out var bucket);
            if (bucket.roles.Contains(role))
            {
              crossOut(item);
              return;
            }
          }
        }
      }
    }

    [HarmonyPostfix]
    public static void Postfix(Game.Interface.HudRoleListAndGraveyardController __instance)
    {
      hrg_controller = __instance;
    }
  }

  [HarmonyPatch(typeof(Game.Interface.RoleCardPanel), "HandleOnMyIdentityChanged")]
  public class PlayerRoleInfoAccessor
  {
    public static int? position = null;
    public static Role? role = null;

    [HarmonyPostfix]
    public static void Postfix(PlayerIdentityData playerIdentityData)
    {
      position = playerIdentityData.position;
      role = playerIdentityData.role;
    }
  }
}
