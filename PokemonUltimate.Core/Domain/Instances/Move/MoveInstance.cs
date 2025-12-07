using System;
using PokemonUltimate.Core.Data.Blueprints;

namespace PokemonUltimate.Core.Domain.Instances.Move
{
    /// <summary>
    /// Runtime instance of a move with PP tracking.
    /// Created from a MoveData blueprint, tracks current PP for a specific Pokemon.
    /// Supports PP Ups (up to 3, increasing MaxPP by 20% each).
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.2: Move Data
    /// **Documentation**: See `docs/features/1-game-data/1.2-move-data/architecture.md`
    /// </remarks>
    public class MoveInstance
    {
        #region Constants

        /// <summary>
        /// Maximum number of PP Ups that can be applied.
        /// </summary>
        public const int MaxPPUps = 3;

        /// <summary>
        /// PP increase per PP Up (20% of base).
        /// </summary>
        public const float PPUpBonus = 0.2f;

        #endregion

        #region Properties

        /// <summary>
        /// Reference to the move blueprint (immutable data).
        /// </summary>
        public MoveData Move { get; }

        /// <summary>
        /// Base maximum PP from the move data.
        /// </summary>
        public int BasePP { get; }

        /// <summary>
        /// Number of PP Ups applied (0-3).
        /// </summary>
        public int PPUps { get; private set; }

        /// <summary>
        /// Maximum PP for this move instance (including PP Ups).
        /// Formula: BasePP * (1 + 0.2 * PPUps)
        /// </summary>
        public int MaxPP => BasePP + (int)(BasePP * PPUpBonus * PPUps);

        /// <summary>
        /// Current PP remaining.
        /// </summary>
        public int CurrentPP { get; private set; }

        /// <summary>
        /// Returns true if this move has PP remaining.
        /// </summary>
        public bool HasPP => CurrentPP > 0;

        /// <summary>
        /// Returns true if more PP Ups can be applied.
        /// </summary>
        public bool CanApplyPPUp => PPUps < MaxPPUps;

        /// <summary>
        /// Returns true if maximum PP Ups have been applied.
        /// </summary>
        public bool IsMaxPP => PPUps >= MaxPPUps;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new move instance from a blueprint.
        /// </summary>
        public MoveInstance(MoveData move) : this(move, 0)
        {
        }

        /// <summary>
        /// Creates a new move instance with PP Ups already applied.
        /// </summary>
        public MoveInstance(MoveData move, int ppUps)
        {
            Move = move ?? throw new ArgumentNullException(nameof(move));
            BasePP = move.MaxPP;
            PPUps = Math.Max(0, Math.Min(MaxPPUps, ppUps));
            CurrentPP = MaxPP;
        }

        #endregion

        #region PP Methods

        /// <summary>
        /// Uses the move, consuming 1 PP.
        /// Returns true if successful, false if no PP remaining.
        /// </summary>
        public bool Use()
        {
            if (CurrentPP <= 0)
                return false;

            CurrentPP--;
            return true;
        }

        /// <summary>
        /// Uses the move, consuming specified PP amount (for Pressure ability).
        /// Returns true if successful, false if not enough PP.
        /// </summary>
        public bool Use(int amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative", nameof(amount));

            if (CurrentPP < amount)
                return false;

            CurrentPP -= amount;
            return true;
        }

        /// <summary>
        /// Restores PP by the specified amount (capped at MaxPP).
        /// </summary>
        public void Restore(int amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative", nameof(amount));

            CurrentPP = Math.Min(CurrentPP + amount, MaxPP);
        }

        /// <summary>
        /// Fully restores PP to maximum.
        /// </summary>
        public void RestoreFully()
        {
            CurrentPP = MaxPP;
        }

        #endregion

        #region PP Up Methods

        /// <summary>
        /// Applies a PP Up, increasing max PP by 20% of base.
        /// Returns true if applied, false if already at max.
        /// </summary>
        public bool ApplyPPUp()
        {
            if (PPUps >= MaxPPUps)
                return false;

            int oldMax = MaxPP;
            PPUps++;
            int ppGain = MaxPP - oldMax;
            CurrentPP += ppGain; // Also restore the gained PP

            return true;
        }

        /// <summary>
        /// Applies PP Max (maxes out PP Ups).
        /// Returns the number of PP Ups that were applied.
        /// </summary>
        public int ApplyPPMax()
        {
            int applied = 0;
            while (ApplyPPUp())
                applied++;
            return applied;
        }

        #endregion
    }
}

