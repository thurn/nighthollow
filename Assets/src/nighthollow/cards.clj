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

(ns nighthollow.cards
  (:require
   [arcadia.core :as arcadia]
   [clojure.test :refer [deftest is]]
   [nighthollow.core :as core]
   [nighthollow.api :as api]
   [nighthollow.data :as data]
   [nighthollow.test :as t]))

(defn card-id [id] [:card id])

(defn new-weight
  "Returns the next weight in sequence for a given card weight. The first 3
  cards drawn decrease the draw probability linearly, after which the values
  halve."
  [weight]
  (case weight
    4000 3000
    3000 2000
    2000 1000
    1000 500
    500 250
    250 125
    125 60
    60 30
    30 15
    15 8
    8 4
    4 2
    1))

(defn draw-card
  "Takes a map with a :user value and a :cards accumulator. Adds a new
  randomly-selected card from the user's deck to their hand, decrementing the
  associated weight value for that card. Returns a map with the updated user
  state and the drawn card added to the user's hand"
  [{{deck :deck, hand :hand, :as user} :user cards :cards}]
  (let [[drawn-card weight] (first deck)
        card-id (core/new-id)
        with-id (assoc drawn-card :id card-id)]
    {:cards (conj cards with-id)
     :user (-> user
               (update :hand assoc [:card card-id] with-id)
               (update-in [:deck 0] assoc 1 (new-weight weight)))}))

(defmethod core/find-entity
  :card
  find-card
  [card-id game]
  (get-in game [:user :hand card-id]))

(deftest test-find-entity
  (is (= t/card
         (core/find-entity [:card t/card-id] t/ongoing-game))))

(defmethod core/update-game-object!
  :card
  update-card
  [_ card]
  (Commands/UpdateCard (api/->card-data card)))

(defn register-card-rules
  [state card]
  (core/register-rules state [:card (:id card)] (:rules card)))

(defmethod core/handle-effect
  :draw-cards
  handle-draw-cards
  [state {quantity :quantity}]
  (let [input {:user (get-in state [:game :user]) :cards []}
        {user :user cards :cards} (nth (iterate draw-card input) quantity)
        state-with-rules (reduce register-card-rules state cards)]
    (-> state-with-rules
        (assoc-in [:game :user] user)
        (core/push-event {:event :start-drawing-cards, :cards cards}))))

(defmethod core/apply-event-commands!
  :start-drawing-cards
  apply-cards-drawn
  [{cards :cards} _]
  (Commands/DrawCards (mapv api/->card-data cards)))

(defmethod core/handle-effect
  :set-can-play-card
  handle-set-can-play-card
  [state {value :value, card-id :card-id}]
  (update-in state [:game :user :hand card-id] assoc :can-play value))
