namespace PokemonUltimate.Combat.Damage.Steps
{
    /// <summary>
    /// Applies STAB (Same Type Attack Bonus) - 1.5x multiplier when
    /// the move type matches one of the attacker's types.
    /// </summary>
    public class StabStep : IDamageStep
    {
        private const float StabMultiplier = 1.5f;

        public void Process(DamageContext context)
        {
            var attacker = context.Attacker.Pokemon;
            var moveType = context.Move.Type;

            // Check if move type matches attacker's primary or secondary type
            bool isStab = attacker.Species.PrimaryType == moveType ||
                          attacker.Species.SecondaryType == moveType;

            context.IsStab = isStab;

            if (isStab)
            {
                context.Multiplier *= StabMultiplier;
            }
        }
    }
}

