using System;
using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Combat.Effects;
using PokemonUltimate.Combat.Events;
using PokemonUltimate.Combat.Factories;
using PokemonUltimate.Combat.Helpers;
using PokemonUltimate.Combat.Messages;
using PokemonUltimate.Combat.Processors.Phases;
using PokemonUltimate.Combat.Providers;
using PokemonUltimate.Content.Catalogs.Field;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Core.Domain.Instances.Move;
using PokemonUltimate.Core.Infrastructure.Factories;
using PokemonUltimate.Core.Domain.Instances;
using PokemonInstance = PokemonUltimate.Core.Domain.Instances.Pokemon.PokemonInstance;

namespace PokemonUltimate.DeveloperTools.Runners
{
    /// <summary>
    /// Calculates turn order and visualizes speed calculations for the Turn Order Debugger.
    /// </summary>
    /// <remarks>
    /// **Feature**: 6: Development Tools
    /// **Sub-Feature**: 6.4: Turn Order Debugger
    /// **Documentation**: See `docs/features/6-development-tools/6.4-turn-order-debugger/README.md`
    /// </remarks>
    public class TurnOrderRunner
    {
        /// <summary>
        /// Configuration for a single Pokemon in turn order calculation.
        /// </summary>
        public class PokemonConfig
        {
            public PokemonSpeciesData Species { get; set; } = null!;
            public MoveData Move { get; set; } = null!;
            public int Level { get; set; } = 50;
            public bool HasParalysis { get; set; } = false;
            public bool HasTailwind { get; set; } = false;
            public int SpeedStatStage { get; set; } = 0;
            public string Name { get; set; } = string.Empty; // Display name for this Pokemon
        }

        /// <summary>
        /// Speed calculation breakdown for a Pokemon.
        /// </summary>
        public class SpeedCalculation
        {
            public string PokemonName { get; set; } = string.Empty;
            public float BaseSpeed { get; set; }
            public float StatStageMultiplier { get; set; }
            public float StatusMultiplier { get; set; }
            public float ItemMultiplier { get; set; }
            public float SideConditionMultiplier { get; set; }
            public float AbilityMultiplier { get; set; }
            public float EffectiveSpeed { get; set; }
            public string Details { get; set; } = string.Empty;
        }

        /// <summary>
        /// Priority information for an action.
        /// </summary>
        public class PriorityInfo
        {
            public string PokemonName { get; set; } = string.Empty;
            public string MoveName { get; set; } = string.Empty;
            public int Priority { get; set; }
            public string Description { get; set; } = string.Empty;
        }

        /// <summary>
        /// Final turn order entry.
        /// </summary>
        public class TurnOrderEntry
        {
            public int Position { get; set; }
            public string PokemonName { get; set; } = string.Empty;
            public string MoveName { get; set; } = string.Empty;
            public int Priority { get; set; }
            public float EffectiveSpeed { get; set; }
            public string Reasoning { get; set; } = string.Empty;
        }

        /// <summary>
        /// Complete turn order calculation result.
        /// </summary>
        public class TurnOrderResult
        {
            public List<SpeedCalculation> SpeedCalculations { get; set; } = new List<SpeedCalculation>();
            public List<PriorityInfo> PriorityInfo { get; set; } = new List<PriorityInfo>();
            public List<TurnOrderEntry> TurnOrder { get; set; } = new List<TurnOrderEntry>();
        }

        /// <summary>
        /// Calculates turn order for the given Pokemon configurations.
        /// </summary>
        public TurnOrderResult CalculateTurnOrder(List<PokemonConfig> configs)
        {
            if (configs == null || configs.Count == 0)
                throw new ArgumentException("At least one Pokemon configuration is required", nameof(configs));

            var result = new TurnOrderResult();

            // Create BattleField
            var field = new BattleField();
            var playerParty = new List<PokemonInstance>();
            var enemyParty = new List<PokemonInstance>();

            // Create Pokemon instances
            var pokemonInstances = new List<PokemonInstance>();
            for (int i = 0; i < configs.Count; i++)
            {
                var config = configs[i];
                if (config.Species == null)
                    throw new ArgumentException($"Pokemon configuration at index {i} has null Species", nameof(configs));
                if (config.Move == null)
                    throw new ArgumentException($"Pokemon configuration at index {i} has null Move", nameof(configs));

                // Create Pokemon instance
                var pokemon = PokemonFactory.Create(config.Species, config.Level);

                // Apply status
                if (config.HasParalysis)
                {
                    pokemon.Status = PersistentStatus.Paralysis;
                }

                pokemonInstances.Add(pokemon);

                // Add to party (alternate between player and enemy for simplicity)
                if (i % 2 == 0)
                {
                    playerParty.Add(pokemon);
                }
                else
                {
                    enemyParty.Add(pokemon);
                }
            }

            // Create custom battle rules for debugger - allow multiple slots
            // For 4 Pokemon, we need at least 2 slots per side
            var debuggerRules = new BattleRules
            {
                PlayerSlots = Math.Max(2, (configs.Count + 1) / 2), // At least 2 slots, or more if needed
                EnemySlots = Math.Max(2, configs.Count / 2), // At least 2 slots
                MaxTurns = 0,
                AllowItems = true,
                AllowSwitching = true
            };

            // Initialize field with custom rules
            field.Initialize(debuggerRules, playerParty, enemyParty);

            // Get all slots and configure them
            var allSlots = new List<BattleSlot>();
            allSlots.AddRange(field.PlayerSide.Slots);
            allSlots.AddRange(field.EnemySide.Slots);

            // Create a mapping of Pokemon instance to slot
            // This ensures we can find the correct slot for each Pokemon even if species IDs match
            var pokemonToSlotMap = new Dictionary<PokemonInstance, BattleSlot>();
            foreach (var slot in allSlots)
            {
                if (slot.Pokemon != null)
                {
                    pokemonToSlotMap[slot.Pokemon] = slot;
                }
            }

            // Debug: Verify all Pokemon instances are mapped
            // If a Pokemon instance is not in the map, it means it wasn't placed in a slot
            // In that case, we need to manually place it
            for (int i = 0; i < pokemonInstances.Count; i++)
            {
                var pokemon = pokemonInstances[i];
                if (!pokemonToSlotMap.ContainsKey(pokemon))
                {
                    // Find an empty slot for this Pokemon
                    BattleSlot? emptySlot = null;
                    if (i % 2 == 0)
                    {
                        // Player side
                        var playerSlotIndex = i / 2;
                        if (playerSlotIndex < field.PlayerSide.Slots.Count)
                        {
                            emptySlot = field.PlayerSide.Slots[playerSlotIndex];
                            if (emptySlot.Pokemon == null)
                            {
                                emptySlot.SetPokemon(pokemon);
                                pokemonToSlotMap[pokemon] = emptySlot;
                            }
                        }
                    }
                    else
                    {
                        // Enemy side
                        var enemySlotIndex = i / 2;
                        if (enemySlotIndex < field.EnemySide.Slots.Count)
                        {
                            emptySlot = field.EnemySide.Slots[enemySlotIndex];
                            if (emptySlot.Pokemon == null)
                            {
                                emptySlot.SetPokemon(pokemon);
                                pokemonToSlotMap[pokemon] = emptySlot;
                            }
                        }
                    }
                }
            }

            // Also create species ID mapping for fallback
            var speciesToSlotMap = new Dictionary<string, List<BattleSlot>>();
            foreach (var slot in allSlots)
            {
                if (slot.Pokemon != null && slot.Pokemon.Species != null)
                {
                    if (!speciesToSlotMap.ContainsKey(slot.Pokemon.Species.Id))
                    {
                        speciesToSlotMap[slot.Pokemon.Species.Id] = new List<BattleSlot>();
                    }
                    speciesToSlotMap[slot.Pokemon.Species.Id].Add(slot);
                }
            }

            // Create a mapping of slot to config index for easy lookup
            var slotToConfigIndexMap = new Dictionary<BattleSlot, int>();

            // Apply configurations to slots
            var actions = new List<BattleAction>();
            for (int i = 0; i < configs.Count; i++)
            {
                var config = configs[i];
                var pokemon = pokemonInstances[i];

                // Find the slot for this Pokemon instance
                BattleSlot? slot = null;

                // First try: direct Pokemon instance mapping (most reliable)
                if (pokemonToSlotMap.TryGetValue(pokemon, out var mappedSlot))
                {
                    slot = mappedSlot;
                }
                // Second try: find by species ID (handle duplicates)
                else if (speciesToSlotMap.TryGetValue(config.Species.Id, out var slotsWithSpecies))
                {
                    // Find a slot that hasn't been mapped yet, or use the first available
                    slot = slotsWithSpecies.FirstOrDefault(s => s.Pokemon != null && !slotToConfigIndexMap.ContainsKey(s))
                        ?? slotsWithSpecies.FirstOrDefault(s => s.Pokemon != null);
                }
                // Third try: use slot index based on party distribution
                else
                {
                    // Calculate which slot this Pokemon should be in based on party distribution
                    if (i % 2 == 0)
                    {
                        // Player side
                        var playerSlotIndex = i / 2;
                        if (playerSlotIndex < field.PlayerSide.Slots.Count)
                        {
                            slot = field.PlayerSide.Slots[playerSlotIndex];
                        }
                    }
                    else
                    {
                        // Enemy side
                        var enemySlotIndex = i / 2;
                        if (enemySlotIndex < field.EnemySide.Slots.Count)
                        {
                            slot = field.EnemySide.Slots[enemySlotIndex];
                        }
                    }
                }

                // Final fallback: use index if still not found
                if (slot == null && i < allSlots.Count)
                {
                    slot = allSlots[i];
                }

                // If slot is null, try to find any empty slot
                if (slot == null)
                {
                    slot = allSlots.FirstOrDefault(s => s.Pokemon == null);
                }

                // If slot exists but has no Pokemon, place this Pokemon there
                if (slot != null && slot.Pokemon == null)
                {
                    slot.SetPokemon(pokemon);
                    pokemonToSlotMap[pokemon] = slot;
                }

                // If slot is null, try to find a slot by index based on party distribution
                if (slot == null)
                {
                    if (i % 2 == 0)
                    {
                        // Player side
                        var playerSlotIndex = i / 2;
                        if (playerSlotIndex < field.PlayerSide.Slots.Count)
                        {
                            slot = field.PlayerSide.Slots[playerSlotIndex];
                            if (slot.Pokemon == null)
                            {
                                slot.SetPokemon(pokemon);
                                pokemonToSlotMap[pokemon] = slot;
                            }
                        }
                    }
                    else
                    {
                        // Enemy side
                        var enemySlotIndex = i / 2;
                        if (enemySlotIndex < field.EnemySide.Slots.Count)
                        {
                            slot = field.EnemySide.Slots[enemySlotIndex];
                            if (slot.Pokemon == null)
                            {
                                slot.SetPokemon(pokemon);
                                pokemonToSlotMap[pokemon] = slot;
                            }
                        }
                    }
                }

                // If still no slot or Pokemon, skip this config
                if (slot == null || slot.Pokemon == null)
                    continue;

                // Map slot to config index for later lookup
                slotToConfigIndexMap[slot] = i;

                // Set stat stages for speed
                if (config.SpeedStatStage != 0)
                {
                    var currentStage = slot.GetStatStage(Stat.Speed);
                    var change = config.SpeedStatStage - currentStage;
                    slot.ModifyStatStage(Stat.Speed, change);
                }

                // Apply Tailwind to the side
                if (config.HasTailwind && slot.Side != null)
                {
                    var tailwindData = SideConditionCatalog.Tailwind;
                    if (tailwindData != null)
                    {
                        slot.Side.AddSideCondition(tailwindData, 4); // 4 turns
                    }
                }

                // Create UseMoveAction for this Pokemon
                var moveInstance = slot.Pokemon.Moves.FirstOrDefault(m => m.Move.Id == config.Move.Id);
                if (moveInstance == null)
                {
                    // Try to learn the move first
                    bool learned = slot.Pokemon.TryLearnMove(config.Move);

                    // Check if it was added to the Pokemon's moves
                    moveInstance = slot.Pokemon.Moves.FirstOrDefault(m => m.Move.Id == config.Move.Id);

                    // If still null (e.g., Pokemon has 4 moves already), create a temporary instance
                    // This is fine for debugging purposes - we just need the move data
                    if (moveInstance == null)
                    {
                        moveInstance = new MoveInstance(config.Move);
                    }
                }

                // Find target (use first enemy slot or first other slot with Pokemon)
                // Prioritize slots with Pokemon, but fallback to any other slot if needed
                var target = allSlots.FirstOrDefault(s => s != slot && s.Pokemon != null);
                if (target == null)
                {
                    // If no Pokemon slot found, use any other slot (even empty)
                    target = allSlots.FirstOrDefault(s => s != slot);
                }

                // If still no target found (shouldn't happen with 2+ Pokemon), skip this action
                if (target == null)
                {
                    continue;
                }

                if (target != null && moveInstance != null)
                {
                    var randomProvider = new RandomProvider();
                    var action = new UseMoveAction(
                        slot,
                        target,
                        moveInstance,
                        randomProvider,
                        new AccuracyChecker(randomProvider),
                        new DamagePipeline(randomProvider),
                        new MoveEffectProcessorRegistry(randomProvider, new DamageContextFactory()),
                        new BattleMessageFormatter(),
                        new BeforeMoveProcessor(),
                        new AfterMoveProcessor(),
                        new TargetResolver()
                    );

                    actions.Add(action);
                }
            }

            // Calculate speed breakdowns
            var resolver = new TurnOrderResolver(new RandomProvider());
            for (int i = 0; i < configs.Count; i++)
            {
                var config = configs[i];
                var pokemon = pokemonInstances[i];

                BattleSlot? slot = null;
                if (pokemonToSlotMap.TryGetValue(pokemon, out var mappedSlot))
                {
                    slot = mappedSlot;
                }
                else if (speciesToSlotMap.TryGetValue(config.Species.Id, out var slotsWithSpecies))
                {
                    slot = slotsWithSpecies.FirstOrDefault(s => s.Pokemon != null);
                }
                else if (i < allSlots.Count)
                {
                    slot = allSlots[i];
                }

                if (slot != null && slot.Pokemon != null)
                {
                    var speedCalc = CalculateSpeedBreakdown(slot, field, resolver);
                    speedCalc.PokemonName = config.Name ?? config.Species.Name;
                    result.SpeedCalculations.Add(speedCalc);
                }
            }

            // Get priority info
            foreach (var action in actions)
            {
                if (action is UseMoveAction moveAction)
                {
                    // Find config by slot instead of by move (to handle multiple Pokemon with same move)
                    var configIndex = -1;
                    if (moveAction.User != null && slotToConfigIndexMap.TryGetValue(moveAction.User, out var idx))
                    {
                        configIndex = idx;
                    }
                    var config = configIndex >= 0 && configIndex < configs.Count ? configs[configIndex] : null;
                    var priority = resolver.GetPriority(action);
                    result.PriorityInfo.Add(new PriorityInfo
                    {
                        PokemonName = (config?.Name ?? action.User?.Pokemon?.Species?.Name ?? "Unknown")!,
                        MoveName = moveAction.MoveInstance.Move.Name,
                        Priority = priority,
                        Description = GetPriorityDescription(priority)
                    });
                }
            }

            // Sort actions using TurnOrderResolver
            var sortedActions = resolver.SortActions(actions, field);

            // Build turn order entries
            for (int i = 0; i < sortedActions.Count; i++)
            {
                var action = sortedActions[i];
                if (action is UseMoveAction moveAction)
                {
                    // Find config by slot instead of by move (to handle multiple Pokemon with same move)
                    var configIndex = -1;
                    if (moveAction.User != null && slotToConfigIndexMap.TryGetValue(moveAction.User, out var idx))
                    {
                        configIndex = idx;
                    }
                    var config = configIndex >= 0 && configIndex < configs.Count ? configs[configIndex] : null;
                    var effectiveSpeed = resolver.GetEffectiveSpeed(action.User, field);
                    var priority = resolver.GetPriority(action);

                    result.TurnOrder.Add(new TurnOrderEntry
                    {
                        Position = i + 1,
                        PokemonName = (config?.Name ?? action.User?.Pokemon?.Species?.Name ?? "Unknown")!,
                        MoveName = moveAction.MoveInstance.Move.Name,
                        Priority = priority,
                        EffectiveSpeed = effectiveSpeed,
                        Reasoning = GetReasoning(priority, effectiveSpeed, i, sortedActions)
                    });
                }
            }

            return result;
        }

        private SpeedCalculation CalculateSpeedBreakdown(BattleSlot slot, BattleField field, TurnOrderResolver resolver)
        {
            if (slot?.Pokemon == null)
                return new SpeedCalculation { EffectiveSpeed = 0 };

            var baseSpeed = slot.Pokemon.Speed;
            var effectiveSpeed = resolver.GetEffectiveSpeed(slot, field);

            // Calculate multipliers (simplified - we'll show the breakdown)
            var details = new List<string>();
            details.Add($"Base Speed: {baseSpeed:F2}");

            // Stat stage multiplier
            var statStage = slot.GetStatStage(Stat.Speed);
            var statStageMult = GetStatStageMultiplier(statStage);
            if (statStageMult != 1.0f)
            {
                details.Add($"Stat Stage ({statStage:+0;-0;0}): {statStageMult:F2}x");
            }

            // Status multiplier
            var statusMult = GetStatusMultiplier(slot.Pokemon.Status);
            if (statusMult != 1.0f)
            {
                details.Add($"Status ({slot.Pokemon.Status}): {statusMult:F2}x");
            }

            // Side condition multiplier (Tailwind)
            var sideMult = GetSideConditionMultiplier(slot.Side);
            if (sideMult != 1.0f)
            {
                details.Add($"Side Condition (Tailwind): {sideMult:F2}x");
            }

            // Item multiplier (simplified - would need to check actual item)
            var itemMult = 1.0f; // Default, could be enhanced

            // Ability multiplier (simplified - would need to check actual ability and weather)
            var abilityMult = 1.0f; // Default, could be enhanced

            return new SpeedCalculation
            {
                BaseSpeed = baseSpeed,
                StatStageMultiplier = statStageMult,
                StatusMultiplier = statusMult,
                ItemMultiplier = itemMult,
                SideConditionMultiplier = sideMult,
                AbilityMultiplier = abilityMult,
                EffectiveSpeed = effectiveSpeed,
                Details = string.Join(" | ", details)
            };
        }

        private static float GetStatStageMultiplier(int stages)
        {
            stages = Math.Max(-6, Math.Min(6, stages));
            if (stages >= 0)
            {
                return (2f + stages) / 2f;
            }
            else
            {
                return 2f / (2f - stages);
            }
        }

        private static float GetStatusMultiplier(PersistentStatus status)
        {
            if (status == PersistentStatus.Paralysis)
            {
                return 0.5f; // Paralysis halves speed
            }
            return 1.0f;
        }

        private static float GetSideConditionMultiplier(BattleSide side)
        {
            if (side == null)
                return 1.0f;

            // Check for Tailwind
            if (side.HasSideCondition(SideCondition.Tailwind))
            {
                var tailwindData = side.GetSideConditionData(SideCondition.Tailwind);
                if (tailwindData != null && tailwindData.ModifiesSpeed)
                {
                    return tailwindData.SpeedMultiplier;
                }
            }

            return 1.0f;
        }

        private string GetPriorityDescription(int priority)
        {
            if (priority > 0)
                return $"Priority +{priority} (goes before normal priority)";
            if (priority < 0)
                return $"Priority {priority} (goes after normal priority)";
            return "Normal priority (0)";
        }

        private string GetReasoning(int priority, float effectiveSpeed, int position, List<BattleAction> sortedActions)
        {
            var reasons = new List<string>();

            if (position == 0)
            {
                reasons.Add("First");
            }

            if (priority > 0)
            {
                reasons.Add($"Priority +{priority}");
            }
            else if (priority < 0)
            {
                reasons.Add($"Priority {priority}");
            }

            reasons.Add($"Speed: {effectiveSpeed:F2}");

            // Check if there are ties
            var samePriority = sortedActions.Where((a, idx) => idx != position &&
                new TurnOrderResolver(new RandomProvider()).GetPriority(a) == priority).ToList();
            if (samePriority.Count > 0)
            {
                reasons.Add("(Speed tie resolved randomly)");
            }

            return string.Join(" | ", reasons);
        }
    }
}

