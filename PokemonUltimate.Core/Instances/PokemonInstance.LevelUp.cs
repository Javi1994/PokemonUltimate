using System;
using System.Collections.Generic;
using System.Linq;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;

namespace PokemonUltimate.Core.Instances
{
    /// <summary>
    /// Level up and move learning methods for PokemonInstance.
    /// Handles experience, leveling, stat recalculation, and move management.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.1: Pokemon Data
    /// **Documentation**: See `docs/features/1-game-data/1.1-pokemon-data/README.md`
    /// </remarks>
    public partial class PokemonInstance
    {
        #region Level Up State

        // Tracks the last level at which moves were checked (for multi-level ups)
        private int _lastMoveCheckLevel;

        // Cache for calculated stats to avoid unnecessary recalculations
        private readonly StatsCache _statsCache = new StatsCache();

        #endregion

        #region Experience & Level Queries

        /// <summary>
        /// Returns true if this Pokemon can level up (has enough EXP and is below level 100).
        /// </summary>
        public bool CanLevelUp()
        {
            if (Level >= 100)
                return false;

            return CurrentExp >= StatCalculator.GetExpForLevel(Level + 1);
        }

        /// <summary>
        /// Gets the experience required to reach the next level.
        /// </summary>
        public int GetExpForNextLevel()
        {
            if (Level >= 100)
                return 0;

            return StatCalculator.GetExpForLevel(Level + 1);
        }

        /// <summary>
        /// Gets the experience still needed to reach the next level.
        /// </summary>
        public int GetExpToNextLevel()
        {
            if (Level >= 100)
                return 0;

            int needed = StatCalculator.GetExpForLevel(Level + 1);
            return Math.Max(0, needed - CurrentExp);
        }

        #endregion

        #region Level Up Methods

        /// <summary>
        /// Adds experience and returns the number of levels gained.
        /// Use GetPendingMoves() to get moves that can be learned from all gained levels.
        /// </summary>
        public int AddExperience(int amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative", nameof(amount));

            if (Level >= 100)
                return 0;

            int startLevel = Level;
            CurrentExp += amount;
            int levelsGained = 0;

            while (CanLevelUp())
            {
                LevelUpInternal();
                levelsGained++;
            }

            return levelsGained;
        }

        /// <summary>
        /// Forces a level up without requiring experience.
        /// Returns true if level up occurred, false if already at max level.
        /// </summary>
        public bool LevelUp()
        {
            if (Level >= 100)
                return false;

            // Set experience to match new level
            CurrentExp = Math.Max(CurrentExp, StatCalculator.GetExpForLevel(Level + 1));
            LevelUpInternal();
            return true;
        }

        /// <summary>
        /// Levels up to a specific level.
        /// Returns levels gained.
        /// </summary>
        public int LevelUpTo(int targetLevel)
        {
            CoreValidators.ValidateLevel(targetLevel);

            if (targetLevel <= Level)
                return 0;

            int levelsGained = 0;
            while (Level < targetLevel)
            {
                if (!LevelUp())
                    break;
                levelsGained++;
            }

            return levelsGained;
        }

        private void LevelUpInternal()
        {
            int oldMaxHP = MaxHP;
            Level++;
            RecalculateStats();

            // Increase current HP proportionally to max HP increase
            int hpGain = MaxHP - oldMaxHP;
            CurrentHP += hpGain;
        }

        private void RecalculateStats()
        {
            // Check if cache is still valid
            if (_statsCache.IsValid(Level, Nature, Species))
            {
                // Use cached values
                var cachedMaxHP = _statsCache.GetMaxHP();
                var cachedAttack = _statsCache.GetAttack();
                var cachedDefense = _statsCache.GetDefense();
                var cachedSpAttack = _statsCache.GetSpAttack();
                var cachedSpDefense = _statsCache.GetSpDefense();
                var cachedSpeed = _statsCache.GetSpeed();

                if (cachedMaxHP.HasValue && cachedAttack.HasValue && cachedDefense.HasValue &&
                    cachedSpAttack.HasValue && cachedSpDefense.HasValue && cachedSpeed.HasValue)
                {
                    MaxHP = cachedMaxHP.Value;
                    Attack = cachedAttack.Value;
                    Defense = cachedDefense.Value;
                    SpAttack = cachedSpAttack.Value;
                    SpDefense = cachedSpDefense.Value;
                    Speed = cachedSpeed.Value;
                    return;
                }
            }

            // Cache invalid or missing - recalculate
            int maxHP = StatCalculator.CalculateHP(Species.BaseStats.HP, Level);
            int attack = StatCalculator.CalculateStat(Species.BaseStats.Attack, Level, Nature, Stat.Attack);
            int defense = StatCalculator.CalculateStat(Species.BaseStats.Defense, Level, Nature, Stat.Defense);
            int spAttack = StatCalculator.CalculateStat(Species.BaseStats.SpAttack, Level, Nature, Stat.SpAttack);
            int spDefense = StatCalculator.CalculateStat(Species.BaseStats.SpDefense, Level, Nature, Stat.SpDefense);
            int speed = StatCalculator.CalculateStat(Species.BaseStats.Speed, Level, Nature, Stat.Speed);

            // Update properties
            MaxHP = maxHP;
            Attack = attack;
            Defense = defense;
            SpAttack = spAttack;
            SpDefense = spDefense;
            Speed = speed;

            // Update cache
            _statsCache.SetStats(Level, Nature, Species, maxHP, attack, defense, spAttack, spDefense, speed);
        }

        #endregion

        #region Move Learning - Pending Moves

        /// <summary>
        /// Gets all moves that can be learned from levels since the last check.
        /// This includes moves from intermediate levels when gaining multiple levels at once.
        /// Call this after AddExperience() or LevelUpTo() to see what moves are available.
        /// </summary>
        public List<LearnableMove> GetPendingMoves()
        {
            var pendingMoves = new List<LearnableMove>();

            if (Species.Learnset == null)
                return pendingMoves;

            // Get moves from all levels since last check
            for (int level = _lastMoveCheckLevel + 1; level <= Level; level++)
            {
                var movesAtLevel = Species.GetMovesAtLevel(level);
                foreach (var learnableMove in movesAtLevel)
                {
                    // Only include moves we don't already know
                    if (!Moves.Any(m => m.Move == learnableMove.Move))
                    {
                        pendingMoves.Add(learnableMove);
                    }
                }
            }

            return pendingMoves;
        }

        /// <summary>
        /// Marks all pending moves as checked (use after GetPendingMoves).
        /// </summary>
        public void ClearPendingMoves()
        {
            _lastMoveCheckLevel = Level;
        }

        /// <summary>
        /// Attempts to learn all moves available at the current level only.
        /// For moves from multiple levels, use GetPendingMoves() instead.
        /// Returns the list of moves that were learned (max 4 total moves).
        /// </summary>
        public List<MoveData> TryLearnLevelUpMoves()
        {
            var learnedMoves = new List<MoveData>();

            if (Species.Learnset == null)
                return learnedMoves;

            var movesToLearn = Species.GetMovesAtLevel(Level);
            foreach (var learnableMove in movesToLearn)
            {
                if (TryLearnMove(learnableMove.Move))
                {
                    learnedMoves.Add(learnableMove.Move);
                }
            }

            _lastMoveCheckLevel = Level;
            return learnedMoves;
        }

        /// <summary>
        /// Attempts to learn all pending moves from level ups.
        /// Returns moves that were learned (limited by 4-move max).
        /// </summary>
        public List<MoveData> TryLearnAllPendingMoves()
        {
            var learnedMoves = new List<MoveData>();
            var pendingMoves = GetPendingMoves();

            foreach (var learnableMove in pendingMoves)
            {
                if (TryLearnMove(learnableMove.Move))
                {
                    learnedMoves.Add(learnableMove.Move);
                }
            }

            _lastMoveCheckLevel = Level;
            return learnedMoves;
        }

        #endregion

        #region Move Management

        /// <summary>
        /// Attempts to learn a specific move.
        /// Returns true if the move was learned, false if moveset is full.
        /// </summary>
        public bool TryLearnMove(MoveData move)
        {
            if (move == null)
                return false;

            // Check if already knows this move
            if (Moves.Any(m => m.Move == move))
                return false;

            // Check if has room
            if (Moves.Count >= 4)
                return false;

            Moves.Add(new MoveInstance(move));
            return true;
        }

        /// <summary>
        /// Replaces a move at the specified index with a new move.
        /// </summary>
        public bool ReplaceMove(int index, MoveData newMove)
        {
            if (index < 0 || index >= Moves.Count)
                return false;

            if (newMove == null)
                return false;

            // Check if already knows this move
            if (Moves.Any(m => m.Move == newMove))
                return false;

            Moves[index] = new MoveInstance(newMove);
            return true;
        }

        /// <summary>
        /// Forgets a move at the specified index.
        /// </summary>
        public bool ForgetMove(int index)
        {
            if (index < 0 || index >= Moves.Count)
                return false;

            Moves.RemoveAt(index);
            return true;
        }

        #endregion
    }
}

