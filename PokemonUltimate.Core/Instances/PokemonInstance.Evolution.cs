using System;
using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Evolution.Conditions;

namespace PokemonUltimate.Core.Instances
{
    /// <summary>
    /// Evolution-related methods for PokemonInstance.
    /// Handles evolution checking, triggering, and transformation.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.1: Pokemon Data
    /// **Documentation**: See `docs/features/1-game-data/1.1-pokemon-data/README.md`
    /// </remarks>
    public partial class PokemonInstance
    {
        #region Evolution Queries

        /// <summary>
        /// Checks if this Pokemon can evolve (has any valid evolution path).
        /// Only checks auto-checkable conditions (level, friendship, knows move).
        /// For item/trade/time evolutions, use CanEvolveWith* methods.
        /// </summary>
        public bool CanEvolve()
        {
            return GetAvailableEvolution() != null;
        }

        /// <summary>
        /// Checks if this Pokemon can evolve using a specific item.
        /// </summary>
        public bool CanEvolveWithItem(string itemName)
        {
            return GetEvolutionWithItem(itemName) != null;
        }

        /// <summary>
        /// Checks if this Pokemon can evolve via trade.
        /// </summary>
        public bool CanEvolveByTrade()
        {
            return GetEvolutionByTrade() != null;
        }

        /// <summary>
        /// Gets the first available evolution (meeting all auto-checkable conditions).
        /// Returns null if no evolution is available.
        /// </summary>
        public Core.Evolution.Evolution GetAvailableEvolution()
        {
            if (Species.Evolutions == null || Species.Evolutions.Count == 0)
                return null;

            return Species.Evolutions.FirstOrDefault(e => e.CanEvolve(this));
        }

        /// <summary>
        /// Gets the evolution that would trigger with the specified item.
        /// Returns null if no matching evolution exists.
        /// </summary>
        public Core.Evolution.Evolution GetEvolutionWithItem(string itemName)
        {
            if (string.IsNullOrEmpty(itemName))
                return null;
            if (Species.Evolutions == null || Species.Evolutions.Count == 0)
                return null;

            foreach (var evolution in Species.Evolutions)
            {
                var itemCondition = evolution.GetCondition<ItemCondition>();
                if (itemCondition != null && itemCondition.IsMet(this, itemName))
                {
                    // Check other conditions too (except item which we just checked)
                    bool allOthersMet = evolution.Conditions
                        .Where(c => !(c is ItemCondition))
                        .All(c => c.IsMet(this));
                    
                    if (allOthersMet)
                        return evolution;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the evolution that would trigger via trade.
        /// Returns null if no matching evolution exists.
        /// </summary>
        public Core.Evolution.Evolution GetEvolutionByTrade()
        {
            if (Species.Evolutions == null || Species.Evolutions.Count == 0)
                return null;

            foreach (var evolution in Species.Evolutions)
            {
                var tradeCondition = evolution.GetCondition<TradeCondition>();
                if (tradeCondition != null && tradeCondition.IsMet(this, wasTraded: true))
                {
                    // Check other conditions too (except trade which we just checked)
                    bool allOthersMet = evolution.Conditions
                        .Where(c => !(c is TradeCondition))
                        .All(c => c.IsMet(this));
                    
                    if (allOthersMet)
                        return evolution;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets all evolutions this Pokemon can currently perform.
        /// </summary>
        public List<Core.Evolution.Evolution> GetAvailableEvolutions()
        {
            if (Species.Evolutions == null || Species.Evolutions.Count == 0)
                return new List<Core.Evolution.Evolution>();

            return Species.Evolutions.Where(e => e.CanEvolve(this)).ToList();
        }

        /// <summary>
        /// Gets all possible evolution targets for this Pokemon (regardless of conditions).
        /// Useful for UI to show what evolutions exist.
        /// </summary>
        public List<PokemonSpeciesData> GetPossibleEvolutions()
        {
            if (Species.Evolutions == null || Species.Evolutions.Count == 0)
                return new List<PokemonSpeciesData>();

            return Species.Evolutions.Select(e => e.Target).ToList();
        }

        #endregion

        #region Evolution Execution

        /// <summary>
        /// Evolves this Pokemon into the specified target species.
        /// Does NOT check conditions - use TryEvolve() for safe evolution.
        /// Preserves nickname, moves, nature, gender, friendship, experience.
        /// Recalculates stats for the new species.
        /// </summary>
        public bool Evolve(PokemonSpeciesData targetSpecies)
        {
            if (targetSpecies == null)
                throw new ArgumentNullException(nameof(targetSpecies), ErrorMessages.TargetSpeciesCannotBeNull);

            // Verify this is a valid evolution target (must be in evolutions list)
            var evolution = Species.Evolutions?.FirstOrDefault(e => e.Target == targetSpecies);
            if (evolution == null)
                throw new ArgumentException(
                    ErrorMessages.Format(ErrorMessages.EvolutionTargetNotValid, Species.Name, targetSpecies.Name),
                    nameof(targetSpecies));

            return ExecuteEvolution(targetSpecies);
        }

        /// <summary>
        /// Evolves using a specific item.
        /// Checks that the item evolution is valid before proceeding.
        /// </summary>
        public PokemonSpeciesData EvolveWithItem(string itemName)
        {
            var evolution = GetEvolutionWithItem(itemName);
            if (evolution == null)
                return null;

            return ExecuteEvolution(evolution.Target) ? evolution.Target : null;
        }

        /// <summary>
        /// Evolves via trade.
        /// Checks that trade evolution is valid before proceeding.
        /// </summary>
        public PokemonSpeciesData EvolveByTrade()
        {
            var evolution = GetEvolutionByTrade();
            if (evolution == null)
                return null;

            return ExecuteEvolution(evolution.Target) ? evolution.Target : null;
        }

        /// <summary>
        /// Attempts to evolve this Pokemon using the first available evolution.
        /// Only works for auto-checkable evolutions (level, friendship, knows move).
        /// Returns the evolved species or null if evolution failed.
        /// </summary>
        public PokemonSpeciesData TryEvolve()
        {
            var evolution = GetAvailableEvolution();
            if (evolution == null)
                return null;

            return ExecuteEvolution(evolution.Target) ? evolution.Target : null;
        }

        /// <summary>
        /// Internal method that performs the actual evolution transformation.
        /// </summary>
        private bool ExecuteEvolution(PokemonSpeciesData targetSpecies)
        {
            if (targetSpecies == null)
                return false;

            // Store HP percentage to maintain proportion
            float hpPercent = MaxHP > 0 ? (float)CurrentHP / MaxHP : 1f;

            // Change species
            Species = targetSpecies;

            // Recalculate stats for new species
            RecalculateStats();

            // Restore HP proportionally (minimum 1 HP if not fainted)
            int newHP = (int)(MaxHP * hpPercent);
            CurrentHP = hpPercent > 0 ? Math.Max(1, newHP) : 0;

            // Reset move check level since species changed
            _lastMoveCheckLevel = Level;

            return true;
        }

        #endregion
    }
}

