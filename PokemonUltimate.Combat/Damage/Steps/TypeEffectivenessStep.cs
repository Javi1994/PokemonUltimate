using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;

namespace PokemonUltimate.Combat.Damage.Steps
{
    /// <summary>
    /// Calculates type effectiveness multiplier based on the move type
    /// versus the defender's type(s).
    /// Uses the existing TypeEffectiveness utility class.
    /// </summary>
    public class TypeEffectivenessStep : IDamageStep
    {
        public void Process(DamageContext context)
        {
            var moveType = context.Move.Type;
            var defender = context.Defender.Pokemon;

            // Use the existing TypeEffectiveness utility
            float effectiveness = TypeEffectiveness.GetEffectiveness(
                moveType,
                defender.Species.PrimaryType,
                defender.Species.SecondaryType);

            context.TypeEffectiveness = effectiveness;
            context.Multiplier *= effectiveness;
        }
    }
}
