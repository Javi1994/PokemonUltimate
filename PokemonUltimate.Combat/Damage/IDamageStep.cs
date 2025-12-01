namespace PokemonUltimate.Combat.Damage
{
    /// <summary>
    /// A single step in the damage calculation pipeline.
    /// Each step processes the context and may modify calculation state.
    /// </summary>
    public interface IDamageStep
    {
        /// <summary>
        /// Process this step of the damage calculation.
        /// </summary>
        /// <param name="context">The damage context to process.</param>
        void Process(DamageContext context);
    }
}

