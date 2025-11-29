using PokemonUltimate.Core.Catalogs;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Registry;

namespace PokemonUltimate.Console;

// Demo application to verify the data system works correctly at runtime.
class Program
{
    static void Main(string[] args)
    {
        PrintHeader();

        // Initialize registries
        var pokemonRegistry = new PokemonRegistry();
        var moveRegistry = new MoveRegistry();

        // Step 1: Load Pokemon
        PrintStep(1, "Loading Pokemon Catalog...");
        PokemonCatalog.RegisterAll(pokemonRegistry);
        PrintSuccess($"{PokemonCatalog.Count} Pokemon loaded");

        // Step 2: Load Moves
        PrintStep(2, "Loading Move Catalog...");
        MoveCatalog.RegisterAll(moveRegistry);
        PrintSuccess($"{MoveCatalog.Count} Moves loaded");

        // Step 3: Query Pokemon by Pokedex Number
        PrintStep(3, "Querying Pokemon by Pokedex #25...");
        var pikachu = pokemonRegistry.GetByPokedexNumber(25);
        PrintSuccess($"Found: {pikachu.Name}");

        // Step 4: Query Pokemon by Name
        PrintStep(4, "Querying Pokemon by name 'Charizard'...");
        var charizard = pokemonRegistry.GetByName("Charizard");
        PrintSuccess($"Found: {charizard.Name} (#{charizard.PokedexNumber})");

        // Step 5: List all starters
        PrintStep(5, "Listing Gen 1 Starters...");
        var starters = new[] { 1, 4, 7 }; // Bulbasaur, Charmander, Squirtle
        foreach (var num in starters)
        {
            var starter = pokemonRegistry.GetByPokedexNumber(num);
            PrintItem($"#{starter.PokedexNumber} {starter.Name}");
        }

        // Step 6: Query Fire-type moves
        PrintStep(6, "Querying Fire-type moves...");
        var fireMoves = moveRegistry.GetByType(PokemonType.Fire);
        foreach (var move in fireMoves)
        {
            PrintItem($"{move.Name} (Power: {move.Power}, Accuracy: {move.Accuracy})");
        }

        // Step 7: Query Status moves
        PrintStep(7, "Querying Status moves...");
        var statusMoves = moveRegistry.GetByCategory(MoveCategory.Status);
        foreach (var move in statusMoves)
        {
            PrintItem($"{move.Name} ({move.Type}) - Acc: {move.Accuracy}%");
        }

        // Step 8: Direct catalog access
        PrintStep(8, "Direct catalog access (no registry)...");
        PrintItem($"PokemonCatalog.Pikachu.Name = \"{PokemonCatalog.Pikachu.Name}\"");
        PrintItem($"MoveCatalog.Thunderbolt.Power = {MoveCatalog.Thunderbolt.Power}");
        PrintItem($"MoveCatalog.Thunderbolt.Type = {MoveCatalog.Thunderbolt.Type}");

        // Step 9: Verify same instance
        PrintStep(9, "Verifying catalog and registry return same instance...");
        var catalogPikachu = PokemonCatalog.Pikachu;
        var registryPikachu = pokemonRegistry.GetByName("Pikachu");
        var sameInstance = ReferenceEquals(catalogPikachu, registryPikachu);
        PrintSuccess($"Same instance: {sameInstance}");

        // Step 10: List all Pokemon
        PrintStep(10, "All registered Pokemon...");
        foreach (var pokemon in pokemonRegistry.GetAll().OrderBy(p => p.PokedexNumber))
        {
            PrintItem($"#{pokemon.PokedexNumber:D3} {pokemon.Name}");
        }

        PrintFooter();
    }

    #region Console Helpers

    static void PrintHeader()
    {
        System.Console.ForegroundColor = ConsoleColor.Cyan;
        System.Console.WriteLine();
        System.Console.WriteLine("═══════════════════════════════════════════════════════");
        System.Console.WriteLine("       POKEMON ULTIMATE - Data System Demo             ");
        System.Console.WriteLine("═══════════════════════════════════════════════════════");
        System.Console.ResetColor();
        System.Console.WriteLine();
    }

    static void PrintFooter()
    {
        System.Console.WriteLine();
        System.Console.ForegroundColor = ConsoleColor.Green;
        System.Console.WriteLine("═══════════════════════════════════════════════════════");
        System.Console.WriteLine("       ✓ All systems operational!                      ");
        System.Console.WriteLine("═══════════════════════════════════════════════════════");
        System.Console.ResetColor();
        System.Console.WriteLine();
    }

    static void PrintStep(int num, string message)
    {
        System.Console.WriteLine();
        System.Console.ForegroundColor = ConsoleColor.Yellow;
        System.Console.Write($"[{num}] ");
        System.Console.ResetColor();
        System.Console.WriteLine(message);
    }

    static void PrintSuccess(string message)
    {
        System.Console.ForegroundColor = ConsoleColor.Green;
        System.Console.Write("    ✓ ");
        System.Console.ResetColor();
        System.Console.WriteLine(message);
    }

    static void PrintItem(string message)
    {
        System.Console.ForegroundColor = ConsoleColor.DarkGray;
        System.Console.Write("    • ");
        System.Console.ResetColor();
        System.Console.WriteLine(message);
    }

    #endregion
}