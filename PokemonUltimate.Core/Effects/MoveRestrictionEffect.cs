using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Effects
{
    /// <summary>
    /// Restricts the target's move usage.
    /// Used by Encore, Disable, Taunt, Torment, Imprison, Heal Block.
    /// </summary>
    public class MoveRestrictionEffect : IMoveEffect
    {
        public EffectType EffectType => EffectType.MoveRestriction;
        public string Description => GetDescription();
        
        /// <summary>Type of move restriction.</summary>
        public MoveRestrictionType RestrictionType { get; set; }
        
        /// <summary>Duration in turns (0 = indefinite).</summary>
        public int Duration { get; set; } = 3;
        
        /// <summary>If true, applies to the user instead of target (Imprison).</summary>
        public bool TargetSelf { get; set; } = false;
        
        public MoveRestrictionEffect() { }
        
        public MoveRestrictionEffect(MoveRestrictionType type, int duration = 3)
        {
            RestrictionType = type;
            Duration = duration;
        }
        
        private string GetDescription()
        {
            switch (RestrictionType)
            {
                case MoveRestrictionType.Encore: return $"Forces target to repeat last move for {Duration} turns.";
                case MoveRestrictionType.Disable: return $"Disables target's last move for {Duration} turns.";
                case MoveRestrictionType.Taunt: return $"Target can only use damaging moves for {Duration} turns.";
                case MoveRestrictionType.Torment: return "Target cannot use same move consecutively.";
                case MoveRestrictionType.Imprison: return "Opponents cannot use moves the user knows.";
                case MoveRestrictionType.HealBlock: return $"Target cannot heal for {Duration} turns.";
                case MoveRestrictionType.Embargo: return $"Target cannot use items for {Duration} turns.";
                case MoveRestrictionType.ThroatChop: return $"Target cannot use sound moves for {Duration} turns.";
                default: return "Restricts move usage.";
            }
        }
    }
    
    /// <summary>
    /// Types of move restrictions.
    /// </summary>
    public enum MoveRestrictionType
    {
        /// <summary>Forces target to repeat last move (Encore).</summary>
        Encore,
        /// <summary>Disables a specific move (Disable).</summary>
        Disable,
        /// <summary>Blocks status moves (Taunt).</summary>
        Taunt,
        /// <summary>Blocks consecutive same move (Torment).</summary>
        Torment,
        /// <summary>Blocks moves user knows (Imprison).</summary>
        Imprison,
        /// <summary>Blocks healing (Heal Block).</summary>
        HealBlock,
        /// <summary>Blocks item use (Embargo).</summary>
        Embargo,
        /// <summary>Blocks sound moves (Throat Chop).</summary>
        ThroatChop,
    }
}

