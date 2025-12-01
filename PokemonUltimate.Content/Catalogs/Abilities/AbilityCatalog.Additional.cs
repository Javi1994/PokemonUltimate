using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Builders;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Content.Catalogs.Abilities
{
    /// <summary>
    /// Additional abilities needed for Gen 1 Pokemon (introduced in later gens).
    /// </summary>
    public static partial class AbilityCatalog
    {
        // ===== HIDDEN ABILITIES FOR STARTERS =====

        /// <summary>
        /// Powers up Special moves in harsh sunlight.
        /// Hidden ability for Charmander line.
        /// </summary>
        public static readonly AbilityData SolarPower = Ability.Define("Solar Power")
            .Description("In sunshine, Sp. Atk is boosted but HP decreases.")
            .Gen(4)
            .Passive()
            .Build();

        /// <summary>
        /// Heals HP in rain.
        /// Hidden ability for Squirtle line.
        /// </summary>
        public static readonly AbilityData RainDish = Ability.Define("Rain Dish")
            .Description("The Pokémon gradually regains HP in rain.")
            .Gen(3)
            .HealsPercentInWeather(Core.Enums.Weather.Rain, 0.0625f)
            .Build();

        /// <summary>
        /// Draws Electric moves to boost Sp. Atk.
        /// Hidden ability for Pikachu line.
        /// </summary>
        public static readonly AbilityData LightningRod = Ability.Define("Lightning Rod")
            .Description("Draws in Electric-type moves to boost Sp. Atk.")
            .Gen(3)
            .TypeImmunity(PokemonType.Electric)
            .Build();

        // ===== EEVEE ABILITIES =====

        /// <summary>
        /// Powers up moves of the same type.
        /// Normal ability for Eevee.
        /// </summary>
        public static readonly AbilityData Adaptability = Ability.Define("Adaptability")
            .Description("Powers up moves of the same type as the Pokémon.")
            .Gen(4)
            .IncreasesStab(2.0f)
            .Build();

        /// <summary>
        /// Escapes from battle.
        /// Secondary ability for Eevee.
        /// </summary>
        public static readonly AbilityData RunAway = Ability.Define("Run Away")
            .Description("Enables a sure getaway from wild Pokémon.")
            .Gen(3)
            .Passive()
            .Build();

        /// <summary>
        /// Senses super-effective moves.
        /// Hidden ability for Eevee.
        /// </summary>
        public static readonly AbilityData Anticipation = Ability.Define("Anticipation")
            .Description("Senses an opposing Pokémon's dangerous moves.")
            .Gen(4)
            .OnSwitchIn()
            .Build();

        // ===== SNORLAX ABILITIES =====

        /// <summary>
        /// Eats Berries earlier.
        /// Hidden ability for Snorlax.
        /// </summary>
        public static readonly AbilityData Gluttony = Ability.Define("Gluttony")
            .Description("Makes the Pokémon eat a held Berry early.")
            .Gen(3)
            .Passive()
            .Build();

        // ===== LEGENDARY ABILITIES =====

        /// <summary>
        /// Protects from status moves.
        /// Mewtwo's hidden ability.
        /// </summary>
        public static readonly AbilityData Unnerve = Ability.Define("Unnerve")
            .Description("Unnerves opposing Pokémon and makes them unable to eat Berries.")
            .Gen(5)
            .Passive()
            .Build();

        /// <summary>
        /// All moves become Normal-type.
        /// Mew would have Synchronize in the games.
        /// </summary>
        public static readonly AbilityData Synchronize = Ability.Define("Synchronize")
            .Description("Passes status problems to the opposing Pokémon.")
            .Gen(3)
            .OnStatusReceived()
            .Build();

        /// <summary>
        /// Normal ability for Mewtwo.
        /// </summary>
        public static readonly AbilityData Pressure = Ability.Define("Pressure")
            .Description("The Pokémon raises opposing Pokémon's PP usage.")
            .Gen(3)
            .Passive()
            .Build();

        // ===== REGISTRATION =====

        static partial void RegisterAdditionalAbilities()
        {
            _all.Add(SolarPower);
            _all.Add(RainDish);
            _all.Add(LightningRod);
            _all.Add(Adaptability);
            _all.Add(RunAway);
            _all.Add(Anticipation);
            _all.Add(Gluttony);
            _all.Add(Unnerve);
            _all.Add(Synchronize);
            _all.Add(Pressure);
        }
    }
}

