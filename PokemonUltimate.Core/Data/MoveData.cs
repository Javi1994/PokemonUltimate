using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Interfaces;

namespace PokemonUltimate.Core.Data
{
    // Blueprint for a move (immutable data).
    // Moves can be retrieved by Name (unique string).
    // Following Composition Pattern: behavior is defined by Effects, not by subclasses.
    public class MoveData : IIdentifiable
    {
        // Unique identifier - the move's name (e.g., "Thunderbolt", "Flamethrower")
        public string Name { get; set; } = string.Empty;

        // Display description for the move
        public string Description { get; set; } = string.Empty;

        // The elemental type of the move (Fire, Water, etc.)
        public PokemonType Type { get; set; }

        // Determines damage calculation: Physical (Atk/Def), Special (SpAtk/SpDef), Status (no damage)
        public MoveCategory Category { get; set; }

        // Base power for damage calculation (0 for Status moves)
        public int Power { get; set; }

        // Base accuracy percentage (0-100, or 0 for moves that never miss)
        public int Accuracy { get; set; }

        // Maximum Power Points (usage count)
        public int MaxPP { get; set; }

        // Priority bracket for turn order (-7 to +5 typically, 0 is normal)
        public int Priority { get; set; }

        // Who this move can target
        public TargetScope TargetScope { get; set; }

        // IIdentifiable implementation - Name serves as the unique ID
        public string Id => Name;

        // Effects will be added later when implementing the combat system
        // public List<IMoveEffect> Effects { get; set; } = new List<IMoveEffect>();
    }
}

