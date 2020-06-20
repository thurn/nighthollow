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

(ns nighthollow.main
  (:require
   [clojure.spec.alpha :as s]
   [clojure.spec.test.alpha :as stest]
   [expound.alpha :as expound]
   [nighthollow.api :as api]
   [nighthollow.apply-event-commands]
   [nighthollow.cards]
   [nighthollow.core :as core]
   [nighthollow.effect-handlers]
   [nighthollow.find-entity]
   [nighthollow.prelude :refer :all]
   [nighthollow.test :as test]
   [nighthollow.update-game-object]))

(defn on-connect []
  (lg "on-connect")
  (alter-var-root #'s/*explain-out* (constantly expound/printer))
  (s/check-asserts true))

(defn on-start-new-game []
  (lg "on-start-new-game")
  (core/start-game! test/new-game [:user] [:user :rules])
  (core/dispatch! {:event :game-start, :entities [[:user]]}))

(defn on-tick []
  (core/on-tick!))

(defn on-card-drawn [card-id]
  (lg "on-card-drawn" card-id)
  (core/dispatch! {:event :card-drawn, :entities [[:card card-id]]}))

(defn on-played-card [card-id target-rank target-file]
  (lg "on-played-card" card-id target-rank target-file)
  (core/dispatch! {:event :card-played
                   :rank (api/<-rank-value target-rank)
                   :file (api/<-file-value target-file)
                   :entities [[:card card-id]]}))

(defn on-creature-collision [creature-id hit-creature-id]
  (lg "on-creature-collision" creature-id hit-creature-id)
  (core/dispatch! {:event :creature-collision
                   :entities [[:creature creature-id]]
                   :hit-creature-id [:creature hit-creature-id]}))

(defn on-ranged-skill-fire [creature-id]
  (lg "on-ranged-skill-fire" creature-id)
  (core/dispatch! {:event :ranged-skill-fire
                   :entities [[:creature creature-id]]}))

(defn on-melee-skill-impact [creature-id hit-creature-ids]
  (lg "on-melee-skill-impact" creature-id hit-creature-ids)
  (core/dispatch! {:event :melee-skill-impact
                   :entities [[:creature creature-id]]
                   :hit-creature-ids (mapv #(vector :creature %)
                                           hit-creature-ids)}))

(defn on-projectile-impact [creature-id hit-creature-ids]
  (lg "on-projectile-impact" creature-id hit-creature-ids)
  (core/dispatch! {:event :projectile-impact
                   :entities [[:creature creature-id]]
                   :hit-creature-ids (mapv #(vector :creature %)
                                           hit-creature-ids)}))

(defn on-skill-complete [creature-id has-melee-collision]
  (lg "on-skill-complete" creature-id has-melee-collision)
  (core/dispatch! {:event :skill-complete
                   :entities [[:creature creature-id]]
                   :has-melee-collision has-melee-collision}))

(defn on-debug-create-enemy []
  (core/dispatch! {:event :create-enemy
                   :creature-id [:creature -1]
                   :creature test/enemy, :file 3}))
