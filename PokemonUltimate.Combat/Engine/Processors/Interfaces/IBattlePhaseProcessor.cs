namespace PokemonUltimate.Combat.Processors.Phases
{
    /// <summary>
    /// Base interface for all battle phase processors.
    /// Each processor handles a specific phase/moment in the battle.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    public interface IBattlePhaseProcessor
    {
        /// <summary>
        /// Gets the phase this processor handles.
        /// </summary>
        BattlePhase Phase { get; }
    }
}
