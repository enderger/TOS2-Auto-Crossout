{
  inputs = {
    nixpkgs.url = "github:nixos/nixpkgs/nixos-unstable";
  };

  outputs = inputs@{self, ...}: let
    lib = inputs.nixpkgs.lib;
    forAllSystems = lib.genAttrs lib.systems.flakeExposed;

    pkgsFor = system: import inputs.nixpkgs { inherit system; };
  in {
    devShells = forAllSystems (system: let
      pkgs = pkgsFor system;
    in {
      default = pkgs.mkShell {
        nativeBuildInputs = with pkgs; [
          omnisharp-roslyn dotnet-sdk_8 just ilspycmd
        ];
      };
    });
  };
}
