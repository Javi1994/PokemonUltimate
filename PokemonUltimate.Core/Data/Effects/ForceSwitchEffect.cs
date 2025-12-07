using PokemonUltimate.Core.Data.Effects.Definition;
using PokemonUltimate.Core.Data.Enums;


namespace PokemonUltimate.Core.Data.Effects
{
    /// <summary>
    /// Forces the target to switch out.
    /// Used by Roar, Whirlwind, Dragon Tail, Circle Throw.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.2: Move Data
    /// **Documentation**: See `docs/features/1-game-data/1.2-move-data/architecture.md`
    /// </remarks>
    public class ForceSwitchEffect : IMoveEffect
    {
        public EffectType EffectType => EffectType.ForceSwitch;
        public string Description => DealsDamage
            ? "Deals damage, then forces target to switch."
            : "Forces target to switch out.";

        /// <summary>Whether this move deals damage before forcing switch (Dragon Tail, Circle Throw).</summary>
        public bool DealsDamage { get; set; } = false;

        /// <summary>Whether the incoming Pokemon is random (Roar) or chosen (Red Card).</summary>
        public bool RandomReplacement { get; set; } = true;

        /// <summary>Whether this works in trainer battles (fails if no bench).</summary>
        public bool WorksInTrainerBattles { get; set; } = true;

        /// <summary>Whether this ends wild battles.</summary>
        public bool EndsWildBattle { get; set; } = true;

        public ForceSwitchEffect() { }

        public ForceSwitchEffect(bool dealsDamage)
        {
            DealsDamage = dealsDamage;
        }
    }
}

