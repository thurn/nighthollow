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

(ns nighthollow.apply-event-commands
  (:require
   [nighthollow.api :as api]
   [nighthollow.core :as core]))

(defmethod core/apply-event-commands!
  :start-drawing-cards
  apply-cards-drawn
  [{cards :cards} _]
  (Commands/DrawCards (mapv (fn [[card-id card]] (api/->card card-id card))
                            cards)))

(defmethod core/apply-event-commands!
  :create-enemy
  apply-create-enemy
  [{creature-id :creature-id, creature :creature, file :file} _]
  (Commands/CreateEnemy (api/->creature creature-id creature)
                        (api/->file-value file)))
