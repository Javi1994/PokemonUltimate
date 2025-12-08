using System.Threading.Tasks;

namespace PokemonUltimate.Combat.Engine.BattleFlow.Definition
{
    /// <summary>
    /// Interface para un step individual en el flujo completo de batalla.
    /// Incluye setup, ejecución y cleanup.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `BATTLE_FLOW_STEPS_PROPOSAL.md`
    /// </remarks>
    public interface IBattleFlowStep
    {
        /// <summary>
        /// Nombre del step para logging y debugging.
        /// </summary>
        string StepName { get; }

        /// <summary>
        /// Ejecuta este step del flujo de batalla.
        /// </summary>
        /// <param name="context">El contexto del flujo de batalla. Puede ser modificado por el step.</param>
        /// <returns>True si el flujo debe continuar con el siguiente step, false si debe detenerse.</returns>
        Task<bool> ExecuteAsync(BattleFlowContext context);
    }
}
