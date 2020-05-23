// Generated Code. Do not edit!

#[derive(Clone, PartialEq, ::prost::Message)]
pub struct UserId {
    #[prost(int32, tag = "1")]
    pub value: i32,
}
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct GameId {
    #[prost(int32, tag = "1")]
    pub value: i32,
}
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct CreatureId {
    #[prost(int32, tag = "1")]
    pub value: i32,
}
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct CardId {
    #[prost(int32, tag = "1")]
    pub value: i32,
}
/// Requests to start a new game with a new game ID. The client should discard
/// all previous state when sending this request.
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct StartGameRequest {}
/// Requests to load the current state of a game. The client should discard
/// all previous state when sending this request.
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct ConnectToGameRequest {
    #[prost(message, optional, tag = "1")]
    pub game_id: ::std::option::Option<GameId>,
}
/// Advance the game to the next phase
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct AdvancePhaseRequest {
    #[prost(message, optional, tag = "1")]
    pub game_id: ::std::option::Option<GameId>,
}
/// Play a creature card at a given position
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct PlayCreatureCard {
    #[prost(enumeration = "RankValue", tag = "1")]
    pub rank_position: i32,
    #[prost(enumeration = "FileValue", tag = "2")]
    pub file_position: i32,
}
/// Play an attachment card on a given creature
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct PlayAttachmentCard {
    #[prost(message, optional, tag = "1")]
    pub creature_id: ::std::option::Option<CreatureId>,
}
/// Play a card which does not require targeting
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct PlayUntargetedCard {}
/// Play a card from the user's hand
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct PlayCardRequest {
    #[prost(message, optional, tag = "1")]
    pub game_id: ::std::option::Option<GameId>,
    #[prost(message, optional, tag = "2")]
    pub card_id: ::std::option::Option<CardId>,
    #[prost(oneof = "play_card_request::PlayCard", tags = "3, 4, 5")]
    pub play_card: ::std::option::Option<play_card_request::PlayCard>,
}
pub mod play_card_request {
    #[derive(Clone, PartialEq, ::prost::Oneof)]
    pub enum PlayCard {
        #[prost(message, tag = "3")]
        PlayCreature(super::PlayCreatureCard),
        #[prost(message, tag = "4")]
        PlayAttachment(super::PlayAttachmentCard),
        #[prost(message, tag = "5")]
        PlayUntargeted(super::PlayUntargetedCard),
    }
}
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct CreaturePositionUpdate {
    #[prost(message, optional, tag = "1")]
    pub game_id: ::std::option::Option<GameId>,
    #[prost(message, optional, tag = "2")]
    pub creature_id: ::std::option::Option<CreatureId>,
    #[prost(enumeration = "RankValue", tag = "3")]
    pub rank_position: i32,
    #[prost(enumeration = "FileValue", tag = "4")]
    pub file_position: i32,
}
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct RepositionCreaturesRequest {
    #[prost(message, optional, tag = "1")]
    pub game_id: ::std::option::Option<GameId>,
    #[prost(message, repeated, tag = "2")]
    pub position_updates: ::std::vec::Vec<CreaturePositionUpdate>,
}
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct RunConsoleCommandRequest {
    #[prost(string, tag = "1")]
    pub command: std::string::String,
    #[prost(message, optional, tag = "2")]
    pub game_id: ::std::option::Option<GameId>,
    #[prost(enumeration = "PlayerName", tag = "3")]
    pub player: i32,
    #[prost(message, optional, tag = "4")]
    pub creature_id: ::std::option::Option<CreatureId>,
    #[prost(message, optional, tag = "5")]
    pub card_id: ::std::option::Option<CardId>,
}
/// Data sent to the server whenever the user does something in the game's user
/// interface
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct Request {
    /// Identifies the user making this request
    #[prost(message, optional, tag = "1")]
    pub user_id: ::std::option::Option<UserId>,
    #[prost(oneof = "request::Request", tags = "2, 3, 4, 5, 6, 7")]
    pub request: ::std::option::Option<request::Request>,
}
pub mod request {
    #[derive(Clone, PartialEq, ::prost::Oneof)]
    pub enum Request {
        #[prost(message, tag = "2")]
        StartGame(super::StartGameRequest),
        #[prost(message, tag = "3")]
        ConnectToGame(super::ConnectToGameRequest),
        #[prost(message, tag = "4")]
        AdvancePhase(super::AdvancePhaseRequest),
        #[prost(message, tag = "5")]
        PlayCard(super::PlayCardRequest),
        #[prost(message, tag = "6")]
        RepositionCreatures(super::RepositionCreaturesRequest),
        #[prost(message, tag = "7")]
        RunConsoleCommand(super::RunConsoleCommandRequest),
    }
}
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct Influence {
    #[prost(enumeration = "InfluenceType", tag = "1")]
    pub influence_type: i32,
    #[prost(int32, tag = "2")]
    pub value: i32,
}
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct Asset {
    #[prost(string, tag = "1")]
    pub address: std::string::String,
    #[prost(enumeration = "AssetType", tag = "2")]
    pub asset_type: i32,
}
/// There are two players in a game named "user" and "enemy". The term
/// "opponent" can be used to contextually refer to either player
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct PlayerData {
    #[prost(enumeration = "PlayerName", tag = "1")]
    pub player_name: i32,
    #[prost(int32, tag = "2")]
    pub current_life: i32,
    #[prost(int32, tag = "3")]
    pub maximum_life: i32,
    #[prost(int32, tag = "4")]
    pub current_mana: i32,
    #[prost(int32, tag = "5")]
    pub maximum_mana: i32,
    #[prost(message, repeated, tag = "6")]
    pub current_influence: ::std::vec::Vec<Influence>,
    #[prost(message, repeated, tag = "7")]
    pub maximum_influence: ::std::vec::Vec<Influence>,
}
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct NoCost {}
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct StandardCost {
    #[prost(int32, tag = "1")]
    pub mana_cost: i32,
    #[prost(message, repeated, tag = "2")]
    pub influence_cost: ::std::vec::Vec<Influence>,
}
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct RichText {
    #[prost(string, tag = "1")]
    pub text: std::string::String,
}
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct AttachmentData {
    #[prost(message, optional, tag = "1")]
    pub image: ::std::option::Option<Asset>,
}
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct UntargetedData {}
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct CardData {
    #[prost(message, optional, tag = "1")]
    pub card_id: ::std::option::Option<CardId>,
    #[prost(message, optional, tag = "2")]
    pub prefab: ::std::option::Option<Asset>,
    #[prost(string, tag = "3")]
    pub name: std::string::String,
    #[prost(enumeration = "PlayerName", tag = "6")]
    pub owner: i32,
    /// Sprite to display as the card image
    #[prost(message, optional, tag = "7")]
    pub image: ::std::option::Option<Asset>,
    #[prost(message, optional, tag = "8")]
    pub text: ::std::option::Option<RichText>,
    /// Should this card be shown face-up?
    #[prost(bool, tag = "9")]
    pub is_revealed: bool,
    /// Can the user play this card? i.e. does it have a valid target and can the
    /// user pay its costs?
    #[prost(bool, tag = "10")]
    pub can_be_played: bool,
    #[prost(oneof = "card_data::Cost", tags = "4, 5")]
    pub cost: ::std::option::Option<card_data::Cost>,
    #[prost(oneof = "card_data::CardType", tags = "11, 12, 13")]
    pub card_type: ::std::option::Option<card_data::CardType>,
}
pub mod card_data {
    #[derive(Clone, PartialEq, ::prost::Oneof)]
    pub enum Cost {
        #[prost(message, tag = "4")]
        NoCost(super::NoCost),
        #[prost(message, tag = "5")]
        StandardCost(super::StandardCost),
    }
    #[derive(Clone, PartialEq, ::prost::Oneof)]
    pub enum CardType {
        #[prost(message, tag = "11")]
        CreatureCard(super::CreatureData),
        #[prost(message, tag = "12")]
        AttachmentCard(super::AttachmentData),
        #[prost(message, tag = "13")]
        UntargetedCard(super::UntargetedData),
    }
}
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct CreatureData {
    #[prost(message, optional, tag = "1")]
    pub creature_id: ::std::option::Option<CreatureId>,
    #[prost(message, optional, tag = "2")]
    pub prefab: ::std::option::Option<Asset>,
    #[prost(enumeration = "PlayerName", tag = "3")]
    pub owner: i32,
    #[prost(enumeration = "RankValue", tag = "4")]
    pub rank_position: i32,
    #[prost(enumeration = "FileValue", tag = "5")]
    pub file_position: i32,
    /// Can the user change this creature's position?
    #[prost(bool, tag = "6")]
    pub can_be_repositioned: bool,
    #[prost(message, repeated, tag = "7")]
    pub attachments: ::std::vec::Vec<AttachmentData>,
}
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct DisplayErrorCommand {
    #[prost(string, tag = "1")]
    pub error: std::string::String,
}
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct WaitCommand {
    #[prost(int32, tag = "1")]
    pub wait_time_milliseconds: i32,
}
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct UpdateInterfaceCommand {
    #[prost(bool, tag = "1")]
    pub main_button_enabled: bool,
    #[prost(string, tag = "2")]
    pub main_button_text: std::string::String,
}
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct UpdatePlayerCommand {
    #[prost(message, optional, tag = "1")]
    pub player: ::std::option::Option<PlayerData>,
}
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct DrawCardCommand {
    #[prost(message, optional, tag = "1")]
    pub card: ::std::option::Option<CardData>,
}
/// Reveal an *existing* card and (optionally) animate it to a specific
/// rank/file position
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct PlayCardCommand {
    #[prost(message, optional, tag = "1")]
    pub card: ::std::option::Option<CardData>,
    /// How long to show the card for before animating it away. 0 should be
    /// interpreted as allowing the client to pick its own default.
    #[prost(int32, tag = "2")]
    pub reveal_delay_milliseconds: i32,
    #[prost(enumeration = "RankValue", tag = "3")]
    pub rank_position: i32,
    #[prost(enumeration = "FileValue", tag = "4")]
    pub file_position: i32,
}
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct CreateOrUpdateCreatureCommand {
    #[prost(message, optional, tag = "1")]
    pub creature: ::std::option::Option<CreatureData>,
}
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct RemoveCreatureCommand {
    #[prost(message, optional, tag = "1")]
    pub creature_id: ::std::option::Option<CreatureId>,
}
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct MeleeEngageCommand {
    #[prost(message, optional, tag = "1")]
    pub creature_id: ::std::option::Option<CreatureId>,
    #[prost(message, optional, tag = "2")]
    pub target_creature_id: ::std::option::Option<CreatureId>,
}
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct ApplyDamageEffect {
    #[prost(int32, tag = "1")]
    pub damage: i32,
    #[prost(bool, tag = "2")]
    pub kills_target: bool,
}
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct FireProjectileEffect {
    #[prost(message, optional, tag = "1")]
    pub prefab: ::std::option::Option<Asset>,
    #[prost(message, optional, tag = "2")]
    pub apply_damage: ::std::option::Option<ApplyDamageEffect>,
    /// If true, the projectil is fired at the opposing player instead of at
    /// a target creature.
    #[prost(bool, tag = "3")]
    pub at_opponent: bool,
}
/// Causes a creature to play a skill animation and optionally target an effect
/// at an opposing creature
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct AttackCommand {
    #[prost(message, optional, tag = "1")]
    pub creature_id: ::std::option::Option<CreatureId>,
    #[prost(message, optional, tag = "2")]
    pub target_creature_id: ::std::option::Option<CreatureId>,
    /// Which skill animation to play
    #[prost(enumeration = "SkillAnimationNumber", tag = "3")]
    pub skill: i32,
    /// How many times this skill is expected to raise the "AttackStart" event
    #[prost(int32, tag = "4")]
    pub hit_count: i32,
    #[prost(oneof = "attack_command::AttackEffect", tags = "5, 6")]
    pub attack_effect: ::std::option::Option<attack_command::AttackEffect>,
}
pub mod attack_command {
    #[derive(Clone, PartialEq, ::prost::Oneof)]
    pub enum AttackEffect {
        #[prost(message, tag = "5")]
        ApplyDamage(super::ApplyDamageEffect),
        #[prost(message, tag = "6")]
        FireProjectile(super::FireProjectileEffect),
    }
}
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct MSkillAnimation {
    /// The animation to perform
    #[prost(enumeration = "SkillAnimationNumber", tag = "1")]
    pub skill: i32,
    /// How many times this skill is expected to reach its impact frame
    #[prost(int32, tag = "2")]
    pub impact_count: i32,
}
/// An effect to apply when a skill's impact event fires for the nth time
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct MOnImpactNumber {
    /// Apply this effect when the skill has reached its impact frame
    /// 'on_impact_number' times. A value of 0 should be interpreted the same
    /// as 1.
    #[prost(uint32, tag = "1")]
    pub impact_number: u32,
    /// What to do on impact
    #[prost(message, optional, tag = "2")]
    pub effect: ::std::option::Option<MOnImpact>,
}
/// What to do when a skill or projectile reaches impact
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct MOnImpact {
    #[prost(oneof = "m_on_impact::OnImpact", tags = "1, 2")]
    pub on_impact: ::std::option::Option<m_on_impact::OnImpact>,
}
pub mod m_on_impact {
    #[derive(Clone, PartialEq, ::prost::Oneof)]
    pub enum OnImpact {
        /// Apply an update
        #[prost(message, tag = "1")]
        Update(super::MCreatureUpdate),
        /// Fire a projectile
        #[prost(message, tag = "2")]
        FireProjectile(super::MFireProjectile),
    }
}
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct MFireProjectile {
    /// Projectile Prefab to create
    #[prost(message, optional, tag = "1")]
    pub projectile: ::std::option::Option<Asset>,
    /// What to do when the projectile hits
    #[prost(message, repeated, tag = "2")]
    pub on_hit: ::std::vec::Vec<MOnImpact>,
    #[prost(oneof = "m_fire_projectile::Target", tags = "3")]
    pub target: ::std::option::Option<m_fire_projectile::Target>,
}
pub mod m_fire_projectile {
    #[derive(Clone, PartialEq, ::prost::Oneof)]
    pub enum Target {
        #[prost(message, tag = "3")]
        TargetCreature(super::CreatureId),
    }
}
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct MCreatureUpdate {
    #[prost(message, optional, tag = "1")]
    pub creature_id: ::std::option::Option<CreatureId>,
    /// Set this creature's health percentage to a specific value, given as a
    /// number between 0 and 1.
    #[prost(float, tag = "2")]
    pub set_health_percentage: f32,
    /// Mark this creature as dead and play its death animation
    #[prost(bool, tag = "3")]
    pub play_death_animation: bool,
    /// Set this creature's mana percentage to a specific value, given as a
    /// number between 0 and 1.
    #[prost(float, tag = "4")]
    pub set_mana_percentage: f32,
    /// Play particle effects on this creature
    #[prost(message, repeated, tag = "5")]
    pub play_particle_effects: ::std::vec::Vec<Asset>,
}
/// Updates the state of a creature
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct UpdateCreatureCommand {
    #[prost(message, optional, tag = "1")]
    pub update: ::std::option::Option<MCreatureUpdate>,
}
/// Causes creatures to use skills
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct UseCreatureSkillCommand {
    /// Creature to apply these effects to
    #[prost(message, optional, tag = "1")]
    pub source_creature: ::std::option::Option<CreatureId>,
    /// The skill animation to perform
    #[prost(message, optional, tag = "2")]
    pub animation: ::std::option::Option<MSkillAnimation>,
    /// What to do when the skill animation reaches its impact frame
    #[prost(message, repeated, tag = "3")]
    pub on_impact: ::std::vec::Vec<MOnImpactNumber>,
    /// Optionally a target for this skill. The creature will move into melee
    /// range with this target before performing the skill animation.
    #[prost(message, optional, tag = "4")]
    pub melee_target: ::std::option::Option<CreatureId>,
}
/// A single instruction to the client UI to perform some action.
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct Command {
    #[prost(
        oneof = "command::Command",
        tags = "1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12"
    )]
    pub command: ::std::option::Option<command::Command>,
}
pub mod command {
    #[derive(Clone, PartialEq, ::prost::Oneof)]
    pub enum Command {
        #[prost(message, tag = "1")]
        Wait(super::WaitCommand),
        #[prost(message, tag = "2")]
        UpdateInterface(super::UpdateInterfaceCommand),
        #[prost(message, tag = "3")]
        DrawCard(super::DrawCardCommand),
        #[prost(message, tag = "4")]
        PlayCard(super::PlayCardCommand),
        #[prost(message, tag = "5")]
        UpdatePlayer(super::UpdatePlayerCommand),
        #[prost(message, tag = "6")]
        CreateOrUpdateCreature(super::CreateOrUpdateCreatureCommand),
        #[prost(message, tag = "7")]
        RemoveCreature(super::RemoveCreatureCommand),
        #[prost(message, tag = "8")]
        MeleeEngage(super::MeleeEngageCommand),
        #[prost(message, tag = "9")]
        Attack(super::AttackCommand),
        #[prost(message, tag = "10")]
        UpdateCreature(super::UpdateCreatureCommand),
        #[prost(message, tag = "11")]
        UseCreatureSkill(super::UseCreatureSkillCommand),
        #[prost(message, tag = "12")]
        DisplayError(super::DisplayErrorCommand),
    }
}
/// Represents a set of commands which should be executed in parallel,
/// operating simultaneously. Position in the command list indicates visually
/// which commands happen first
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct CommandGroup {
    #[prost(message, repeated, tag = "1")]
    pub commands: ::std::vec::Vec<Command>,
}
/// Represents a sequence of groups of commands which should be executed in
/// serial, one after another. Each group is completely executed before the
/// next group begins.
#[derive(Clone, PartialEq, ::prost::Message)]
pub struct CommandList {
    #[prost(message, repeated, tag = "1")]
    pub command_groups: ::std::vec::Vec<CommandGroup>,
}
#[derive(Clone, Copy, Debug, PartialEq, Eq, Hash, PartialOrd, Ord, ::prost::Enumeration)]
#[repr(i32)]
pub enum PlayerName {
    PlayerUnspecified = 0,
    User = 1,
    Enemy = 2,
}
#[derive(Clone, Copy, Debug, PartialEq, Eq, Hash, PartialOrd, Ord, ::prost::Enumeration)]
#[repr(i32)]
pub enum RankValue {
    RankUnspecified = 0,
    Rank0 = 1,
    Rank1 = 2,
    Rank2 = 3,
    Rank3 = 4,
    Rank4 = 5,
    Rank5 = 6,
}
#[derive(Clone, Copy, Debug, PartialEq, Eq, Hash, PartialOrd, Ord, ::prost::Enumeration)]
#[repr(i32)]
pub enum FileValue {
    FileUnspecified = 0,
    File0 = 1,
    File1 = 2,
    File2 = 3,
    File3 = 4,
    File4 = 5,
    File5 = 6,
}
#[derive(Clone, Copy, Debug, PartialEq, Eq, Hash, PartialOrd, Ord, ::prost::Enumeration)]
#[repr(i32)]
pub enum InfluenceType {
    InfluenceUnspecified = 0,
    Light = 1,
    Sky = 2,
    Flame = 3,
    Ice = 4,
    Earth = 5,
    Shadow = 6,
}
#[derive(Clone, Copy, Debug, PartialEq, Eq, Hash, PartialOrd, Ord, ::prost::Enumeration)]
#[repr(i32)]
pub enum AssetType {
    TypeUnspecified = 0,
    Prefab = 1,
    Sprite = 2,
}
#[derive(Clone, Copy, Debug, PartialEq, Eq, Hash, PartialOrd, Ord, ::prost::Enumeration)]
#[repr(i32)]
pub enum SkillAnimationNumber {
    SkillUnspecified = 0,
    Skill1 = 1,
    Skill2 = 2,
    Skill3 = 3,
    Skill4 = 4,
    Skill5 = 5,
}
