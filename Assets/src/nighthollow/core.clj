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

(ns nighthollow.core)

(defonce ^:private state (atom {:game nil, :rules {}, :events []}))

(defonce ^:private root (atom {}))

(defmulti find-entity
  "Multimethod for locating entities within the game. Implementations should
  accept two parameters: the entity ID (a vector whose first element must be
  an entity type keyword used for dispatch) and the current game state. They
  should return the corresponding entity or nil if no such entity exists."
  (fn [[entity-type _] game] entity-type))

(defmulti handle-effect
  "Multimethod for implementing effect handlers. Effect handlers are functions
  which return a new state for the game. Each implementation will receive two
  parameters: the current value of the 'state' atom, and an 'effect', which
  describes the mutation to perform. The 'effect' will contain:

  - An :effect key which identifes the effect and is used for dispatch
  - The :entity-id of the rule which created the effect
  - The :index of the rule which created the effect
  - Any additional keys needed to provide effect context

  The handler should return a new value for the state atom. New events to
  handle can be added to the :events vector and new rules can be added to the
  :rules vector, but in both cases they will not be 'seen' until the
  current batch of effects is done executing"
  (fn [state effect] (:effect effect)))

(defmulti apply-effect-commands!
  "Multimethod for applying effects to mutate the state of the game interface.
  This is the final step of event resolution, and it is only invoked after all
  effect handlers have been recursively executed. The function will receive
  three parameters:

  - An effect as described above (see 'handle-effect').
  - The current value of the :game key of the 'state' atom
  - The current value of the 'root' atom

  The function should return a set of entity IDs which were modified by this
  effect. Those entities' game objects will be updated automatically to their
  new state. Additionally, for more complex use-cases like playing animations,
  implementations are free to directly modify game objects themselves."
  (fn [effect game root] (:effect effect)))

(defmethod apply-effect-commands! :default [effect state root] #{})

(defmulti update-game-object!
  "Multimethod for updating GameObjects after effect resolution has mutated
  the state of the game. The function will receive 3 arguments:

  - An 'entity-type', used for dispatch
  - An 'entity' in the game
  - The current value of the 'root' atom

  The implementation should directly modify the Unity GameObject corresponding
  to 'entity' to reflect its new state. No return value is expected."
  (fn [entity-type entity root] entity-type))

(defn- rules-for-event
  "Returns a sequence of rules which are interested in this event based on the
  trigger values they provided at registration time"
  [{event :event source :source entities :entities}]
  (let [rules (event (:rules @state))]
    (if entities
      (concat
       (rules [:global])
       (mapcat rules (map #(vector :entity %) (:entities event)))
       (rules [:source (:source event)]))
      ;; If the event has no ':entities', *all* rules are invoked
      (flatten (vals rules)))))

(defn- invoke-rule
  "Given the current game state and the context for a rule, runs the rule,
  returning a sequence of the resulting effects or nil if the rule did not
  run."
  [game event {rule :rule entity-id :entity-id index :index}]
  (if-let [entity (find-entity entity-id game)]
    (rule entity event game index)))

(defn- invoke-rules
  "Given the current game state and an event, invokes all rule functions which
  are interested in this event (based on their :trigger values supplied at
  registration). Returns a sequence of the resulting effects."
  [game event]
  (mapcat #(invoke-rule game event %) (rules-for-event event)))

(defn- pop-event!
  "Mutably removes and returns the first value of the :events vector inside
  'state', or nil if there is no such value."
  []
  (let [[event rest] (split-at 1 (:events @state))]
    (swap! state update :events (constantly (vec rest)))
    (first event)))

(defn- apply-effects!
  "Applies game state updates by running effect handlers. This process involves
  three steps:

  - Iterates through the current :events vector of the 'state' atom, invoking
    the appropriate rules for each event in turn.
  - Iterates through the effects returned by each rule and mutates
    'state' by applying those effects
  - If any new events were added by effect handlers, recursively repeats this
    process for each event added.

  The return value is a sequence of all effects produced."
  [initial-event]
  (loop [event initial-event all-effects []]
    (if event
      (let [effects (invoke-rules (:game @state) event)]
        (doseq [effect effects]
          (swap! state handle-effect effect))
        (recur (pop-event!) (concat all-effects effects)))
      all-effects)))

(defn dispatch!
  "Raises a new game event. The argument should be an event specification map
  with an :event key mapping to a keyword identifying type of event that
  occurred. It can optionally also contain:

  - A :source key with the entity ID of the entity that raised this event
  - An :entities key containing a list of entity IDs affected by this event

  Any registered rules for this event will be invoked to trigger mutations to
  the state of the game. Returns nil."
  [event]
  (let [effects (apply-effects! event)
        game (:game @state)
        apply-commands! (fn [effect]
                          (apply-effect-commands! effect game @root))
        updated-ids (doall (mapcat apply-commands! effects))]
    (doseq [entity-id updated-ids]
      (when-let [entity (find-entity entity-id game)]
        (update-game-object! entity-id entity @root)))))

(defn- register-rule
  "Returns a new state with a single [index rule-function] tuple associated with
  the given entity id"
  [entity-id state [index function]]
  (let [{trigger :trigger, event :event} (meta function)
        rule-key (if (= :global trigger)
                   [:global]
                   [(or trigger :entity) entity-id])
        context {:rule @function, :entity-id entity-id, :index index}]
    (update-in state [:rules event rule-key] conj context)))

(defn register-rules
  "Registers rules for an entity, returning a new 'state' value with the rules
  added. Rules are pure functions which are invoked when some action
  occurs within the game. The 'rules' parameter is expected to contain a vector
  of clojure vars mapping to rule functions with associated rule metadata.

  Rule functions will be invoked with four parameters:

  1) The current value of the entity owning this rule (as indicated by
     the 'entity-id' parameter)
  2) The event in question (see 'dispatch' above for structure)
  3) The current value of the :game key of the 'state' atom
  4) The index of the rule within its parent entity

  The rule should return a sequence of Effects (see 'handle-effect' below
  for structure) to mutate the state of the game in some way.

  Rule vars should have attached metadata which indicates when the rule
  function should be invoked. All rules must have an :event metadata key
  indicating the event name for which this rule should be invoked.

  Rules can also optionally have a :trigger metadata key, which indicates when
  this rule should be invoked. Note that triggers are only considered for
  events which have an associated :entites list. Events with no entities are
  called 'global events' and always run all rules. Possible trigger values are:

  - :entity, meaning it should run when its entity appears in the :entities
    list of the event. This is the default behavior.
  - :global, indicating the rule should run every time the event fires
  - :source, meaning it should run when its entity appears in the :source
    list of the event

  Rules currently cannot be unregistered, but rules for entities which no
  longer exist will not be invoked."
  [state entity-id rules]
  (let [indices (map-indexed vector rules)]
    (reduce (partial register-rule entity-id) state indices)))

(defn start-game!
  "Initiaties a new game by resetting the game state to the value provided in
  'game'. Registers initial game rules associated with the entity with id
  'entity-id' by looking for the vector at the provided 'rules-path', which
  should follow the navigation syntax of the core 'get-in' function. Returns
  nil."
  [game entity-id rules-path]
  (reset! state (register-rules {:game game :events [] :rules {}}
                                entity-id
                                (get-in game rules-path)))
  nil)
