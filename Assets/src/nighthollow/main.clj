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
   [arcadia.core :as arcadia]
   [clojure.spec.alpha :as s]
   [clojure.spec.test.alpha :as stest]
   [expound.alpha :as expound]
   [nighthollow.api :as api]
   [nighthollow.cards]
   [nighthollow.core :as core]
   [nighthollow.data :as data]
   [nighthollow.test :as test]))

(defn on-connect []
  (arcadia/log "on-connect")
  (alter-var-root #'s/*explain-out* (constantly expound/printer))
  (s/check-asserts true))

(defn on-start-new-game []
  (arcadia/log "on-start-new-game")
  (core/start-game! test/new-game data/user-id [:user :rules])
  (core/dispatch! {:event :game-start}))

(defn on-card-drawn [card-id]
  (arcadia/log "on-card-drawn" card-id)
  (core/dispatch! {:event :card-drawn, :entities [[:card card-id]]}))

(defn on-played-card [card-id target-rank target-file]
  (arcadia/log "on-played-card" card-id target-rank target-file)
  (core/dispatch! {:event :card-played
                   :rank (api/<-rank-value target-rank)
                   :file (api/<-file-value target-file)
                   :entities [[:card card-id]]}))
