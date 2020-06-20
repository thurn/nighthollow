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

(ns nighthollow.mutations
  (:require
   [clojure.test :refer [deftest is]]
   [nighthollow.core :as core]
   [nighthollow.prelude :refer :all]
   [nighthollow.test :as t]))

(defn- subtract-or-zero [x y]
  (let [result (- x y)]
    (if (neg? result) 0 result)))

(defn- subtract-or-error [x y]
  (let [result (- x y)]
    (if (neg? result)
      (error! "Underflow in subtracting" y "from" x)
      result)))

(defn- add-or-maximum [x y]
  (let [result (+ x y)]
    (if (> result 999) 999 result)))

(defn- update-key
  [entity key op amount on-underflow]
  (let [updated (update entity key op amount)]
    (if (< (key updated) 0)
      (case on-underflow
        :zero (assoc entity key 0)
        :error (error! "Key" key "cannot be < 0."
                       "Subtacting" amount
                       "from" (key entity)))
      updated)))

(defn- damage-total
  [damage]
  (reduce + (vals damage)))

(defn- apply-creature-mutation
  [creature {damage :apply-damage
             healing :heal-damage
             added-energy :gain-energy
             lost-energy :spend-energy}]
  (cond-> creature
    damage (update-key :damage + (damage-total damage) :error)
    healing (update-key :damage - healing :zero)
    added-energy (update-key :energy + added-energy :error)
    lost-energy (update-key :energy - lost-energy :error)))

(defn mutate-creature
  [state {creature-id :creature-id :as mutation}]
  (let [creature (get-in state [:game :creatures creature-id])
        was-alive (< (:damage creature) (:health creature))
        mutated (apply-creature-mutation creature mutation)
        killed (and was-alive (>= (:damage mutated) (:health mutated)))]
    (cond-> state
      true (update-in [:game :creatures] assoc creature-id mutated)
      true (core/push-event (assoc mutation
                                   :event :creature-mutated
                                   :entities [creature-id]))
      killed (core/push-event (assoc mutation
                                     :event :creature-killed
                                     :entities [creature-id])))))

(defn mutate-user
  [state {lose-life :lose-life, gain-life :gain-life, spend-mana :spend-mana
          lose-mana :lose-mana, gain-mana :gain-mana,
          lose-influence :lose-influence, gain-influence :gain-influence
          :as mutation}]
  (cond-> state
    lose-life (update-in [:game :user :life] subtract-or-zero lose-life)
    gain-life (update-in [:game :user :life] add-or-maximum gain-life)
    spend-mana (update-in [:game :user :mana] subtract-or-error spend-mana)
    lose-mana (update-in [:game :user :mana] subtract-or-zero lose-mana)
    gain-mana (update-in [:game :user :mana] add-or-maximum gain-mana)
    lose-influence (update-in [:game :user :influence]
                              #(merge-with subtract-or-zero % lose-influence))
    gain-influence (update-in [:game :user :influence]
                              #(merge-with + % gain-influence))
    true (core/push-event (assoc mutation
                                 :event :user-mutated
                                 :entities [[:user]]))))

(deftest test-mutate-creature
  (let [state (update-in t/state [:game :creatures t/creature-id]
                         assoc :damage 10 :energy 10)
        event (fn [args] (assoc args :creature-id t/creature-id))
        value (fn [key] [:game :creatures t/creature-id key])]
    (is (= 20 (get-in
               (mutate-creature state (event {:apply-damage {:fire 10}}))
               (value :damage))))
    (is (= 5 (get-in
              (mutate-creature state (event {:heal-damage 5}))
              (value :damage))))
    (is (= 0 (get-in
              (mutate-creature state (event {:heal-damage 50}))
              (value :damage))))
    (is (= 35 (get-in
               (mutate-creature state (event {:gain-energy 25}))
               (value :energy))))
    (is (= 5 (get-in
              (mutate-creature state (event {:spend-energy 5}))
              (value :energy))))
    (is (thrown? Exception
                 (mutate-creature state (event {:spend-energy 50}))))))

(deftest test-mutate-creature-events
  (let [state (mutate-creature t/state {:creature-id t/creature-id
                                        :apply-damage {:fire 1000}})]
    (is (t/has-event-entity state :creature-mutated t/creature-id))
    (is (t/has-event-entity state :creature-killed t/creature-id))))

(deftest test-mutate-user
  (let [state (update-in t/state [:game :user] assoc
                         :life 10
                         :mana 50
                         :influence {:flame 2})
        value (fn [key] [:game :user key])]
    (is (= 8 (get-in (mutate-user state {:lose-life 2}) (value :life))))
    (is (= 0 (get-in (mutate-user state {:lose-life 20}) (value :life))))
    (is (= 12 (get-in (mutate-user state {:gain-life 2}) (value :life))))
    (is (= 55 (get-in (mutate-user state {:gain-mana 5}) (value :mana))))
    (is (= 45 (get-in (mutate-user state {:lose-mana 5}) (value :mana))))
    (is (= 0 (get-in (mutate-user state {:lose-mana 75}) (value :mana))))
    (is (= 45 (get-in (mutate-user state {:spend-mana 5}) (value :mana))))
    (is (thrown? Exception (mutate-user state {:spend-mana 75})))
    (is (= {:flame 3} (get-in (mutate-user state {:gain-influence {:flame 1}})
                              (value :influence))))
    (is (= {:light 1, :flame 2} (get-in (mutate-user state
                                                     {:gain-influence {:light 1}})
                                        (value :influence))))
    (is (= {:flame 1} (get-in (mutate-user state {:lose-influence {:flame 1}})
                              (value :influence))))
    (is (= {:flame 0} (get-in (mutate-user state {:lose-influence {:flame 10}})
                              (value :influence))))
    (is (t/has-event-entity (mutate-user state {:lose-life 2})
                            :user-mutated
                            :user))))
