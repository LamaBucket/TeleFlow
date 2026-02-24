#!/usr/bin/env bash
set -euo pipefail

usage() {
  echo "Usage:"
  echo "  build/pack.sh <version> [--smoke]"
  echo ""
  echo "Examples:"
  echo "  build/pack.sh 2.0.0"
  echo "  build/pack.sh 2.0.0 --smoke"
}

if [[ $# -lt 1 || $# -gt 2 ]]; then
  usage
  exit 2
fi

VERSION="$1"
RUN_SMOKE="false"

if [[ $# -eq 2 ]]; then
  if [[ "$2" == "--smoke" ]]; then
    RUN_SMOKE="true"
  else
    usage
    exit 2
  fi
fi

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
ARTIFACTS="$REPO_ROOT/.artifacts"
SMOKE_DIR="$REPO_ROOT/.smoke"

ABSTRACTIONS="$REPO_ROOT/src/TeleFlow.Abstractions/TeleFlow.Abstractions.csproj"
CORE="$REPO_ROOT/src/TeleFlow.Core/TeleFlow.Core.csproj"
DI="$REPO_ROOT/src/TeleFlow.Extensions.DI/TeleFlow.Extensions.DI.csproj"
POLLING="$REPO_ROOT/src/TeleFlow.Extensions.DI.Polling/TeleFlow.Extensions.DI.Polling.csproj"

META="$REPO_ROOT/src/meta/TeleFlow/TeleFlow.csproj"
META_POLLING="$REPO_ROOT/src/meta/TeleFlow.Polling/TeleFlow.Polling.csproj"

echo "===> TeleFlow pack: $VERSION"
echo "Repo:      $REPO_ROOT"
echo "Artifacts: $ARTIFACTS"

for p in "$ABSTRACTIONS" "$CORE" "$DI" "$POLLING" "$META" "$META_POLLING"; do
  if [[ ! -f "$p" ]]; then
    echo "Error: project not found: $p"
    exit 1
  fi
done

rm -rf "$ARTIFACTS"
mkdir -p "$ARTIFACTS"

cd "$REPO_ROOT"

# 1) Pack CORE first (this will build those projects; no restore/build of whole repo yet)
echo "===> Packing CORE packages first"
dotnet pack "$ABSTRACTIONS" -c Release -o "$ARTIFACTS" -p:TeleFlowVersion="$VERSION"
dotnet pack "$CORE"         -c Release -o "$ARTIFACTS" -p:TeleFlowVersion="$VERSION"
dotnet pack "$DI"           -c Release -o "$ARTIFACTS" -p:TeleFlowVersion="$VERSION"
dotnet pack "$POLLING"      -c Release -o "$ARTIFACTS" -p:TeleFlowVersion="$VERSION"

echo "===> CORE packages:"
ls -lah "$ARTIFACTS"/*.nupkg

# 2) Now META can restore TeleFlow.* from .artifacts (via your root NuGet.Config)
echo "===> Packing META packages"
dotnet pack "$META"         -c Release -o "$ARTIFACTS" -p:TeleFlowVersion="$VERSION"
dotnet pack "$META_POLLING" -c Release -o "$ARTIFACTS" -p:TeleFlowVersion="$VERSION"

echo "===> Produced packages:"
ls -lah "$ARTIFACTS"/*.nupkg

if ls "$ARTIFACTS"/*.snupkg >/dev/null 2>&1; then
  echo "===> Symbol packages:"
  ls -lah "$ARTIFACTS"/*.snupkg
fi

if [[ "$RUN_SMOKE" == "true" ]]; then
  echo "===> Running smoke test (consumer restore/build)"

  rm -rf "$SMOKE_DIR"
  mkdir -p "$SMOKE_DIR"
  cd "$SMOKE_DIR"

  dotnet new console --name TeleFlow.ConsumerSmoke
  cd TeleFlow.ConsumerSmoke

  # install from local artifacts
  dotnet add package TeleFlow.Polling --version "$VERSION" --source "$ARTIFACTS"

  # ensure restore uses the same local source (belt & suspenders)
  dotnet restore --source "$ARTIFACTS" --source "https://api.nuget.org/v3/index.json"
  dotnet build -c Release

  echo "===> Smoke test OK"
fi