#!/usr/bin/env bash

# Copyright © 2020-present Derek Thurn
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
# https://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

set -euo pipefail

cd "$( dirname "${BASH_SOURCE[0]}" )/.."
rsync -a --delete . /tmp/nighthollow
cd /tmp/nighthollow/
UNITY="/Applications/Unity/Hub/Editor/2019.3.15f1/Unity.app/Contents/MacOS/Unity"
METHOD="Nighthollow.Services.Tests.RunTests"
echo $UNITY -projectPath `pwd` -quit -batchmode -logFile out/log.txt -executeMethod $METHOD
$UNITY -projectPath `pwd` -quit -batchmode -logFile out/log.txt -executeMethod $METHOD
echo "All tests passed!"
