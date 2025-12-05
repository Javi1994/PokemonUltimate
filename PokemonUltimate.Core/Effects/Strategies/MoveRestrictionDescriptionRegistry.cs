using System.Collections.Generic;
using PokemonUltimate.Core.Effects.Strategies.Implementations;
using MoveRestrictionType = PokemonUltimate.Core.Effects.MoveRestrictionType;

namespace PokemonUltimate.Core.Effects.Strategies
{
    /// <summary>
    /// Registry for move restriction description strategies.
    /// </summary>
    public static class MoveRestrictionDescriptionRegistry
    {
        private static readonly Dictionary<MoveRestrictionType, IMoveRestrictionDescriptionStrategy> _strategies;

        static MoveRestrictionDescriptionRegistry()
        {
            _strategies = new Dictionary<MoveRestrictionType, IMoveRestrictionDescriptionStrategy>
            {
                { MoveRestrictionType.Encore, new EncoreDescriptionStrategy() },
                { MoveRestrictionType.Disable, new DisableDescriptionStrategy() },
                { MoveRestrictionType.Taunt, new TauntDescriptionStrategy() },
                { MoveRestrictionType.Torment, new TormentDescriptionStrategy() },
                { MoveRestrictionType.Imprison, new ImprisonDescriptionStrategy() },
                { MoveRestrictionType.HealBlock, new HealBlockDescriptionStrategy() },
                { MoveRestrictionType.Embargo, new EmbargoDescriptionStrategy() },
                { MoveRestrictionType.ThroatChop, new ThroatChopDescriptionStrategy() }
            };
        }

        /// <summary>
        /// Gets the description for the specified move restriction type.
        /// </summary>
        public static string GetDescription(MoveRestrictionType type, int duration)
        {
            if (_strategies.TryGetValue(type, out var strategy))
            {
                return strategy.GetDescription(duration);
            }

            return "Restricts move usage.";
        }
    }
}
