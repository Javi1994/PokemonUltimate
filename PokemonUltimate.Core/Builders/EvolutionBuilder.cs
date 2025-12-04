using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Evolution.Conditions;

namespace PokemonUltimate.Content.Builders
{
    /// <summary>
    /// Fluent builder for creating Evolution instances.
    /// </summary>
    /// <remarks>
    /// **Feature**: 3: Content Expansion
    /// **Sub-Feature**: 3.9: Builders
    /// **Documentation**: See `docs/features/3-content-expansion/3.9-builders/README.md`
    /// </remarks>
    public class EvolutionBuilder
    {
        private readonly Core.Evolution.Evolution _evolution;

        public EvolutionBuilder(PokemonSpeciesData target)
        {
            _evolution = new Core.Evolution.Evolution { Target = target };
        }

        /// <summary>
        /// Evolution requires reaching a minimum level.
        /// </summary>
        public EvolutionBuilder AtLevel(int level)
        {
            _evolution.Conditions.Add(new LevelCondition(level));
            return this;
        }

        /// <summary>
        /// Evolution requires using a specific item.
        /// </summary>
        public EvolutionBuilder WithItem(string itemName)
        {
            _evolution.Conditions.Add(new ItemCondition(itemName));
            return this;
        }

        /// <summary>
        /// Evolution requires high friendship.
        /// </summary>
        public EvolutionBuilder WithFriendship(int minFriendship = 220)
        {
            _evolution.Conditions.Add(new FriendshipCondition(minFriendship));
            return this;
        }

        /// <summary>
        /// Evolution requires a specific time of day.
        /// </summary>
        public EvolutionBuilder During(TimeOfDay time)
        {
            _evolution.Conditions.Add(new TimeOfDayCondition(time));
            return this;
        }

        /// <summary>
        /// Evolution requires trading.
        /// </summary>
        public EvolutionBuilder ByTrade()
        {
            _evolution.Conditions.Add(new TradeCondition());
            return this;
        }

        /// <summary>
        /// Evolution requires knowing a specific move.
        /// </summary>
        public EvolutionBuilder KnowsMove(MoveData move)
        {
            _evolution.Conditions.Add(new KnowsMoveCondition(move));
            return this;
        }

        /// <summary>
        /// Build the Evolution instance.
        /// </summary>
        public Core.Evolution.Evolution Build()
        {
            return _evolution;
        }
    }
}

