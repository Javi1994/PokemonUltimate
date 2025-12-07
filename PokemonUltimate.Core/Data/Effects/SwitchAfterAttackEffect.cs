using PokemonUltimate.Core.Data.Effects.Definition;
using PokemonUltimate.Core.Data.Enums;


namespace PokemonUltimate.Core.Data.Effects
{
    /// <summary>
    /// User switches out after attacking.
    /// Used by U-turn, Volt Switch, Flip Turn, Parting Shot.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.2: Move Data
    /// **Documentation**: See `docs/features/1-game-data/1.2-move-data/architecture.md`
    /// </remarks>
    public class SwitchAfterAttackEffect : IMoveEffect
    {
        public EffectType EffectType => EffectType.SwitchAfterAttack;
        public string Description => DealsDamage
            ? "Deals damage, then user switches out."
            : "User switches out after using.";

        /// <summary>Whether this move deals damage (U-turn) or just switches (Teleport with effect).</summary>
        public bool DealsDamage { get; set; } = true;

        /// <summary>Whether switch is mandatory or optional.</summary>
        public bool MandatorySwitch { get; set; } = true;

        /// <summary>Stat changes to apply to target before switching (Parting Shot = -1 Atk, -1 SpA).</summary>
        public StatChangeEffect[] StatChanges { get; set; }

        public SwitchAfterAttackEffect() { }

        public SwitchAfterAttackEffect(bool dealsDamage)
        {
            DealsDamage = dealsDamage;
        }
    }
}

