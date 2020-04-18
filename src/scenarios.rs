use crate::{
    card::{Card, Cost},
    primitives::{ManaValue, Result, School},
    state::{GamePhase, InterfaceError, InterfaceOptions, InterfaceState, PlayerState},
};

pub fn load_scenario(state: &mut InterfaceState, name: String) -> Result<()> {
    state.reset();
    match name.as_str() {
        "empty" => Ok(()),
        "opening" => Ok(opening_hands(state)),
        _ => InterfaceError::result(format!("Unknown scenario {}", name)),
    }
}

fn opening_hands(state: &mut InterfaceState) {
    state.update(InterfaceState {
        options: InterfaceOptions { auto_advance: true },
        phase: GamePhase::Main,
        player: PlayerState {
            mana: 0,
            hand: vec![
                Card::new_unit(
                    "Demon Wolf",
                    Cost::mana_cost(School::Flame, ManaValue::new(2), 1),
                    100,
                    10,
                ),
                Card::new_unit(
                    "Cyclops",
                    Cost::mana_cost(School::Flame, ManaValue::new(4), 2),
                    200,
                    10,
                ),
                Card::new_unit(
                    "Metalon",
                    Cost::mana_cost(School::Flame, ManaValue::new(3), 1),
                    250,
                    10,
                ),
            ],
            reserve: vec![],
            defenders: vec![],
            attackers: vec![],
        },
        enemy: PlayerState {
            mana: 0,
            hand: vec![
                Card::new_unit(
                    "Demon Wolf",
                    Cost::mana_cost(School::Flame, ManaValue::new(1), 1),
                    100,
                    10,
                ),
                Card::new_unit(
                    "Cyclops",
                    Cost::mana_cost(School::Flame, ManaValue::new(3), 1),
                    200,
                    10,
                ),
                Card::new_unit(
                    "Metalon",
                    Cost::mana_cost(School::Flame, ManaValue::new(2), 1),
                    250,
                    10,
                ),
            ],
            reserve: vec![],
            defenders: vec![],
            attackers: vec![],
        },
    });
}
