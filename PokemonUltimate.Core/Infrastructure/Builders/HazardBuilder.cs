using System;
using System.Collections.Generic;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Core.Infrastructure.Builders
{
    /// <summary>
    /// Fluent builder for creating HazardData.
    /// </summary>
    /// <remarks>
    /// **Feature**: 3: Content Expansion
    /// **Sub-Feature**: 3.9: Builders
    /// **Documentation**: See `docs/features/3-content-expansion/3.9-builders/README.md`
    /// </remarks>
    public class HazardBuilder
    {
        private readonly string _id;
        private readonly string _name;
        private string _description = string.Empty;
        private HazardType _type = HazardType.None;
        private int _maxLayers = 1;
        private bool _affectsFlying = false;
        private bool _affectsLevitate = false;
        private PokemonType? _damageType;
        private List<float> _damageByLayer = new List<float>();
        private List<PersistentStatus> _statusByLayer = new List<PersistentStatus>();
        private bool _absorbedByPoisonTypes = false;
        private Stat? _statToLower;
        private int _statStages = 0;
        private List<string> _removedByMoves = new List<string> { "Rapid Spin", "Defog" };

        private HazardBuilder(string name)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _id = name.ToLowerInvariant().Replace(" ", "-");
        }

        /// <summary>
        /// Start defining a new hazard.
        /// </summary>
        public static HazardBuilder Define(string name) => new HazardBuilder(name);

        #region Identity

        public HazardBuilder Description(string description)
        {
            _description = description;
            return this;
        }

        public HazardBuilder Type(HazardType type)
        {
            _type = type;
            return this;
        }

        #endregion

        #region Layers

        public HazardBuilder MaxLayers(int layers)
        {
            _maxLayers = layers;
            return this;
        }

        #endregion

        #region Targeting

        public HazardBuilder AffectsFlying()
        {
            _affectsFlying = true;
            _affectsLevitate = true;
            return this;
        }

        public HazardBuilder GroundedOnly()
        {
            _affectsFlying = false;
            _affectsLevitate = false;
            return this;
        }

        #endregion

        #region Damage

        public HazardBuilder DealsDamage(PokemonType type, params float[] damagePerLayer)
        {
            _damageType = type;
            _damageByLayer.AddRange(damagePerLayer);
            return this;
        }

        public HazardBuilder DealsFixedDamage(params float[] damagePerLayer)
        {
            _damageType = null;
            _damageByLayer.AddRange(damagePerLayer);
            return this;
        }

        #endregion

        #region Status

        public HazardBuilder AppliesStatus(params PersistentStatus[] statusPerLayer)
        {
            _statusByLayer.AddRange(statusPerLayer);
            return this;
        }

        public HazardBuilder AbsorbedByPoisonTypes()
        {
            _absorbedByPoisonTypes = true;
            return this;
        }

        #endregion

        #region Stats

        public HazardBuilder LowersStat(Stat stat, int stages = -1)
        {
            _statToLower = stat;
            _statStages = stages;
            return this;
        }

        #endregion

        #region Removal

        public HazardBuilder RemovedBy(params string[] moves)
        {
            _removedByMoves = new List<string>(moves);
            return this;
        }

        #endregion

        #region Build

        public HazardData Build()
        {
            return new HazardData(
                _id,
                _name,
                _description,
                _type,
                _maxLayers,
                _affectsFlying,
                _affectsLevitate,
                _damageType,
                _damageByLayer.ToArray(),
                _statusByLayer.ToArray(),
                _absorbedByPoisonTypes,
                _statToLower,
                _statStages,
                _removedByMoves.ToArray());
        }

        #endregion
    }

    /// <summary>
    /// Alias for HazardBuilder for cleaner syntax.
    /// </summary>
    public static class Hazard
    {
        public static HazardBuilder Define(string name) => HazardBuilder.Define(name);
    }
}

