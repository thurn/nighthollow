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

(ns nighthollow.data
  (:require
   [arcadia.core :as arcadia]
   [nighthollow.core :as core]))

(def user-id [:user])

(defn creature-id [id] [:creature id])

(defn find-id
  "Returns a predicate which checks if an entity's id is 'id'."
  [id]
  (fn [map] (if (= (:id map) id) map nil)))

(defmethod core/find-entity :user find-user [_ game] (:user game))

(defn ^{:event :card-drawn}
  default-can-play-card
  [{card-id :entity-id}]
  [{:effect :set-can-play-card
    :can-play true
    :card-id card-id}])

(defn ^{:event :card-played}
  default-played-card
  [{[_ id :as card-id] :entity-id, {rank :rank, file :file} :event}]
  [{:effect :play-creature
    :card-id card-id
    :creature-id [:creature id]
    :rank rank
    :file file}])

(def card
  {:owner :user
   :can-play false
   :card-prefab "Content/Card"
   :rules [#'default-can-play-card #'default-played-card]})

(def creature
  (merge card
         {:base-damage {}
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
          :damage-reduction {}}))

(def wizard-card
  (merge creature
         {:name "Wizard"
          :image "CreatureImages/Wizard"
          :cost {:mana 100 :influence {:flame 1}}
          :creature-prefab "Creatures/Player/Wizard"
          :health 100
          :base-damage {:physical 10}}))

(def user
  {:current-life 25
   :maximum-life 25
   :mana 0
   :influence {}
   :hand {}
   :creatures {}})

(defn ^{:event :game-start}
  draw-opening-hand
  [_]
  [{:effect :draw-cards :quantity 5}])

(def user-rules
  [#'draw-opening-hand])
