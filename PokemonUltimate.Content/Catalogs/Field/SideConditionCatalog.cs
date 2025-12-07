using System.Collections.Generic;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Infrastructure.Builders;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Content.Catalogs.Field
{
    /// <summary>
    /// Central catalog of all side condition definitions.
    /// </summary>
    /// <remarks>
    /// **Feature**: 3: Content Expansion
    /// **Sub-Feature**: 3.6: Field Conditions Expansion
    /// **Documentation**: See `docs/features/3-content-expansion/3.6-field-conditions-expansion/README.md`
    /// </remarks>
    public static class SideConditionCatalog
    {
        private static readonly List<SideConditionData> _all = new List<SideConditionData>();

        /// <summary>
        /// Gets all registered side conditions.
        /// </summary>
        public static IReadOnlyList<SideConditionData> All => _all;

        #region Screen Definitions

        /// <summary>
        /// Reflect - Reduces physical damage by 50% (Singles) or 33% (Doubles).
        /// Duration: 5 turns (8 with Light Clay).
        /// </summary>
        public static readonly SideConditionData Reflect = Screen.Define("Reflect")
            .Description("A wondrous wall of light reduces physical damage.")
            .Type(SideCondition.Reflect)
            .Duration(5)
            .ExtendedBy("Light Clay", 8)
            .ReducesPhysicalDamage(0.5f, 0.66f)
            .RemovedBy("Brick Break", "Psychic Fangs", "Defog")
            .Build();

        /// <summary>
        /// Light Screen - Reduces special damage by 50% (Singles) or 33% (Doubles).
        /// Duration: 5 turns (8 with Light Clay).
        /// </summary>
        public static readonly SideConditionData LightScreen = Screen.Define("Light Screen")
            .Description("A wondrous wall of light reduces special damage.")
            .Type(SideCondition.LightScreen)
            .Duration(5)
            .ExtendedBy("Light Clay", 8)
            .ReducesSpecialDamage(0.5f, 0.66f)
            .RemovedBy("Brick Break", "Psychic Fangs", "Defog")
            .Build();

        /// <summary>
        /// Aurora Veil - Reduces all damage by 50% (Singles) or 33% (Doubles).
        /// Can only be set in Hail or Snow. Duration: 5 turns (8 with Light Clay).
        /// </summary>
        public static readonly SideConditionData AuroraVeil = Screen.Define("Aurora Veil")
            .Description("A veil of light reduces damage from physical and special moves.")
            .Type(SideCondition.AuroraVeil)
            .Duration(5)
            .ExtendedBy("Light Clay", 8)
            .ReducesAllDamage(0.5f, 0.66f)
            .RequiresWeather(Core.Data.Enums.Weather.Hail, Core.Data.Enums.Weather.Snow)
            .RemovedBy("Brick Break", "Psychic Fangs", "Defog")
            .Build();

        #endregion

        #region Speed/Status Conditions

        /// <summary>
        /// Tailwind - Doubles Speed for the side.
        /// Duration: 4 turns.
        /// </summary>
        public static readonly SideConditionData Tailwind = Screen.Define("Tailwind")
            .Description("A turbulent wind blows, boosting the Speed of the team.")
            .Type(SideCondition.Tailwind)
            .Duration(4)
            .DoublesSpeed()
            .Build();

        /// <summary>
        /// Safeguard - Prevents status conditions.
        /// Duration: 5 turns.
        /// </summary>
        public static readonly SideConditionData Safeguard = Screen.Define("Safeguard")
            .Description("A mystical veil prevents status conditions.")
            .Type(SideCondition.Safeguard)
            .Duration(5)
            .PreventsStatus()
            .Build();

        /// <summary>
        /// Mist - Prevents stat reductions from opponents.
        /// Duration: 5 turns.
        /// </summary>
        public static readonly SideConditionData Mist = Screen.Define("Mist")
            .Description("A mist prevents the team's stats from being lowered.")
            .Type(SideCondition.Mist)
            .Duration(5)
            .PreventsStatReduction()
            .Build();

        /// <summary>
        /// Lucky Chant - Prevents critical hits against the side.
        /// Duration: 5 turns.
        /// </summary>
        public static readonly SideConditionData LuckyChant = Screen.Define("Lucky Chant")
            .Description("A lucky chant prevents the opposing team from landing critical hits.")
            .Type(SideCondition.LuckyChant)
            .Duration(5)
            .PreventsCriticalHits()
            .Build();

        #endregion

        #region Protection Conditions

        /// <summary>
        /// Wide Guard - Protects from spread moves for the turn.
        /// </summary>
        public static readonly SideConditionData WideGuard = Screen.Define("Wide Guard")
            .Description("Protects the team from wide-ranging attacks.")
            .Type(SideCondition.WideGuard)
            .SingleTurn()
            .BlocksSpreadMoves()
            .Build();

        /// <summary>
        /// Quick Guard - Protects from priority moves for the turn.
        /// </summary>
        public static readonly SideConditionData QuickGuard = Screen.Define("Quick Guard")
            .Description("Protects the team from priority moves.")
            .Type(SideCondition.QuickGuard)
            .SingleTurn()
            .BlocksPriorityMoves()
            .Build();

        /// <summary>
        /// Mat Block - Protects from damaging moves for the turn.
        /// Only works on first turn out.
        /// </summary>
        public static readonly SideConditionData MatBlock = Screen.Define("Mat Block")
            .Description("Blocks damaging moves with a mat. Only works on first turn.")
            .Type(SideCondition.MatBlock)
            .SingleTurn()
            .FirstTurnOnly()
            .BlocksDamagingMoves()
            .Build();

        #endregion

        #region Lookup Methods

        /// <summary>
        /// Gets side condition data by type.
        /// </summary>
        public static SideConditionData GetByType(SideCondition type)
        {
            switch (type)
            {
                case SideCondition.Reflect: return Reflect;
                case SideCondition.LightScreen: return LightScreen;
                case SideCondition.AuroraVeil: return AuroraVeil;
                case SideCondition.Tailwind: return Tailwind;
                case SideCondition.Safeguard: return Safeguard;
                case SideCondition.Mist: return Mist;
                case SideCondition.LuckyChant: return LuckyChant;
                case SideCondition.WideGuard: return WideGuard;
                case SideCondition.QuickGuard: return QuickGuard;
                case SideCondition.MatBlock: return MatBlock;
                default: return null;
            }
        }

        /// <summary>
        /// Gets side condition data by name.
        /// </summary>
        public static SideConditionData GetByName(string name)
        {
            return _all.Find(s => s.Name == name);
        }

        /// <summary>
        /// Gets all screen-type conditions (damage reducers).
        /// </summary>
        public static IEnumerable<SideConditionData> GetScreens()
        {
            foreach (var condition in _all)
            {
                if (condition.ReducesDamage) yield return condition;
            }
        }

        #endregion

        #region Static Constructor

        static SideConditionCatalog()
        {
            // Screens
            _all.Add(Reflect);
            _all.Add(LightScreen);
            _all.Add(AuroraVeil);

            // Speed/Status
            _all.Add(Tailwind);
            _all.Add(Safeguard);
            _all.Add(Mist);
            _all.Add(LuckyChant);

            // Protection
            _all.Add(WideGuard);
            _all.Add(QuickGuard);
            _all.Add(MatBlock);
        }

        #endregion
    }
}

