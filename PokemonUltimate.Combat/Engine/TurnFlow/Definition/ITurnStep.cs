using System.Threading.Tasks;

namespace PokemonUltimate.Combat.Engine.TurnFlow.Definition
{
    /// <summary>
    /// Interface para un step individual en el pipeline de ejecución de turnos.
    /// Cada step representa una fase del turno y puede modificar el contexto.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `TURN_STEPS_PROPOSAL.md`
    /// </remarks>
    public interface ITurnStep
    {
        /// <summary>
        /// Nombre del step para logging y debugging.
        /// </summary>
        string StepName { get; }

        /// <summary>
        /// Indica si este step debe ejecutarse incluso si hay Pokemon debilitados.
        /// </summary>
        bool ExecuteEvenIfFainted { get; }

        /// <summary>
        /// Ejecuta este step del turno.
        /// </summary>
        /// <param name="context">El contexto del turno. Puede ser modificado por el step.</param>
        /// <returns>True si el turno debe continuar con el siguiente step, false si debe detenerse.</returns>
        Task<bool> ExecuteAsync(TurnContext context);
    }
}
