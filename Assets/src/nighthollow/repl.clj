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

(ns nighthollow.repl
  (:require
   [clojure.pprint :as pprint]
   [clojure.repl]
   [clojure.string :as string]
   [nighthollow.prelude :refer :all]
   [nighthollow.test :as t]))

(defn add-standard-functions
  []
  (use 'clojure.repl)
  (use 'clojure.pprint)
  (require '[nighthollow.test :as t])
  (require '[nighthollow.core :as c])
  (require '[nighthollow.api :as a])
  (require '[nighthollow.main :as m])
  (use '[nighthollow.repl]))

(defn populate []
  (doseq [n (map ns-name (all-ns))]
    (when (string/starts-with? (name n) "nighthollow")
      (println "Adding to" n)
      (in-ns n)
      (add-standard-functions))))

(defn !d [event]
  (nighthollow.core/dispatch! event))

(defn !r []
  (require '[nighthollow.gameplay.base :reload-all true])
  (require '[nighthollow.gameplay.creatures :reload-all true])
  (require '[nighthollow.gameplay.enemies :reload-all true])
  (require '[nighthollow.test :reload-all true])
  (Commands/ResetState)
  (nighthollow.main/on-start-new-game))

(defn ?s []
  @nighthollow.core/state)

(defn ?h []
  (keys (get-in @nighthollow.core/state [:game :user :hand])))

(defn ?ca [id]
  (keys (get-in @nighthollow.core/state [:game :user :hand [:card id]])))

(defn ?cs []
  (keys (get-in @nighthollow.core/state [:game :creatures])))

(defn ?cr [id]
  (get-in @nighthollow.core/state [:game :creatures [:creature id]]))

(defn !e []
  (!d {:event :create-enemy
       :creature-id [:creature 17]
       :creature t/enemy, :file 3}))

(defn !clean []
  (map #(ns-unmap *ns* %) (keys (ns-interns *ns*))))

(defn ?rk []
  (keys (:rules-map @nighthollow.core/state)))

(defn ?rules [key]
  (key (:rules-map @nighthollow.core/state)))

(defn !$ []
  (swap! nighthollow.core/state update-in [:game :user] assoc
         :mana 999 :influence {:flame 5}))
