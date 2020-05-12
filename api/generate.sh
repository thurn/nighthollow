#!/bin/sh
set -e

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
cd $DIR
protoc -I $DIR --csharp_out="$DIR/../Assets/Magewatch/API/" "$DIR/api.proto"
cd ..
export OUT_DIR="$DIR/../src"
echo `pwd`
echo $OUT_DIR
echo cargo run --bin generate_protos
cargo run --bin generate_protos

echo "// Generated Code. Do not edit!\n\n$(cat src/api.rs)" > src/api.rs
cargo fmt