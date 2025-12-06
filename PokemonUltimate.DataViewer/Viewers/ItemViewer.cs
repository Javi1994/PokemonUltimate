using System;
using System.Linq;
using PokemonUltimate.Content.Catalogs.Items;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.DataViewer.Viewers;

/// <summary>
/// Viewer for displaying Item data from the catalog.
/// </summary>
/// <remarks>
/// **Feature**: 6: Development Tools
/// **Sub-Feature**: 6.7: Data Viewer
/// **Documentation**: See `docs/features/6-development-tools/6.7-data-viewer/README.md`
/// </remarks>
public static class ItemViewer
{
    public static void DisplayAll()
    {
        Console.WriteLine("═══════════════════════════════════════════════════════");
        Console.WriteLine("ITEM CATALOG");
        Console.WriteLine("═══════════════════════════════════════════════════════");
        Console.WriteLine($"Total Items: {ItemCatalog.All.Count}");
        Console.WriteLine();

        var itemList = ItemCatalog.All.OrderBy(i => i.Name).ToList();

        foreach (var item in itemList)
        {
            DisplayItem(item);
            Console.WriteLine();
        }
    }

    public static void DisplaySummary()
    {
        Console.WriteLine("═══════════════════════════════════════════════════════");
        Console.WriteLine("ITEM CATALOG SUMMARY");
        Console.WriteLine("═══════════════════════════════════════════════════════");
        Console.WriteLine($"Total Items: {ItemCatalog.All.Count}");
        Console.WriteLine();

        var itemList = ItemCatalog.All.OrderBy(i => i.Name).ToList();

        foreach (var item in itemList)
        {
            var categoryStr = item.Category.ToString();
            Console.WriteLine($"{item.Name,-25} {categoryStr,-15} {item.Id}");
        }
    }

    private static void DisplayItem(ItemData item)
    {
        Console.WriteLine($"{item.Name}");
        Console.WriteLine($"  ID: {item.Id}");
        Console.WriteLine($"  Category: {item.Category}");
        
        if (!string.IsNullOrEmpty(item.Description))
            Console.WriteLine($"  Description: {item.Description}");
    }
}

