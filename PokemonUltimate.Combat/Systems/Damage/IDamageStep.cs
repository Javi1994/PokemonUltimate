namespace PokemonUltimate.Combat.Systems.Damage
{
    /// <summary>
    /// A single step in the damage calculation pipeline.
    /// Each step processes the context and may modify calculation state.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.4: Damage Calculation Pipeline
    /// **Documentation**: See `docs/features/2-combat-system/2.4-damage-calculation-pipeline/architecture.md`
    /// </remarks>
    public interface IDamageStep
    {
        /// <summary>
        /// Process this step of the damage calculation.
        /// </summary>
        /// <param name="context">The damage context to process.</param>
        void Process(DamageContext context);
    }
}

