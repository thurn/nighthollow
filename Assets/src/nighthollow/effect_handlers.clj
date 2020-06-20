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

(ns nighthollow.effect-handlers
  (:require
   [nighthollow.core :as core]
   [nighthollow.cards :as cards]
   [nighthollow.mutations :as mutations]
   [nighthollow.prelude :refer :all]))

(defn register-card-rules
  [state card-id card]
  (core/register-rules state card-id (:rules card)))

(defmethod core/handle-effect
  :draw-cards
  handle-draw-cards
  [state {quantity :quantity}]
  (let [input {:user (get-in state [:game :user]) :cards {}}
        {user :user cards :cards} (nth (iterate cards/draw-card input) quantity)
        state-with-rules (reduce-kv register-card-rules state cards)]
    (-> state-with-rules
        (assoc-in [:game :user] user)
        (core/push-event {:event :start-drawing-cards, :cards cards}))))

(defmethod core/handle-effect
  :set-can-play-card
  handle-set-can-play-card
  [state {can-play :can-play, card-id :card-id}]
  (update-in state [:game :user :hand card-id] assoc :can-play can-play))

(defmethod core/handle-effect
  :play-creature
  handle-play-creature
  [state {card-id :card-id
          creature-id :creature-id
          creature :creature
          rank :rank
          file :file}]
  (let [updated (assoc creature
                       :rank rank
                       :file file
                       :placed-time (get-in state [:game :tick]))]
    (-> state
        (update-in [:game :user :hand] dissoc card-id)
        (update-in [:game :creatures] assoc creature-id updated)
        (core/register-rules creature-id (:rules creature)))))

(defmethod core/handle-effect
  :create-enemy
  handle-create-enemy
  [state {creature-id :creature-id, creature :creature, file :file}]
  (-> state
      (update-in [:game :creatures] assoc creature-id creature)
      (core/register-rules creature-id (:rules creature))))

(defmethod core/handle-effect
  :use-skill
  handle-use-skill
  [state effect]
  (core/push-event state (assoc effect :event :use-skill)))

(defmethod core/handle-effect
  :mutate-creature
  handle-mutate-creature
  [state mutation]
  (mutations/mutate-creature state mutation))

(defmethod core/handle-effect
  :mutate-user
  [state mutation]
  (mutations/mutate-user state mutation))
