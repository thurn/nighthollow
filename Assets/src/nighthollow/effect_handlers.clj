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
  [state {quantity :quantity}]
  (let [input {:user (get-in state [:game :user]) :cards {}}
        {user :user cards :cards} (nth (iterate cards/draw-card input) quantity)
        state-with-rules (reduce-kv register-card-rules state cards)]
    (-> state-with-rules
        (assoc-in [:game :user] user)
        (core/push-event {:event :start-drawing-cards, :cards cards}))))

(defmethod core/handle-effect
  :set-can-play-card
  [state {can-play :can-play, card-id :card-id}]
  (-> state
      (update-in [:game :user :hand card-id] assoc :can-play can-play)
      (core/push-event {:event :card-mutated, :entities [card-id]})))

(defmethod core/handle-effect
  :play-creature
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
        (core/register-rules creature-id (:rules creature))
        (core/push-event {:event :creature-played
                          :entities [creature-id]
                          :creature updated}))))

(defmethod core/handle-effect
  :create-enemy
  [state {creature-id :creature-id, creature :creature, file :file}]
  (let [updated (assoc creature
                       :file file
                       :placed-time (get-in state [:game :tick]))]
    (-> state
        (update-in [:game :creatures] assoc creature-id updated)
        (core/register-rules creature-id (:rules creature))
        (core/push-event {:event :enemy-appeared
                          :entities [creature-id]
                          :creature updated}))))

(def all-files [1 2 3 4 5])

(defmethod core/handle-effect
  :create-random-enemy
  [state _]
  (lg "create random enemy")
  (let [creature (assoc (rand-nth (get-in state [:game :enemy :creatures]))
                        :file (rand-nth all-files))
        creature-id [:creature (core/new-id)]]
    (-> state
        (assoc-in [:game :creatures creature-id] creature)
        (core/register-rules creature-id (:rules creature))
        (core/push-event {:event :enemy-appeared
                          :entities [creature-id]
                          :creature creature}))))

(defmethod core/handle-effect
  :use-skill
  [state {creature-id :creature-id, skill :skill-animation-number :as effect}]
  (-> state
    (assoc-in [:game :creatures creature-id :current-skill] skill)
    (core/push-event (assoc effect :event :use-skill))))

(defmethod core/handle-effect
  :fire-projectile
  [state effect]
  (core/push-event state (assoc effect :event :fire-projectile)))

(defmethod core/handle-effect
  :clear-current-skill
  [state {creature-id :creature-id}]
  (-> state
    (update-in [:game :creatures creature-id] dissoc :current-skill)))

(defmethod core/handle-effect
  :mutate-creature
  [state mutation]
  (mutations/mutate-creature state mutation))

(defmethod core/handle-effect
  :mutate-user
  [state mutation]
  (mutations/mutate-user state mutation))