// Shared utilities for AutoCrossout
module AutoCrossout.Utils

let write_log message: unit = System.Console.WriteLine (sprintf "[%s] %s" AutoCrossout.ModInfo.PLUGIN_GUID message)

// vim:ft=fsharp
