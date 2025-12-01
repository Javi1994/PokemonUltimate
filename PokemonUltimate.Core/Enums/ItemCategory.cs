namespace PokemonUltimate.Core.Enums
{
    /// <summary>
    /// Categories of items in Pokemon.
    /// </summary>
    public enum ItemCategory
    {
        /// <summary>No item.</summary>
        None,
        
        /// <summary>Items that can be held for passive effects (Leftovers, Choice Band).</summary>
        HeldItem,
        
        /// <summary>Berries that activate at certain conditions.</summary>
        Berry,
        
        /// <summary>Healing items (Potion, Full Restore).</summary>
        Medicine,
        
        /// <summary>Status healing items (Antidote, Awakening).</summary>
        StatusHeal,
        
        /// <summary>Battle items (X Attack, Guard Spec).</summary>
        BattleItem,
        
        /// <summary>Pokeballs for catching.</summary>
        Pokeball,
        
        /// <summary>Evolution items (Fire Stone, etc.).</summary>
        EvolutionItem,
        
        /// <summary>Key items (cannot be discarded).</summary>
        KeyItem,
        
        /// <summary>TMs and HMs.</summary>
        TM,
        
        /// <summary>Mail items.</summary>
        Mail,
    }
}

