using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Damage.Steps
{
    /// <summary>
    /// Applies the burn damage penalty: Physical attacks deal 50% damage when burned.
    /// Special attacks are not affected by burn.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.4: Damage Calculation Pipeline
    /// **Documentation**: See `docs/features/2-combat-system/2.4-damage-calculation-pipeline/architecture.md`
    /// </remarks>
    public class BurnStep : IDamageStep
    {
        private const float BurnPenalty = 0.5f;

        public void Process(DamageContext context)
        {
            var attacker = context.Attacker.Pokemon;
            var move = context.Move;

            // Burn only affects physical attacks
            if (attacker.Status == PersistentStatus.Burn &&
                move.Category == MoveCategory.Physical)
            {
                context.Multiplier *= BurnPenalty;
            }
        }
    }
}

