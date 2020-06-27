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
   [nighthollow.markers :as markers]
   [nighthollow.core :as core]
   [nighthollow.prelude :refer :all]
   [nighthollow.specs :as specs]))

(defmethod core/apply-event-commands!
  :start-drawing-cards
  [{cards :cards} _]
  (Commands/DrawCards (mapv (fn [[card-id card]] (api/->card card-id card))
                            cards)))

(defmethod core/apply-event-commands!
  :enemy-appeared
  [{entities :entities, creature :creature} _]
  (Commands/CreateEnemy (api/->creature (first entities) creature)
                        (api/->file-value (specs/grab creature :d/file))))

(defmethod core/apply-event-commands!
  :use-skill
  [{creature-id :creature-id
    skill :skill-animation-number
    skill-type :skill-type} _]
  (markers/start :use-skill-internal)
  (Commands/UseSkill (api/->creature-id creature-id)
                     (api/->skill-animation-number skill)
                     (api/->skill-type skill-type))
  (markers/stop :use-skill-internal))

(defmethod core/apply-event-commands!
  :fire-projectile
  [{projectile :projectile} _]
  (markers/start :fire-projectile-internal)
  (Commands/FireProjectile (api/->projectile projectile))
  (markers/stop :fire-projectile-internal))

(defmethod core/apply-event-commands!
  :creature-killed
  [{creature-id :creature-id} _]
  (Commands/PlayDeathAnimation (api/->creature-id creature-id)))
