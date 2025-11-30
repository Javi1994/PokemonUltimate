using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Core.Evolution.Conditions
{
    /// <summary>
    /// Evolution condition: Pokemon must have high friendship.
    /// </summary>
    public class FriendshipCondition : IEvolutionCondition
    {
        public EvolutionConditionType ConditionType => EvolutionConditionType.Friendship;
        public string Description => $"Friendship {MinFriendship}+";

        /// <summary>
        /// Minimum friendship value required (default 220, max is 255).
        /// </summary>
        public int MinFriendship { get; set; } = 220;

        public FriendshipCondition() { }

        public FriendshipCondition(int minFriendship)
        {
            MinFriendship = minFriendship;
        }

        public bool IsMet(PokemonInstance pokemon)
        {
            return pokemon != null && pokemon.Friendship >= MinFriendship;
        }
    }
}
