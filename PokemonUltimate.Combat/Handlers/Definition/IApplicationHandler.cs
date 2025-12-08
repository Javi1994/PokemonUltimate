namespace PokemonUltimate.Combat.Handlers.Definition
{
    /// <summary>
    /// Interfaz base para handlers que verifican si algo puede aplicarse.
    /// Usado por handlers de aplicación (Status, StatChange, Healing, Switch, Field Conditions).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/PROCESSOR_REFACTOR_PROPOSAL.md`
    /// </remarks>
    public interface IApplicationHandler
    {
        // Esta interfaz marca los handlers que verifican aplicaciones.
        // No define métodos específicos porque cada handler tiene diferentes parámetros y tipos de retorno.
        // Su propósito es:
        // 1. Agrupar conceptualmente los handlers de verificación
        // 2. Facilitar la identificación de handlers de aplicación
        // 3. Permitir futuras extensiones comunes si es necesario
    }
}
