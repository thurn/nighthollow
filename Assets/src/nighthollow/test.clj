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
   [nighthollow.core :as core]
   [nighthollow.cards :as cards]
   [nighthollow.gameplay.base :as base]
   [nighthollow.gameplay.creatures :as creatures]
   [nighthollow.gameplay.enemies :as enemies]
   [nighthollow.prelude :refer :all]))

(def card-id [:card 0])

(def card creatures/acolyte-card)

(def hand {card-id card})

(def user (merge base/user
                 {:deck (cards/make-deck
                         [creatures/acolyte-card
                          creatures/wizard-card
                          creatures/berserker-card])}))

(def enemy enemies/viking)

(def creature-id [:creature 0])

(def creature (:creature card))

(def new-game
  {:tick 0
   :game-id 1
   :user user
   :enemy (merge base/enemy
                 {:creatures [enemies/viking]})
   :creatures {}})

(def ongoing-game
  {:tick 0
   :game-id 2
   :user (merge user {:hand hand})
   :enemy (merge base/enemy
                 {:creatures [enemies/viking]})
   :creatures {creature-id creature}})

(def perftest-game
  {:tick 0
   :game-id 3
   :user (merge user {:hand hand
                      :life 10
                      :mana 999
                      :mana-gain-interval 5
                      :mana-gain-amount 10
                      :card-draw-interval 9999
                      :opening-hand-size 10})
   :enemy (merge base/enemy
                 {:enemy-creation-interval 1
                  :enemy-creation-quantity 10
                  :creatures [enemies/viking]})
   :creatures {}})

(def state
  {:game ongoing-game
   :rules-map {}
   :events []})

(defn has-event-entity
  [state event-name entity-id]
  (let [e (filter (fn [{event :event}] (= event event-name)) (:events state))]
    (some #{entity-id} (:entities (first e)))))

(defn make-perftest-creature
  [creature rank]
  (fn [file]
    {:creature-id [:creature (core/new-id)]
     :creature creature
     :rank rank
     :file file}))

(defn perftest-creatures
  []
  (concat
   (mapv (make-perftest-creature creatures/acolyte 1) (range 1 6))
   (mapv (make-perftest-creature creatures/acolyte 2) (range 1 6))
   (mapv (make-perftest-creature creatures/acolyte 3) (range 1 6))
   (mapv (make-perftest-creature creatures/wizard 4) (range 1 4))
   (mapv (make-perftest-creature creatures/berserker 4) (range 4 6))
   (mapv (make-perftest-creature creatures/wizard 5) (range 1 4))
   (mapv (make-perftest-creature creatures/berserker 5) (range 4 6))
   (mapv (make-perftest-creature creatures/berserker 6) (range 1 6))
   (mapv (make-perftest-creature creatures/berserker 7) (range 1 6))
   (mapv (make-perftest-creature creatures/berserker 8) (range 1 6))))
