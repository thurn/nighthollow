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
   [nighthollow.prelude :refer :all]))

(defn update-key
  [creature key op amount on-underflow]
  (let [updated (update creature key op amount)]
    (if (< (key updated) 0)
      (case on-underflow
        :zero (assoc creature key 0)
        :error (error! "Key cannot be < 0" key "subtacting" amount))
      updated)))

(defn damage-total
  [damage]
  (reduce + (vals damage)))

(defn mutate
  [creature {damage :apply-damage
             healing :heal-damage
             added-energy :gain-energy
             lost-energy :lose-energy}]
  (lg "mutate" damage)
  (cond-> creature
    damage (update-key :damage + (damage-total damage) :error)
    healing (update-key :damage - healing :zero)
    added-energy (update-key :energy + added-energy :error)
    lost-energy (update-key :energy - lost-energy :error)))
