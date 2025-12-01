using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Effects
{
    /// <summary>
    /// Protects the user from incoming attacks.
    /// Success rate decreases with consecutive use.
    /// Used by Protect, Detect, King's Shield, Baneful Bunker, etc.
    /// </summary>
    public class ProtectionEffect : IMoveEffect
    {
        public EffectType EffectType => EffectType.Protection;
        public string Description => GetDescription();
        
        /// <summary>Type of protection provided.</summary>
        public ProtectionType Type { get; set; } = ProtectionType.Full;
        
        /// <summary>If true, only blocks moves targeting the user (not spread moves).</summary>
        public bool SingleTargetOnly { get; set; } = false;
        
        /// <summary>Effect on attacker if they make contact (for King's Shield, etc.).</summary>
        public ContactPenalty ContactEffect { get; set; } = ContactPenalty.None;
        
        /// <summary>Stat to lower if contact is made.</summary>
        public Stat? ContactStatDrop { get; set; }
        
        /// <summary>Stages to drop on contact.</summary>
        public int ContactStatStages { get; set; } = -2;
        
        /// <summary>Status to apply on contact (Baneful Bunker = Poison).</summary>
        public PersistentStatus? ContactStatus { get; set; }
        
        /// <summary>Damage dealt on contact as fraction of attacker's max HP (Spiky Shield).</summary>
        public float ContactDamage { get; set; } = 0f;
        
        public ProtectionEffect() { }
        
        public ProtectionEffect(ProtectionType type)
        {
            Type = type;
        }
        
        private string GetDescription()
        {
            switch (Type)
            {
                case ProtectionType.Full: return "Protects from all moves.";
                case ProtectionType.WideGuard: return "Protects team from spread moves.";
                case ProtectionType.QuickGuard: return "Protects team from priority moves.";
                case ProtectionType.CraftyShield: return "Protects team from status moves.";
                case ProtectionType.MatBlock: return "Protects team from damaging moves.";
                default: return "Provides protection.";
            }
        }
    }
    
    /// <summary>
    /// Types of protection moves.
    /// </summary>
    public enum ProtectionType
    {
        /// <summary>Blocks all moves (Protect, Detect).</summary>
        Full,
        /// <summary>Blocks spread moves (Wide Guard).</summary>
        WideGuard,
        /// <summary>Blocks priority moves (Quick Guard).</summary>
        QuickGuard,
        /// <summary>Blocks status moves (Crafty Shield).</summary>
        CraftyShield,
        /// <summary>Blocks damaging moves, first turn only (Mat Block).</summary>
        MatBlock,
        /// <summary>Blocks and damages on contact (Spiky Shield).</summary>
        SpikyShield,
        /// <summary>Blocks and lowers Attack on contact (King's Shield).</summary>
        KingsShield,
        /// <summary>Blocks and poisons on contact (Baneful Bunker).</summary>
        BanefulBunker,
        /// <summary>Blocks and burns on contact (Obstruct lowers Defense).</summary>
        Obstruct,
    }
    
    /// <summary>
    /// Penalty applied to attackers who make contact with a protection move.
    /// </summary>
    public enum ContactPenalty
    {
        None,
        Damage,
        StatDrop,
        Status,
    }
}

