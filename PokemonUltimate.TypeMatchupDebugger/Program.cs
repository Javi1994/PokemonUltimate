using System;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;

namespace PokemonUltimate.TypeMatchupDebugger
{
    /// <summary>
    /// Type Matchup Debugger - Test type effectiveness combinations.
    /// Perfect for verifying the complete type chart and effectiveness calculations.
    /// </summary>
    class Program
    {
        // ========== CONFIGURACIÓN - Edita aquí lo que quieres probar ==========

        /// <summary>
        /// Tipo de ataque que quieres probar
        /// </summary>
        static PokemonType AttackingType = PokemonType.Fire;

        /// <summary>
        /// Tipo primario del defensor
        /// </summary>
        static PokemonType DefenderPrimaryType = PokemonType.Grass;

        /// <summary>
        /// Tipo secundario del defensor (null = sin tipo secundario)
        /// </summary>
        static PokemonType? DefenderSecondaryType = PokemonType.Poison;

        // ========================================================================

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            PrintHeader();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Probando: {AttackingType} vs {DefenderPrimaryType}" +
                            (DefenderSecondaryType.HasValue ? $"/{DefenderSecondaryType.Value}" : ""));
            Console.ResetColor();
            Console.WriteLine();

            // Calcular efectividad
            var effectiveness = TypeEffectiveness.GetEffectiveness(
                AttackingType,
                DefenderPrimaryType,
                DefenderSecondaryType);

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
            Console.WriteLine("RESULTADO:");
            Console.WriteLine("═══════════════════════════════════════════════════════════════");
            Console.WriteLine($"Tipo Atacante: {AttackingType}");
            Console.WriteLine($"Tipo Defensor: {DefenderPrimaryType}" +
                            (DefenderSecondaryType.HasValue ? $"/{DefenderSecondaryType.Value}" : ""));
            Console.WriteLine();

            Console.ForegroundColor = GetEffectivenessColor(effectiveness);
            Console.WriteLine($"Efectividad: {effectivenessText}");
            Console.ResetColor();

            // Mostrar desglose si es tipo dual
            if (DefenderSecondaryType.HasValue)
            {
                var primaryEff = TypeEffectiveness.GetEffectiveness(AttackingType, DefenderPrimaryType);
                var secondaryEff = TypeEffectiveness.GetEffectiveness(AttackingType, DefenderSecondaryType.Value);
                
                Console.WriteLine();
                Console.WriteLine("Desglose:");
                Console.WriteLine($"  {AttackingType} vs {DefenderPrimaryType}: {primaryEff:F2}x");
                Console.WriteLine($"  {AttackingType} vs {DefenderSecondaryType.Value}: {secondaryEff:F2}x");
                Console.WriteLine($"  Total: {primaryEff:F2}x × {secondaryEff:F2}x = {effectiveness:F2}x");
            }

            // Mostrar tabla completa del tipo atacante contra todos los tipos
            Console.WriteLine("\n═══════════════════════════════════════════════════════════════");
            Console.WriteLine($"Tabla completa: {AttackingType} vs todos los tipos");
            Console.WriteLine("═══════════════════════════════════════════════════════════════");
            Console.WriteLine("Tipo Defensor | Efectividad | Descripción");
            Console.WriteLine(new string('-', 70));

            var allTypes = Enum.GetValues<PokemonType>();
            foreach (var defendingType in allTypes)
            {
                var eff = TypeEffectiveness.GetEffectiveness(AttackingType, defendingType);
                var desc = GetEffectivenessDescription(eff);
                var color = GetEffectivenessColor(eff);

                Console.ForegroundColor = color;
                Console.WriteLine($"{defendingType,-13} | {eff,11:F2}x | {desc}");
                Console.ResetColor();
            }

            Console.WriteLine("\n═══════════════════════════════════════════════════════════════");
            Console.WriteLine("Presiona cualquier tecla para salir...");
            Console.ReadKey();
        }

        static void PrintHeader()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(new string('═', 100));
            Console.WriteLine("  POKEMON ULTIMATE - TYPE MATCHUP DEBUGGER");
            Console.WriteLine("  Test Type Effectiveness Combinations");
            Console.WriteLine(new string('═', 100));
            Console.ResetColor();
            Console.WriteLine();
        }

        static string GetEffectivenessDescription(float effectiveness)
        {
            return effectiveness switch
            {
                0.0f => "IMMUNE",
                0.25f => "Not Very Effective (0.25x)",
                0.5f => "Not Very Effective (0.5x)",
                1.0f => "Normal",
                2.0f => "Super Effective (2x)",
                4.0f => "Super Effective (4x)",
                _ => effectiveness > 1.0f ? "Super Effective" : "Not Very Effective"
            };
        }

        static ConsoleColor GetEffectivenessColor(float effectiveness)
        {
            return effectiveness switch
            {
                0.0f => ConsoleColor.Red,
                > 0.0f and < 1.0f => ConsoleColor.Yellow,
                1.0f => ConsoleColor.White,
                > 1.0f => ConsoleColor.Green,
                _ => ConsoleColor.White
            };
        }
    }
}
