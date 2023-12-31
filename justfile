set dotenv-load

[private]
_build *ARGS:
  @: {{ if env("STEAM_LIBRARY_PATH") == "" { error("Please set your Steam library path in the .env file, then try again") } else {""} }};
  dotnet build /p:SteamLibraryPath="$STEAM_LIBRARY_PATH" {{ARGS}}

build dev *ARGS:
  just _build {{ARGS}}
