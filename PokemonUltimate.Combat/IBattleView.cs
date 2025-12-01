using System.Threading.Tasks;

namespace PokemonUltimate.Combat
{
    /// <summary>
    /// Interface for battle visualization/presentation.
    /// Implementations handle animations, UI updates, and player feedback.
    /// </summary>
    public interface IBattleView
    {
        /// <summary>
        /// Displays a message to the player.
        /// </summary>
        /// <param name="message">The message to display.</param>
        Task ShowMessage(string message);

        /// <summary>
        /// Plays a damage animation for a slot.
        /// </summary>
        /// <param name="slot">The slot that took damage.</param>
        Task PlayDamageAnimation(BattleSlot slot);

        /// <summary>
        /// Updates the HP bar for a slot (smooth drain effect).
        /// </summary>
        /// <param name="slot">The slot to update.</param>
        Task UpdateHPBar(BattleSlot slot);

        /// <summary>
        /// Plays a move animation.
        /// </summary>
        /// <param name="user">The slot using the move.</param>
        /// <param name="target">The target slot.</param>
        /// <param name="moveId">The move identifier for animation lookup.</param>
        Task PlayMoveAnimation(BattleSlot user, BattleSlot target, string moveId);

        /// <summary>
        /// Plays the faint animation for a Pokemon.
        /// </summary>
        /// <param name="slot">The slot with the fainted Pokemon.</param>
        Task PlayFaintAnimation(BattleSlot slot);

        /// <summary>
        /// Plays an animation for a status effect being applied.
        /// </summary>
        /// <param name="slot">The slot receiving the status.</param>
        /// <param name="statusName">The name of the status for animation lookup.</param>
        Task PlayStatusAnimation(BattleSlot slot, string statusName);

        /// <summary>
        /// Updates the display after a stat change.
        /// </summary>
        /// <param name="slot">The slot whose stat changed.</param>
        /// <param name="statName">The name of the stat.</param>
        /// <param name="stages">The number of stages changed (+/-).</param>
        Task ShowStatChange(BattleSlot slot, string statName, int stages);

        /// <summary>
        /// Plays a switch-out animation.
        /// </summary>
        /// <param name="slot">The slot being switched out.</param>
        Task PlaySwitchOutAnimation(BattleSlot slot);

        /// <summary>
        /// Plays a switch-in animation.
        /// </summary>
        /// <param name="slot">The slot being switched in.</param>
        Task PlaySwitchInAnimation(BattleSlot slot);
    }
}

