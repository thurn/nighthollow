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

(ns nighthollow.test-data
  (:require [nighthollow.data :as data]))

(def card
  {:owner :user
   :can-play false
   :card-prefab "Content/Card"})

(def wizard-card
  (merge card
         {:name "Wizard"
          :type :creature
          :image "CreatureImages/Wizard"
          :cost {:mana 100 :influence {:flame 1}}
          :creature-prefab "Creatures/Player/Wizard"
          :health 100
          :damage {:physical 10}}))

(def standard-deck
  [wizard-card])

(defn make-deck
  [cards]
  (mapv #(vector % 4000) cards))

(def standard-game
  {:game-id 1
   :user {:rules data/default-user-rules
          :state data/default-user-state
          :deck (make-deck standard-deck)
          :hand []}})

(defn load-scenario
  "Returns a Game corresponding to a specific test scenario name"
  [name]
  standard-game)
