using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Content.Builders;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Content.Catalogs.Abilities
{
    /// <summary>
    /// Generation 5 abilities (Black/White).
    /// </summary>
    /// <remarks>
    /// **Feature**: 3: Content Expansion
    /// **Sub-Feature**: 3.4: Ability Expansion
    /// **Documentation**: See `docs/features/3-content-expansion/3.4-ability-expansion/README.md`
    /// </remarks>
    public static partial class AbilityCatalog
    {
        // ===== STAT MODIFICATION ABILITIES =====

        /// <summary>
        /// Boosts Attack after knocking out any Pokémon.
        /// </summary>
        public static readonly AbilityData Moxie = Ability.Define("Moxie")
            .Description("Boosts Attack after knocking out any Pokémon.")
            .Gen(5)
            .RaisesStatOnKO(Stat.Attack, 1)
            .Build();

        // ===== CONTACT DAMAGE ABILITIES =====

        /// <summary>
        /// Damages attackers that make contact.
        /// </summary>
        public static readonly AbilityData IronBarbs = Ability.Define("Iron Barbs")
            .Description("Damages attackers that make contact.")
            .Gen(5)
            .DamagesOnContact(0.125f) // 1/8 HP
            .Build();

        // ===== REGISTRATION =====

        static partial void RegisterGen5Abilities()
        {
            _all.Add(Moxie);
            _all.Add(IronBarbs);
        }
    }
}

