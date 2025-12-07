using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Combat.Foundation.Field;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Infrastructure.Validation
{
    /// <summary>
    /// Validates battle state invariants and consistency.
    /// Ensures the battle state remains valid throughout execution.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.1: Battle Foundation
    /// **Documentation**: See `docs/features/2-combat-system/2.1-battle-foundation/architecture.md`
    /// </remarks>
    public class BattleStateValidator : IBattleStateValidator
    {
        private const int MinStatStage = -6;
        private const int MaxStatStage = 6;

        /// <summary>
        /// Validates that a slot's state is consistent.
        /// </summary>
        public IReadOnlyList<string> ValidateSlot(BattleSlot slot)
        {
            var errors = new List<string>();

            if (slot == null)
            {
                errors.Add("Slot cannot be null.");
                return errors;
            }

            // Validate stat stages
            errors.AddRange(ValidateStatStages(slot));

            // Validate state counters
            errors.AddRange(ValidateStateCounters(slot));

            // Validate Pokemon consistency
            if (!slot.IsEmpty && slot.Pokemon == null)
            {
                errors.Add($"Slot {slot.SlotIndex} is marked as non-empty but Pokemon is null.");
            }

            if (slot.IsEmpty && slot.Pokemon != null)
            {
                errors.Add($"Slot {slot.SlotIndex} is marked as empty but has a Pokemon.");
            }

            return errors;
        }

        /// <summary>
        /// Validates that a side's state is consistent.
        /// </summary>
        public IReadOnlyList<string> ValidateSide(BattleSide side)
        {
            var errors = new List<string>();

            if (side == null)
            {
                errors.Add("Side cannot be null.");
                return errors;
            }

            // Validate party consistency
            errors.AddRange(ValidatePartyConsistency(side));

            // Validate each slot
            foreach (var slot in side.Slots)
            {
                errors.AddRange(ValidateSlot(slot));
            }

            return errors;
        }

        /// <summary>
        /// Validates that the battlefield state is consistent.
        /// </summary>
        public IReadOnlyList<string> ValidateField(BattleField field)
        {
            var errors = new List<string>();

            if (field == null)
            {
                errors.Add("Field cannot be null.");
                return errors;
            }

            // Validate player side
            if (field.PlayerSide == null)
            {
                errors.Add("Player side cannot be null.");
            }
            else
            {
                errors.AddRange(ValidateSide(field.PlayerSide));
            }

            // Validate enemy side
            if (field.EnemySide == null)
            {
                errors.Add("Enemy side cannot be null.");
            }
            else
            {
                errors.AddRange(ValidateSide(field.EnemySide));
            }

            // Validate weather state
            if (field.Weather != Weather.None && field.WeatherDuration < 0)
            {
                errors.Add($"Weather {field.Weather} has invalid duration: {field.WeatherDuration}");
            }

            // Validate terrain state
            if (field.Terrain != Terrain.None && field.TerrainDuration < 0)
            {
                errors.Add($"Terrain {field.Terrain} has invalid duration: {field.TerrainDuration}");
            }

            return errors;
        }

        /// <summary>
        /// Validates stat stages are within valid range (-6 to +6).
        /// </summary>
        public IReadOnlyList<string> ValidateStatStages(BattleSlot slot)
        {
            var errors = new List<string>();

            if (slot == null)
                return errors;

            var statsToCheck = new[]
            {
                Stat.Attack,
                Stat.Defense,
                Stat.SpAttack,
                Stat.SpDefense,
                Stat.Speed,
                Stat.Accuracy,
                Stat.Evasion
            };

            foreach (var stat in statsToCheck)
            {
                var stage = slot.GetStatStage(stat);
                if (stage < MinStatStage || stage > MaxStatStage)
                {
                    errors.Add($"Slot {slot.SlotIndex} has invalid stat stage for {stat}: {stage} (must be between {MinStatStage} and {MaxStatStage})");
                }
            }

            return errors;
        }

        /// <summary>
        /// Validates that state counters are within valid ranges.
        /// </summary>
        public IReadOnlyList<string> ValidateStateCounters(BattleSlot slot)
        {
            var errors = new List<string>();

            if (slot == null)
                return errors;

            // Validate protect consecutive uses (should be non-negative)
            if (slot.ProtectConsecutiveUses < 0)
            {
                errors.Add($"Slot {slot.SlotIndex} has invalid ProtectConsecutiveUses: {slot.ProtectConsecutiveUses} (must be >= 0)");
            }

            // Validate damage tracking (should be non-negative)
            if (slot.PhysicalDamageTakenThisTurn < 0)
            {
                errors.Add($"Slot {slot.SlotIndex} has invalid PhysicalDamageTakenThisTurn: {slot.PhysicalDamageTakenThisTurn} (must be >= 0)");
            }

            if (slot.SpecialDamageTakenThisTurn < 0)
            {
                errors.Add($"Slot {slot.SlotIndex} has invalid SpecialDamageTakenThisTurn: {slot.SpecialDamageTakenThisTurn} (must be >= 0)");
            }

            return errors;
        }

        /// <summary>
        /// Validates that party consistency is maintained.
        /// </summary>
        public IReadOnlyList<string> ValidatePartyConsistency(BattleSide side)
        {
            var errors = new List<string>();

            if (side == null || side.Party == null)
                return errors;

            // Get all active Pokemon from slots
            var activePokemon = side.Slots
                .Where(s => !s.IsEmpty && s.Pokemon != null)
                .Select(s => s.Pokemon)
                .ToList();

            // Check that all active Pokemon are in the party
            foreach (var pokemon in activePokemon)
            {
                if (!side.Party.Contains(pokemon))
                {
                    errors.Add($"Active Pokemon {pokemon.DisplayName} is not in the party.");
                }
            }

            // Check that party Pokemon are not duplicated in slots
            var activePokemonSet = activePokemon.ToHashSet();
            if (activePokemonSet.Count != activePokemon.Count)
            {
                errors.Add("Duplicate Pokemon found in active slots.");
            }

            return errors;
        }
    }
}
