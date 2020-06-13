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

(ns nighthollow.data
  (:require [nighthollow.core :as core]))

(def user-id [:user])

(defn creature-id [id] [:creature id])

(defmethod core/find-entity :user find-user [_ game] (:user game))

(def default-user-state
  {:current-life 25
   :maximum-life 25
   :mana 0
   :influence []})

(defn ^{:event :game-start}
  draw-opening-hand
  [& _]
  [{:effect :draw-cards :quantity 5}])

(def default-user-rules
  [#'draw-opening-hand])
