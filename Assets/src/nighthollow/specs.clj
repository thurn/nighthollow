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
   [arcadia.core :as arcadia]
   [clojure.spec.alpha :as s]
   [clojure.spec.test.alpha :as stest]))

(defn validate
  "Wrapper around clojure spec assert that adds a custom failure message"
  [message spec x]
  (if (s/valid? spec x)
    x
    (let [m (str "\n>>>>>>>>>>\n"
                 "Error in " message
                 "\nSpec " spec " does not match value " x
                 "\n<<<<<<<<<<\n")]
      (arcadia/log m)
      (println m)
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

(s/def :d/current-life nat-int?)
(s/def :d/maximum-life nat-int?)
(s/def :d/mana nat-int?)
(s/def :d/influence (s/map-of :d/school nat-int?))
(s/def :d/deck vector?)
(s/def :d/hand (s/map-of :d/card-id :d/card))

(s/def :d/user (s/keys :req-un [:d/rules
                                :d/current-life
                                :d/maximum-life
                                :d/mana
                                :d/influence
                                :d/deck
                                :d/hand]))

(s/def :id/id int?)
(s/def :d/owner :d/player-name)
(s/def :d/can-play boolean?)
(s/def :d/card-prefab string?)
(s/def :d/name string?)
(s/def :d/image string?)
(s/def :d/cost (s/keys :req-un [:d/mana, :d/influence]))
(s/def :d/card-id (s/tuple #{:card} int?))

(s/def :d/card (s/keys :req-un [:d/rules
                                :d/id
                                :d/owner
                                :d/can-play
                                :d/card-prefab
                                :d/name
                                :d/image
                                :d/cost]))
(s/def :d/cards (s/coll-of :d/card))

(s/def :d/creature-prefab string?)
(s/def :d/health nat-int?)
(s/def :d/damage-type #{:radiant :lightning :fire :cold :physical :necrotic})
(s/def :d/base-damage (s/map-of :d/damage-type nat-int?))
(s/def :d/speed nat-int?)
(s/def :d/starting-energy nat-int?)
(s/def :d/maximum-energy nat-int?)
(s/def :d/energy-regeneration nat-int?)
(s/def :d/crit-chance nat-int?)
(s/def :d/crit-multiplier nat-int?)
(s/def :d/accuracy nat-int?)
(s/def :d/evasion nat-int?)
(s/def :d/damage-resistance (s/map-of :d/damage-type nat-int?))
(s/def :d/damage-reduction (s/map-of :d/damage-type nat-int?))
(s/def :d/creature-id (s/tuple #{:creature} int?))

(s/def :d/creature (s/keys :req-un [:d/owner
                                    :d/creature-prefab
                                    :d/health
                                    :d/base-damage
                                    :d/speed
                                    :d/starting-energy
                                    :d/maximum-energy
                                    :d/energy-regeneration
                                    :d/crit-chance
                                    :d/crit-multiplier
                                    :d/accuracy
                                    :d/evasion
                                    :d/damage-resistance
                                    :d/damage-reduction]))

(s/def :d/skill-animation-number #{:skill1 :skill2 :skill3 :skill4 :skill5})

(defn spec-for-entity-id
  [[entity-type & _]]
  (case entity-type
    :user :d/user
    :card :d/card))

(s/def :d/creatures (s/map-of :d/creature-id :d/creature))
(s/def :d/game-id int?)
(s/def :d/game (s/keys :req-un [:d/game-id
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

(defmulti event-spec :event)
(defmethod event-spec :game-start [_]
  (s/keys))
(defmethod event-spec :start-drawing-cards [_]
  (s/keys :req-un [:d/cards]))
(defmethod event-spec :card-drawn [_]
  (s/keys :req-un [:d/entities]))
(defmethod event-spec :card-played [_]
  (s/keys :req-un [:d/entities :d/rank :d/file]))
(defmethod event-spec :create-enemy [_]
  (s/keys :req-un [:d/creature-id :d/creature :d/file]))
(s/def :d/event (s/multi-spec event-spec :event))

(s/def :d/quantity nat-int?)

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
(s/def :d/effect (s/multi-spec effect-spec :effect))
