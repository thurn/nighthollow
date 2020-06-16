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

(ns nighthollow.test
  (:require [nighthollow.data :as data]))

(def card-id 0)

(def card (assoc data/wizard-card :id card-id))

(def deck
  (mapv #(vector % 4000) [data/wizard-card]))

(def hand [card])

(def user
  (merge data/user
         {:rules data/user-rules
          :state data/user
          :deck deck
          :hand {}}))

(def new-game
  {:game-id 1
   :user user})

(def ongoing-game
  {:game-id 1
   :user (merge user
          {:hand {[:card-id card-id] card}})})
