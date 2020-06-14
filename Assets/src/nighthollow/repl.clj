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
   [clojure.repl :as repl]
   [clojure.test :as test]))

(defmacro add-standard-functions []
  '(do
     (use 'clojure.repl)
     (use 'clojure.pprint)
     (require '[nighthollow.test :as t])
     (require '[nighthollow.data :as d])
     (require '[nighthollow.core :as c])
     (require '[nighthollow.api :as a])
     (require '[nighthollow.main :as m])))

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

(defn run-tests []
  (binding [test/report report]
    (test/run-tests)))
