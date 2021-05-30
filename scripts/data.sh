#!/usr/bin/env bash

set -Eeuo pipefail
script_dir=$(cd "$(dirname "${BASH_SOURCE[0]}")" &>/dev/null && pwd -P)

echo python3 "$script_dir/recordGenerator.py" "$script_dir/../Assets/Nighthollow/Data/"
python3 "$script_dir/recordGenerator.py" "$script_dir/../Assets/Nighthollow/Data/"

echo python3 "$script_dir/recordGenerator.py" "$script_dir/../Assets/Nighthollow/Rules/Conditions"
python3 "$script_dir/recordGenerator.py" "$script_dir/../Assets/Nighthollow/Rules/Conditions"

echo python3 "$script_dir/recordGenerator.py" "$script_dir/../Assets/Nighthollow/Rules/Effects"
python3 "$script_dir/recordGenerator.py" "$script_dir/../Assets/Nighthollow/Rules/Effects"

echo python3 "$script_dir/recordGenerator.py" "$script_dir/../Assets/Nighthollow/World/Data"
python3 "$script_dir/recordGenerator.py" "$script_dir/../Assets/Nighthollow/World/Data"

echo python3 "$script_dir/recordGenerator.py" "$script_dir/../Assets/Nighthollow/Editing/Scenarios"
python3 "$script_dir/recordGenerator.py" "$script_dir/../Assets/Nighthollow/Editing/Scenarios"

echo dotnet mpc -i "$script_dir/../Assets/" -o "$script_dir/../Assets/Nighthollow/Generated/MessagePackGenerated.cs"
dotnet mpc -i "$script_dir/../Assets/" -o "$script_dir/../Assets/Nighthollow/Generated/MessagePackGenerated.cs"

echo perl -p -i -e "s/\?Formatter/Formatter/g" "$script_dir/../Assets/Nighthollow/Generated/MessagePackGenerated.cs"
perl -p -i -e "s/\?Formatter/Formatter/g" "$script_dir/../Assets/Nighthollow/Generated/MessagePackGenerated.cs"
