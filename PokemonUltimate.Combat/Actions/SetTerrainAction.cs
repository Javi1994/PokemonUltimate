using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions.Validation;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Handlers.Registry;
using PokemonUltimate.Combat.View.Definition;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Actions
{
    /// <summary>
    /// Sets or changes the terrain condition on the battlefield.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.13: Terrain System
    /// **Documentation**: See `docs/features/2-combat-system/2.13-terrain-system/README.md`
    /// </remarks>
    public class SetTerrainAction : BattleAction
    {
        private readonly CombatEffectHandlerRegistry _handlerRegistry;

        /// <summary>
        /// The terrain condition to set.
        /// </summary>
        public Terrain Terrain { get; }

        /// <summary>
        /// The duration of the terrain in turns (0 = infinite).
        /// </summary>
        public int Duration { get; }

        /// <summary>
        /// The terrain data for this terrain condition.
        /// </summary>
        public TerrainData TerrainData { get; }

        /// <summary>
        /// Creates a new set terrain action.
        /// </summary>
        /// <param name="user">The slot that initiated this terrain change. Can be null for system actions.</param>
        /// <param name="terrain">The terrain to set. Use Terrain.None to clear.</param>
        /// <param name="duration">Duration in turns. 0 means infinite duration.</param>
        /// <param name="terrainData">The terrain data for this terrain condition. Can be null if not available.</param>
        /// <param name="handlerRegistry">The handler registry. If null, creates and initializes a default one.</param>
        public SetTerrainAction(BattleSlot user, Terrain terrain, int duration, TerrainData terrainData = null, CombatEffectHandlerRegistry handlerRegistry = null) : base(user)
        {
            Terrain = terrain;
            Duration = duration;
            TerrainData = terrainData;
            _handlerRegistry = handlerRegistry ?? CombatEffectHandlerRegistry.CreateDefault();
        }

        /// <summary>
        /// Sets the terrain on the battlefield.
        /// </summary>
        public override IEnumerable<BattleAction> ExecuteLogic(BattleField field)
        {
            ActionValidators.ValidateField(field);

            // Clear terrain if None is specified
            if (Terrain == Terrain.None)
            {
                field.ClearTerrain();
                return Enumerable.Empty<BattleAction>();
            }

            // Use Field Condition Handler to validate terrain can be set (for consistency, even though terrains can always be overwritten)
            var fieldHandler = _handlerRegistry.GetFieldConditionHandler();
            if (!fieldHandler.CanSetTerrain(field, Terrain))
            {
                return Enumerable.Empty<BattleAction>();
            }

            // Set the terrain (terrains can always be overwritten, unlike primal weather)
            field.SetTerrain(Terrain, Duration, TerrainData);

            return Enumerable.Empty<BattleAction>();
        }

        /// <summary>
        /// Displays a message about the terrain change.
        /// </summary>
        public override Task ExecuteVisual(IBattleView view)
        {
            ActionValidators.ValidateView(view);

            // Terrain animation not yet implemented in IBattleView
            // For now, just return completed task
            return Task.CompletedTask;
        }
    }
}

