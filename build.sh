#!/usr/bin/env bash

set -eu
set -o pipefail

dotnet paket restore

dotnet run --project ./build/Build.fsproj "$@"