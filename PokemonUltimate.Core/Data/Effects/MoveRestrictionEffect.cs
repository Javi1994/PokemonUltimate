using PokemonUltimate.Core.Data.Effects.Definition;
using PokemonUltimate.Core.Data.Enums;

using PokemonUltimate.Core.Strategies.Effect;

namespace PokemonUltimate.Core.Data.Effects
{
    /// <summary>
    /// Restricts the target's move usage.
    /// Used by Encore, Disable, Taunt, Torment, Imprison, Heal Block.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.2: Move Data
    /// **Documentation**: See `docs/features/1-game-data/1.2-move-data/architecture.md`
    /// </remarks>
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
            return MoveRestrictionDescriptionRegistry.GetDescription(RestrictionType, Duration);
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

