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
  [card _]
  [{:effect :set-can-play-card
    :value true
    :card-id [:card (:id card)]}])

(def card
  {:owner :user
   :can-play false
   :card-prefab "Content/Card"
   :rules [#'default-can-play-card]})

(def wizard-card
  (merge card
         {:name "Wizard"
          :type :creature
          :image "CreatureImages/Wizard"
          :cost {:mana 100 :influence {:flame 1}}
          :creature-prefab "Creatures/Player/Wizard"
          :health 100
          :damage {:physical 10}}))

(def user
  {:current-life 25
   :maximum-life 25
   :mana 0
   :influence []})

(defn ^{:event :game-start}
  draw-opening-hand
  [_ _]
  [{:effect :draw-cards :quantity 5}])

(def user-rules
  [#'draw-opening-hand])
