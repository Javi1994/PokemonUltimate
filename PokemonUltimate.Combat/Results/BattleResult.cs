using System.Collections.Generic;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Combat
{
    /// <summary>
    /// Contains detailed information about the result of a battle.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public class BattleResult
    {
        /// <summary>
        /// The outcome of the battle.
        /// </summary>
        public BattleOutcome Outcome { get; set; }

        /// <summary>
        /// The Pokemon that dealt the most damage (MVP).
        /// Can be null if no damage was dealt.
        /// </summary>
        public BattleSlot MvpPokemon { get; set; }

        /// <summary>
        /// Number of turns taken in the battle.
        /// </summary>
        public int TurnsTaken { get; set; }

        /// <summary>
        /// List of enemy Pokemon that were defeated during the battle.
        /// </summary>
        public List<PokemonInstance> DefeatedEnemies { get; set; } = new List<PokemonInstance>();

        /// <summary>
        /// Experience points earned by each Pokemon.
        /// Key: Pokemon instance, Value: Experience points earned.
        /// </summary>
        public Dictionary<PokemonInstance, int> ExpEarned { get; set; } = new Dictionary<PokemonInstance, int>();

        /// <summary>
        /// Items dropped by defeated enemies.
        /// </summary>
        public List<ItemData> LootDropped { get; set; } = new List<ItemData>();
    }
}

