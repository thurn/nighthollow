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

(ns nighthollow.gameplay.creatures
  (:require
   [nighthollow.gameplay.base :as base :refer [inherit]]
   [nighthollow.prelude :refer :all]))

(def wizard
  (inherit base/melee-creature
           {:name "Wizard"
            :creature-prefab "Creatures/User/Wizard"
            :owner :user
            :health 200
            :melee-skill :skill3
            :base-attack {:physical 25}}))

(def wizard-card
  (inherit base/card
           {:image "CreatureImages/Wizard"
            :cost {:mana 100 :influence {:flame 1}}
            :creature wizard}))

(defn ^{:event :tick}
  gain-mana-over-time
  [{creature :entity, creature-id :entity-id, {tick :tick} :event}]
  (when (= 0 (mod (- tick (:placed-time creature))
                  (:mana-gain-interval creature))))
  [{:effect :mutate-user :gain-mana (:mana-gain-amount creature)}])

(def acolyte
  (inherit base/melee-creature
           {:name "Acolyte"
            :creature-prefab "Creatures/User/Acolyte"
            :owner :user
            :health 100
            :melee-skill :skill2
            :mana-gain-interval 5
            :mana-gain-amount 5
            :base-attack {:physical 5}
            :rules [#'gain-mana-over-time]}))

(def acolyte-card
  (inherit base/card
           {:image "CreatureImages/Acolyte"
            :cost {:mana 25, :influence {}}
            :creature acolyte}))
