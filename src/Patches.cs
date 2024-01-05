// Patches which give the mod functionality
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using HarmonyLib;
using Server.Shared.Info;
using Server.Shared.State;

namespace AutoCrossout.Patches
{
  [HarmonyPatch(typeof(Game.Interface.HudGraveyardPanel), nameof(Game.Interface.HudGraveyardPanel.CreateGraveyardItem))]
  public class CrossOutDeadRoles
  {
    [HarmonyPostfix]
    public static void Postfix(Server.Shared.State.KillRecord killRecord)
    {
      if (SML.ModSettings.GetBool(ModInfo.SETTING_CROSS_OUT_DEAD))
      {
        Server.Shared.State.Role roleKilled = killRecord.playerRole;

        if (killRecord.playerId != Accessors.PlayerRoleInfoAccessor.position || !SML.ModSettings.GetBool(ModInfo.SETTING_CROSS_OUT_PLAYER))
        {
          Utils.WriteLog("Crossing out dead player's role : " + roleKilled);
          Accessors.RoleListAccessor.crossOutA(roleKilled);
        }
      }
    }
  }

  public static class CrossOutPlayerRole
  {
    internal static void HandlePostfix(GameInfo gameInfo)
    {
      if (SML.ModSettings.GetBool(ModInfo.SETTING_CROSS_OUT_PLAYER))
      {
        if (gameInfo.playPhase == PlayPhase.FIRST_DISCUSSION && Accessors.PlayerRoleInfoAccessor.role.HasValue)
        {
          Utils.WriteLog("Crossing out current player's role : " + Accessors.PlayerRoleInfoAccessor.role);
          Accessors.RoleListAccessor.crossOutA(Accessors.PlayerRoleInfoAccessor.role.Value);
        }
      }
    }

    // HACK: I need to handle both old and new chats, so this is split out into two patches
    // Unfortunetely, I couldn't find a better place to hook into, as I need the role list ready to perform the crossout
    [HarmonyPatch(typeof(Game.Interface.ChatViewSwitcher), nameof(Game.Interface.ChatViewSwitcher.HandleGamePhaseChanged))]
    public class OldChat
    {
      [HarmonyPostfix]
      public static void Postfix(GameInfo gameInfo)
      {
        HandlePostfix(gameInfo);
      }
    }

    [HarmonyPatch(typeof(Game.Interface.PooledChatViewSwitcher), nameof(Game.Interface.PooledChatViewSwitcher.HandleGamePhaseChanged))]
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

