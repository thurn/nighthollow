#!/bin/sh
set -e

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
protoc -I $DIR --csharp_out="$DIR/../Assets/Magewatch/API/" "$DIR/api.proto"
