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
   [nighthollow.core :as core]
   [nighthollow.api :as api]
   [nighthollow.test :as t]))

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
  state and a map with the drawn card added to the user's hand."
  [{{deck :deck, hand :hand, :as user} :user cards :cards}]
  (let [[card weight] (first deck)
        card-id [:card (core/new-id)]]
    {:cards (assoc cards card-id card)
     :user (-> user
               (update :hand assoc card-id card)
               (update-in [:deck 0] assoc 1 (new-weight weight)))}))
