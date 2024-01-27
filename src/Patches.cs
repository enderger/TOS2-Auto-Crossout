// Patches which give the mod functionality
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using HarmonyLib;
using Server.Shared.Info;
using Server.Shared.State;
using System.Collections.Generic;

namespace AutoCrossout.Patches
{
  // HACK: Delay crossing out the current player's role until after the game loads
  public static class CrossOutStartingRoles
  {
    public static List<(Role, uint)> StartingRoles = new List<(Role, uint)>();

    internal static void HandlePostfix(GameInfo gameInfo)
    {
      if (gameInfo.playPhase == PlayPhase.FIRST_DISCUSSION)
      {
        foreach (var role in StartingRoles)
        {
          Utils.WriteDebug("Crossing out known player role at start of game : " + role);
          Accessors.RoleListAccessor.CrossOutA(role.Item1, role.Item2);
        }

        StartingRoles.Clear();
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

  [HarmonyPatch(typeof(Game.Simulation.GameObservations), nameof(Game.Simulation.GameObservations.HandlePlayerIdentityObservation))]
  public class CrossOutRoles
  {
    [HarmonyPostfix]
    public static void Postfix(PlayerIdentityObservation playerIdentityObservation)
    {
      var data = playerIdentityObservation.Data;
      // TODO: uncomment this once BMG actually sets the MemoryType field
      //if ((data.memoryType == MemoryType.SELF && SML.ModSettings.GetBool(ModInfo.SETTING_CROSS_OUT_PLAYER))
      //    || (data.memoryType == MemoryType.WHO_DIED_AND_HOW && SML.ModSettings.GetBool(ModInfo.SETTING_CROSS_OUT_DEAD))
      //    || (data.memoryType == MemoryType.REVEAL && SML.ModSettings.GetBool(ModInfo.SETTING_CROSS_OUT_REVEALED))
      //    || data.memoryType == MemoryType.NONE
      //    )
      //{
        if (data.role != Role.UNKNOWN && data.role != Role.NONE && data.role != 0)
        {
          Utils.WriteDebug("Crossing out role " + data.role + " for player at position " + data.position);

          if (Pepper.IsGamePhasePlay() && Accessors.RoleListAccessor.hrg_controller != null)
            Accessors.RoleListAccessor.CrossOutA(data.role, (uint)data.position);
          else if (Pepper.IsRoleRevealPhase() || Pepper.IsGamePhasePlay())
            CrossOutStartingRoles.StartingRoles.Add((data.role, (uint)data.position));
          else
          {
            Utils.WriteDebug("Cancelling role crossout: not in correct game phase");
            return;
          }
        }
      //}
    }
  }
}

