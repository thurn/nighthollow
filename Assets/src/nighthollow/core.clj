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

(ns nighthollow.core
  (:require
   [clojure.spec.alpha :as s]
   [nighthollow.prelude :refer :all]
   [nighthollow.markers :as markers]
   [nighthollow.specs :as specs]))

(defonce ^:private state (atom nil))

(defonce ^:private last-id-generated (atom 0))

(defn new-id
  "Returns a new unique integer ID"
  []
  (swap! last-id-generated inc))

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
  :rules-map vector, but in both cases they will not be 'seen' until the
  current batch of effects is done executing"
  (fn [state effect] (:effect effect)))

(defn- run-handle-effect
  [state effect]
  (s/assert :d/state (handle-effect (s/assert :d/state state)
                                    (s/assert :d/effect effect))))

(defmulti apply-event-commands!
  "Multimethod for applying mutations to the state of the game interface based
  on events raised. This is the final step of event resolution, and it is only
  invoked after all effect handlers have been recursively executed. The
  function will receive two parameters An event (see 'dispatch!'), and the
  current value of the :game key of the 'state' atom.

  The function should mutate the interface state appropriately for this event.
  Note that entities which appear in the :entities list of an event are
  automatically updated (see 'update-game-object!'), so updating them here is
  not required. This function is intended to support more complex use-cases
  like playing animations. Returns nil."
  (fn [event game] (:event event)))

(defn- run-apply-event-commands!
  [event game]
  (specs/validate "apply-event-commands! event" :d/event event)
  (specs/validate "apply-event-commands! game" :d/game game)
  (apply-event-commands! event game))

(defmethod apply-event-commands! :default [event game] nil)

(defmulti update-game-object!
  "Multimethod for updating GameObjects after effect resolution has mutated
  the state of the game. The function will receive two arguments: The
  'entity-id', used for dispatch, and an 'entity' in the game.

  The implementation should directly modify the Unity GameObject corresponding
  to 'entity' to reflect its new state. Returns nil."
  (fn [[entity-type & rest] entity] entity-type))

(defn- run-update-game-object!
  [entity-id entity]
  (update-game-object! (s/assert :d/entity-id entity-id)
                       (s/assert (specs/spec-for-entity-id entity-id) entity)))

(s/fdef rules-for-event :args (s/cat :event :d/event,
                                     :source-id :d/entity-id
                                     :entities (s/coll-of :d/entity-id)))
(defn- rules-for-event
  "Returns a sequence of rules which are interested in this event based on the
  trigger values they provided at registration time"
  [{event :event source-id :source entities :entities}]
  (let [rules (event (:rules-map @state))]
    (if (and (seq entities) (seq rules))
      (concat
       (rules [:global])
       (mapcat rules (map #(vector :entity %) entities))
       (rules [:source source-id]))
      ;; If the event has no ':entities', *all* rules are invoked
      (flatten (vals rules)))))

(s/fdef invoke-rule :args (s/cat :game :d/game,
                                 :event :d/event,
                                 :context map?))
(defn- invoke-rule
  "Given the current game state and the context for a rule, runs the rule,
  returning a sequence of the resulting effects or nil if the rule did not
  run."
  [game event {rule :rule entity-id :entity-id index :index}]
  (when-let [entity (find-entity entity-id game)]
    (specs/validate (str "rule entity argument\n" rule)
                    (specs/spec-for-entity-id entity-id) entity)
    (specs/validate (str "rule returned effects\n" rule)
                    (s/coll-of :d/effect)
                    (rule {:entity entity
                           :entity-id entity-id
                           :index index
                           :event event
                           :game game}))))

(s/fdef invoke-rules :args (s/cat :game :d/game, :event :d/event))
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

(s/fdef apply-effects! :args (s/cat :initial-event :d/event))
(defn- apply-effects!
  "Applies game state updates by running effect handlers. This process involves
  three steps:

  - Iterates through the current :events vector of the 'state' atom, invoking
    the appropriate rules for each event in turn.
  - Iterates through the effects returned by each rule and mutates
    'state' by applying those effects
  - If any new events were added by effect handlers, recursively repeats this
    process for each event added.

  The return value is a sequence of all events processed.

  TODO: Make this return a new state value instead of mutating in place."
  [initial-event]
  (loop [event initial-event all-events []]
    (if event
      (let [effects (invoke-rules (:game @state) event)]
        (doseq [effect effects]
          (markers/start (:effect effect))
          (swap! state run-handle-effect effect)
          (markers/stop (:effect effect)))
        (recur (pop-event!) (conj all-events event)))
      all-events)))

(s/fdef push-event :args (s/cat :state :d/state, :event :d/event))
(defn push-event
  "Helper to return a new state value with 'event' added to the :events list"
  [state event]
  (update state :events conj event))

(s/fdef dispatch! :args (s/cat :event :d/event))
(defn dispatch!
  "Raises a new game event. The argument should be an event specification map
  with an :event key mapping to a keyword identifying type of event that
  occurred. It can optionally also contain:

  - A :source key with the entity ID of the entity that raised this event
  - An :entities key containing a list of entity IDs affected by this event

  Any registered rules for this event will be invoked to trigger mutations to
  the state of the game. Returns nil."
  [event]
  (markers/start (:event event))

  (markers/start :apply-effects)
  (let [events (apply-effects! event)
        game (:game @state)]
    (markers/stop :apply-effects)

    (markers/start :apply-event-commands)
    (doseq [event events]
      (markers/start :event-command)
      (run-apply-event-commands! event game)
      (markers/stop :event-command))
    (markers/stop :apply-event-commands)
    
    (markers/start :update-game-objects)
    (doseq [entity-id (distinct (mapcat :entities events))]
      (when-let [entity (find-entity entity-id game)]
        (run-update-game-object! entity-id entity)))
    (markers/stop :update-game-objects))

  (markers/stop (:event event)))

(s/fdef register-rule :args (s/cat :entity-id :d/entity-id
                                   :state :d/state
                                   :rule (s/tuple nat-int? var?)))
(defn- register-rule
  "Returns a new state with a single [index rule-function] tuple associated with
  the given entity id"
  [entity-id state [index function]]
  (let [{trigger :trigger, event :event} (meta function)
        rule-key (if (= :global trigger)
                   [:global]
                   [(or trigger :entity) entity-id])
        context {:rule @function, :entity-id entity-id, :index index}]
    (update-in state [:rules-map event rule-key] conj context)))

(s/fdef register-rules :args (s/cat :state :d/state
                                    :entity-id :d/entity-id
                                    :rules-map (s/coll-of var?)))
(defn register-rules
  "Registers rules for an entity, returning a new 'state' value with the rules
  added. Rules are pure functions which are invoked when some action
  occurs within the game. The 'rules' parameter is expected to contain a vector
  of clojure vars mapping to rule functions with associated rule metadata.

  Rule functions will be invoked with a rule context map containing the
  following keys:

  - :entity, the entity owning this rule (as indicated by 'entity-id' here)
  - :entity-id, the value passed as 'entity-id' to this function
  - :index, the index of the rule within its parent entity
  - :event, the event in question (see 'dispatch' above for structure)
  - :game, containing the current value of the :game key of the 'state' atom

  The rule should return a sequence of Effects (see 'handle-effect' for
  structure) to mutate the state of the game in some way.

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

(defn on-tick!
  "Updates the global game timer and dispatches the :tick event."
  []
  (when @state
    (let [updated (swap! state update-in [:game :tick] inc)]
      (dispatch! {:event :tick, :tick (get-in updated [:game :tick])}))))

(s/fdef start-game! :args (s/cat :game :d/game
                                 :entity-id some?
                                 :rules-path (s/coll-of keyword?)))
(defn start-game!
  "Initiaties a new game by resetting the game state to the value provided in
  'game'. Registers initial game rules associated with the entity with id
  'entity-id' by looking for the vector at the provided 'rules-path', which
  should follow the navigation syntax of the core 'get-in' function. Returns
  nil."
  [game]
  (reset! state (-> {:game game :events [] :rules-map {}}
                    (register-rules [:user] (get-in game [:user :rules]))
                    (register-rules [:enemy] (get-in game [:enemy :rules]))))
  nil)

(defn end-game!
  "Ends the current game, clearing out the value of 'state'."
  []
  (reset! state nil)
  (reset! last-id-generated 0))

(specs/instrument! *ns*)
