using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.UnifiedDebuggerUI.Runners
{
    public class MoveRunner
    {
        public class MoveTestStatistics
        {
            public List<int> DamageValues { get; set; } = new List<int>();
            public int CriticalHits { get; set; }
            public int Misses { get; set; }
            public Dictionary<string, int> StatusEffectsCaused { get; set; } = new Dictionary<string, int>();
            public Dictionary<string, int> VolatileStatusEffectsCaused { get; set; } = new Dictionary<string, int>();
            public Dictionary<string, int> ActionsGenerated { get; set; } = new Dictionary<string, int>();
        }

        public class MoveTestConfig
        {
            public MoveData MoveToTest { get; set; }
            public PokemonSpeciesData AttackerPokemon { get; set; }
            public PokemonSpeciesData TargetPokemon { get; set; }
            public int Level { get; set; } = 50;
            public int NumberOfTests { get; set; } = 100;
            public bool DetailedOutput { get; set; } = false;
        }

        public class SingleTestResult
        {
            public int Damage { get; set; }
            public bool WasCritical { get; set; }
            public bool WasMiss { get; set; }
            public List<PersistentStatus> StatusEffects { get; set; } = new List<PersistentStatus>();
            public List<VolatileStatus> VolatileStatusEffects { get; set; } = new List<VolatileStatus>();
            public List<string> ActionsGenerated { get; set; } = new List<string>();
        }

        public async Task<MoveTestStatistics> RunTestsAsync(MoveTestConfig config, IProgress<int>? progress = null)
        {
            var stats = new MoveTestStatistics();

            for (int i = 0; i < config.NumberOfTests; i++)
            {
                var result = await RunSingleTestAsync(config, stats);

                // Actualizar progreso
                progress?.Report((i + 1) * 100 / config.NumberOfTests);
                
                // Permitir que la UI se actualice
                await Task.Yield();
            }

            return stats;
        }

        private Task<SingleTestResult> RunSingleTestAsync(MoveTestConfig config, MoveTestStatistics stats)
        {
            // Crear Pokemon
            var attacker = PokemonFactory.Create(config.AttackerPokemon, config.Level);
            var target = PokemonFactory.Create(config.TargetPokemon, config.Level);

            // Crear campo de batalla
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            var playerParty = new[] { attacker };
            var enemyParty = new[] { target };

            var field = new BattleField();
            field.Initialize(rules, playerParty, enemyParty);
            var playerSlot = field.PlayerSide.Slots[0];
            var enemySlot = field.EnemySide.Slots[0];

            // Crear instancia del movimiento
            var moveInstance = new MoveInstance(config.MoveToTest);
            if (!attacker.Moves.Contains(moveInstance))
            {
                attacker.Moves.Add(moveInstance);
            }

            // Crear acción de usar movimiento
            var useMoveAction = new UseMoveAction(playerSlot, enemySlot, moveInstance);

            // Capturar HP antes
            var hpBefore = enemySlot.Pokemon.CurrentHP;
            var volatileBefore = enemySlot.VolatileStatus;

            // Ejecutar la acción
            var reactions = useMoveAction.ExecuteLogic(field);
            var reactionList = reactions?.ToList() ?? new List<BattleAction>();

            // Procesar reactions para encontrar daño y efectos
            int damageDealt = 0;
            bool wasCritical = false;
            bool wasMiss = false;
            var statusEffects = new List<PersistentStatus>();
            var volatileStatusEffects = new List<VolatileStatus>();
            var actionsGenerated = new List<string>();

            foreach (var reaction in reactionList)
            {
                // Rastrear tipo de acción generada
                var actionType = reaction.GetType().Name.Replace("Action", "");
                actionsGenerated.Add(actionType);
                
                if (!stats.ActionsGenerated.ContainsKey(actionType))
                {
                    stats.ActionsGenerated[actionType] = 0;
                }
                stats.ActionsGenerated[actionType]++;

                if (reaction is DamageAction damageAction)
                {
                    damageDealt = damageAction.Context?.FinalDamage ?? 0;
                    wasCritical = damageAction.Context?.IsCritical ?? false;
                    if (damageDealt > 0)
                    {
                        stats.DamageValues.Add(damageDealt);

                        if (wasCritical)
                        {
                            stats.CriticalHits++;
                        }
                    }
                }
                else if (reaction is ApplyStatusAction statusAction && statusAction.Status != PersistentStatus.None)
                {
                    statusEffects.Add(statusAction.Status);
                    var statusName = statusAction.Status.ToString();
                    if (!stats.StatusEffectsCaused.ContainsKey(statusName))
                    {
                        stats.StatusEffectsCaused[statusName] = 0;
                    }
                    stats.StatusEffectsCaused[statusName]++;
                }
            }

            // Verificar si fue un fallo
            if (damageDealt == 0 && config.MoveToTest.Accuracy < 100)
            {
                if (config.MoveToTest.Category != MoveCategory.Status && config.MoveToTest.Power > 0)
                {
                    wasMiss = true;
                    stats.Misses++;
                }
            }

            // Verificar estados volátiles aplicados
            var volatileAfter = enemySlot.VolatileStatus;
            var newVolatileStatuses = volatileAfter & ~volatileBefore;

            if (newVolatileStatuses != VolatileStatus.None)
            {
                foreach (VolatileStatus status in Enum.GetValues(typeof(VolatileStatus)))
                {
                    if (status != VolatileStatus.None && (newVolatileStatuses & status) != 0)
                    {
                        volatileStatusEffects.Add(status);
                        var statusName = $"Volatile_{status}";
                        if (!stats.VolatileStatusEffectsCaused.ContainsKey(statusName))
                        {
                            stats.VolatileStatusEffectsCaused[statusName] = 0;
                        }
                        stats.VolatileStatusEffectsCaused[statusName]++;
                    }
                }
            }

            return Task.FromResult(new SingleTestResult
            {
                Damage = damageDealt,
                WasCritical = wasCritical,
                WasMiss = wasMiss,
                StatusEffects = statusEffects,
                VolatileStatusEffects = volatileStatusEffects,
                ActionsGenerated = actionsGenerated
            });
        }
    }
}

