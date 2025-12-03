namespace PokemonUltimate.Combat.Damage.Steps
{
    /// <summary>
    /// Applies STAB (Same Type Attack Bonus) - 1.5x multiplier when
    /// the move type matches one of the attacker's types.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.4: Damage Calculation Pipeline
    /// **Documentation**: See `docs/features/2-combat-system/2.4-damage-calculation-pipeline/architecture.md`
    /// </remarks>
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

