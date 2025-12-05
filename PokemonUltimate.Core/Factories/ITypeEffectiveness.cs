using System.Collections.Generic;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Factories
{
    /// <summary>
    /// Calculates type effectiveness for damage calculations.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.8: Type Effectiveness Table
    /// **Documentation**: See `docs/features/1-game-data/1.8-type-effectiveness-table/README.md`
    /// </remarks>
    public interface ITypeEffectiveness
    {
        /// <summary>
        /// Gets the type effectiveness multiplier for a single defender type.
        /// </summary>
        float GetEffectiveness(PokemonType attackType, PokemonType defenderType);

        /// <summary>
        /// Gets the combined type effectiveness for a dual-type defender.
        /// </summary>
        float GetEffectiveness(PokemonType attackType, PokemonType primaryType, PokemonType? secondaryType);

        /// <summary>
        /// Checks if the attack is super effective (>1x).
        /// </summary>
        bool IsSuperEffective(float effectiveness);

        /// <summary>
        /// Checks if the attack is not very effective (<1x but >0x).
        /// </summary>
        bool IsNotVeryEffective(float effectiveness);

        /// <summary>
        /// Checks if the defender is immune (0x).
        /// </summary>
        bool IsImmune(float effectiveness);

        /// <summary>
        /// Gets a human-readable description of the effectiveness.
        /// </summary>
        string GetEffectivenessDescription(float effectiveness);

        /// <summary>
        /// Calculates STAB bonus if the move type matches the attacker's type.
        /// </summary>
        float GetSTABMultiplier(PokemonType moveType, PokemonType primaryType, PokemonType? secondaryType);

        /// <summary>
        /// Gets all types that the given type is super effective against.
        /// </summary>
        List<PokemonType> GetSuperEffectiveAgainst(PokemonType attackType);

        /// <summary>
        /// Gets all types that the given type is resisted by.
        /// </summary>
        List<PokemonType> GetResistedBy(PokemonType attackType);

        /// <summary>
        /// Gets all types that are immune to the given attack type.
        /// </summary>
        List<PokemonType> GetImmuneTypes(PokemonType attackType);
    }
}
