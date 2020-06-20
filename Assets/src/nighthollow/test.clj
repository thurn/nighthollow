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

(ns nighthollow.test
  (:require
   [nighthollow.gameplay.base :as base]
   [nighthollow.gameplay.creatures :as creatures]
   [nighthollow.gameplay.enemies :as enemies]
   [nighthollow.prelude :refer :all]))

(def card-id [:card 0])

(def card creatures/acolyte-card)

(def deck
  (mapv #(vector % 4000) [card]))

(def hand {card-id card})

(def user (merge base/user {:deck deck}))

(def enemy enemies/viking)

(def creature-id [:creature 0])

(def creature (:creature card))

(def new-game
  {:tick 0
   :game-id 1
   :user user
   :creatures {}})

(def ongoing-game
  {:tick 0
   :game-id 1
   :user (merge user {:hand hand})
   :creatures {creature-id creature}})

(def state
  {:game ongoing-game
   :rules-map {}
   :events []})

(defn has-event-entity
  [state event-name entity-id]
  (let [e (filter (fn [{event :event}] (= event event-name)) (:events state))]
    (some #{entity-id} (:entities (first e)))))
