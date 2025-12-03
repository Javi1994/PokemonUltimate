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
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.2: Move Data
    /// **Documentation**: See `docs/features/1-game-data/1.2-move-data/architecture.md`
    /// </remarks>
    public class MoveData : IIdentifiable
    {
        #region Basic Properties

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
        /// Base accuracy percentage (1-100). Use NeverMisses flag for always-hit moves.
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

        #endregion

        #region Move Flags

        /// <summary>
        /// True if this move makes physical contact (triggers abilities like Static, Iron Barbs).
        /// Generally true for Physical moves, but not always (e.g., Earthquake doesn't make contact).
        /// </summary>
        public bool MakesContact { get; set; }

        /// <summary>
        /// True if this move is sound-based (bypasses Substitute, blocked by Soundproof).
        /// </summary>
        public bool IsSoundBased { get; set; }

        /// <summary>
        /// True if this move never misses (Swift, Aerial Ace, Shock Wave).
        /// Ignores accuracy/evasion checks.
        /// </summary>
        public bool NeverMisses { get; set; }

        /// <summary>
        /// True if this move has an increased critical hit ratio (Slash, Stone Edge, Crabhammer).
        /// </summary>
        public bool HighCritRatio { get; set; }

        /// <summary>
        /// True if this move always results in a critical hit (Frost Breath, Storm Throw).
        /// </summary>
        public bool AlwaysCrits { get; set; }

        /// <summary>
        /// True if the user must recharge next turn (Hyper Beam, Giga Impact).
        /// </summary>
        public bool RequiresRecharge { get; set; }

        /// <summary>
        /// True if this is a two-turn move where user is semi-invulnerable (Fly, Dig, Dive).
        /// </summary>
        public bool IsTwoTurn { get; set; }

        /// <summary>
        /// True if this move is a punch-based move (boosted by Iron Fist).
        /// </summary>
        public bool IsPunch { get; set; }

        /// <summary>
        /// True if this move is a biting move (boosted by Strong Jaw).
        /// </summary>
        public bool IsBite { get; set; }

        /// <summary>
        /// True if this move is a pulse/aura move (boosted by Mega Launcher).
        /// </summary>
        public bool IsPulse { get; set; }

        /// <summary>
        /// True if this move is a bullet/ball move (blocked by Bulletproof).
        /// </summary>
        public bool IsBullet { get; set; }

        /// <summary>
        /// True if this move can be reflected by Magic Coat/Magic Bounce.
        /// </summary>
        public bool IsReflectable { get; set; }

        /// <summary>
        /// True if this move can be stolen by Snatch.
        /// </summary>
        public bool IsSnatched { get; set; }

        /// <summary>
        /// True if this move ignores the target's stat changes.
        /// </summary>
        public bool IgnoresTargetStatChanges { get; set; }

        /// <summary>
        /// True if this move ignores the user's stat changes (negative only).
        /// </summary>
        public bool IgnoresUserStatChanges { get; set; }

        /// <summary>
        /// True if this move can hit Pokemon using Protect (Feint, Shadow Force).
        /// </summary>
        public bool BypassesProtect { get; set; }

        #endregion

        #region Effects

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

        #endregion

        #region Computed Properties

        /// <summary>
        /// True if this is a damaging move (has power > 0 or is not Status category).
        /// </summary>
        public bool IsDamaging => Category != MoveCategory.Status;

        /// <summary>
        /// True if this is a status move.
        /// </summary>
        public bool IsStatus => Category == MoveCategory.Status;

        /// <summary>
        /// True if this is a physical move.
        /// </summary>
        public bool IsPhysical => Category == MoveCategory.Physical;

        /// <summary>
        /// True if this is a special move.
        /// </summary>
        public bool IsSpecial => Category == MoveCategory.Special;

        #endregion
    }
}

