using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Builders;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Content.Catalogs.Abilities
{
    /// <summary>
    /// Generation 3 abilities (Ruby/Sapphire - first abilities).
    /// </summary>
    public static partial class AbilityCatalog
    {
        // ===== STAT MODIFICATION ABILITIES =====

        /// <summary>
        /// Lowers opposing Pokemon's Attack on switch-in.
        /// </summary>
        public static readonly AbilityData Intimidate = Ability.Define("Intimidate")
            .Description("Lowers the opposing Pokémon's Attack stat.")
            .Gen(3)
            .LowersOpponentStat(Stat.Attack, -1)
            .Build();

        /// <summary>
        /// Raises Speed at the end of each turn.
        /// </summary>
        public static readonly AbilityData SpeedBoost = Ability.Define("Speed Boost")
            .Description("Its Speed stat is boosted every turn.")
            .Gen(3)
            .RaisesStatEachTurn(Stat.Speed, 1)
            .Build();

        /// <summary>
        /// Prevents stats from being lowered.
        /// </summary>
        public static readonly AbilityData ClearBody = Ability.Define("Clear Body")
            .Description("Prevents other Pokémon from lowering its stats.")
            .Gen(3)
            .PreventsStatLoss()
            .Build();

        /// <summary>
        /// Doubles Attack stat.
        /// </summary>
        public static readonly AbilityData HugePower = Ability.Define("Huge Power")
            .Description("Doubles the Pokémon's Attack stat.")
            .Gen(3)
            .PassiveStatMultiplier(Stat.Attack, 2.0f)
            .Build();

        // ===== STATUS-RELATED ABILITIES =====

        /// <summary>
        /// May paralyze on contact.
        /// </summary>
        public static readonly AbilityData Static = Ability.Define("Static")
            .Description("May cause paralysis on contact.")
            .Gen(3)
            .ChanceToStatusOnContact(PersistentStatus.Paralysis, 0.30f)
            .Build();

        /// <summary>
        /// May poison on contact.
        /// </summary>
        public static readonly AbilityData PoisonPoint = Ability.Define("Poison Point")
            .Description("May poison on contact.")
            .Gen(3)
            .ChanceToStatusOnContact(PersistentStatus.Poison, 0.30f)
            .Build();

        /// <summary>
        /// May burn on contact.
        /// </summary>
        public static readonly AbilityData FlameBody = Ability.Define("Flame Body")
            .Description("May burn on contact.")
            .Gen(3)
            .ChanceToStatusOnContact(PersistentStatus.Burn, 0.30f)
            .Build();

        /// <summary>
        /// Prevents paralysis.
        /// </summary>
        public static readonly AbilityData Limber = Ability.Define("Limber")
            .Description("Prevents paralysis.")
            .Gen(3)
            .PreventsStatus(PersistentStatus.Paralysis)
            .Build();

        /// <summary>
        /// Prevents poison.
        /// </summary>
        public static readonly AbilityData Immunity = Ability.Define("Immunity")
            .Description("Prevents poisoning.")
            .Gen(3)
            .PreventsStatus(PersistentStatus.Poison)
            .Build();

        // ===== TYPE IMMUNITY ABILITIES =====

        /// <summary>
        /// Ground moves have no effect.
        /// </summary>
        public static readonly AbilityData Levitate = Ability.Define("Levitate")
            .Description("Ground-type moves have no effect.")
            .Gen(3)
            .GroundImmunity()
            .Build();

        /// <summary>
        /// Fire moves boost Fire power instead of dealing damage.
        /// </summary>
        public static readonly AbilityData FlashFire = Ability.Define("Flash Fire")
            .Description("Powers up Fire moves if hit by fire.")
            .Gen(3)
            .TypeImmunity(PokemonType.Fire)
            .Build();

        /// <summary>
        /// Water moves heal instead of dealing damage.
        /// </summary>
        public static readonly AbilityData WaterAbsorb = Ability.Define("Water Absorb")
            .Description("Heals HP if hit by Water-type moves.")
            .Gen(3)
            .HealsFromType(PokemonType.Water, 0.25f)
            .Build();

        /// <summary>
        /// Electric moves heal instead of dealing damage.
        /// </summary>
        public static readonly AbilityData VoltAbsorb = Ability.Define("Volt Absorb")
            .Description("Heals HP if hit by Electric-type moves.")
            .Gen(3)
            .HealsFromType(PokemonType.Electric, 0.25f)
            .Build();

        // ===== POWER BOOST ABILITIES =====

        /// <summary>
        /// Boosts Fire moves when HP is low.
        /// </summary>
        public static readonly AbilityData Blaze = Ability.Define("Blaze")
            .Description("Powers up Fire moves when HP is low.")
            .Gen(3)
            .BoostsTypeWhenLowHP(PokemonType.Fire)
            .Build();

        /// <summary>
        /// Boosts Water moves when HP is low.
        /// </summary>
        public static readonly AbilityData Torrent = Ability.Define("Torrent")
            .Description("Powers up Water moves when HP is low.")
            .Gen(3)
            .BoostsTypeWhenLowHP(PokemonType.Water)
            .Build();

        /// <summary>
        /// Boosts Grass moves when HP is low.
        /// </summary>
        public static readonly AbilityData Overgrow = Ability.Define("Overgrow")
            .Description("Powers up Grass moves when HP is low.")
            .Gen(3)
            .BoostsTypeWhenLowHP(PokemonType.Grass)
            .Build();

        /// <summary>
        /// Boosts Bug moves when HP is low.
        /// </summary>
        public static readonly AbilityData Swarm = Ability.Define("Swarm")
            .Description("Powers up Bug moves when HP is low.")
            .Gen(3)
            .BoostsTypeWhenLowHP(PokemonType.Bug)
            .Build();

        // ===== DAMAGE REDUCTION ABILITIES =====

        /// <summary>
        /// Reduces Fire and Ice damage by 50%.
        /// </summary>
        public static readonly AbilityData ThickFat = Ability.Define("Thick Fat")
            .Description("Halves damage from Fire and Ice moves.")
            .Gen(3)
            .ReducesDamageFromType(PokemonType.Fire, 0.5f, PokemonType.Ice)
            .Build();

        // ===== WEATHER ABILITIES =====

        /// <summary>
        /// Summons rain on switch-in.
        /// </summary>
        public static readonly AbilityData Drizzle = Ability.Define("Drizzle")
            .Description("Summons rain on entry.")
            .Gen(3)
            .SummonsWeather(Weather.Rain)
            .Build();

        /// <summary>
        /// Summons sun on switch-in.
        /// </summary>
        public static readonly AbilityData Drought = Ability.Define("Drought")
            .Description("Summons harsh sunlight on entry.")
            .Gen(3)
            .SummonsWeather(Weather.Sun)
            .Build();

        /// <summary>
        /// Summons sandstorm on switch-in.
        /// </summary>
        public static readonly AbilityData SandStream = Ability.Define("Sand Stream")
            .Description("Summons a sandstorm on entry.")
            .Gen(3)
            .SummonsWeather(Weather.Sandstorm)
            .Build();

        /// <summary>
        /// Doubles Speed in rain.
        /// </summary>
        public static readonly AbilityData SwiftSwim = Ability.Define("Swift Swim")
            .Description("Doubles Speed in rain.")
            .Gen(3)
            .SpeedBoostInWeather(Weather.Rain)
            .Build();

        /// <summary>
        /// Doubles Speed in sun.
        /// </summary>
        public static readonly AbilityData Chlorophyll = Ability.Define("Chlorophyll")
            .Description("Doubles Speed in sunlight.")
            .Gen(3)
            .SpeedBoostInWeather(Weather.Sun)
            .Build();

        // ===== SURVIVAL ABILITIES =====

        /// <summary>
        /// Survives a fatal hit at full HP.
        /// </summary>
        public static readonly AbilityData Sturdy = Ability.Define("Sturdy")
            .Description("Cannot be knocked out with one hit if at full HP.")
            .Gen(3)
            .SurvivesFatalHit()
            .Build();

        // ===== CONTACT DAMAGE ABILITIES =====

        /// <summary>
        /// Damages attacker on contact.
        /// </summary>
        public static readonly AbilityData RoughSkin = Ability.Define("Rough Skin")
            .Description("Damages attackers that make contact.")
            .Gen(3)
            .DamagesOnContact(0.125f)
            .Build();

        // ===== REGISTRATION =====

        static partial void RegisterGen3Abilities()
        {
            _all.Add(Intimidate);
            _all.Add(SpeedBoost);
            _all.Add(ClearBody);
            _all.Add(HugePower);
            _all.Add(Static);
            _all.Add(PoisonPoint);
            _all.Add(FlameBody);
            _all.Add(Limber);
            _all.Add(Immunity);
            _all.Add(Levitate);
            _all.Add(FlashFire);
            _all.Add(WaterAbsorb);
            _all.Add(VoltAbsorb);
            _all.Add(Blaze);
            _all.Add(Torrent);
            _all.Add(Overgrow);
            _all.Add(Swarm);
            _all.Add(ThickFat);
            _all.Add(Drizzle);
            _all.Add(Drought);
            _all.Add(SandStream);
            _all.Add(SwiftSwim);
            _all.Add(Chlorophyll);
            _all.Add(Sturdy);
            _all.Add(RoughSkin);
        }
    }
}

