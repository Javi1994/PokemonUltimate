using PokemonUltimate.Core.Domain.Instances.Pokemon;

namespace PokemonUltimate.Combat.Actions.Definition
{
    /// <summary>
    /// Interfaz base para verificadores de comportamiento específico.
    /// Los checkers encapsulan lógica de verificación de comportamientos de Pokemon, items y habilidades.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/MEJORAS_PROPUESTAS.md`
    /// </remarks>
    public interface IBehaviorChecker
    {
        /// <summary>
        /// Verifica si el Pokemon tiene este comportamiento.
        /// </summary>
        /// <param name="pokemon">El Pokemon a verificar. Puede ser null.</param>
        /// <returns>True si el Pokemon tiene el comportamiento, false en caso contrario.</returns>
        bool HasBehavior(PokemonInstance pokemon);
    }
}
