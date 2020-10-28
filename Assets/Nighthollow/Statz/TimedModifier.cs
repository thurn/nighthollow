// Copyright © 2020-present Derek Thurn

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

//    https://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#nullable enable

using UnityEngine;

namespace Nighthollow.Statz
{
  public sealed class TimedModifier<TOperation> : IModifier<TOperation> where TOperation : IOperation
  {
    readonly float _endTimeSeconds;

    public TOperation Operation { get; }

    public bool IsValid() => Time.time < _endTimeSeconds;

    public TimedModifier(TOperation operation, float endTimeSeconds)
    {
      Operation = operation;
      _endTimeSeconds = endTimeSeconds;
    }
  }
}