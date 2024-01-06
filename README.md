# TOS2-Auto-Crossout - A mod which automatically crosses out known roles in the role list

## NOTES
- This mod can be fooled by enchanters. Keep a close eye: if you see something which doesn't add up you may have one on your hands!

## BUILDING
This project can be built in one of 2 ways:
- Using the `just` build runner (which automates the setting of the Steam library path)
- Manually passing arguments to `dotnet build`

In both cases, you're result will be found as `dist/AutoCrossout.dll`

### With Just
1. Install [just](https://github.com/casey/just) and one of it's supported `bash` shells (see the page for a list of those)
2. Edit `.env` to have the path to your Steam library (the folder containing `steamapps`)
3. Run `just build-dev` or `just build-release` to build a development or release build

### Manual
1. Run `dotnet build -p:SteamLibraryPath="C:\\PATH\TO\YOUR\STEAM\LIBRARY"` (with `-c Release` if you want a release build)

## FUTURE IDEAS
- Find some way to incorporate manually crossed-out roles
- Release on Salem Mod Loader

## CREDITS
- [Curtis](https://github.com/Curtbot9000), who helped me learn the ropes of TOS2 modding (and who I got the rewritten code using Role Buckets from)
- patr. on Discord, who told me about the `GameObservations` hook that drastically shortened this code and made it more compatible

## MIRRORS
- Contributions should be done on the main repository at <https://sr.ht/~hutzdog/TOS2-Auto-Crossout>
- A mirror on Codeberg exists at <https://codeberg.org/Hutzdog/TOS2-Auto-Crossout>
- A mirror on GitHub exists at <https://github.com/enderger/TOS2-Auto-Crossout>
