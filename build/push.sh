#!/usr/bin/env bash
set -euo pipefail

usage() {
  echo "Usage:"
  echo "  build/push.sh [--env <path>] [--source <url>] [--skip-duplicates] [--no-symbols] [--delete-on-success]"
  echo ""
  echo "Defaults:"
  echo "  - loads build/.env from repo root if present"
  echo "  - pushes symbols by default"
  echo "  - source from env NUGET_SOURCE or nuget.org"
  echo ""
  echo "Examples:"
  echo "  build/push.sh"
  echo "  build/push.sh --env build/.env --skip-duplicates --delete-on-success"
}

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
ARTIFACTS="$REPO_ROOT/.artifacts"

ENV_FILE="$REPO_ROOT/build/.env"
SOURCE_DEFAULT="https://api.nuget.org/v3/index.json"

SKIP_DUPLICATES="false"
PUSH_SYMBOLS="true"
DELETE_ON_SUCCESS="false"
SOURCE=""

while [[ $# -gt 0 ]]; do
  case "$1" in
    --env)
      if [[ $# -lt 2 ]]; then echo "Error: --env requires a value"; exit 2; fi
      ENV_FILE="$REPO_ROOT/$2"
      shift 2
      ;;
    --source)
      if [[ $# -lt 2 ]]; then echo "Error: --source requires a value"; exit 2; fi
      SOURCE="$2"
      shift 2
      ;;
    --skip-duplicates)
      SKIP_DUPLICATES="true"
      shift
      ;;
    --no-symbols)
      PUSH_SYMBOLS="false"
      shift
      ;;
    --delete-on-success)
      DELETE_ON_SUCCESS="true"
      shift
      ;;
    -h|--help)
      usage
      exit 0
      ;;
    *)
      echo "Unknown argument: $1"
      usage
      exit 2
      ;;
  esac
done

if [[ -f "$ENV_FILE" ]]; then
  set -a
  # shellcheck disable=SC1090
  source "$ENV_FILE"
  set +a
fi

: "${NUGET_API_KEY:?NUGET_API_KEY is required (set it in build/.env or pass env file with --env)}"

if [[ -z "${SOURCE}" ]]; then
  SOURCE="${NUGET_SOURCE:-$SOURCE_DEFAULT}"
fi

if [[ ! -d "$ARTIFACTS" ]]; then
  echo "Error: artifacts folder not found: $ARTIFACTS"
  echo "Run: build/pack.sh <version> [--smoke]"
  exit 1
fi

NUPKGS="$(find "$ARTIFACTS" -maxdepth 1 -type f -name "*.nupkg" ! -name "*.snupkg" | sort || true)"
if [[ -z "$NUPKGS" ]]; then
  echo "Error: no .nupkg files found in $ARTIFACTS"
  exit 1
fi

SNUPKGS="$(find "$ARTIFACTS" -maxdepth 1 -type f -name "*.snupkg" | sort || true)"

NUPKG_COUNT="$(echo "$NUPKGS" | wc -l | tr -d '[:space:]')"
SNUPKG_COUNT="0"
if [[ -n "$SNUPKGS" ]]; then
  SNUPKG_COUNT="$(echo "$SNUPKGS" | wc -l | tr -d '[:space:]')"
fi

echo "===> Pushing packages"
echo "Repo:      $REPO_ROOT"
echo "Artifacts: $ARTIFACTS"
echo "Source:    $SOURCE"
echo "Nupkgs:    $NUPKG_COUNT"
echo "Snupkgs:   $SNUPKG_COUNT"
echo "Symbols:   $PUSH_SYMBOLS"
echo "Skip dup:  $SKIP_DUPLICATES"
echo "Delete:    $DELETE_ON_SUCCESS"

push_one() {
  pkg="$1"

  cmd=(dotnet nuget push "$pkg" --api-key "$NUGET_API_KEY" --source "$SOURCE")
  if [[ "$SKIP_DUPLICATES" == "true" ]]; then
    cmd+=(--skip-duplicate)
  fi

  echo "===> dotnet nuget push $(basename "$pkg")"
  "${cmd[@]}"
}

echo "$NUPKGS" | while IFS= read -r pkg; do
  [[ -z "$pkg" ]] && continue
  push_one "$pkg"
done

if [[ "$PUSH_SYMBOLS" == "true" ]]; then
  if [[ -z "$SNUPKGS" ]]; then
    echo "===> No .snupkg found (nothing to push)"
  else
    echo "$SNUPKGS" | while IFS= read -r sym; do
      [[ -z "$sym" ]] && continue
      push_one "$sym"
    done
  fi
fi

if [[ "$DELETE_ON_SUCCESS" == "true" ]]; then
  echo "===> Deleting pushed artifacts from $ARTIFACTS"
  rm -f "$ARTIFACTS"/*.nupkg || true
  rm -f "$ARTIFACTS"/*.snupkg || true
fi

echo "===> Done"