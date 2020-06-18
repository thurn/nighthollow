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

(ns nighthollow.find-entity
  (:require
   [nighthollow.core :as core]
   [nighthollow.prelude :refer :all]))

(defmethod core/find-entity :card find-card
  [card-id game]
  (get-in game [:user :hand card-id]))

(defmethod core/find-entity :user find-user
  [_ game]
  (:user game))

(defmethod core/find-entity :creature find-creature
  [creature-id game]
  (get-in game [:creatures creature-id]))
