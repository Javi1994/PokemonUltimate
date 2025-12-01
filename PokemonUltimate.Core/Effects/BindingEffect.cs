using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Effects
{
    /// <summary>
    /// Traps and damages the target over multiple turns.
    /// Used by Wrap, Bind, Fire Spin, Whirlpool, Sand Tomb, Magma Storm.
    /// </summary>
    public class BindingEffect : IMoveEffect
    {
        public EffectType EffectType => EffectType.Binding;
        public string Description => $"Traps target for {MinTurns}-{MaxTurns} turns, dealing {DamagePerTurn * 100}% HP each turn.";
        
        /// <summary>Minimum duration in turns.</summary>
        public int MinTurns { get; set; } = 4;
        
        /// <summary>Maximum duration in turns.</summary>
        public int MaxTurns { get; set; } = 5;
        
        /// <summary>Extended duration with Grip Claw (7 turns).</summary>
        public int ExtendedTurns { get; set; } = 7;
        
        /// <summary>Damage per turn as fraction of max HP (1/8 default, 1/6 with Binding Band).</summary>
        public float DamagePerTurn { get; set; } = 0.125f;
        
        /// <summary>Enhanced damage with Binding Band.</summary>
        public float EnhancedDamagePerTurn { get; set; } = 0.167f;
        
        /// <summary>Whether target can switch while bound.</summary>
        public bool PreventsSwitch { get; set; } = true;
        
        /// <summary>Move type for visual/message purposes.</summary>
        public string BindType { get; set; } = "trapped";
        
        public BindingEffect() { }
        
        public BindingEffect(string bindType, float damagePerTurn = 0.125f)
        {
            BindType = bindType;
            DamagePerTurn = damagePerTurn;
        }
    }
}

