namespace PokemonUltimate.Core.Data.Enums
{
    /// <summary>
    /// Pokemon stats used for damage calculation and stat modifications.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.10: Enums & Constants
    /// **Documentation**: See `docs/features/1-game-data/1.10-enums-constants/README.md`
    /// </remarks>
    public enum Stat
    {
        /// <summary>Hit Points - determines how much damage a Pokemon can take.</summary>
        HP,

        /// <summary>Physical Attack - used for Physical moves.</summary>
        Attack,

        /// <summary>Physical Defense - reduces damage from Physical moves.</summary>
        Defense,

        /// <summary>Special Attack - used for Special moves.</summary>
        SpAttack,

        /// <summary>Special Defense - reduces damage from Special moves.</summary>
        SpDefense,

        /// <summary>Speed - determines turn order and some move effects.</summary>
        Speed,

        /// <summary>Accuracy - affects chance to hit (battle-only, not a base stat).</summary>
        Accuracy,

        /// <summary>Evasion - affects chance to dodge (battle-only, not a base stat).</summary>
        Evasion
    }
}
