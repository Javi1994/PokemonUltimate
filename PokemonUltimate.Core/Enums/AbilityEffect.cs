namespace PokemonUltimate.Core.Enums
{
    /// <summary>
    /// The type of effect an ability has when triggered.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.10: Enums & Constants
    /// **Documentation**: See `docs/features/1-game-data/1.10-enums-constants/README.md`
    /// </remarks>
    public enum AbilityEffect
    {
        None,

        // === STAT MODIFICATION ===
        /// <summary>Lowers opponent's stat (Intimidate).</summary>
        LowerOpponentStat,
        /// <summary>Raises own stat (Speed Boost).</summary>
        RaiseOwnStat,
        /// <summary>Prevents own stat from being lowered (Clear Body).</summary>
        PreventStatLoss,
        /// <summary>Raises stat when own stat is lowered (Defiant, Competitive).</summary>
        RaiseStatOnLoss,
        /// <summary>Raises stat when knocking out opponent (Moxie, Beast Boost).</summary>
        RaiseStatOnKO,

        // === STATUS EFFECTS ===
        /// <summary>Chance to apply status on contact (Static, Poison Point).</summary>
        ChanceToStatusOnContact,
        /// <summary>Prevents certain status (Limber, Immunity).</summary>
        PreventStatus,
        /// <summary>Heals status at end of turn (Shed Skin).</summary>
        ChanceToHealStatus,

        // === DAMAGE MODIFICATION ===
        /// <summary>Reduces damage from certain types (Thick Fat).</summary>
        ReduceTypeDamage,
        /// <summary>Immune to certain type, may boost stat (Flash Fire, Volt Absorb).</summary>
        TypeImmunityWithBoost,
        /// <summary>Immune to Ground type (Levitate).</summary>
        GroundImmunity,
        /// <summary>Boosts move power of certain type (Blaze, Torrent, Overgrow).</summary>
        TypePowerBoostWhenLowHP,

        // === SURVIVAL ===
        /// <summary>Survives fatal hit at full HP (Sturdy).</summary>
        SurviveFatalHit,
        /// <summary>Heals when hit by certain type (Water Absorb, Volt Absorb).</summary>
        HealFromType,

        // === PASSIVE STAT BOOST ===
        /// <summary>Permanently boosts a stat (Huge Power, Pure Power).</summary>
        PassiveStatMultiplier,
        /// <summary>Boosts speed in certain weather (Swift Swim, Chlorophyll).</summary>
        SpeedBoostInWeather,

        // === WEATHER ===
        /// <summary>Summons weather on switch-in (Drizzle, Drought).</summary>
        SummonWeather,
        /// <summary>Heals in certain weather (Rain Dish, Ice Body).</summary>
        HealInWeather,
        /// <summary>Immune to weather damage (Sand Veil, Snow Cloak).</summary>
        WeatherDamageImmunity,

        // === TERRAIN ===
        /// <summary>Summons terrain on switch-in (Grassy Surge, Electric Surge).</summary>
        SummonTerrain,
        /// <summary>Heals or benefits in certain terrain (Grass Pelt).</summary>
        TerrainBoost,

        // === CONTACT DAMAGE ===
        /// <summary>Damages attacker on contact (Rough Skin, Iron Barbs).</summary>
        DamageOnContact,

        // === STAB MODIFICATION ===
        /// <summary>Increases STAB bonus (Adaptability - 2.0x instead of 1.5x).</summary>
        IncreaseStab,

        // === MISC ===
        /// <summary>Copies opponent's ability (Trace).</summary>
        CopyAbility,
        /// <summary>Changes form based on conditions (Forecast).</summary>
        FormChange,
        /// <summary>Skips turn every other turn (Truant).</summary>
        SkipTurn,
        /// <summary>Custom effect handled by specific logic.</summary>
        Custom,
    }
}

