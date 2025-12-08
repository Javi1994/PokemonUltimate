using System;
using PokemonUltimate.Combat.Actions.Definition;
using PokemonUltimate.Combat.Utilities.Extensions;
using PokemonUltimate.Core.Domain.Instances.Pokemon;

namespace PokemonUltimate.Combat.Actions.Checkers
{
    /// <summary>
    /// Verificador de prevención de OHKO (Focus Sash, Sturdy).
    /// Determina si un Pokemon puede prevenir un OHKO y calcula el daño modificado.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/MEJORAS_PROPUESTAS.md`
    /// </remarks>
    public class OHKOPreventionChecker : IBehaviorChecker
    {
        /// <summary>
        /// Verifica si el Pokemon puede prevenir OHKO (tiene Focus Sash o Sturdy y está a HP completo).
        /// </summary>
        /// <param name="pokemon">El Pokemon a verificar. Puede ser null.</param>
        /// <returns>True si el Pokemon puede prevenir OHKO, false en caso contrario.</returns>
        public bool HasBehavior(PokemonInstance pokemon)
        {
            if (pokemon == null)
                return false;

            // Verificar Focus Sash (item)
            if (pokemon.HasFocusSash())
            {
                return pokemon.CurrentHP >= pokemon.MaxHP; // Solo funciona a HP completo
            }

            // Verificar Sturdy (habilidad)
            if (pokemon.HasSturdy())
            {
                return pokemon.CurrentHP >= pokemon.MaxHP; // Solo funciona a HP completo
            }

            return false;
        }

        /// <summary>
        /// Obtiene el tipo de prevención de OHKO (Item o Ability).
        /// </summary>
        /// <param name="pokemon">El Pokemon a verificar. Puede ser null.</param>
        /// <returns>El tipo de prevención, o None si no puede prevenir OHKO.</returns>
        public OHKOPreventionType GetPreventionType(PokemonInstance pokemon)
        {
            if (pokemon == null)
                return OHKOPreventionType.None;

            // Verificar Focus Sash primero (tiene prioridad)
            if (pokemon.HasFocusSash() && pokemon.CurrentHP >= pokemon.MaxHP)
                return OHKOPreventionType.Item;

            // Verificar Sturdy
            if (pokemon.HasSturdy() && pokemon.CurrentHP >= pokemon.MaxHP)
                return OHKOPreventionType.Ability;

            return OHKOPreventionType.None;
        }

        /// <summary>
        /// Calcula el daño modificado después de prevenir OHKO.
        /// Deja al Pokemon con 1 HP si puede prevenir el OHKO.
        /// </summary>
        /// <param name="pokemon">El Pokemon que recibe el daño. No puede ser null.</param>
        /// <param name="originalDamage">El daño original que se aplicaría.</param>
        /// <returns>El daño modificado (deja 1 HP) si puede prevenir OHKO, o el daño original en caso contrario.</returns>
        /// <exception cref="ArgumentNullException">Si pokemon es null.</exception>
        public int CalculateModifiedDamage(PokemonInstance pokemon, int originalDamage)
        {
            if (pokemon == null)
                throw new ArgumentNullException(nameof(pokemon));

            if (!HasBehavior(pokemon))
                return originalDamage;

            // Reducir daño para dejar al Pokemon con 1 HP
            return Math.Max(0, pokemon.CurrentHP - 1);
        }
    }

    /// <summary>
    /// Tipo de prevención de OHKO.
    /// </summary>
    public enum OHKOPreventionType
    {
        /// <summary>
        /// No puede prevenir OHKO.
        /// </summary>
        None,

        /// <summary>
        /// Previene OHKO mediante item (Focus Sash).
        /// </summary>
        Item,

        /// <summary>
        /// Previene OHKO mediante habilidad (Sturdy).
        /// </summary>
        Ability
    }
}
