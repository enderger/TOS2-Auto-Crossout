# TOS2-Auto-Crossout - A mod which automatically crosses out known roles in the role list

## NOTES
- This mod can be fooled by enchanters. Keep a close eye: if you see something which doesn't add up you may have one on your hands!
- This mod does not (yet) cross out your role in the list

## BUILDING
This project can be built in one of 2 ways:
- Using the `just` build runner (which automates the setting of the Steam library path)
- Manually passing arguments to `dotnet build`
-
In both cases, you're result will be found as `dist/AutoCrossout.dll`

### With Just
1. Install [just](https://github.com/casey/just)
2. Edit `.env` to have the path to your Steam library (the folder containing `steamapps`)
3. Run `just build-dev` or `just build-release` to build a development or release build

### Manual
1. Run `dotnet build -p:SteamLibraryPath="C:\\PATH\TO\YOUR\STEAM\LIBRARY"` (with `-c Release` if you want a release build)

