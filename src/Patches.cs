// Patch which crosses out known roles in the list
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using HarmonyLib;
using Server.Shared.Extensions;
using Server.Shared.Info;
using Server.Shared.State;

namespace AutoCrossout.Patches
{
  [HarmonyPatch(typeof(Game.Interface.HudRoleListAndGraveyardController), "Start")]
  public class RoleListAccessor
  {
    public static Game.Interface.HudRoleListAndGraveyardController hrg_controller = null;

    public static void crossOut(Game.Interface.RoleListItem it)
    {
      it.isCrossedOut = true;
      it.ValidateCrossOut();
    }

    public static int? crossOutA(Role role)
    {
      for (int i = 0; i < hrg_controller.roleListPanel.roleListItems.Count; i++)
      {
        var item = hrg_controller.roleListPanel.roleListItems[i];
        if (!item.isCrossedOut)
        {
          if (item.role == role)
          {
            crossOut(item);
            return i;
          }
          else if (item.role.IsBucket())
          {
            SharedRoleData.roleBucketLookup.TryGetValue(item.role, out var bucket);
            if (bucket.roles.Contains(role))
            {
              crossOut(item);
              return i;
            }
          }
        }
      }

      return null;
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
      Utils.WriteLog("Writing current player role info (player position: " + playerIdentityData.position + ")");
      position = playerIdentityData.position;
      role = playerIdentityData.role;
    }
  }

  [HarmonyPatch(typeof(Game.Interface.HudGraveyardPanel), "CreateGraveyardItem")]
  public class CrossOutDeadRoles
  {
    [HarmonyPostfix]
    public static void Postfix(Server.Shared.State.KillRecord killRecord)
    {
      Server.Shared.State.Role roleKilled = killRecord.playerRole;

      if (killRecord.playerId != PlayerRoleInfoAccessor.position)
      {
        Utils.WriteLog("Crossing out dead player's role : " + roleKilled);
        RoleListAccessor.crossOutA(roleKilled);
      }
    }
  }
  
  public class CrossOutPlayerRole
  {
    public static void HandlePostfix(GameInfo gameInfo)
    {
      if (gameInfo.playPhase == PlayPhase.FIRST_DISCUSSION && PlayerRoleInfoAccessor.role.HasValue)
      {
        Utils.WriteLog("Crossing out current player's role : " + PlayerRoleInfoAccessor.role);
        RoleListAccessor.crossOutA(PlayerRoleInfoAccessor.role.Value);
      }
    }

    // HACK: I need to handle both old and new chats, so this is split out into two patches
    // Unfortunetely, I couldn't find a better place to hook into, as I need the role list ready to perform the crossout
    [HarmonyPatch(typeof(Game.Interface.ChatViewSwitcher), "HandleGamePhaseChanged")]
    public class OldChat
    {
      [HarmonyPostfix]
      public static void Postfix(GameInfo gameInfo)
      {
        HandlePostfix(gameInfo);
      }
    }

    [HarmonyPatch(typeof(Game.Interface.PooledChatViewSwitcher), "HandleGamePhaseChanged")]
    public class NewChat
    {
      [HarmonyPostfix]
      public static void Postfix(GameInfo gameInfo)
      {
        HandlePostfix(gameInfo);

      }
    }
  }
}

