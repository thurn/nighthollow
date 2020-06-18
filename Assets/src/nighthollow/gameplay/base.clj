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

(ns nighthollow.gameplay.base
  (:require
   [nighthollow.prelude :refer :all]
   [nighthollow.specs :as specs]))

(defn ^{:event :card-drawn}
  default-can-play-card
  [{card-id :entity-id}]
  [{:effect :set-can-play-card
    :can-play true
    :card-id card-id}])

(defn ^{:event :card-played}
  default-played-card
  [{[_ id :as card-id] :entity-id
    {rank :rank, file :file} :event
    {creature :creature} :entity}]
  [{:effect :play-creature
    :card-id card-id
    :creature-id [:creature id]
    :creature creature
    :rank rank
    :file file}])

(def card
  {:owner :user
   :can-play false
   :card-prefab "Content/Card"
   :rules [#'default-can-play-card #'default-played-card]})

(def creature
  {:damage 0
   :energy 0
   :base-attack {}
   :speed 0
   :health-regeneration 0
   :starting-energy 0
   :maximum-energy 0
   :energy-regeneration 0
   :crit-chance 0
   :crit-multiplier 0
   :accuracy 0
   :evasion 0
   :damage-resistance {}
   :damage-reduction {}})

(defn ^{:event :creature-collision}
  melee-creature-collision
  [{creature-id :entity-id, {melee-skill :melee-skill} :entity}]
  (specs/validate "creature melee skill" :d/melee-skill melee-skill)
  [{:effect :use-skill
    :creature-id creature-id
    :skill-animation-number melee-skill
    :skill-type :melee}])

(defn ^{:event :melee-skill-impact}
  default-melee-skill-impact
  [{{base-attack :base-attack} :entity
    {hit-creature-ids :hit-creature-ids} :event}]
  (mapv (fn [creature-id] {:effect :mutate-creature
                           :creature-id creature-id
                           :apply-damage base-attack})
        hit-creature-ids))

(defn ^{:event :skill-complete}
  default-melee-skill-complete
  [{{has-melee-collision :has-melee-collision} :event :as args}]
  (if has-melee-collision
    (melee-creature-collision args)
    []))

(def melee-creature
  (merge creature
         {:rules (concat (:rules creature)
                         [#'melee-creature-collision
                          #'default-melee-skill-impact
                          #'default-melee-skill-complete])}))

(defn ^{:event :game-start}
  draw-opening-hand
  [_]
  [{:effect :draw-cards :quantity 5}])

(defn ^{:event :create-enemy}
  create-enemy
  [{{creature-id :creature-id, creature :creature, file :file} :event}]
  [{:effect :create-enemy
    :creature-id creature-id
    :creature creature
    :file file}])

(def user-rules
  [#'draw-opening-hand, #'create-enemy])

(def user
  {:current-life 25
   :maximum-life 25
   :mana 0
   :influence {}
   :hand {}
   :rules [#'draw-opening-hand #'create-enemy]})
