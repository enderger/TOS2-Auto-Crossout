// Shared utilities for AutoCrossout
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
module AutoCrossout.Utils

let write_log message: unit = System.Console.WriteLine (sprintf "[%s] %s" AutoCrossout.ModInfo.PLUGIN_GUID message)

// vim:ft=fsharp
