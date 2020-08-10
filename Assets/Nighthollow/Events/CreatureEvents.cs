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

using System;
using System.Collections.Generic;
using Nighthollow.Components;
using UnityEngine;

namespace Nighthollow.Events
{
  [Serializable]
  public sealed class CreatureEvents
  {
    [SerializeField] List<CreatureEventHandler> _enteredPlay;
    [SerializeField] List<MultipleCreaturesEventHandler> _died;
    [SerializeField] List<TwoCreaturesEventHandler> _creatureCollision;
    [SerializeField] List<CreatureEventHandler> _rangedSkillFired;
    [SerializeField] List<MultipleCreaturesEventHandler> _meleeSkillImpact;
    [SerializeField] List<MultipleCreaturesEventHandler> _projectileImpact;
    [SerializeField] List<CreatureEventHandler> _skillComplete;

    public void OnEnteredPlay(Creature source) =>
      _enteredPlay.ForEach(c => c.Execute(source));

    public void OnDied(Creature source, Creature killedBy) =>
      _died.ForEach(c => c.Execute(source, new List<Creature> {killedBy}));

    public void OnCollision(Creature source, Creature target) =>
      _creatureCollision.ForEach(c => c.Execute(source, target));

    public void OnRangedSkillFired(Creature source) =>
      _rangedSkillFired.ForEach(c => c.Execute(source));

    public void OnMeleeSkillImpact(Creature source, List<Creature> hitTargets) =>
      _meleeSkillImpact.ForEach(c => c.Execute(source, hitTargets));

    public void OnProjectileImpact(Creature source, List<Creature> hitTargets) =>
      _projectileImpact.ForEach(c => c.Execute(source, hitTargets));

    public void OnSkillComplete(Creature source) =>
      _skillComplete.ForEach(c => c.Execute(source));
  }
}