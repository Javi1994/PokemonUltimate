using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Builders;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Content.Catalogs.Items
{
    /// <summary>
    /// Berry items that activate at certain conditions.
    /// </summary>
    public static partial class ItemCatalog
    {
        // ===== HP RESTORATION BERRIES =====

        /// <summary>
        /// Restores 10 HP when below 50% HP.
        /// </summary>
        public static readonly ItemData OranBerry = Item.Define("Oran Berry")
            .Description("Restores 10 HP when below half.")
            .Berry()
            .HealsAtLowHP(10, 0.5f)
            .Build();

        /// <summary>
        /// Restores 25% max HP when below 25% HP.
        /// </summary>
        public static readonly ItemData SitrusBerry = Item.Define("Sitrus Berry")
            .Description("Restores 25% HP when below quarter.")
            .Berry()
            .HealsPercentAtLowHP(0.25f, 0.25f)
            .Build();

        // ===== STATUS HEALING BERRIES =====

        /// <summary>
        /// Cures paralysis.
        /// </summary>
        public static readonly ItemData CheriBerrry = Item.Define("Cheri Berry")
            .Description("Cures paralysis.")
            .Berry()
            .CuresStatus(PersistentStatus.Paralysis)
            .Build();

        /// <summary>
        /// Cures sleep.
        /// </summary>
        public static readonly ItemData ChestoBerry = Item.Define("Chesto Berry")
            .Description("Cures sleep.")
            .Berry()
            .CuresStatus(PersistentStatus.Sleep)
            .Build();

        /// <summary>
        /// Cures poison.
        /// </summary>
        public static readonly ItemData PechaBerry = Item.Define("Pecha Berry")
            .Description("Cures poison.")
            .Berry()
            .CuresStatus(PersistentStatus.Poison)
            .Build();

        /// <summary>
        /// Cures burn.
        /// </summary>
        public static readonly ItemData RawstBerry = Item.Define("Rawst Berry")
            .Description("Cures burn.")
            .Berry()
            .CuresStatus(PersistentStatus.Burn)
            .Build();

        /// <summary>
        /// Cures freeze.
        /// </summary>
        public static readonly ItemData AspearBerry = Item.Define("Aspear Berry")
            .Description("Cures freeze.")
            .Berry()
            .CuresStatus(PersistentStatus.Freeze)
            .Build();

        /// <summary>
        /// Cures any status condition.
        /// </summary>
        public static readonly ItemData LumBerry = Item.Define("Lum Berry")
            .Description("Cures any status condition.")
            .Berry()
            .CuresAllStatuses()
            .Build();

        // ===== REGISTRATION =====

        static partial void RegisterBerries()
        {
            _all.Add(OranBerry);
            _all.Add(SitrusBerry);
            _all.Add(CheriBerrry);
            _all.Add(ChestoBerry);
            _all.Add(PechaBerry);
            _all.Add(RawstBerry);
            _all.Add(AspearBerry);
            _all.Add(LumBerry);
        }
    }
}

