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

(ns nighthollow.specs
  (:require
   [clojure.spec.alpha :as s]
   [clojure.spec.test.alpha :as stest]
   [nighthollow.prelude :refer :all]))

(defn validate
  "Wrapper around clojure spec assert that adds a custom failure message"
  [message spec x]
  (if (s/valid? spec x)
    x
    (let [m (str "Error in: " message
                 "\nSpec " spec " does not match value " x
                 "\n<<<<<<<<<<\n")]
      (log-error m)
      (s/assert spec x))))

(defn instrument! [namespace]
  (when (s/check-asserts?)
    (stest/instrument (map #(symbol (name (ns-name namespace)) (name %))
                           (keys (ns-publics namespace)))))
  nil)

(s/def :d/player-name #{:user :enemy})
(s/def :d/rank #{1 2 3 4 5 6 7 8})
(s/def :d/file #{1 2 3 4 5})
(s/def :d/school #{:light :sky :flame :ice :earth :shadow})
(s/def :d/asset-type #{:prefab :sprite})

(s/def :d/life nat-int?)
(s/def :d/mana nat-int?)
(s/def :d/influence (s/map-of :d/school nat-int?))
(s/def :d/deck map?)
(s/def :d/hand (s/map-of :d/card-id :d/card))

(s/def :d/user (s/keys :req-un [:d/rules
                                :d/life
                                :d/mana
                                :d/influence
                                :d/deck
                                :d/hand]))

(s/def :d/owner :d/player-name)
(s/def :d/can-play boolean?)
(s/def :d/card-prefab string?)
(s/def :d/name string?)
(s/def :d/image string?)
(s/def :d/cost (s/keys :req-un [:d/mana, :d/influence]))
(s/def :d/card-id (s/tuple #{:card} int?))
(s/def :d/card (s/keys :req-un [:d/rules
                                :d/owner
                                :d/can-play
                                :d/card-prefab
                                :d/image
                                :d/cost
                                :d/creature]))
(s/def :d/cards (s/map-of :d/card-id :d/card))

(s/def :d/creature-prefab string?)
(s/def :d/health nat-int?)
(s/def :d/damage nat-int?)
(s/def :d/energy nat-int?)
(s/def :d/damage-type #{:radiant :lightning :fire :cold :physical :necrotic})
(s/def :d/damage-map (s/map-of :d/damage-type nat-int?))
(s/def :d/base-attack :d/damage-map)
(s/def :d/speed nat-int?)
(s/def :d/starting-energy nat-int?)
(s/def :d/maximum-energy nat-int?)
(s/def :d/energy-regeneration nat-int?)
(s/def :d/crit-chance nat-int?)
(s/def :d/crit-multiplier nat-int?)
(s/def :d/accuracy nat-int?)
(s/def :d/evasion nat-int?)
(s/def :d/damage-resistance :d/damage-map)
(s/def :d/damage-reduction :d/damage-map)
(s/def :d/creature-id (s/tuple #{:creature} int?))
(s/def :d/placed-time nat-int?)

(s/def :d/creature (s/keys :req-un [:d/rules
                                    :d/name
                                    :d/owner
                                    :d/creature-prefab
                                    :d/health
                                    :d/damage
                                    :d/energy
                                    :d/base-attack
                                    :d/speed
                                    :d/starting-energy
                                    :d/maximum-energy
                                    :d/energy-regeneration
                                    :d/crit-chance
                                    :d/crit-multiplier
                                    :d/accuracy
                                    :d/evasion
                                    :d/damage-resistance
                                    :d/damage-reduction]
                           :opt-un [:d/rank
                                    :d/file
                                    :d/placed-time]))

(s/def :d/skill-animation-number #{:skill1 :skill2 :skill3 :skill4 :skill5})
(s/def :d/skill-type #{:melee :ranged})

(defn spec-for-entity-id
  [[entity-type & _]]
  (case entity-type
    :user :d/user
    :card :d/card
    :creature :d/creature))

(s/def :d/creatures (s/map-of :d/creature-id :d/creature))
(s/def :d/game-id int?)
(s/def :d/game (s/keys :req-un [:d/tick
                                :d/game-id
                                :d/user
                                :d/creatures]))

(s/def :d/rules (s/coll-of var?))
(s/def :d/rules-map (s/map-of keyword? map?))
(s/def :d/events vector?)
(s/def :d/state (s/keys :req-un [:d/game
                                 :d/rules-map
                                 :d/events]))

(s/def :d/user-id (s/tuple #{:user}))
(s/def :d/entity-id (s/or :user :d/user-id
                          :creature :d/creature-id
                          :card :d/card-id))
(s/def :d/entities (s/coll-of :d/entity-id))
(s/def :d/hit-creature-id :d/creature-id)
(s/def :d/hit-creature-ids (s/coll-of :d/creature-id))
(s/def :d/has-melee-collision boolean?)
(s/def :d/melee-skill :d/skill-animation-number)
(s/def :d/tick nat-int?)

(def creature-mutation-keys)


(def user-mutation-keys)


(defmulti event-spec :event)
(defmethod event-spec :game-start [_]
  (s/keys :req-un [:d/entities]))
(defmethod event-spec :start-drawing-cards [_]
  (s/keys :req-un [:d/cards]))
(defmethod event-spec :card-drawn [_]
  (s/keys :req-un [:d/entities]))
(defmethod event-spec :card-played [_]
  (s/keys :req-un [:d/entities :d/rank :d/file]))
(defmethod event-spec :card-mutated [_]
  (s/keys :req-un [:d/entities]))
(defmethod event-spec :create-enemy [_]
  (s/keys :req-un [:d/creature-id :d/creature :d/file]))
(defmethod event-spec :creature-collision [_]
  (s/keys :req-un [:d/entities :d/hit-creature-id]))
(defmethod event-spec :ranged-skill-fire [_]
  (s/keys :req-un [:d/entities]))
(defmethod event-spec :melee-skill-impact [_]
  (s/keys :req-un [:d/entities :d/hit-creature-ids]))
(defmethod event-spec :projectile-impact [_]
  (s/keys :req-un [:d/entities :d/hit-creature-ids]))
(defmethod event-spec :skill-complete [_]
  (s/keys :req-un [:d/entities :d/has-melee-collision]))
(defmethod event-spec :use-skill [_]
  (s/keys :req-un [:d/creature-id :d/skill-animation-number :d/skill-type]))
(defmethod event-spec :creature-killed [_]
  (s/keys :req-un [:d/entities]))
(defmethod event-spec :tick [_]
  (s/keys :req-un [:d/tick]))
(defmethod event-spec :creature-mutated [_]
  (s/keys :req-un [:d/entities]
          :opt-un [:d/apply-damage
                   :d/heal-damage
                   :d/gain-energy
                   :d/spend-energy]))
(defmethod event-spec :user-mutated [_]
  (s/keys :req-un [:d/entities]
          :opt-un [:d/lose-life
                   :d/gain-life
                   :d/spend-mana
                   :d/lose-mana
                   :d/gain-mana
                   :d/lose-influence
                   :d/gain-influence]))

(s/def :d/event (s/multi-spec event-spec :event))

(s/def :d/quantity nat-int?)
(s/def :d/heal-damage nat-int?)
(s/def :d/apply-damage :d/damage-map)
(s/def :d/gain-energy nat-int?)
(s/def :d/spend-energy nat-int?)
(s/def :d/lose-life :d/life)
(s/def :d/gain-life :d/life)
(s/def :d/spend-mana :d/mana)
(s/def :d/lose-mana :d/mana)
(s/def :d/gain-mana :d/mana)
(s/def :d/lose-influence :d/influence)
(s/def :d/gain-influence :d/influence)

(defmulti effect-spec :effect)
(defmethod effect-spec :draw-cards [_]
  (s/keys :req-un [:d/quantity]))
(defmethod effect-spec :set-can-play-card [_]
  (s/keys :req-un [:d/can-play]))
(defmethod effect-spec :play-creature [_]
  (s/keys :req-un [:d/card-id
                   :d/creature-id
                   :d/creature
                   :d/rank
                   :d/file]))
(defmethod effect-spec :create-enemy [_]
  (s/keys :req-un [:d/creature-id :d/creature :d/file]))
(defmethod effect-spec :use-skill [_]
  (s/keys :req-un [:d/creature-id :d/skill-animation-number :d/skill-type]))
(defmethod effect-spec :mutate-creature [_]
  (s/keys :req-un [:d/creature-id]
          :opt-un [:d/apply-damage
                   :d/heal-damage
                   :d/gain-energy
                   :d/spend-energy]))
(defmethod effect-spec :mutate-user [_]
  (s/keys :opt-un [:d/lose-life
                   :d/gain-life
                   :d/spend-mana
                   :d/lose-mana
                   :d/gain-mana
                   :d/lose-influence
                   :d/gain-influence]))

(s/def :d/effect (s/multi-spec effect-spec :effect))
