using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Effects
{
    /// <summary>
    /// User switches out after attacking.
    /// Used by U-turn, Volt Switch, Flip Turn, Parting Shot.
    /// </summary>
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

