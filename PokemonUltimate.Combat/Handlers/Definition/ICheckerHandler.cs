namespace PokemonUltimate.Combat.Handlers.Definition
{
    /// <summary>
    /// Interfaz base para handlers que verifican condiciones o estados existentes.
    /// Usado por handlers que comprueban si algo ya está presente o si una condición se cumple.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/PROCESSOR_REFACTOR_PROPOSAL.md`
    ///
    /// **Diferencia con IApplicationHandler**:
    /// - IApplicationHandler: Verifica si algo puede APLICARSE (status, stat change, healing, switch, field conditions)
    /// - ICheckerHandler: Verifica si algo YA ESTÁ presente o si una condición se cumple (protection, semi-invulnerable, accuracy, execution, move state)
    /// </remarks>
    public interface ICheckerHandler
    {
        // Esta interfaz marca los handlers que verifican condiciones/estados existentes.
        // No define métodos específicos porque cada handler tiene diferentes parámetros y tipos de retorno.
        // Su propósito es:
        // 1. Agrupar conceptualmente los handlers de verificación de condiciones
        // 2. Facilitar la identificación de handlers que verifican estados existentes
        // 3. Permitir futuras extensiones comunes si es necesario
    }
}
