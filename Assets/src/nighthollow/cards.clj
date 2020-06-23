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
   [clojure.data.generators :as generators]
   [nighthollow.core :as core]
   [nighthollow.prelude :refer :all]))

(defn make-deck
  [cards]
  {:cards cards :weights (zipmap (range (count cards)) (repeat 4000))})

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

(defn- random-card
  "Given a deck returns a tuple of a weighted random card from the deck and the
   updated weight values."
  [{cards :cards, weights :weights}]
  (let [index (generators/weighted weights)]
    [(nth cards index) (assoc weights index (new-weight (weights index)))]))

(defn draw-card
  "Takes a map with a :user value and a :cards accumulator. Adds a new
  randomly-selected card from the user's deck to their hand, decrementing the
  associated weight value for that card. Returns a map with :user containing
  the updated user state and :cards with a map of the drawn cards."
  [{{deck :deck, :as user} :user cards :cards}]
  (let [[card new-weights] (random-card deck)
        card-id [:card (core/new-id)]]
    {:cards (assoc cards card-id card)
     :user (-> user
               (assoc-in [:hand card-id] card)
               (assoc-in [:deck :weights] new-weights))}))
