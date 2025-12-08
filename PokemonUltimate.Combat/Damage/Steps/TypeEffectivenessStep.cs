using PokemonUltimate.Combat.Damage.Definition;
using PokemonUltimate.Core.Services;

namespace PokemonUltimate.Combat.Damage.Steps
{
    /// <summary>
    /// Calculates type effectiveness multiplier based on the move type
    /// versus the defender's type(s).
    /// Uses the existing TypeEffectiveness utility class.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.4: Damage Calculation Pipeline
    /// **Documentation**: See `docs/features/2-combat-system/2.4-damage-calculation-pipeline/architecture.md`
    /// </remarks>
    public class TypeEffectivenessStep : IDamageStep
    {
        public void Process(DamageContext context)
        {
            var moveType = context.Move.Type;
            var defender = context.Defender.Pokemon;

            // Use the existing TypeEffectiveness utility
            float effectiveness = TypeEffectivenessService.GetEffectiveness(
                moveType,
                defender.Species.PrimaryType,
                defender.Species.SecondaryType);

            context.TypeEffectiveness = effectiveness;
            context.Multiplier *= effectiveness;
        }
    }
}
