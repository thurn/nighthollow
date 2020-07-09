;; Copyright © 2020-present Derek Thurn
;;
;; Licensed under the Apache License, Version 2.0 (the "License");
;; you may not use this file except in compliance with the License.
;; You may obtain a copy of the License at
;;
;; https://www.apache.org/licenses/LICENSE-2.0
;;
;; Unless required by applicable law or agreed to in writing, software
;; distributed under the License is distributed on an "AS IS" BASIS,
;; WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
;; See the License for the specific language governing permissions and
;; limitations under the License.

(ns nighthollow.markers
  (:require
   [nighthollow.prelude :refer :all]))

(defn get-marker
  [key]
  (case key
    :dispatch Markers/Dispatch
    :apply-effects Markers/ApplyEffects
    :apply-event-commands Markers/ApplyEventCommands
    :event-command Markers/EventCommand
    :update-game-objects Markers/UpdateGameObjects
    :draw-cards Markers/DrawCards
    :set-can-play-card Markers/SetCanPlayCard
    :play-creature Markers/PlayCreature
    :create-enemy Markers/CreateEnemy
    :create-random-enemy Markers/CreateRandomEnemy
    :use-skill Markers/UseSkill
    :clear-current-skill Markers/ClearCurrentSkill
    :mutate-creature Markers/MutateCreature
    :mutate-user Markers/MutateUser
    :fire-projectile Markers/FireProjectile
    :game-start Markers/GameStart
    :tick Markers/Tick
    :start-drawing-cards Markers/ApplyStartDrawingCards
    :card-drawn Markers/CardDrawn
    :card-played Markers/CardPlayed
    :creature-played Markers/CreaturePlayed
    :enemy-appeared Markers/ApplyCreateEnemy
    :card-mutated Markers/CardMutated
    :creature-collision Markers/CreatureCollision
    :ranged-skill-fire Markers/RangedSkillFire
    :melee-skill-impact Markers/MeleeSkillImpact
    :projectile-impact Markers/ProjectileImpact
    :skill-complete Markers/SkillComplete
    :creature-killed Markers/CreatureKilled
    :creature-mutated Markers/CreatureMutated
    :user-mutated Markers/UserMutated
    :fire-projectile-internal Markers/FireProjectileInternal
    :use-skill-internal Markers/UseSkillInternal
    Markers/Unknown))

(defn start
  [key]
  (when-let [m (get-marker key)]
    (.Begin m)))

(defn stop
  [key]
  (when-let [m (get-marker key)]
    (.End m)))
