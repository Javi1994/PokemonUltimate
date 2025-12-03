namespace PokemonUltimate.Core.Enums
{
    /// <summary>
    /// Defines who a move can target during battle.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.10: Enums & Constants
    /// **Documentation**: See `docs/features/1-game-data/1.10-enums-constants/README.md`
    /// </remarks>
    public enum TargetScope
    {
        /// <summary>Targets the user (Swords Dance, Recover).</summary>
        Self,

        /// <summary>Targets a single enemy (Tackle, Thunderbolt).</summary>
        SingleEnemy,

        /// <summary>Targets a single ally (Helping Hand).</summary>
        SingleAlly,

        /// <summary>Targets all enemies (Earthquake, Surf in singles).</summary>
        AllEnemies,

        /// <summary>Targets all allies (rare).</summary>
        AllAllies,

        /// <summary>Targets everyone except user (Explosion).</summary>
        AllOthers,

        /// <summary>Targets all adjacent Pokemon including allies (Earthquake in doubles).</summary>
        AllAdjacent,

        /// <summary>Targets all adjacent enemies only (Razor Leaf, Heat Wave).</summary>
        AllAdjacentEnemies,

        /// <summary>Can target anyone (Telekinesis).</summary>
        Any,

        /// <summary>Targets a random enemy (Outrage, Petal Dance).</summary>
        RandomEnemy,

        /// <summary>Affects the entire field (Stealth Rock, Weather).</summary>
        Field,

        /// <summary>Targets user and all allies (Aromatherapy, Heal Bell).</summary>
        UserAndAllies
    }
}
