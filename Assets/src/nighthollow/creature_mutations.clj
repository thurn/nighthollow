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

(ns nighthollow.creature-mutations
  (:require
   [clojure.test :refer [deftest is]]
   [nighthollow.core :as core]
   [nighthollow.prelude :refer :all]
   [nighthollow.test :as t]))

(defn- update-key
  [creature key op amount on-underflow]
  (let [updated (update creature key op amount)]
    (if (< (key updated) 0)
      (case on-underflow
        :zero (assoc creature key 0)
        :error (error! "Key" key "cannot be < 0."
                       "Subtacting" amount
                       "from" (key creature)))
      updated)))

(defn- damage-total
  [damage]
  (reduce + (vals damage)))

(defn- apply-mutation
  [creature {damage :apply-damage
             healing :heal-damage
             added-energy :gain-energy
             lost-energy :lose-energy}]
  (cond-> creature
    damage (update-key :damage + (damage-total damage) :error)
    healing (update-key :damage - healing :zero)
    added-energy (update-key :energy + added-energy :error)
    lost-energy (update-key :energy - lost-energy :error)))

(defn mutate
  [state {creature-id :creature-id :as mutation}]
  (let [creature (get-in state [:game :creatures creature-id])
        was-alive (< (:damage creature) (:health creature))
        mutated (apply-mutation creature mutation)
        killed (and was-alive (>= (:damage mutated) (:health mutated)))]
    (cond-> state
      true (update-in [:game :creatures] assoc creature-id mutated)
      true (core/push-event (assoc mutation
                                   :event :creature-mutated
                                   :entities [creature-id]))
      killed (core/push-event (assoc mutation
                                     :event :creature-killed
                                     :entities [creature-id])))))

(deftest test-mutate
  (let [state (update-in t/state [:game :creatures t/creature-id]
                         assoc :damage 10 :energy 10)]
    (is (= 20 (get-in
               (mutate state t/creature-id {:apply-damage {:fire 10}})
               [:game :creatures t/creature-id :damage])))
    (is (= 5 (get-in
               (mutate state t/creature-id {:heal-damage 5})
               [:game :creatures t/creature-id :damage])))
    (is (= 0 (get-in
               (mutate state t/creature-id {:heal-damage 50})
               [:game :creatures t/creature-id :damage])))
    (is (= 35 (get-in
               (mutate state t/creature-id {:gain-energy 25})
               [:game :creatures t/creature-id :energy])))
    (is (= 5 (get-in
               (mutate state t/creature-id {:lose-energy 5})
               [:game :creatures t/creature-id :energy])))
    (is (thrown? Exception
                 (mutate state t/creature-id {:lose-energy 50})))))

(deftest test-mutate-events
  (let [state (mutate t/state t/creature-id {:apply-damage {:fire 1000}})]
    (is (t/has-event-entity state :creature-mutated t/creature-id))
    (is (t/has-event-entity state :creature-killed t/creature-id))))
