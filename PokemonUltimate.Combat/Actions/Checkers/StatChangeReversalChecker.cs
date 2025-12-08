using PokemonUltimate.Combat.Actions.Definition;
using PokemonUltimate.Combat.Utilities.Extensions;
using PokemonUltimate.Core.Domain.Instances.Pokemon;

namespace PokemonUltimate.Combat.Actions.Checkers
{
    /// <summary>
    /// Verificador de inversión de cambios de estadísticas (Contrary).
    /// Determina si un Pokemon invierte los cambios de estadísticas y aplica la inversión.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/MEJORAS_PROPUESTAS.md`
    /// </remarks>
    public class StatChangeReversalChecker : IBehaviorChecker
    {
        /// <summary>
        /// Verifica si el Pokemon invierte cambios de estadísticas (tiene la habilidad Contrary).
        /// </summary>
        /// <param name="pokemon">El Pokemon a verificar. Puede ser null.</param>
        /// <returns>True si el Pokemon tiene Contrary, false en caso contrario.</returns>
        public bool HasBehavior(PokemonInstance pokemon)
        {
            return pokemon?.HasContrary() == true;
        }

        /// <summary>
        /// Invierte el cambio de estadísticas si el Pokemon tiene Contrary.
        /// </summary>
        /// <param name="pokemon">El Pokemon que recibe el cambio de stats. Puede ser null.</param>
        /// <param name="statChange">El cambio de estadística original (positivo o negativo).</param>
        /// <returns>El cambio invertido si tiene Contrary, o el cambio original en caso contrario.</returns>
        public int ReverseStatChange(PokemonInstance pokemon, int statChange)
        {
            if (HasBehavior(pokemon))
                return -statChange;

            return statChange;
        }
    }
}
