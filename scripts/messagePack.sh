#!/usr/bin/env bash

set -Eeuo pipefail
script_dir=$(cd "$(dirname "${BASH_SOURCE[0]}")" &>/dev/null && pwd -P)

echo dotnet mpc -i "$script_dir/../Assets/" -o "$script_dir/../Assets/Nighthollow/Generated/MessagePackGenerated.cs"
dotnet mpc -i "$script_dir/../Assets/" -o "$script_dir/../Assets/Nighthollow/Generated/MessagePackGenerated.cs"

echo perl -p -i -e "s/\?Formatter/Formatter/g" "$script_dir/../Assets/Nighthollow/Generated/MessagePackGenerated.cs"
perl -p -i -e "s/\?Formatter/Formatter/g" "$script_dir/../Assets/Nighthollow/Generated/MessagePackGenerated.cs"
