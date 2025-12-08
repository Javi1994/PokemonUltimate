using PokemonUltimate.Combat.Handlers.Definition;
using PokemonUltimate.Core.Domain.Instances.Pokemon;

namespace PokemonUltimate.Combat.Handlers.Checkers
{
    /// <summary>
    /// Handler para verificación de aplicación de curación.
    /// Valida si un Pokemon puede ser curado y calcula la cantidad de curación efectiva.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/PROCESSOR_REFACTOR_PROPOSAL.md`
    /// </remarks>
    public class HealingApplicationHandler : IApplicationHandler
    {
        /// <summary>
        /// Verifica si un Pokemon puede recibir curación.
        /// </summary>
        /// <param name="pokemon">El Pokemon a verificar. Puede ser null.</param>
        /// <param name="healAmount">La cantidad de curación a aplicar.</param>
        /// <returns>True si el Pokemon puede ser curado, false en caso contrario.</returns>
        public bool CanHeal(PokemonInstance pokemon, int healAmount)
        {
            if (pokemon == null)
                return false;

            // No healing if amount is zero or negative
            if (healAmount <= 0)
                return false;

            // Can't heal fainted Pokemon (they need to be revived first)
            if (pokemon.IsFainted)
                return false;

            // Can heal if HP is below max (even if at full HP, healing is allowed but won't do anything)
            return true;
        }

        /// <summary>
        /// Calcula la cantidad efectiva de curación que se aplicará.
        /// Considera el HP actual y máximo del Pokemon.
        /// </summary>
        /// <param name="pokemon">El Pokemon que será curado. No puede ser null.</param>
        /// <param name="healAmount">La cantidad de curación solicitada.</param>
        /// <returns>La cantidad efectiva de curación (puede ser menor si causaría overhealing).</returns>
        public int CalculateEffectiveHealing(PokemonInstance pokemon, int healAmount)
        {
            if (pokemon == null || healAmount <= 0)
                return 0;

            int currentHP = pokemon.CurrentHP;
            int maxHP = pokemon.MaxHP;

            // Calculate how much HP can actually be healed
            int hpDeficit = maxHP - currentHP;

            // Return the minimum of requested healing and HP deficit (prevents overhealing)
            return healAmount > hpDeficit ? hpDeficit : healAmount;
        }

        /// <summary>
        /// Verifica si la curación sería efectiva (el Pokemon no está a HP completo).
        /// </summary>
        /// <param name="pokemon">El Pokemon a verificar. Puede ser null.</param>
        /// <returns>True si el Pokemon necesita curación, false si está a HP completo.</returns>
        public bool NeedsHealing(PokemonInstance pokemon)
        {
            if (pokemon == null || pokemon.IsFainted)
                return false;

            return pokemon.CurrentHP < pokemon.MaxHP;
        }
    }
}
