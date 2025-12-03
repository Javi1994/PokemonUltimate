using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Effects
{
    /// <summary>
    /// Damage occurs after a delay.
    /// Used by Future Sight, Doom Desire, Wish.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.2: Move Data
    /// **Documentation**: See `docs/features/1-game-data/1.2-move-data/architecture.md`
    /// </remarks>
    public class DelayedDamageEffect : IMoveEffect
    {
        public EffectType EffectType => EffectType.DelayedDamage;
        public string Description => IsHealing 
            ? $"Heals the Pokemon in this slot after {TurnsDelay} turns."
            : $"Damages the Pokemon in this slot after {TurnsDelay} turns.";
        
        /// <summary>Number of turns until effect activates.</summary>
        public int TurnsDelay { get; set; } = 2;
        
        /// <summary>Whether this heals instead of damages (Wish).</summary>
        public bool IsHealing { get; set; } = false;
        
        /// <summary>Heal amount as fraction of max HP (Wish = 0.5).</summary>
        public float HealFraction { get; set; } = 0.5f;
        
        /// <summary>Whether the effect targets the slot (Future Sight) or a specific Pokemon (Wish).</summary>
        public bool TargetsSlot { get; set; } = true;
        
        /// <summary>Whether damage uses caster's stats at time of use.</summary>
        public bool UsesCasterStats { get; set; } = true;
        
        /// <summary>Move type used for damage calculation (Future Sight = Psychic).</summary>
        public PokemonType? DamageType { get; set; }
        
        /// <summary>Whether the delayed attack can be blocked by Protect.</summary>
        public bool BypassesProtect { get; set; } = true;
        
        public DelayedDamageEffect() { }
        
        public DelayedDamageEffect(int delay, bool isHealing = false)
        {
            TurnsDelay = delay;
            IsHealing = isHealing;
        }
    }
}

