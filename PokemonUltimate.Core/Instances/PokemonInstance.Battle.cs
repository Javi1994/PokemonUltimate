using System;
using PokemonUltimate.Core.Constants;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Managers;

namespace PokemonUltimate.Core.Instances
{
    /// <summary>
    /// Battle-related methods for PokemonInstance.
    /// Handles stats, damage, healing, status effects, and friendship.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.1: Pokemon Data
    /// **Documentation**: See `docs/features/1-game-data/1.1-pokemon-data/README.md`
    /// </remarks>
    public partial class PokemonInstance
    {
        #region Stat Calculation

        /// <summary>
        /// Gets the effective stat value in battle, applying stat stages and status effects.
        /// - Burn: Physical Attack is halved
        /// - Paralysis: Speed is quartered (Gen7+)
        /// </summary>
        public int GetEffectiveStat(Stat stat)
        {
            int baseStat = GetBaseStat(stat);
            int stage = StatStageManager.Default.GetStage(StatStages, stat);

            if (stat == Stat.Accuracy || stat == Stat.Evasion)
            {
                float multiplier = StatCalculator.GetAccuracyStageMultiplier(stage);
                return (int)(baseStat * multiplier);
            }

            int effective = StatCalculator.GetEffectiveStat(baseStat, stage);

            // Apply status effect modifiers
            effective = ApplyStatusModifier(stat, effective);

            return effective;
        }

        /// <summary>
        /// Gets the effective stat without status modifiers (for display purposes).
        /// </summary>
        public int GetEffectiveStatRaw(Stat stat)
        {
            int baseStat = GetBaseStat(stat);
            int stage = StatStageManager.Default.GetStage(StatStages, stat);

            if (stat == Stat.Accuracy || stat == Stat.Evasion)
            {
                float multiplier = StatCalculator.GetAccuracyStageMultiplier(stage);
                return (int)(baseStat * multiplier);
            }

            return StatCalculator.GetEffectiveStat(baseStat, stage);
        }

        /// <summary>
        /// Applies status condition modifiers to a stat.
        /// </summary>
        private int ApplyStatusModifier(Stat stat, int value)
        {
            // Burn halves physical Attack
            if (stat == Stat.Attack && Status == PersistentStatus.Burn)
            {
                return value / 2;
            }

            // Paralysis quarters Speed (Gen 7+)
            if (stat == Stat.Speed && Status == PersistentStatus.Paralysis)
            {
                return value / 4;
            }

            return value;
        }

        /// <summary>
        /// Modifies a stat stage by the given amount, clamped to -6/+6.
        /// Returns the actual change applied.
        /// </summary>
        public int ModifyStatStage(Stat stat, int change)
        {
            return StatStageManager.Default.ModifyStage(StatStages, stat, change);
        }

        #endregion

        #region Battle State Management

        /// <summary>
        /// Resets all volatile battle state (stat stages, volatile status).
        /// Called when switching out or after battle.
        /// </summary>
        public void ResetBattleState()
        {
            VolatileStatus = VolatileStatus.None;
            StatusTurnCounter = 0;

            // Reset all stat stages to 0
            StatStageManager.Default.ResetStages(StatStages);
        }

        /// <summary>
        /// Fully heals this Pokemon (restores HP, clears status, restores PP).
        /// </summary>
        public void FullHeal()
        {
            CurrentHP = MaxHP;
            Status = PersistentStatus.None;
            VolatileStatus = VolatileStatus.None;
            StatusTurnCounter = 0;

            foreach (var move in Moves)
            {
                move.RestoreFully();
            }
        }

        #endregion

        #region HP Management

        /// <summary>
        /// Applies damage to this Pokemon.
        /// Returns actual damage dealt (may be less if would overkill).
        /// </summary>
        public int TakeDamage(int amount)
        {
            if (amount < 0)
                throw new ArgumentException("Damage cannot be negative", nameof(amount));

            int actualDamage = Math.Min(amount, CurrentHP);
            CurrentHP -= actualDamage;
            return actualDamage;
        }

        /// <summary>
        /// Heals this Pokemon.
        /// Returns actual HP restored (may be less if would overheal).
        /// </summary>
        public int Heal(int amount)
        {
            if (amount < 0)
                throw new ArgumentException("Heal amount cannot be negative", nameof(amount));

            int actualHeal = Math.Min(amount, MaxHP - CurrentHP);
            CurrentHP += actualHeal;
            return actualHeal;
        }

        #endregion

        #region Friendship

        /// <summary>
        /// Increases friendship by the specified amount (capped at 255).
        /// Returns the actual change applied.
        /// </summary>
        public int IncreaseFriendship(int amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative", nameof(amount));

            int oldFriendship = Friendship;
            Friendship = Math.Min(CoreConstants.MaxFriendship, Friendship + amount);
            return Friendship - oldFriendship;
        }

        /// <summary>
        /// Decreases friendship by the specified amount (minimum 0).
        /// Returns the actual change applied (as positive number).
        /// </summary>
        public int DecreaseFriendship(int amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative", nameof(amount));

            int oldFriendship = Friendship;
            Friendship = Math.Max(0, Friendship - amount);
            return oldFriendship - Friendship;
        }

        #endregion
    }
}

