using System.Threading.Tasks;
using PokemonUltimate.Combat.Field;

namespace PokemonUltimate.Combat.Engine.TurnFlow.Definition
{
    /// <summary>
    /// Interface para un step individual en eventos de batalla (inicio/fin).
    /// Similar a ITurnStep pero para eventos de batalla completa, no de turno.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `ENGINE_RESPONSIBILITY_REVIEW.md`
    /// </remarks>
    public interface IBattleStep
    {
        /// <summary>
        /// Nombre del step para logging y debugging.
        /// </summary>
        string StepName { get; }

        /// <summary>
        /// Ejecuta este step de batalla.
        /// </summary>
        /// <param name="field">El campo de batalla. No puede ser null.</param>
        /// <returns>Task que completa cuando el step termina.</returns>
        Task ExecuteAsync(BattleField field);
    }
}
