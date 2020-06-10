; Copyright © 2020-present Derek Thurn
;
; Licensed under the Apache License, Version 2.0 (the "License");
; you may not use this file except in compliance with the License.
; You may obtain a copy of the License at
;
; https://www.apache.org/licenses/LICENSE-2.0
;
; Unless required by applicable law or agreed to in writing, software
; distributed under the License is distributed on an "AS IS" BASIS,
; WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
; See the License for the specific language governing permissions and
; limitations under the License.

(ns nighthollow.core
  (:require [arcadia.core :refer :all]
            [arcadia.linear :as l])
  (:import [UnityEngine]))

(def root (atom (ensure-cmpt (object-named "Root") Nighthollow.Root)))

(defn child-typed [go t]
  (.GetComponentInChildren (.transform (gobj go)) t))

(defn prefab-asset [address]
  (Nighthollow.Asset. address "prefab"))

(def attachment (prefab-asset "Content/Attachment"))
(def card (prefab-asset "Content/Card"))
(def cursor (prefab-asset "Content/Cursor"))
(def debug-button (prefab-asset "Content/DebugButton"))
(def health-bar (prefab-asset "Content/HealthBar"))
(def influence (prefab-asset "Content/Influence"))
(def content-prefabs [attachment card cursor debug-button health-bar influence])

(defn create [prefab object-name type parent]
  (let [result (child+ (gobj parent) (instantiate (.GetPrefab @root prefab)))]
    (set! (.-name result) object-name)
    (cmpt result type)))

(defn create-debug-button [name on-click]
  (let [instance (create debug-button name
                         UnityEngine.UI.Button (.. @root DebugPanel))]
    (set! (.-text (child-typed instance UnityEngine.UI.Text)) name)
    (.AddListener (.-onClick instance) on-click)
    instance))

(defn show-debug-buttons []
  (doseq [c (children (.. @root DebugPanel gameObject))]
    (destroy c))
  (create-debug-button "Draw" #(log "on draw clicked"))
  (log "Added child"))

(defn main []
  (let [r (ensure-cmpt (object-named "Root") Nighthollow.Root)]
    (log "Running")
    (reset! root r)
    (.LoadAssets r
                 content-prefabs
                 (fn []
                   (show-debug-buttons)))))
