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
  (:require [nighthollow.core :as core]))

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

(defn draw-random-card
  "Adds a new randomly-selected card from the user's deck to their hand, and
  decrements the associated weight value for that card."
  [{deck :deck hand :hand :as user}]
  (let [[card weight] (first deck)]
    (-> user
        (update :hand conj card)
        (update-in [:deck 0] assoc 1 (new-weight weight)))))

(defmethod core/handle-effect
  :draw-cards
  draw-cards
  [state {quantity :quantity}]
  (update-in state [:game :user] draw-random-card))
