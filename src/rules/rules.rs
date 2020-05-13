// Copyright © 2020-present Derek Thurn

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

//    https://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

//! Rules Engine Structure:
//! The basic structure of the rules engine is as follows:
//!
//! An "Action" is a change to the state of the game. User input like "play
//! this card" is represented as an Action, and so are consequences of user
//! input, such as "reduce the player's available mana" or "damage this
//! creature".
//!
//! A "Rule" is a pure function that operates over Actions. Each entity in the
//! game (cards, creatures, players, etc) can have one or more Rules associated
//! with it.
//!
//! For each Action, the Rules in the game are evaluated. As output, Rules
//! produce zero or more new Actions to execute in response to the input action.
//! Once the current Action has been evaluated for every Rule, the new Actions
//! are evaluated in sequence in the same manner.
//!
//! Once an evaluation step is completed and no new Actions are produced, the
//! simulation proceeds to the "execution" step. Execution applies each Action
//! in sequence to mutate the current game state. Some Actions, called
//! "ephemeral Actions", have no direct effect on game state, since they exist
//! only to be consumed by future rules. These Actions are skipped.
