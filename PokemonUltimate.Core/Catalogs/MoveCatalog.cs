using System.Collections.Generic;
using PokemonUltimate.Core.Data;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Interfaces;

namespace PokemonUltimate.Core.Catalogs
{
    // Static catalog of all Moves in the game.
    // Provides direct access to Move data and bulk registration to registries.
    public static class MoveCatalog
    {
        #region Normal Type Moves

        public static readonly MoveData Tackle = new MoveData
        {
            Name = "Tackle",
            Description = "A physical attack in which the user charges and slams into the target with its whole body.",
            Type = PokemonType.Normal,
            Category = MoveCategory.Physical,
            Power = 40,
            Accuracy = 100,
            MaxPP = 35,
            Priority = 0,
            TargetScope = TargetScope.SingleEnemy,
            Effects = { new DamageEffect() }
        };

        public static readonly MoveData Scratch = new MoveData
        {
            Name = "Scratch",
            Description = "Hard, pointed, sharp claws rake the target to inflict damage.",
            Type = PokemonType.Normal,
            Category = MoveCategory.Physical,
            Power = 40,
            Accuracy = 100,
            MaxPP = 35,
            Priority = 0,
            TargetScope = TargetScope.SingleEnemy,
            Effects = { new DamageEffect() }
        };

        public static readonly MoveData QuickAttack = new MoveData
        {
            Name = "Quick Attack",
            Description = "The user lunges at the target at a speed that makes it almost invisible. This move always goes first.",
            Type = PokemonType.Normal,
            Category = MoveCategory.Physical,
            Power = 40,
            Accuracy = 100,
            MaxPP = 30,
            Priority = 1,
            TargetScope = TargetScope.SingleEnemy,
            Effects = { new DamageEffect() }
        };

        public static readonly MoveData HyperBeam = new MoveData
        {
            Name = "Hyper Beam",
            Description = "The target is attacked with a powerful beam. The user can't move on the next turn.",
            Type = PokemonType.Normal,
            Category = MoveCategory.Special,
            Power = 150,
            Accuracy = 90,
            MaxPP = 5,
            Priority = 0,
            TargetScope = TargetScope.SingleEnemy,
            Effects = { new DamageEffect() }
            // Note: Recharge effect will be added later
        };

        public static readonly MoveData Growl = new MoveData
        {
            Name = "Growl",
            Description = "The user growls in an endearing way, making opposing Pokémon less wary. This lowers their Attack stats.",
            Type = PokemonType.Normal,
            Category = MoveCategory.Status,
            Power = 0,
            Accuracy = 100,
            MaxPP = 40,
            Priority = 0,
            TargetScope = TargetScope.AllEnemies,
            Effects = { new StatChangeEffect(Stat.Attack, -1, targetSelf: false) }
        };

        #endregion

        #region Fire Type Moves

        public static readonly MoveData Ember = new MoveData
        {
            Name = "Ember",
            Description = "The target is attacked with small flames. This may also leave the target with a burn.",
            Type = PokemonType.Fire,
            Category = MoveCategory.Special,
            Power = 40,
            Accuracy = 100,
            MaxPP = 25,
            Priority = 0,
            TargetScope = TargetScope.SingleEnemy,
            Effects = 
            { 
                new DamageEffect(),
                new StatusEffect(PersistentStatus.Burn, 10)
            }
        };

        public static readonly MoveData Flamethrower = new MoveData
        {
            Name = "Flamethrower",
            Description = "The target is scorched with an intense blast of fire. This may also leave the target with a burn.",
            Type = PokemonType.Fire,
            Category = MoveCategory.Special,
            Power = 90,
            Accuracy = 100,
            MaxPP = 15,
            Priority = 0,
            TargetScope = TargetScope.SingleEnemy,
            Effects = 
            { 
                new DamageEffect(),
                new StatusEffect(PersistentStatus.Burn, 10)
            }
        };

        public static readonly MoveData FireBlast = new MoveData
        {
            Name = "Fire Blast",
            Description = "The target is attacked with an intense blast of all-consuming fire. This may also leave the target with a burn.",
            Type = PokemonType.Fire,
            Category = MoveCategory.Special,
            Power = 110,
            Accuracy = 85,
            MaxPP = 5,
            Priority = 0,
            TargetScope = TargetScope.SingleEnemy,
            Effects = 
            { 
                new DamageEffect(),
                new StatusEffect(PersistentStatus.Burn, 10)
            }
        };

        #endregion

        #region Water Type Moves

        public static readonly MoveData WaterGun = new MoveData
        {
            Name = "Water Gun",
            Description = "The target is blasted with a forceful shot of water.",
            Type = PokemonType.Water,
            Category = MoveCategory.Special,
            Power = 40,
            Accuracy = 100,
            MaxPP = 25,
            Priority = 0,
            TargetScope = TargetScope.SingleEnemy,
            Effects = { new DamageEffect() }
        };

        public static readonly MoveData Surf = new MoveData
        {
            Name = "Surf",
            Description = "The user attacks everything around it by swamping its surroundings with a giant wave.",
            Type = PokemonType.Water,
            Category = MoveCategory.Special,
            Power = 90,
            Accuracy = 100,
            MaxPP = 15,
            Priority = 0,
            TargetScope = TargetScope.AllEnemies,
            Effects = { new DamageEffect() }
        };

        public static readonly MoveData HydroPump = new MoveData
        {
            Name = "Hydro Pump",
            Description = "The target is blasted by a huge volume of water launched under great pressure.",
            Type = PokemonType.Water,
            Category = MoveCategory.Special,
            Power = 110,
            Accuracy = 80,
            MaxPP = 5,
            Priority = 0,
            TargetScope = TargetScope.SingleEnemy,
            Effects = { new DamageEffect() }
        };

        #endregion

        #region Grass Type Moves

        public static readonly MoveData VineWhip = new MoveData
        {
            Name = "Vine Whip",
            Description = "The target is struck with slender, whiplike vines to inflict damage.",
            Type = PokemonType.Grass,
            Category = MoveCategory.Physical,
            Power = 45,
            Accuracy = 100,
            MaxPP = 25,
            Priority = 0,
            TargetScope = TargetScope.SingleEnemy,
            Effects = { new DamageEffect() }
        };

        public static readonly MoveData RazorLeaf = new MoveData
        {
            Name = "Razor Leaf",
            Description = "Sharp-edged leaves are launched to slash at opposing Pokémon. Critical hits land more easily.",
            Type = PokemonType.Grass,
            Category = MoveCategory.Physical,
            Power = 55,
            Accuracy = 95,
            MaxPP = 25,
            Priority = 0,
            TargetScope = TargetScope.AllEnemies,
            Effects = { new DamageEffect { CritStages = 1 } } // High crit ratio
        };

        public static readonly MoveData SolarBeam = new MoveData
        {
            Name = "Solar Beam",
            Description = "In this two-turn attack, the user gathers light, then blasts a bundled beam on the next turn.",
            Type = PokemonType.Grass,
            Category = MoveCategory.Special,
            Power = 120,
            Accuracy = 100,
            MaxPP = 10,
            Priority = 0,
            TargetScope = TargetScope.SingleEnemy,
            Effects = { new DamageEffect() }
            // Note: Charge effect will be added later
        };

        #endregion

        #region Electric Type Moves

        public static readonly MoveData ThunderShock = new MoveData
        {
            Name = "Thunder Shock",
            Description = "A jolt of electricity crashes down on the target to inflict damage. This may also leave the target with paralysis.",
            Type = PokemonType.Electric,
            Category = MoveCategory.Special,
            Power = 40,
            Accuracy = 100,
            MaxPP = 30,
            Priority = 0,
            TargetScope = TargetScope.SingleEnemy,
            Effects = 
            { 
                new DamageEffect(),
                new StatusEffect(PersistentStatus.Paralysis, 10)
            }
        };

        public static readonly MoveData Thunderbolt = new MoveData
        {
            Name = "Thunderbolt",
            Description = "A strong electric blast crashes down on the target. This may also leave the target with paralysis.",
            Type = PokemonType.Electric,
            Category = MoveCategory.Special,
            Power = 90,
            Accuracy = 100,
            MaxPP = 15,
            Priority = 0,
            TargetScope = TargetScope.SingleEnemy,
            Effects = 
            { 
                new DamageEffect(),
                new StatusEffect(PersistentStatus.Paralysis, 10)
            }
        };

        public static readonly MoveData Thunder = new MoveData
        {
            Name = "Thunder",
            Description = "A wicked thunderbolt is dropped on the target to inflict damage. This may also leave the target with paralysis.",
            Type = PokemonType.Electric,
            Category = MoveCategory.Special,
            Power = 110,
            Accuracy = 70,
            MaxPP = 10,
            Priority = 0,
            TargetScope = TargetScope.SingleEnemy,
            Effects = 
            { 
                new DamageEffect(),
                new StatusEffect(PersistentStatus.Paralysis, 30)
            }
        };

        public static readonly MoveData ThunderWave = new MoveData
        {
            Name = "Thunder Wave",
            Description = "The user launches a weak jolt of electricity that paralyzes the target.",
            Type = PokemonType.Electric,
            Category = MoveCategory.Status,
            Power = 0,
            Accuracy = 90,
            MaxPP = 20,
            Priority = 0,
            TargetScope = TargetScope.SingleEnemy,
            Effects = { new StatusEffect(PersistentStatus.Paralysis, 100) }
        };

        #endregion

        #region Ground Type Moves

        public static readonly MoveData Earthquake = new MoveData
        {
            Name = "Earthquake",
            Description = "The user sets off an earthquake that strikes every Pokémon around it.",
            Type = PokemonType.Ground,
            Category = MoveCategory.Physical,
            Power = 100,
            Accuracy = 100,
            MaxPP = 10,
            Priority = 0,
            TargetScope = TargetScope.AllOthers,
            Effects = { new DamageEffect() }
        };

        #endregion

        #region Psychic Type Moves

        public static readonly MoveData Psychic = new MoveData
        {
            Name = "Psychic",
            Description = "The target is hit by a strong telekinetic force. This may also lower the target's Sp. Def stat.",
            Type = PokemonType.Psychic,
            Category = MoveCategory.Special,
            Power = 90,
            Accuracy = 100,
            MaxPP = 10,
            Priority = 0,
            TargetScope = TargetScope.SingleEnemy,
            Effects = 
            { 
                new DamageEffect(),
                new StatChangeEffect(Stat.SpDefense, -1, targetSelf: false, chancePercent: 10)
            }
        };

        #endregion

        #region All Moves & Registration

        // All moves defined in this catalog
        public static IEnumerable<MoveData> All
        {
            get
            {
                // Normal
                yield return Tackle;
                yield return Scratch;
                yield return QuickAttack;
                yield return HyperBeam;
                yield return Growl;
                // Fire
                yield return Ember;
                yield return Flamethrower;
                yield return FireBlast;
                // Water
                yield return WaterGun;
                yield return Surf;
                yield return HydroPump;
                // Grass
                yield return VineWhip;
                yield return RazorLeaf;
                yield return SolarBeam;
                // Electric
                yield return ThunderShock;
                yield return Thunderbolt;
                yield return Thunder;
                yield return ThunderWave;
                // Ground
                yield return Earthquake;
                // Psychic
                yield return Psychic;
            }
        }

        // Register all moves from this catalog into a registry
        public static void RegisterAll(IMoveRegistry registry)
        {
            foreach (var move in All)
            {
                registry.Register(move);
            }
        }

        // Get count of all moves in catalog
        public static int Count => 20;

        #endregion
    }
}

