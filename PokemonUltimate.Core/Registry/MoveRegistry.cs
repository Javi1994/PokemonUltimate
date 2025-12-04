using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Registry
{
    /// <summary>
    /// Specialized registry for Moves that supports lookup by Name and filtering by Type/Category/Power.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.17: Registry System
    /// **Documentation**: See `docs/features/1-game-data/1.17-registry-system/README.md`
    /// </remarks>
    public class MoveRegistry : GameDataRegistry<MoveData>, IMoveRegistry
    {
        /// <summary>
        /// Retrieve by Name (delegates to base Get since Name is the ID)
        /// </summary>
        public MoveData GetByName(string name)
        {
            return Get(name);
        }

        /// <summary>
        /// Get all moves of a specific type
        /// </summary>
        public IEnumerable<MoveData> GetByType(PokemonType type)
        {
            return GetAll().Where(m => m.Type == type);
        }

        /// <summary>
        /// Get all moves of a specific category
        /// </summary>
        public IEnumerable<MoveData> GetByCategory(MoveCategory category)
        {
            return GetAll().Where(m => m.Category == category);
        }

        /// <summary>
        /// Get all damaging moves (Physical or Special, not Status).
        /// </summary>
        public IEnumerable<MoveData> GetDamaging()
        {
            return GetAll().Where(m => m.IsDamaging);
        }

        /// <summary>
        /// Get all status moves.
        /// </summary>
        public IEnumerable<MoveData> GetStatus()
        {
            return GetAll().Where(m => m.IsStatus);
        }

        /// <summary>
        /// Get all moves with at least the specified power.
        /// </summary>
        public IEnumerable<MoveData> GetByMinPower(int minPower)
        {
            return GetAll().Where(m => m.Power >= minPower);
        }

        /// <summary>
        /// Get all moves with at most the specified power.
        /// </summary>
        public IEnumerable<MoveData> GetByMaxPower(int maxPower)
        {
            return GetAll().Where(m => m.Power <= maxPower);
        }

        /// <summary>
        /// Get all moves within a power range (inclusive).
        /// </summary>
        public IEnumerable<MoveData> GetByPowerRange(int minPower, int maxPower)
        {
            return GetAll().Where(m => m.Power >= minPower && m.Power <= maxPower);
        }

        /// <summary>
        /// Get all moves with at least the specified accuracy.
        /// </summary>
        public IEnumerable<MoveData> GetByMinAccuracy(int minAccuracy)
        {
            return GetAll().Where(m => m.Accuracy >= minAccuracy);
        }

        /// <summary>
        /// Get all moves that never miss.
        /// </summary>
        public IEnumerable<MoveData> GetNeverMiss()
        {
            return GetAll().Where(m => m.NeverMisses || m.Accuracy == 0);
        }

        /// <summary>
        /// Get all moves with the specified priority.
        /// </summary>
        public IEnumerable<MoveData> GetByPriority(int priority)
        {
            return GetAll().Where(m => m.Priority == priority);
        }

        /// <summary>
        /// Get all priority moves (priority > 0).
        /// </summary>
        public IEnumerable<MoveData> GetPriorityMoves()
        {
            return GetAll().Where(m => m.Priority > 0);
        }

        /// <summary>
        /// Get all moves that make contact.
        /// </summary>
        public IEnumerable<MoveData> GetContactMoves()
        {
            return GetAll().Where(m => m.MakesContact);
        }

        /// <summary>
        /// Get all sound-based moves.
        /// </summary>
        public IEnumerable<MoveData> GetSoundMoves()
        {
            return GetAll().Where(m => m.IsSoundBased);
        }
    }
}
