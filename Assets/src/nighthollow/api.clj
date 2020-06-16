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

(ns nighthollow.api
  (:require
   [clojure.spec.alpha :as s]
   [nighthollow.core :as core]
   [nighthollow.data :as data]
   [nighthollow.specs :as specs])) 

(s/fdef ->creature-id :args (s/cat :id int?))
(defn ->creature-id [id]
  (CreatureId. id))

(s/fdef ->card-id :args (s/cat :id int?))
(defn ->card-id [id] (CardId. id))

(s/fdef ->player-name :args (s/cat :name :d/player-name))
(defn ->player-name [name]
  (case name
    :user PlayerName/User
    :enemy PlayerName/Enemy))

(s/fdef ->rank-value :args (s/cat :rank :d/rank))
(defn ->rank-value [rank]
  (case :d/rank rank
    1 RankValue/Rank1
    2 RankValue/Rank2
    3 RankValue/Rank3
    4 RankValue/Rank4
    5 RankValue/Rank5
    6 RankValue/Rank6
    7 RankValue/Rank7
    8 RankValue/Rank8))

(s/fdef ->file-value :args (s/cat :file :d/file))
(defn ->file-value [file]
  (case file
    1 FileValue/File1
    2 FileValue/File2
    3 FileValue/File3
    4 FileValue/File4
    5 FileValue/File5))

(s/fdef ->school :args (s/cat :school :d/school))
(defn ->school [school]
  (case school
    :light  School/Light
    :sky    School/Sky
    :flame  School/Flame
    :ice    School/Ice
    :earth  School/Earth
    :shadow School/Shadow))

(s/fdef ->influence :args (s/cat :influence (s/tuple :d/school nat-int?)))
(defn ->influence [[school value]]
  (Influence. (->school school) value))

(s/fdef ->asset-type :args (s/cat :asset-type :d/asset-type))
(defn ->asset-type [asset-type]
  (case asset-type
    :prefab AssetType/Prefab
    :sprite AssetType/Sprite))

(s/fdef ->asset-data :args (s/cat :type :d/asset-type :address string?))
(defn ->asset-data [type address]
  (AssetData. address (->asset-type type)))

(s/fdef ->user-data :args (s/cat :user :d/user))
(defn ->user-data [{current-life :current-life
                    maximum-life :maximum-life
                    mana :mana
                    influence :influence}]
  (UserData. current-life maximum-life mana (mapv ->influence (seq influence))))

(s/fdef ->cost :args (s/cat :cst :d/cost))
(defn ->cost [{mana :mana, influence :influence}]
  (Cost. mana (mapv ->influence (seq influence))))

(s/fdef ->attachment-data :args (s/cat :address string?))
(defn ->attachment-data [address]
  (AttachmentData. (->asset-data address)))

(s/fdef ->creature-data :args (s/cat :creature :d/creature))
(defn ->creature-data [{creature-id :id
                        address :creature-prefab
                        owner :owner
                        speed :speed
                        attachments :attachments}]
  (CreatureData. (->creature-id creature-id)
                 (->asset-data :prefab address)
                 (->player-name owner)
                 (or speed 0)
                 (mapv ->attachment-data attachments)))

(s/fdef ->card-data :args (s/cat :card :d/card))
(defn ->card-data [{card-id :id
                    address :card-prefab
                    can-play :can-play
                    cost :cost
                    image-address :image
                    :as card}]
  (CardData. (->card-id card-id)
             (->asset-data :prefab address)
             can-play
             (->cost cost)
             (->asset-data :sprite image-address)
             (->creature-data card)))

(defn ->skill-animation-number [skill-animation-number]
  (case skill-animation-number
    :skill1 SkillAnimationNumber/Skill1
    :skill2 SkillAnimationNumber/Skill2
    :skill3 SkillAnimationNumber/Skill3
    :skill4 SkillAnimationNumber/Skill4
    :skill5 SkillAnimationNumber/Skill5))

(specs/instrument! *ns*)
