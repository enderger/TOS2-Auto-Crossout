// Automatically crosses out taken slots (mod for Town of Salem 2)
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace AutoCrossout

open SML

[<Mod.SalemMod>]
type Main() = class
  member it.Start(): unit =
    try
      ignore <| HarmonyLib.Harmony.CreateAndPatchAll typeof<AutoCrossout.Patches.RoleListAccessor>
      ignore <| HarmonyLib.Harmony.CreateAndPatchAll typeof<AutoCrossout.Patches.CrossOutRoles>
    with
      | e -> Utils.write_log (sprintf "%A" e)

    AutoCrossout.Utils.write_log "Plugin Loaded!"
    ()
end


// vim:ft=fsharp
