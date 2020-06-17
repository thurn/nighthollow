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
   [clojure.test :as test]))

(defn add-standard-functions
  []
  (use 'clojure.repl)
  (use 'clojure.pprint)
  (require '[nighthollow.test :as t])
  (require '[nighthollow.data :as d])
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

(defn report [{type :type :as args}]
  (case type
    :begin-test-var (println "Running" (:var args))
    :fail (do
            (print "Failed! ")
            (when-let [file (:file args)] (print "File: " file " "))
            (when (> (:line args) 0) (print "Line: " (:line args) " "))
            (when-let [message (:message args)] (print message))
            (print "\n")
            (println "Expected: " (:expected args))
            (println "Actual: " (:actual args)))
    nil))

(defn !t []
  (binding [test/report report]
    (doseq [n (map ns-name (all-ns))]
      (when (string/starts-with? (name n) "nighthollow")
        (println "Testing" n)
        (test/run-tests n)))))

(defn !d [event]
  (nighthollow.core/dispatch! event))

(defn !r []
  (Commands/ResetState)
  (nighthollow.main/on-start-new-game))

(defn ?h []
  (keys (get-in @nighthollow.core/state [:game :user :hand])))

(defn ?ca [id]
  (keys (get-in @nighthollow.core/state [:game :user :hand [:card id]])))

(defn ?cs []
  (keys (get-in @nighthollow.core/state [:game :creatures])))

(defn ?cr [id]
  (get-in @nighthollow.core/state [:game :creatures [:creature id]]))
