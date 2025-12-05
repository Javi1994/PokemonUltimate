using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Effects
{
    /// <summary>
    /// Move takes 2 turns: charge turn (no damage), then attack turn (damage).
    /// User is vulnerable during charge turn.
    /// Used by Solar Beam, Skull Bash, Sky Attack, etc.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.15: Advanced Move Mechanics
    /// **Documentation**: See `docs/features/2-combat-system/2.15-advanced-move-mechanics/README.md`
    /// </remarks>
    public class MultiTurnEffect : IMoveEffect
    {
        public EffectType EffectType => EffectType.Charging;
        public string Description => "Move takes 2 turns: charge then attack.";

        /// <summary>
        /// Message shown on charge turn (e.g., "is charging sunlight!").
        /// </summary>
        public string ChargeMessage { get; set; }
    }
}

