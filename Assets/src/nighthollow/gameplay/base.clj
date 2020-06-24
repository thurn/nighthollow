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

(ns nighthollow.gameplay.base
  (:require
   [clojure.test :refer [deftest is]]
   [nighthollow.prelude :refer :all]
   [nighthollow.specs :as specs]))

(defn inherit [parent child]
  (merge parent
         (assoc child :rules (concat (:rules parent) (:rules child)))))

(defn- influence<=
  [cost current]
  (every? (fn [[school value]] (<= value (get current school 0))) cost))

(defn- calculate-can-play-card
  [game card-id {{influence :influence, mana :mana} :cost}]
  [{:effect :set-can-play-card
    :card-id card-id
    :can-play (and (<= mana (get-in game [:user :mana]))
                   (influence<= influence (get-in game [:user :influence])))}])

(defn ^{:event :card-drawn}
  drawn-can-play-card
  [{game :game, card-id :entity-id, card :entity}]
  (calculate-can-play-card game card-id card))

(defn ^{:event :user-mutated, :trigger :global}
  mutated-can-play-card
  [{game :game, card-id :entity-id, card :entity}]
  (calculate-can-play-card game card-id card))

(defn ^{:event :card-played}
  default-played-card
  [{[_ id :as card-id] :entity-id
    {rank :rank, file :file} :event
    {creature :creature, {mana :mana} :cost} :entity}]
  [{:effect :mutate-user, :spend-mana mana}
   {:effect :play-creature
    :card-id card-id
    :creature-id [:creature id]
    :creature creature
    :rank rank
    :file file}])

(def card
  {:owner :user
   :can-play false
   :card-prefab "Content/Card"
   :rules [#'drawn-can-play-card
           #'mutated-can-play-card
           #'default-played-card]})

(def creature
  {:damage 0
   :energy 0
   :base-attack {}
   :speed 0
   :health-regeneration 0
   :starting-energy 0
   :maximum-energy 0
   :energy-regeneration 0
   :crit-chance 0
   :crit-multiplier 0
   :accuracy 0
   :evasion 0
   :damage-resistance {}
   :damage-reduction {}})

(defn default-melee-skill-effect
  [creature-id creature]
  {:effect :use-skill
   :creature-id creature-id
   :skill-animation-number (specs/grab creature :d/melee-skill)
   :skill-type :melee})

(defn ^{:event :creature-collision}
  melee-creature-collision
  [{creature-id :entity-id, creature :entity}]
  (if (:current-skill creature)
    []
    [(default-melee-skill-effect creature-id creature)]))

(defn ^{:event :melee-skill-impact}
  default-melee-skill-impact
  [{{base-attack :base-attack} :entity
    {hit-creature-ids :hit-creature-ids} :event}]
  (mapv (fn [creature-id] {:effect :mutate-creature
                           :creature-id creature-id
                           :apply-damage base-attack})
        hit-creature-ids))

(defn ^{:event :skill-complete}
  default-melee-skill-complete
  [{creature-id :entity-id
    creature :entity
    {has-melee-collision :has-melee-collision} :event}]
  (if has-melee-collision
    [(default-melee-skill-effect creature-id creature)]
    [{:effect :clear-current-skill, :creature-id creature-id}]))

(def melee-creature
  (inherit creature
           {:rules (concat (:rules creature)
                           [#'melee-creature-collision
                            #'default-melee-skill-impact
                            #'default-melee-skill-complete])}))

(defn use-projectile-skill-effect
  [creature-id creature]
  {:effect :use-skill
      :creature-id creature-id
      :skill-animation-number (specs/grab creature :d/projectile-skill)
      :skill-type :ranged})

(defn ^{:event :enemy-appeared, :trigger :global}
  projectile-skill-on-appeared
  [{creature-id :entity-id, creature :entity, {enemy :creature} :event}]
  (if (or (:current-skill creature)
          (not= (specs/grab creature :d/file) (specs/grab enemy :d/file)))
    []
    [(use-projectile-skill-effect creature-id creature)]))

(defn fire-projectile-effect
  [creature-id creature]
  {:effect :fire-projectile
    :projectile (assoc (specs/grab creature :d/default-projectile)
                       :fired-by creature-id
                       :owner (:owner creature))})

(defn ^{:event :ranged-skill-fire}
  default-ranged-skill-fire
  [{creature-id :entity-id, creature :entity}]
  [(fire-projectile-effect creature-id creature)])

(defn ^{:event :projectile-impact}
  default-projectile-skill-impact
  [{{base-attack :base-attack} :entity
    {hit-creature-ids :hit-creature-ids} :event}]
  (mapv (fn [creature-id] {:effect :mutate-creature
                           :creature-id creature-id
                           :apply-damage base-attack})
        hit-creature-ids))

(defn has-creatures-on-file
  [game file]
  true)

(defn ^{:event :skill-complete}
  default-projectile-skill-complete
  [{creature-id :entity-id, creature :entity, game :game}]
  (if (has-creatures-on-file game (specs/grab creature :d/file))
    [(use-projectile-skill-effect creature-id creature)]
    [{:effect :clear-current-skill, :creature-id creature-id}]))

(def projectile-creature
  (inherit creature
           {:rules (concat (:rules creature)
                           [#'projectile-skill-on-appeared
                            #'default-ranged-skill-fire
                            #'default-projectile-skill-impact
                            #'default-projectile-skill-complete])}))

(defn ^{:event :game-start}
  draw-opening-hand
  [_]
  [{:effect :draw-cards :quantity 5}])

(defn ^{:event :create-enemy}
  create-enemy
  [{{creature-id :creature-id, creature :creature, file :file} :event}]
  [{:effect :create-enemy
    :creature-id creature-id
    :creature creature
    :file file}])

(defn ^{:event :tick}
  gain-mana-over-time
  [{entity :entity, {tick :tick} :event}]
  (if (= 0 (mod tick (:mana-gain-interval entity)))
    [{:effect :mutate-user :gain-mana (:mana-gain-amount entity)}]
    []))

(defn ^{:event :tick}
  draw-cards-over-time
  [{user :entity {tick :tick} :event}]
  (if (and (= 0 (mod tick (:card-draw-interval user)))
           (<= (count (:hand user)) 10))
    [{:effect :draw-cards :quantity 1}]
    []))

(def user
  {:life 10
   :mana 250
   :placed-time 0
   :mana-gain-interval 5
   :mana-gain-amount 10
   :card-draw-interval 25
   :influence {}
   :hand {}
   :rules [#'draw-opening-hand
           #'create-enemy
           #'gain-mana-over-time
           #'draw-cards-over-time]})

(deftest test-influence<=
  (is (influence<= {:flame 1} {:flame 2}))
  (is (influence<= {:flame 2} {:flame 2}))
  (is (not (influence<= {:flame 3} {:flame 2})))
  (is (not (influence<= {:flame 3, :light 1} {:flame 2})))
  (is (influence<= {:flame 2} {:flame 2, :light 1}))
  (is (influence<= {:flame 2, :light 1} {:flame 2, :light 1}))
  (is (not (influence<= {:flame 2, :light 2} {:flame 2, :light 1}))))
