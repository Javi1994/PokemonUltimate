using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.AI;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Combat.Factories;
using PokemonUltimate.Combat.Helpers;
using PokemonUltimate.Combat.Providers;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.MoveDebugger
{
    /// <summary>
    /// Move Debugger - Test specific moves multiple times against targets and collect statistics.
    /// Perfect for verifying move power, type effectiveness, accuracy, effects, and damage averages.
    /// </summary>
    class Program
    {
        // ========== CONFIGURACIÓN - Edita aquí lo que quieres probar ==========

        /// <summary>
        /// Movimiento que quieres probar
        /// </summary>
        static MoveData MoveToTest = MoveCatalog.Thunderbolt;

        /// <summary>
        /// Pokemon que usa el movimiento
        /// </summary>
        static PokemonSpeciesData AttackerPokemon = PokemonCatalog.Pikachu;

        /// <summary>
        /// Pokemon contra el que probar el movimiento
        /// </summary>
        static PokemonSpeciesData TargetPokemon = PokemonCatalog.Charmander;

        /// <summary>
        /// Nivel de ambos Pokemon
        /// </summary>
        static int Level = 50;

        /// <summary>
        /// Número de pruebas a ejecutar
        /// </summary>
        static int NumberOfTests = 100;

        /// <summary>
        /// Modo de salida: true = detallado (muestra cada prueba), false = resumen (solo estadísticas)
        /// </summary>
        static bool DetailedOutput = false;

        // ========================================================================

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            PrintHeader();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Probando: {MoveToTest.Name} ({MoveToTest.Type})");
            Console.WriteLine($"Usuario: {AttackerPokemon.Name} ({AttackerPokemon.PrimaryType}" +
                            (AttackerPokemon.SecondaryType.HasValue ? $"/{AttackerPokemon.SecondaryType.Value}" : "") + ")");
            Console.WriteLine($"Objetivo: {TargetPokemon.Name} ({TargetPokemon.PrimaryType}" +
                            (TargetPokemon.SecondaryType.HasValue ? $"/{TargetPokemon.SecondaryType.Value}" : "") + ")");
            Console.WriteLine($"Nivel: {Level}");
            Console.WriteLine($"Pruebas: {NumberOfTests}");
            Console.ResetColor();
            Console.WriteLine();

            // Calcular efectividad de tipo
            var effectiveness = TypeEffectiveness.GetEffectiveness(
                MoveToTest.Type,
                TargetPokemon.PrimaryType,
                TargetPokemon.SecondaryType);

            var effectivenessText = effectiveness switch
            {
                0.0f => "IMMUNE (0x)",
                0.25f => "Not Very Effective (0.25x)",
                0.5f => "Not Very Effective (0.5x)",
                1.0f => "Normal (1x)",
                2.0f => "Super Effective (2x)",
                4.0f => "Super Effective (4x)",
                _ => $"{effectiveness:F2}x"
            };

            Console.WriteLine("═══════════════════════════════════════════════════════════════");
            Console.WriteLine("INFORMACIÓN DEL MOVIMIENTO:");
            Console.WriteLine("═══════════════════════════════════════════════════════════════");
            Console.WriteLine($"Movimiento: {MoveToTest.Name}");
            Console.WriteLine($"  Tipo: {MoveToTest.Type}");
            Console.WriteLine($"  Poder: {MoveToTest.Power}");
            Console.WriteLine($"  Categoría: {MoveToTest.Category}");
            Console.WriteLine($"  Precisión: {MoveToTest.Accuracy}%");
            Console.WriteLine($"  PP: {MoveToTest.MaxPP}");
            Console.WriteLine($"  Prioridad: {MoveToTest.Priority}");
            
            // Mostrar efectos del movimiento
            if (MoveToTest.Effects != null && MoveToTest.Effects.Count > 0)
            {
                Console.WriteLine();
                Console.WriteLine("  Efectos del movimiento:");
                foreach (var effect in MoveToTest.Effects)
                {
                    var effectType = effect.GetType().Name.Replace("Effect", "");
                    Console.WriteLine($"    - {effectType}: {effect.Description}");
                }
            }
            Console.WriteLine();
            Console.ForegroundColor = effectiveness switch
            {
                0.0f => ConsoleColor.Red,
                0.25f => ConsoleColor.Yellow,
                0.5f => ConsoleColor.Yellow,
                1.0f => ConsoleColor.White,
                2.0f => ConsoleColor.Green,
                4.0f => ConsoleColor.Green,
                _ => ConsoleColor.White
            };
            Console.WriteLine($"Efectividad de Tipo: {effectivenessText}");
            Console.ResetColor();
            Console.WriteLine();

            // Ejecutar pruebas múltiples
            var stats = await RunMultipleTests();

            // Mostrar estadísticas
            DisplayStatistics(stats, effectiveness);

            Console.WriteLine("\n═══════════════════════════════════════════════════════════════");
            Console.WriteLine("Presiona cualquier tecla para salir...");
            Console.ReadKey();
        }

        static async Task<MoveTestStatistics> RunMultipleTests()
        {
            var stats = new MoveTestStatistics
            {
                DamageValues = new List<int>(),
                CriticalHits = 0,
                Misses = 0,
                StatusEffectsCaused = new Dictionary<string, int>(),
                VolatileStatusEffectsCaused = new Dictionary<string, int>()
            };

            for (int i = 0; i < NumberOfTests; i++)
            {
                if (DetailedOutput)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\n{new string('═', 100)}");
                    Console.WriteLine($"PRUEBA #{i + 1}");
                    Console.WriteLine(new string('═', 100));
                    Console.ResetColor();
                }

                var result = await RunSingleTest(stats);

                if (DetailedOutput && i < NumberOfTests - 1)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("\nPresiona cualquier tecla para continuar...");
                    Console.ResetColor();
                    Console.ReadKey(true);
                }
            }

            return stats;
        }

        static Task<SingleTestResult> RunSingleTest(MoveTestStatistics stats)
        {
            // Crear Pokemon
            var attacker = PokemonFactory.Create(AttackerPokemon, Level);
            var target = PokemonFactory.Create(TargetPokemon, Level);

            // Crear campo de batalla
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            var playerParty = new[] { attacker };
            var enemyParty = new[] { target };

            var field = new BattleField();
            field.Initialize(rules, playerParty, enemyParty);
            var playerSlot = field.PlayerSide.Slots[0];
            var enemySlot = field.EnemySide.Slots[0];

            // Crear instancia del movimiento
            var moveInstance = new MoveInstance(MoveToTest);
            if (!attacker.Moves.Contains(moveInstance))
            {
                // Agregar el movimiento si no está en la lista
                attacker.Moves.Add(moveInstance);
            }

            // Crear acción de usar movimiento
            var useMoveAction = new UseMoveAction(playerSlot, enemySlot, moveInstance);

            // Capturar HP antes
            var hpBefore = enemySlot.Pokemon.CurrentHP;

            // Ejecutar la acción
            var reactions = useMoveAction.ExecuteLogic(field);
            var reactionList = reactions?.ToList() ?? new List<BattleAction>();

            // Procesar reactions para encontrar daño y efectos
            int damageDealt = 0;
            bool wasCritical = false;
            bool wasMiss = false;
            var statusEffects = new List<PersistentStatus>();
            var volatileStatusEffects = new List<VolatileStatus>();

            foreach (var reaction in reactionList)
            {
                // Rastrear tipo de acción generada
                var actionType = reaction.GetType().Name.Replace("Action", "");
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

            // Verificar si fue un fallo (no hubo daño y el movimiento tiene precisión)
            if (damageDealt == 0 && MoveToTest.Accuracy < 100)
            {
                // Verificar si el movimiento debería haber causado daño
                if (MoveToTest.Category != MoveCategory.Status && MoveToTest.Power > 0)
                {
                    wasMiss = true;
                    stats.Misses++;
                }
            }

            // Verificar estados volátiles aplicados
            var volatileBefore = VolatileStatus.None;
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

            if (DetailedOutput)
            {
                Console.WriteLine($"Daño causado: {damageDealt}");
                if (wasCritical)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("¡GOLPE CRÍTICO!");
                    Console.ResetColor();
                }
                if (wasMiss)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("¡FALLO!");
                    Console.ResetColor();
                }
                
                // Mostrar acciones generadas
                if (reactionList.Count > 0)
                {
                    Console.WriteLine($"\nAcciones generadas ({reactionList.Count}):");
                    foreach (var action in reactionList)
                    {
                        var actionType = action.GetType().Name.Replace("Action", "");
                        Console.WriteLine($"  - {actionType}");
                        
                        // Mostrar detalles específicos según el tipo de acción
                        if (action is DamageAction da)
                        {
                            Console.WriteLine($"      Daño: {da.Context?.FinalDamage ?? 0} HP");
                            if (da.Context?.IsCritical == true)
                                Console.WriteLine($"      Crítico: Sí");
                        }
                        else if (action is ApplyStatusAction sa)
                        {
                            Console.WriteLine($"      Estado: {sa.Status}");
                        }
                        else if (action is MessageAction ma)
                        {
                            // Los MessageAction no tienen propiedades públicas fácilmente accesibles
                            // pero podemos mostrar el tipo
                        }
                    }
                }
                
                if (statusEffects.Count > 0)
                {
                    Console.WriteLine($"\nEfectos de estado causados: {string.Join(", ", statusEffects)}");
                }
                if (volatileStatusEffects.Count > 0)
                {
                    Console.WriteLine($"Estados volátiles causados: {string.Join(", ", volatileStatusEffects)}");
                }
            }

            return Task.FromResult(new SingleTestResult
            {
                Damage = damageDealt,
                WasCritical = wasCritical,
                WasMiss = wasMiss,
                StatusEffects = statusEffects,
                VolatileStatusEffects = volatileStatusEffects
            });
        }

        static void DisplayStatistics(MoveTestStatistics stats, float typeEffectiveness)
        {
            Console.WriteLine("\n═══════════════════════════════════════════════════════════════");
            Console.WriteLine("ESTADÍSTICAS:");
            Console.WriteLine("═══════════════════════════════════════════════════════════════");

            // Estadísticas de daño
            if (stats.DamageValues.Count > 0)
            {
                var avgDamage = stats.DamageValues.Average();
                var minDamage = stats.DamageValues.Min();
                var maxDamage = stats.DamageValues.Max();
                var medianDamage = stats.DamageValues.OrderBy(d => d).Skip(stats.DamageValues.Count / 2).First();

                Console.WriteLine("\nDAÑO:");
                Console.WriteLine($"  Total de golpes exitosos: {stats.DamageValues.Count} / {NumberOfTests}");
                Console.WriteLine($"  Daño promedio: {avgDamage:F1} HP");
                Console.WriteLine($"  Daño mínimo: {minDamage} HP");
                Console.WriteLine($"  Daño máximo: {maxDamage} HP");
                Console.WriteLine($"  Daño mediano: {medianDamage} HP");

                if (stats.CriticalHits > 0)
                {
                    var critRate = (stats.CriticalHits * 100.0) / stats.DamageValues.Count;
                    Console.WriteLine($"  Golpes críticos: {stats.CriticalHits} ({critRate:F1}% de los golpes exitosos)");
                }
            }

            if (stats.Misses > 0)
            {
                var missRate = (stats.Misses * 100.0) / NumberOfTests;
                Console.WriteLine($"  Fallos: {stats.Misses} ({missRate:F1}% del total)");
            }

            // Estadísticas de efectos de estado
            if (stats.StatusEffectsCaused.Count > 0)
            {
                var totalStatusEffects = stats.StatusEffectsCaused.Values.Sum();
                Console.WriteLine("\nEFECTOS DE ESTADO CAUSADOS:");
                Console.WriteLine($"  Total: {totalStatusEffects} efectos en {NumberOfTests} pruebas");
                foreach (var effect in stats.StatusEffectsCaused.OrderByDescending(e => e.Value))
                {
                    var percentage = (effect.Value * 100.0) / NumberOfTests;
                    Console.WriteLine($"  {effect.Key}: {effect.Value} veces ({percentage:F1}%)");
                }
            }

            if (stats.VolatileStatusEffectsCaused.Count > 0)
            {
                var totalVolatileEffects = stats.VolatileStatusEffectsCaused.Values.Sum();
                Console.WriteLine("\nESTADOS VOLÁTILES CAUSADOS:");
                Console.WriteLine($"  Total: {totalVolatileEffects} efectos en {NumberOfTests} pruebas");
                foreach (var effect in stats.VolatileStatusEffectsCaused.OrderByDescending(e => e.Value))
                {
                    var percentage = (effect.Value * 100.0) / NumberOfTests;
                    Console.WriteLine($"  {effect.Key}: {effect.Value} veces ({percentage:F1}%)");
                }
            }

            // Estadísticas de acciones generadas
            if (stats.ActionsGenerated.Count > 0)
            {
                Console.WriteLine("\nACCIONES GENERADAS:");
                Console.WriteLine($"  Total de acciones generadas: {stats.ActionsGenerated.Values.Sum()}");
                foreach (var action in stats.ActionsGenerated.OrderByDescending(a => a.Value))
                {
                    var percentage = (action.Value * 100.0) / NumberOfTests;
                    Console.WriteLine($"  {action.Key}: {action.Value} veces ({percentage:F1}% de las pruebas)");
                }
            }

            // Resumen de efectividad
            Console.WriteLine("\nEFECTIVIDAD:");
            Console.WriteLine($"  Efectividad de tipo: {typeEffectiveness:F2}x");
            if (stats.DamageValues.Count > 0)
            {
                var avgDamage = stats.DamageValues.Average();
                Console.WriteLine($"  Daño promedio observado: {avgDamage:F1} HP");
            }
        }

        static void PrintHeader()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(new string('═', 100));
            Console.WriteLine("  POKEMON ULTIMATE - MOVE DEBUGGER");
            Console.WriteLine("  Test Moves with Statistics");
            Console.WriteLine(new string('═', 100));
            Console.ResetColor();
            Console.WriteLine();
        }
    }

    class MoveTestStatistics
    {
        public List<int> DamageValues { get; set; } = new List<int>();
        public int CriticalHits { get; set; }
        public int Misses { get; set; }
        public Dictionary<string, int> StatusEffectsCaused { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> VolatileStatusEffectsCaused { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> ActionsGenerated { get; set; } = new Dictionary<string, int>();
    }

    class SingleTestResult
    {
        public int Damage { get; set; }
        public bool WasCritical { get; set; }
        public bool WasMiss { get; set; }
        public List<PersistentStatus> StatusEffects { get; set; } = new List<PersistentStatus>();
        public List<VolatileStatus> VolatileStatusEffects { get; set; } = new List<VolatileStatus>();
    }
}
