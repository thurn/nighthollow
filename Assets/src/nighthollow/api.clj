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
   [nighthollow.core :as core]
   [nighthollow.data :as data]))

(defn ->creature-id [id] (CreatureId. id))

(defn ->card-id [id] (CardId. id))

(defn ->player-name [name]
  (case name
    :user PlayerName/User
    :enemy PlayerName/Enemy))

(defn ->rank-value [rank]
  (case rank
    1 RankValue/Rank1
    2 RankValue/Rank2
    3 RankValue/Rank3
    4 RankValue/Rank4
    5 RankValue/Rank5
    6 RankValue/Rank6
    7 RankValue/Rank7
    8 RankValue/Rank8))

(defn ->file-value [file]
  (case file
    1 FileValue/File1
    2 FileValue/File2
    3 FileValue/File3
    4 FileValue/File4
    5 FileValue/File5))

(defn ->school [school]
  (case school
    :light  School/Light
    :sky    School/Sky
    :flame  School/Flame
    :ice    School/Ice
    :earth  School/Earth
    :shadow School/Shadow))

(defn ->influence [[school value]]
  (Influence. (->school school) value))

(defn ->asset-type [asset-type]
  (case asset-type
    :prefab AssetType/Prefab
    :sprite AssetType/Sprite))

(defn ->asset-data [type address]
  (AssetData. address (->asset-type type)))

(defn ->user-data [{current-life :current-life
                    maximum-life :maximum-life
                    mana :mana
                    influence :influence}]
  (UserData. current-life maximum-life mana (mapv ->influence (seq influence))))

(defn ->cost [{mana :mana, influence :influence}]
  (Cost. mana (mapv ->influence (seq influence))))

(defn ->attachment-data [address]
  (AttachmentData. (->asset-data address)))

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
