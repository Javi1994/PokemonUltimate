using System.Collections.Generic;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Blueprints
{
    /// <summary>
    /// Blueprint for a move (immutable data).
    /// Moves can be retrieved by Name (unique string).
    /// Following Composition Pattern: behavior is defined by Effects, not by subclasses.
    /// </summary>
    public class MoveData : IIdentifiable
    {
        /// <summary>
        /// Unique identifier - the move's name (e.g., "Thunderbolt", "Flamethrower")
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Display description for the move
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// The elemental type of the move (Fire, Water, etc.)
        /// </summary>
        public PokemonType Type { get; set; }

        /// <summary>
        /// Determines damage calculation: Physical (Atk/Def), Special (SpAtk/SpDef), Status (no damage)
        /// </summary>
        public MoveCategory Category { get; set; }

        /// <summary>
        /// Base power for damage calculation (0 for Status moves)
        /// </summary>
        public int Power { get; set; }

        /// <summary>
        /// Base accuracy percentage (0-100, or 0 for moves that never miss)
        /// </summary>
        public int Accuracy { get; set; }

        /// <summary>
        /// Maximum Power Points (usage count)
        /// </summary>
        public int MaxPP { get; set; }

        /// <summary>
        /// Priority bracket for turn order (-7 to +5 typically, 0 is normal)
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Who this move can target
        /// </summary>
        public TargetScope TargetScope { get; set; }

        /// <summary>
        /// IIdentifiable implementation - Name serves as the unique ID
        /// </summary>
        public string Id => Name;

        /// <summary>
        /// Composed effects that define what the move does (Composition Pattern)
        /// </summary>
        public List<IMoveEffect> Effects { get; set; } = new List<IMoveEffect>();

        /// <summary>
        /// Helper to check if move has a specific effect type
        /// </summary>
        public bool HasEffect<T>() where T : IMoveEffect
        {
            foreach (var effect in Effects)
            {
                if (effect is T) return true;
            }
            return false;
        }

        /// <summary>
        /// Helper to get a specific effect type (or null if not found)
        /// </summary>
        public T GetEffect<T>() where T : class, IMoveEffect
        {
            foreach (var effect in Effects)
            {
                if (effect is T typedEffect) return typedEffect;
            }
            return null;
        }
    }
}

