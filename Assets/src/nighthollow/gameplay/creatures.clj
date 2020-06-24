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

(def berserker
  (inherit base/melee-creature
           {:name "Berserker"
            :creature-prefab "Creatures/User/Berserker"
            :owner :user
            :health 250
            :melee-skill :skill2
            :base-attack {:physical 50}}))

(def berserker-card
  (inherit base/card
           {:image "CreatureImages/Berserker"
            :cost {:mana 100 :influence {:flame 1}}
            :creature berserker}))

(def wizard
  (inherit base/projectile-creature
           {:name "Wizard"
            :creature-prefab "Creatures/User/Wizard"
            :owner :user
            :health 200
            :projectile-skill :skill2
            :base-attack {:physical 25}
            :default-projectile
            {:projectile-prefab "Projectiles/Projectile 1"
             :speed 10000
             :hitbox-size 1000}}))

(def wizard-card
  (inherit base/card
           {:image "CreatureImages/Wizard"
            :cost {:mana 100 :influence {:flame 1}}
            :creature wizard}))

(defn ^{:event :creature-played}
  gain-influence-on-play
  [{{influence :influence-gain-amount} :entity}]
  [{:effect :mutate-user, :gain-influence influence}])

(def acolyte
  (inherit base/melee-creature
           {:name "Acolyte"
            :creature-prefab "Creatures/User/Acolyte"
            :owner :user
            :health 100
            :melee-skill :skill2
            :mana-gain-interval 5
            :mana-gain-amount 10
            :influence-gain-amount {:flame 1}
            :base-attack {:physical 5}
            :rules [#'base/gain-mana-over-time
                    #'gain-influence-on-play]}))

(def acolyte-card
  (inherit base/card
           {:image "CreatureImages/Acolyte"
            :cost {:mana 50, :influence {}}
            :creature acolyte}))
