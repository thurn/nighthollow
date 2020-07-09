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

(ns nighthollow.prelude
  (:require
   [clojure.string :as string]
   [clojure.pprint]
   [clojure.tools.namespace.repl])
  (:import [UnityEngine Debug]))

(defmacro re-export [ns vlist]
  `(do ~@(for [i vlist]
           `(def ~i ~(symbol (str ns "/" i))))))

(defn lg [& args]
  (apply prn args)
  (Debug/Log (string/join " " args)))

(defn log-error [& args]
  (apply prn args)
  (Debug/LogError (string/join " " args)))

(defn error! [& args]
  (throw (Exception. (string/join " " args))))

(defn refresh []
  (clojure.tools.namespace.repl/refresh))

(defmacro doc [arg]
  `(clojure.repl/doc ~arg))

(defn pprint [& args]
  (apply clojure.pprint/pprint args))
