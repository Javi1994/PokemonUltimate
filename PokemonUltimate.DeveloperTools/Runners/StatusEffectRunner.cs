using System;
using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Content.Catalogs.Status;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Core.Infrastructure.Factories;
using PokemonUltimate.Core.Domain.Instances;
using PokemonInstance = PokemonUltimate.Core.Domain.Instances.Pokemon.PokemonInstance;

namespace PokemonUltimate.DeveloperTools.Runners
{
    /// <summary>
    /// Tests status effects and their interactions for the Status Effect Debugger.
    /// </summary>
    /// <remarks>
    /// **Feature**: 6: Development Tools
    /// **Sub-Feature**: 6.3: Status Effect Debugger
    /// **Documentation**: See `docs/features/6-development-tools/6.3-status-effect-debugger/README.md`
    /// </remarks>
    public class StatusEffectRunner
    {
        /// <summary>
        /// Configuration for status effect testing.
        /// </summary>
        public class StatusEffectConfig
        {
            public PokemonSpeciesData PokemonSpecies { get; set; } = null!;
            public int Level { get; set; } = 50;
            public PersistentStatus PersistentStatus { get; set; } = PersistentStatus.None;
            public VolatileStatus VolatileStatus { get; set; } = VolatileStatus.None;
        }

        /// <summary>
        /// Stat modification information from status effects.
        /// </summary>
        public class StatModification
        {
            public Stat Stat { get; set; }
            public int BaseValue { get; set; }
            public int ModifiedValue { get; set; }
            public float Multiplier { get; set; }
            public string Description { get; set; } = string.Empty;
        }

        /// <summary>
        /// Damage per turn information from status effects.
        /// </summary>
        public class DamagePerTurn
        {
            public string StatusName { get; set; } = string.Empty;
            public float DamageFraction { get; set; }
            public int DamageAmount { get; set; }
            public int MaxHP { get; set; }
            public bool Escalates { get; set; }
            public string Description { get; set; } = string.Empty;
        }

        /// <summary>
        /// Status interaction information.
        /// </summary>
        public class StatusInteraction
        {
            public string StatusName { get; set; } = string.Empty;
            public bool CanApplyWithCurrentStatus { get; set; }
            public string Reason { get; set; } = string.Empty;
            public bool IsImmune { get; set; }
            public string ImmuneReason { get; set; } = string.Empty;
        }

        /// <summary>
        /// Complete status effect analysis result.
        /// </summary>
        public class StatusEffectResult
        {
            public PokemonInstance Pokemon { get; set; } = null!;
            public PersistentStatus CurrentPersistentStatus { get; set; }
            public VolatileStatus CurrentVolatileStatus { get; set; }
            public StatusEffectData PersistentStatusData { get; set; } = null!;
            public List<StatusEffectData> VolatileStatusDataList { get; set; } = new List<StatusEffectData>();
            public List<StatModification> StatModifications { get; set; } = new List<StatModification>();
            public List<DamagePerTurn> DamagePerTurnList { get; set; } = new List<DamagePerTurn>();
            public List<StatusInteraction> StatusInteractions { get; set; } = new List<StatusInteraction>();
        }

        /// <summary>
        /// Applies status effects and analyzes their effects.
        /// </summary>
        public StatusEffectResult ApplyStatus(StatusEffectConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            if (config.PokemonSpecies == null)
                throw new ArgumentException("PokemonSpecies cannot be null", nameof(config));

            // Create Pokemon instance
            var pokemon = PokemonFactory.Create(config.PokemonSpecies, config.Level);

            // Apply persistent status
            if (config.PersistentStatus != PersistentStatus.None)
            {
                pokemon.Status = config.PersistentStatus;
            }

            // Apply volatile status
            if (config.VolatileStatus != VolatileStatus.None)
            {
                pokemon.VolatileStatus = config.VolatileStatus;
            }

            // Build result
            var result = new StatusEffectResult
            {
                Pokemon = pokemon,
                CurrentPersistentStatus = pokemon.Status,
                CurrentVolatileStatus = pokemon.VolatileStatus
            };

            // Get status data
            if (pokemon.Status != PersistentStatus.None)
            {
                result.PersistentStatusData = StatusCatalog.GetByStatus(pokemon.Status);
            }

            // Get volatile status data
            if (pokemon.VolatileStatus != VolatileStatus.None)
            {
                foreach (VolatileStatus status in Enum.GetValues<VolatileStatus>())
                {
                    if (status != VolatileStatus.None && pokemon.VolatileStatus.HasFlag(status))
                    {
                        var statusData = StatusCatalog.GetByStatus(status);
                        if (statusData != null)
                        {
                            result.VolatileStatusDataList.Add(statusData);
                        }
                    }
                }
            }

            // Calculate stat modifications
            result.StatModifications = GetStatModifications(pokemon);

            // Calculate damage per turn
            result.DamagePerTurnList = GetDamagePerTurn(pokemon, result.PersistentStatusData, result.VolatileStatusDataList);

            // Calculate status interactions
            result.StatusInteractions = GetStatusInteractions(pokemon, config.PokemonSpecies);

            return result;
        }

        /// <summary>
        /// Gets stat modifications from status effects.
        /// </summary>
        private List<StatModification> GetStatModifications(PokemonInstance pokemon)
        {
            var modifications = new List<StatModification>();

            // Check Attack modification (Burn)
            if (pokemon.Status == PersistentStatus.Burn)
            {
                int baseAttack = pokemon.GetEffectiveStatRaw(Stat.Attack);
                int modifiedAttack = pokemon.GetEffectiveStat(Stat.Attack);
                modifications.Add(new StatModification
                {
                    Stat = Stat.Attack,
                    BaseValue = baseAttack,
                    ModifiedValue = modifiedAttack,
                    Multiplier = 0.5f,
                    Description = "Burn halves physical Attack"
                });
            }

            // Check Speed modification (Paralysis)
            if (pokemon.Status == PersistentStatus.Paralysis)
            {
                int baseSpeed = pokemon.GetEffectiveStatRaw(Stat.Speed);
                int modifiedSpeed = pokemon.GetEffectiveStat(Stat.Speed);
                modifications.Add(new StatModification
                {
                    Stat = Stat.Speed,
                    BaseValue = baseSpeed,
                    ModifiedValue = modifiedSpeed,
                    Multiplier = 0.25f,
                    Description = "Paralysis reduces Speed to 25%"
                });
            }

            return modifications;
        }

        /// <summary>
        /// Gets damage per turn from status effects.
        /// </summary>
        private List<DamagePerTurn> GetDamagePerTurn(PokemonInstance pokemon, StatusEffectData persistentData, List<StatusEffectData> volatileDataList)
        {
            var damageList = new List<DamagePerTurn>();

            // Persistent status damage
            if (persistentData != null && persistentData.EndOfTurnDamage > 0)
            {
                float damageFraction = persistentData.EndOfTurnDamage;
                if (persistentData.DamageEscalates)
                {
                    // For escalating damage, show first turn damage
                    damageFraction = persistentData.GetEscalatingDamage(1);
                }

                int damageAmount = (int)(pokemon.MaxHP * damageFraction);
                damageList.Add(new DamagePerTurn
                {
                    StatusName = persistentData.Name,
                    DamageFraction = damageFraction,
                    DamageAmount = damageAmount,
                    MaxHP = pokemon.MaxHP,
                    Escalates = persistentData.DamageEscalates,
                    Description = persistentData.DamageEscalates
                        ? $"Deals {(int)(damageFraction * 100)}% max HP damage, increasing each turn"
                        : $"Deals {(int)(damageFraction * 100)}% max HP damage per turn"
                });
            }

            // Volatile status damage
            foreach (var volatileData in volatileDataList)
            {
                if (volatileData.EndOfTurnDamage > 0)
                {
                    float damageFraction = volatileData.EndOfTurnDamage;
                    int damageAmount = (int)(pokemon.MaxHP * damageFraction);
                    damageList.Add(new DamagePerTurn
                    {
                        StatusName = volatileData.Name,
                        DamageFraction = damageFraction,
                        DamageAmount = damageAmount,
                        MaxHP = pokemon.MaxHP,
                        Escalates = false,
                        Description = $"Deals {(int)(damageFraction * 100)}% max HP damage per turn"
                    });
                }
            }

            return damageList;
        }

        /// <summary>
        /// Gets status interaction information (which statuses can/cannot be applied).
        /// </summary>
        private List<StatusInteraction> GetStatusInteractions(PokemonInstance pokemon, PokemonSpeciesData species)
        {
            var interactions = new List<StatusInteraction>();

            // Check all persistent statuses
            foreach (PersistentStatus status in Enum.GetValues<PersistentStatus>())
            {
                if (status == PersistentStatus.None) continue;

                var statusData = StatusCatalog.GetByStatus(status);
                if (statusData == null) continue;

                bool canApply = CanApplyPersistentStatus(pokemon, status, statusData, species, out string reason);
                bool isImmune = IsTypeImmune(species, statusData, out string immuneReason);

                interactions.Add(new StatusInteraction
                {
                    StatusName = statusData.Name,
                    CanApplyWithCurrentStatus = canApply && !isImmune,
                    Reason = isImmune ? immuneReason : reason,
                    IsImmune = isImmune,
                    ImmuneReason = immuneReason
                });
            }

            // Check all volatile statuses
            foreach (VolatileStatus status in Enum.GetValues<VolatileStatus>())
            {
                if (status == VolatileStatus.None) continue;

                var statusData = StatusCatalog.GetByStatus(status);
                if (statusData == null) continue;

                bool canApply = CanApplyVolatileStatus(pokemon, status, statusData, species, out string reason);
                bool isImmune = IsTypeImmune(species, statusData, out string immuneReason);

                interactions.Add(new StatusInteraction
                {
                    StatusName = statusData.Name,
                    CanApplyWithCurrentStatus = canApply && !isImmune,
                    Reason = isImmune ? immuneReason : reason,
                    IsImmune = isImmune,
                    ImmuneReason = immuneReason
                });
            }

            return interactions;
        }

        /// <summary>
        /// Checks if a persistent status can be applied.
        /// </summary>
        private bool CanApplyPersistentStatus(PokemonInstance pokemon, PersistentStatus status, StatusEffectData statusData, PokemonSpeciesData species, out string reason)
        {
            // Can only have one persistent status at a time
            if (pokemon.Status != PersistentStatus.None && pokemon.Status != status)
            {
                reason = $"Cannot apply {statusData.Name}: Pokemon already has {pokemon.Status}";
                return false;
            }

            // Already has this status
            if (pokemon.Status == status)
            {
                reason = $"Pokemon already has {statusData.Name}";
                return false;
            }

            reason = "Can be applied";
            return true;
        }

        /// <summary>
        /// Checks if a volatile status can be applied.
        /// </summary>
        private bool CanApplyVolatileStatus(PokemonInstance pokemon, VolatileStatus status, StatusEffectData statusData, PokemonSpeciesData species, out string reason)
        {
            // Volatile statuses can stack (flags enum)
            // Already has this volatile status
            if (pokemon.VolatileStatus.HasFlag(status))
            {
                reason = $"Pokemon already has {statusData.Name}";
                return false;
            }

            reason = "Can be applied";
            return true;
        }

        /// <summary>
        /// Checks if a Pokemon type is immune to a status effect.
        /// </summary>
        private bool IsTypeImmune(PokemonSpeciesData species, StatusEffectData statusData, out string reason)
        {
            if (statusData.ImmuneTypes == null || statusData.ImmuneTypes.Length == 0)
            {
                reason = "";
                return false;
            }

            foreach (var immuneType in statusData.ImmuneTypes)
            {
                if (species.PrimaryType == immuneType || species.SecondaryType == immuneType)
                {
                    reason = $"{species.Name} is {immuneType}-type and immune to {statusData.Name}";
                    return true;
                }
            }

            reason = "";
            return false;
        }
    }
}

