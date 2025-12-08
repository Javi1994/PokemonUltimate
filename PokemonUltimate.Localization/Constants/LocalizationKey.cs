namespace PokemonUltimate.Localization.Constants
{
    /// <summary>
    /// Constants for localization keys.
    /// Organized by category for maintainability.
    /// </summary>
    /// <remarks>
    /// **Feature**: 4: Unity Integration
    /// **Sub-Feature**: 4.9: Localization System
    /// **Documentation**: See `docs/features/4-unity-integration/4.9-localization-system/architecture.md`
    /// </remarks>
    public static class LocalizationKey
    {
        #region Battle Messages

        public const string BattleUsedMove = "battle_used_move";
        public const string BattleFlinched = "battle_flinched";
        public const string BattleMissed = "battle_missed";
        public const string BattleProtected = "battle_protected";
        public const string BattleNoPP = "battle_no_pp";
        public const string BattleAsleep = "battle_asleep";
        public const string BattleFrozen = "battle_frozen";
        public const string BattleParalyzed = "battle_paralyzed";
        public const string BattleSwitchingOut = "battle_switching_out";

        #endregion

        #region Type Effectiveness

        public const string TypeNoEffect = "type_no_effect";
        public const string TypeSuperEffective = "type_super_effective";
        public const string TypeSuperEffective4x = "type_super_effective_4x";
        public const string TypeNotVeryEffective = "type_not_very_effective";
        public const string TypeNotVeryEffective025x = "type_not_very_effective_025x";

        #endregion

        #region Pokemon Types

        public const string Type_Normal = "type_normal";
        public const string Type_Fire = "type_fire";
        public const string Type_Water = "type_water";
        public const string Type_Grass = "type_grass";
        public const string Type_Electric = "type_electric";
        public const string Type_Ice = "type_ice";
        public const string Type_Fighting = "type_fighting";
        public const string Type_Poison = "type_poison";
        public const string Type_Ground = "type_ground";
        public const string Type_Flying = "type_flying";
        public const string Type_Psychic = "type_psychic";
        public const string Type_Bug = "type_bug";
        public const string Type_Rock = "type_rock";
        public const string Type_Ghost = "type_ghost";
        public const string Type_Dragon = "type_dragon";
        public const string Type_Dark = "type_dark";
        public const string Type_Steel = "type_steel";
        public const string Type_Fairy = "type_fairy";

        #endregion

        #region Status Effects

        public const string StatusBurnDamage = "status_burn_damage";
        public const string StatusPoisonDamage = "status_poison_damage";

        // Persistent Status Names
        public const string Status_None = "status_none";
        public const string Status_Burn = "status_burn";
        public const string Status_Paralysis = "status_paralysis";
        public const string Status_Sleep = "status_sleep";
        public const string Status_Poison = "status_poison";
        public const string Status_BadlyPoisoned = "status_badly_poisoned";
        public const string Status_Freeze = "status_freeze";

        // Volatile Status Names
        public const string VolatileStatus_None = "volatile_status_none";
        public const string VolatileStatus_Confusion = "volatile_status_confusion";
        public const string VolatileStatus_Flinch = "volatile_status_flinch";
        public const string VolatileStatus_LeechSeed = "volatile_status_leech_seed";
        public const string VolatileStatus_Attract = "volatile_status_attract";
        public const string VolatileStatus_Curse = "volatile_status_curse";
        public const string VolatileStatus_Encore = "volatile_status_encore";
        public const string VolatileStatus_Taunt = "volatile_status_taunt";
        public const string VolatileStatus_Torment = "volatile_status_torment";
        public const string VolatileStatus_Disable = "volatile_status_disable";
        public const string VolatileStatus_SemiInvulnerable = "volatile_status_semi_invulnerable";
        public const string VolatileStatus_Charging = "volatile_status_charging";
        public const string VolatileStatus_Protected = "volatile_status_protected";
        public const string VolatileStatus_SwitchingOut = "volatile_status_switching_out";
        public const string VolatileStatus_Focusing = "volatile_status_focusing";
        public const string VolatileStatus_FollowMe = "volatile_status_follow_me";
        public const string VolatileStatus_RagePowder = "volatile_status_rage_powder";

        #endregion

        #region Weather & Terrain

        public const string WeatherSandstormDamage = "weather_sandstorm_damage";
        public const string WeatherHailDamage = "weather_hail_damage";
        public const string TerrainHealing = "terrain_healing";

        #endregion

        #region Abilities & Items

        public const string AbilityActivated = "ability_activated";
        public const string ItemActivated = "item_activated";
        public const string MoveFailed = "move_failed";
        public const string TruantLoafing = "truant_loafing";
        public const string HurtByItem = "hurt_by_item";
        public const string HurtByRecoil = "hurt_by_recoil";
        public const string HurtByContact = "hurt_by_contact";
        public const string HeldOnUsingItem = "held_on_using_item";
        public const string EnduredHit = "endured_hit";

        #endregion

        #region Content Name Prefixes

        public const string MoveNamePrefix = "move_name_";
        public const string PokemonNamePrefix = "pokemon_name_";
        public const string AbilityNamePrefix = "ability_name_";
        public const string ItemNamePrefix = "item_name_";

        #endregion

        #region Entry Hazards

        public const string HazardSpikesDamage = "hazard_spikes_damage";
        public const string HazardStealthRockDamage = "hazard_stealth_rock_damage";
        public const string HazardToxicSpikesAbsorbed = "hazard_toxic_spikes_absorbed";
        public const string HazardToxicSpikesStatus = "hazard_toxic_spikes_status";
        public const string HazardStickyWebSpeed = "hazard_sticky_web_speed";

        #endregion

        #region Multi-Hit

        public const string HitsExactly = "hits_exactly";
        public const string HitsRange = "hits_range";

        #endregion

        #region Move Execution Extended

        public const string MoveProtectFailed = "move_protect_failed";
        public const string MoveCountered = "move_countered";
        public const string MoveFocusing = "move_focusing";
        public const string MoveFocusLost = "move_focus_lost";
        public const string MoveSemiInvulnerable = "move_semi_invulnerable";

        #endregion

        #region Move Descriptions

        public const string MoveDescriptionPrefix = "move_description_";

        #endregion

        #region Ability Descriptions

        public const string AbilityDescriptionPrefix = "ability_description_";

        #endregion

        #region Item Descriptions

        public const string ItemDescriptionPrefix = "item_description_";

        #endregion

        #region Pokedex Data

        public const string PokedexDescriptionPrefix = "pokedex_description_";

        #endregion

        #region Party Management

        public const string PartyIsFull = "party_is_full";
        public const string PartyTooSmallForBattle = "party_too_small_for_battle";
        public const string CannotRemoveLastActivePokemon = "party_cannot_remove_last_active";
        public const string InvalidPartyIndex = "party_invalid_index";

        #endregion

        #region UI - Windows Forms Applications


        // Nature
        public const string Nature_Hardy = "nature_hardy";
        public const string Nature_Docile = "nature_docile";
        public const string Nature_Serious = "nature_serious";
        public const string Nature_Bashful = "nature_bashful";
        public const string Nature_Quirky = "nature_quirky";
        public const string Nature_Lonely = "nature_lonely";
        public const string Nature_Brave = "nature_brave";
        public const string Nature_Adamant = "nature_adamant";
        public const string Nature_Naughty = "nature_naughty";
        public const string Nature_Bold = "nature_bold";
        public const string Nature_Relaxed = "nature_relaxed";
        public const string Nature_Impish = "nature_impish";
        public const string Nature_Lax = "nature_lax";
        public const string Nature_Timid = "nature_timid";
        public const string Nature_Hasty = "nature_hasty";
        public const string Nature_Jolly = "nature_jolly";
        public const string Nature_Naive = "nature_naive";
        public const string Nature_Modest = "nature_modest";
        public const string Nature_Mild = "nature_mild";
        public const string Nature_Quiet = "nature_quiet";
        public const string Nature_Rash = "nature_rash";
        public const string Nature_Calm = "nature_calm";
        public const string Nature_Gentle = "nature_gentle";
        public const string Nature_Sassy = "nature_sassy";
        public const string Nature_Careful = "nature_careful";

        // Move Category
        public const string MoveCategory_Physical = "move_category_physical";
        public const string MoveCategory_Special = "move_category_special";
        public const string MoveCategory_Status = "move_category_status";

        // Stat
        public const string Stat_HP = "stat_hp";
        public const string Stat_Attack = "stat_attack";
        public const string Stat_Defense = "stat_defense";
        public const string Stat_SpAttack = "stat_spattack";
        public const string Stat_SpDefense = "stat_spdefense";
        public const string Stat_Speed = "stat_speed";
        public const string Stat_Accuracy = "stat_accuracy";
        public const string Stat_Evasion = "stat_evasion";

        #endregion
    }
}
