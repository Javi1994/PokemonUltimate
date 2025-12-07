using System.Collections.Generic;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Infrastructure.Builders;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Content.Catalogs.Field
{
    /// <summary>
    /// Central catalog of all entry hazard definitions.
    /// </summary>
    /// <remarks>
    /// **Feature**: 3: Content Expansion
    /// **Sub-Feature**: 3.6: Field Conditions Expansion
    /// **Documentation**: See `docs/features/3-content-expansion/3.6-field-conditions-expansion/README.md`
    /// </remarks>
    public static class HazardCatalog
    {
        private static readonly List<HazardData> _all = new List<HazardData>();

        /// <summary>
        /// Gets all registered hazards.
        /// </summary>
        public static IReadOnlyList<HazardData> All => _all;

        #region Hazard Definitions

        /// <summary>
        /// Stealth Rock - Deals damage based on type effectiveness vs Rock.
        /// 0.25x = 3.125%, 0.5x = 6.25%, 1x = 12.5%, 2x = 25%, 4x = 50%.
        /// Affects all Pokemon including Flying types.
        /// </summary>
        public static readonly HazardData StealthRock = Hazard.Define("Stealth Rock")
            .Description("Pointed stones float around the opposing team, damaging Pokemon that switch in.")
            .Type(HazardType.StealthRock)
            .MaxLayers(1)
            .AffectsFlying()
            .DealsDamage(PokemonType.Rock, 0.125f) // Base 12.5%, modified by type effectiveness
            .Build();

        /// <summary>
        /// Spikes - Deals percentage HP damage based on layers.
        /// 1 layer = 12.5%, 2 layers = 16.67%, 3 layers = 25%.
        /// Does not affect Flying types or Levitate.
        /// </summary>
        public static readonly HazardData Spikes = Hazard.Define("Spikes")
            .Description("Sharp spikes hurt Pokemon that switch in.")
            .Type(HazardType.Spikes)
            .MaxLayers(3)
            .GroundedOnly()
            .DealsFixedDamage(0.125f, 0.167f, 0.25f)
            .Build();

        /// <summary>
        /// Toxic Spikes - Applies status based on layers.
        /// 1 layer = Poison, 2 layers = Badly Poisoned.
        /// Poison types absorb on entry. Steel types immune.
        /// Does not affect Flying types or Levitate.
        /// </summary>
        public static readonly HazardData ToxicSpikes = Hazard.Define("Toxic Spikes")
            .Description("Poison spikes that poison Pokemon that switch in.")
            .Type(HazardType.ToxicSpikes)
            .MaxLayers(2)
            .GroundedOnly()
            .AppliesStatus(PersistentStatus.Poison, PersistentStatus.BadlyPoisoned)
            .AbsorbedByPoisonTypes()
            .Build();

        /// <summary>
        /// Sticky Web - Lowers Speed by 1 stage on entry.
        /// Does not affect Flying types or Levitate.
        /// </summary>
        public static readonly HazardData StickyWeb = Hazard.Define("Sticky Web")
            .Description("A sticky web that lowers the Speed of Pokemon that switch in.")
            .Type(HazardType.StickyWeb)
            .MaxLayers(1)
            .GroundedOnly()
            .LowersStat(Stat.Speed, -1)
            .Build();

        #endregion

        #region Lookup Methods

        /// <summary>
        /// Gets hazard data by type.
        /// </summary>
        public static HazardData GetByType(HazardType type)
        {
            switch (type)
            {
                case HazardType.StealthRock: return StealthRock;
                case HazardType.Spikes: return Spikes;
                case HazardType.ToxicSpikes: return ToxicSpikes;
                case HazardType.StickyWeb: return StickyWeb;
                default: return null;
            }
        }

        /// <summary>
        /// Gets hazard data by name.
        /// </summary>
        public static HazardData GetByName(string name)
        {
            return _all.Find(h => h.Name == name);
        }

        #endregion

        #region Static Constructor

        static HazardCatalog()
        {
            _all.Add(StealthRock);
            _all.Add(Spikes);
            _all.Add(ToxicSpikes);
            _all.Add(StickyWeb);
        }

        #endregion
    }
}

