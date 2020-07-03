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

(ns nighthollow.testing
  (:require
   [clojure.string :as string]
   [clojure.test :as test]
   [nighthollow.prelude :refer :all]))

(defn report [{type :type :as args}]
  (case type
    :begin-test-var (println "Running" (:var args))
    :fail (do
            (test/inc-report-counter :fail)
            (log-error "Failed! ")
            (when-let [file (:file args)] (log-error "File: " file " "))
            (when (> (:line args) 0) (log-error "Line: " (:line args) " "))
            (when-let [message (:message args)] (log-error message))
            (log-error "Expected: " (:expected args))
            (log-error "Actual: " (:actual args)))
    :error (do
             (test/inc-report-counter :error)
             (log-error "Exception! ")
             (when-let [file (:file args)] (log-error "File: " file " "))
             (when (> (:line args) 0) (log-error "Line: " (:line args) " "))
             (log-error "Expected: " (:expected args))
             (log-error "Actual: " (:actual args)))
     :pass (test/inc-report-counter :pass)
     nil))

(defn run-with-count
  "Runs tests for a namespace, counting errors"
  [error-count namespace]
  (let [result (test/run-tests namespace)]
    (+ error-count
       (:error result)
       (:fail result))))

(defn run-all-tests
  "Runs all tests which can be discovered in the project. Returns a count of
  failures or errors."
  []
  (lg "Running All Tests")
  (let [error-count (atom 0)]
    (binding [test/report report]
      (doseq [n (map ns-name (all-ns))]
        (when (string/starts-with? (name n) "nighthollow")
          (lg "Testing" n)
          (swap! error-count run-with-count n))))
    @error-count))

(defn run-tests
  "Runs tests for the current namespace"
  []
  (binding [test/report report]
    (test/run-tests)))
