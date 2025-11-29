using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Effects
{
    /// <summary>
    /// Standard damage effect using the Pokemon damage formula.
    /// Most damaging moves have this effect.
    /// </summary>
    public class DamageEffect : IMoveEffect
    {
        public EffectType EffectType => EffectType.Damage;
        public string Description => "Deals damage to the target using the standard damage formula.";
        
        /// <summary>Multiplier applied to final damage (default 1.0 = normal damage).</summary>
        public float DamageMultiplier { get; set; } = 1.0f;
        
        /// <summary>Whether this move can score critical hits.</summary>
        public bool CanCrit { get; set; } = true;
        
        /// <summary>Additional crit stages (0 = normal, 1 = high crit, 2+ = very high).</summary>
        public int CritStages { get; set; } = 0;
    }
}
