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

using Nighthollow.Components;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nighthollow.Events
{
  [Serializable]
  public sealed class CreatureEvents
  {
    [SerializeField] List<CreatureEventHandler> _enteredPlay;
    [SerializeField] List<MultipleCreaturesEventHandler> _died;

    public void OnEnteredPlay(Creature source) =>
      _enteredPlay.ForEach(c => c.Execute(source));

    public void OnDied(Creature source, Creature killedBy) =>
      _died.ForEach(c => c.Execute(source, new List<Creature> { killedBy }));
  }
}