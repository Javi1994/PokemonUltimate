using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.ValueObjects
{
    /// <summary>
    /// Value Object representing the current terrain state on the battlefield.
    /// Encapsulates terrain type, duration, and associated data.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.13: Terrain System
    /// **Documentation**: See `docs/features/2-combat-system/2.13-terrain-system/README.md`
    /// </remarks>
    public class TerrainState
    {
        /// <summary>
        /// Creates a new TerrainState instance with no terrain active.
        /// </summary>
        public TerrainState()
        {
            Terrain = Terrain.None;
            Duration = 0;
            TerrainData = null;
        }

        /// <summary>
        /// Creates a new TerrainState instance with the specified values.
        /// </summary>
        /// <param name="terrain">The terrain type.</param>
        /// <param name="duration">The remaining duration in turns. 0 means infinite duration.</param>
        /// <param name="terrainData">The terrain data. Can be null.</param>
        private TerrainState(Terrain terrain, int duration, TerrainData terrainData)
        {
            Terrain = terrain;
            Duration = duration;
            TerrainData = terrainData;
        }

        /// <summary>
        /// The current terrain condition on the battlefield.
        /// </summary>
        public Terrain Terrain { get; }

        /// <summary>
        /// The remaining duration of the current terrain in turns.
        /// 0 means infinite duration.
        /// </summary>
        public int Duration { get; }

        /// <summary>
        /// The terrain data for the current terrain condition.
        /// Null if no terrain is active.
        /// </summary>
        public TerrainData TerrainData { get; }

        /// <summary>
        /// True if terrain is currently active (not None).
        /// </summary>
        public bool IsActive => Terrain != Terrain.None;

        /// <summary>
        /// True if the terrain has infinite duration (Duration == 0 and terrain is active).
        /// </summary>
        public bool IsInfinite => IsActive && Duration == 0;

        /// <summary>
        /// Creates a new TerrainState instance with the terrain set.
        /// </summary>
        /// <param name="terrain">The terrain to set. Use Terrain.None to clear.</param>
        /// <param name="duration">The duration in turns. 0 means infinite.</param>
        /// <param name="terrainData">The terrain data. Can be null.</param>
        /// <returns>A new TerrainState instance with the terrain set.</returns>
        public TerrainState SetTerrain(Terrain terrain, int duration, TerrainData terrainData = null)
        {
            if (terrain == Terrain.None)
                return new TerrainState();

            return new TerrainState(terrain, duration, terrainData);
        }

        /// <summary>
        /// Creates a new TerrainState instance with the terrain cleared.
        /// </summary>
        /// <returns>A new TerrainState instance with no terrain active.</returns>
        public TerrainState Clear()
        {
            return new TerrainState();
        }

        /// <summary>
        /// Creates a new TerrainState instance with the duration decremented by one turn.
        /// If duration reaches 0 and terrain is not infinite, clears the terrain.
        /// </summary>
        /// <returns>A new TerrainState instance with decremented duration, or cleared if duration expired.</returns>
        public TerrainState DecrementDuration()
        {
            if (!IsActive)
                return this;

            if (Duration == 0)
                return this; // Infinite duration

            var newDuration = Duration - 1;
            if (newDuration <= 0)
                return new TerrainState(); // Duration expired, clear terrain

            return new TerrainState(Terrain, newDuration, TerrainData);
        }
    }
}
