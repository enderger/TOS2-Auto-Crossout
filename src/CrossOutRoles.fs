// Patch which crosses out known roles in the list
namespace AutoCrossout.Patches

open HarmonyLib
open Server.Shared.State

[<HarmonyPatch(typeof<Game.Interface.HudRoleListAndGraveyardController>, "Start")>]
type RoleListAccessor() = class
  static member val hrg_controller = null with get, set

  [<HarmonyPostfix>]
  static member Postfix (__instance: Game.Interface.HudRoleListAndGraveyardController): unit =
    RoleListAccessor.hrg_controller <- __instance
end

[<HarmonyPatch(typeof<Game.Interface.HudGraveyardPanel>, "HandleOnKillRecordsChanged")>]
type CrossOutRoles = class
  static let combine_role_align_and_subalign (align: RoleAlignment) (subalign: SubAlignment): Role option =
    match align, subalign with
      | RoleAlignment.TOWN, SubAlignment.POWER -> Some Role.TOWN_POWER
      | RoleAlignment.TOWN, SubAlignment.KILLING -> Some Role.TOWN_KILLING
      | RoleAlignment.TOWN, SubAlignment.SUPPORT -> Some Role.TOWN_SUPPORT
      | RoleAlignment.TOWN, SubAlignment.PROTECTIVE -> Some Role.TOWN_PROTECTIVE
      | RoleAlignment.TOWN, SubAlignment.INVESTIGATIVE -> Some Role.TOWN_INVESTIGATIVE
      | RoleAlignment.COVEN, SubAlignment.KILLING -> Some Role.COVEN_KILLING
      | RoleAlignment.COVEN, SubAlignment.UTILITY -> Some Role.COVEN_UTILITY
      | RoleAlignment.COVEN, SubAlignment.DECEPTION -> Some Role.COVEN_DECEPTION
      | RoleAlignment.COVEN, SubAlignment.POWER -> Some Role.COVEN_POWER
      | RoleAlignment.NEUTRAL, SubAlignment.EVIL -> Some Role.NEUTRAL_EVIL
      | RoleAlignment.NEUTRAL, SubAlignment.KILLING -> Some Role.NEUTRAL_KILLING
      | RoleAlignment.NEUTRAL, SubAlignment.APOCALYPSE -> Some Role.NEUTRAL_APOCALYPSE
      | _ -> None

  static let find_role_slot (roles: ResizeArray<Game.Interface.RoleListItem>) (role: Role): Game.Interface.RoleListItem option =
    let data = Server.Shared.State.SharedRoleData.GetRoleData role
    let alignment = combine_role_align_and_subalign data.roleAlignment data.subAlignment

    // NOTE: this may be optimizable into one loop
    let role_cmp = lazy roles.Find (fun it -> it.role = data.role)
    let align_cmp = lazy roles.Find (fun it -> alignment.IsSome && alignment.Value = it.role)
    let any_cmp = lazy roles.Find (fun it -> it.role = Role.ANY)

    if not <| isNull (role_cmp.Force ()) then
      Some role_cmp.Value
    else if not <| isNull (align_cmp.Force ()) then
      Some align_cmp.Value
    else
      match data.role with
       // This is a hopefully comprehensive list of possible reasons why a role may be hidden
       | Role.STONED | Role.HIDDEN -> None
       | _ ->
         if isNull <| any_cmp.Force () then
           AutoCrossout.Utils.write_log (sprintf "WARN: Could not find any slot to cross out")
           None
         else Some any_cmp.Value

  [<HarmonyPostfix>]
  static member Postfix (killRecords: ResizeArray<KillRecord>): unit =
    let kill_records = List.ofSeq killRecords
    for kill_record in kill_records do
      let roles = RoleListAccessor.hrg_controller.roleListPanel.roleListItems
      match find_role_slot roles kill_record.playerRole with
        | Some role_info ->
          AutoCrossout.Utils.write_log (sprintf "Automatically crossing out role %A" role_info.role)
          role_info.isCrossedOut <- true
          role_info.roleLabel.GetComponent<TMPro.TMP_Text>().fontStyle <- role_info.roleLabel.GetComponent<TMPro.TMP_Text>().fontStyle ||| TMPro.FontStyles.Strikethrough
        | None -> ()
end

// vim:ft=fsharp
