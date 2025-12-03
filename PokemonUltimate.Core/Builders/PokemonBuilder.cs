using System;
using System.Collections.Generic;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Content.Builders
{
    // Note: AbilityData must be imported from PokemonUltimate.Core.Blueprints
    /// <summary>
    /// Fluent builder for creating PokemonSpeciesData instances.
    /// Usage: Pokemon.Define("Pikachu", 25).Type(Electric).Stats(...).Build()
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.11: Builders
    /// **Documentation**: See `docs/features/1-game-data/1.11-builders/architecture.md`
    /// </remarks>
    public class PokemonBuilder
    {
        private readonly PokemonSpeciesData _pokemon;
        private readonly List<Core.Evolution.Evolution> _evolutions = new List<Core.Evolution.Evolution>();
        private List<LearnableMove> _learnset = new List<LearnableMove>();

        private PokemonBuilder(string name, int pokedexNumber)
        {
            _pokemon = new PokemonSpeciesData
            {
                Name = name,
                PokedexNumber = pokedexNumber
            };
        }

        /// <summary>
        /// Start defining a new Pokemon.
        /// </summary>
        public static PokemonBuilder Define(string name, int pokedexNumber)
        {
            return new PokemonBuilder(name, pokedexNumber);
        }

        /// <summary>
        /// Set the Pokemon's primary type (mono-type).
        /// </summary>
        public PokemonBuilder Type(PokemonType primary)
        {
            _pokemon.PrimaryType = primary;
            _pokemon.SecondaryType = null;
            return this;
        }

        /// <summary>
        /// Set the Pokemon's types (dual-type).
        /// </summary>
        public PokemonBuilder Types(PokemonType primary, PokemonType secondary)
        {
            _pokemon.PrimaryType = primary;
            _pokemon.SecondaryType = secondary;
            return this;
        }

        /// <summary>
        /// Set the Pokemon's base stats.
        /// </summary>
        public PokemonBuilder Stats(int hp, int attack, int defense, int spAttack, int spDefense, int speed)
        {
            _pokemon.BaseStats = new BaseStats(hp, attack, defense, spAttack, spDefense, speed);
            return this;
        }

        /// <summary>
        /// Set the Pokemon's gender ratio (percentage chance of being male).
        /// Use Genderless(), MaleOnly(), or FemaleOnly() for special cases.
        /// </summary>
        public PokemonBuilder GenderRatio(float malePercentage)
        {
            _pokemon.GenderRatio = malePercentage;
            return this;
        }

        /// <summary>
        /// Mark this Pokemon as genderless (Magnemite, Ditto, etc.).
        /// </summary>
        public PokemonBuilder Genderless()
        {
            _pokemon.GenderRatio = -1f;
            return this;
        }

        /// <summary>
        /// Mark this Pokemon as male-only (Tauros, Hitmonlee, etc.).
        /// </summary>
        public PokemonBuilder MaleOnly()
        {
            _pokemon.GenderRatio = 100f;
            return this;
        }

        /// <summary>
        /// Mark this Pokemon as female-only (Chansey, Jynx, etc.).
        /// </summary>
        public PokemonBuilder FemaleOnly()
        {
            _pokemon.GenderRatio = 0f;
            return this;
        }

        #region Abilities

        /// <summary>
        /// Set the Pokemon's primary ability.
        /// </summary>
        public PokemonBuilder Ability(AbilityData ability)
        {
            _pokemon.Ability1 = ability;
            return this;
        }

        /// <summary>
        /// Set the Pokemon's abilities (primary and optional secondary).
        /// </summary>
        public PokemonBuilder Abilities(AbilityData primary, AbilityData secondary = null)
        {
            _pokemon.Ability1 = primary;
            _pokemon.Ability2 = secondary;
            return this;
        }

        /// <summary>
        /// Set the Pokemon's hidden ability.
        /// </summary>
        public PokemonBuilder HiddenAbility(AbilityData ability)
        {
            _pokemon.HiddenAbility = ability;
            return this;
        }

        #endregion

        /// <summary>
        /// Define the Pokemon's learnset using a fluent builder.
        /// </summary>
        public PokemonBuilder Moves(Action<LearnsetBuilder> configure)
        {
            var builder = new LearnsetBuilder();
            configure(builder);
            _learnset = builder.Build();
            return this;
        }

        /// <summary>
        /// Define an evolution for this Pokemon.
        /// </summary>
        public PokemonBuilder EvolvesTo(PokemonSpeciesData target, Action<EvolutionBuilder> configure)
        {
            var builder = new EvolutionBuilder(target);
            configure(builder);
            _evolutions.Add(builder.Build());
            return this;
        }

        /// <summary>
        /// Build the final PokemonSpeciesData instance.
        /// </summary>
        public PokemonSpeciesData Build()
        {
            _pokemon.Learnset = _learnset;
            _pokemon.Evolutions = _evolutions;
            return _pokemon;
        }
    }

    /// <summary>
    /// Static entry point for the Pokemon builder.
    /// </summary>
    public static class Pokemon
    {
        /// <summary>
        /// Start defining a new Pokemon species.
        /// </summary>
        public static PokemonBuilder Define(string name, int pokedexNumber)
        {
            return PokemonBuilder.Define(name, pokedexNumber);
        }
    }
}

