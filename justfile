set dotenv-load

[private]
_build *ARGS:
  @: {{ if env("STEAM_LIBRARY_PATH") == "" { error("Please set your Steam library path in the .env file, then try again") } else {""} }};
  dotnet build /p:SteamLibraryPath="$STEAM_LIBRARY_PATH" {{ARGS}}

build-dev *ARGS:
  just _build {{ARGS}}

build-release *ARGS:
  just _build -c Release {{ARGS}}

install-dev *ARGS: (build-dev ARGS)
  cp dist/AutoCrossout.dll "${STEAM_LIBRARY_PATH}/steamapps/common/Town of Salem 2/SalemModLoader/Mods/"

install-release *ARGS: (build-release ARGS)
  cp dist/AutoCrossout.dll "${STEAM_LIBRARY_PATH}/steamapps/common/Town of Salem 2/SalemModLoader/Mods/"
