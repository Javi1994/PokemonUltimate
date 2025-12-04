using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Engine;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Field;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.Engine
{
    /// <summary>
    /// Tests for EntryHazardProcessor - processing hazards when Pokemon switch in.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.14: Hazards System
    /// **Documentation**: See `docs/features/2-combat-system/2.14-hazards-system/README.md`
    /// </remarks>
    [TestFixture]
    public class EntryHazardProcessorTests
    {
        private BattleField _field;
        private BattleSlot _slot;
        private List<PokemonInstance> _party;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            _party = new List<PokemonInstance>
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50) // Grounded Pokemon
            };
            _field.Initialize(rules, _party, new[] { PokemonFactory.Create(PokemonCatalog.Squirtle, 50) });
            _slot = _field.PlayerSide.Slots[0];
        }

        #region Spikes Tests

        [Test]
        public void ProcessHazards_Spikes_OneLayer_DamagesOnEntry()
        {
            var hazardData = HazardCatalog.GetByType(HazardType.Spikes);
            // Hazards are on the opposing side (EnemySide)
            _field.EnemySide.AddHazard(hazardData);

            var pokemon = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);
            int maxHP = pokemon.MaxHP;
            int expectedDamage = (int)(maxHP * 0.125f); // 12.5%

            var actions = EntryHazardProcessor.ProcessHazards(_slot, pokemon, _field, HazardCatalog.GetByType).ToList();

            var damageAction = actions.OfType<DamageAction>().FirstOrDefault();
            Assert.That(damageAction, Is.Not.Null);
            Assert.That(damageAction.Context.FinalDamage, Is.EqualTo(expectedDamage));
        }

        [Test]
        public void ProcessHazards_Spikes_ThreeLayers_DamagesCorrectly()
        {
            var hazardData = HazardCatalog.GetByType(HazardType.Spikes);
            _field.EnemySide.AddHazard(hazardData);
            _field.EnemySide.AddHazard(hazardData);
            _field.EnemySide.AddHazard(hazardData); // 3 layers

            var pokemon = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);
            int maxHP = pokemon.MaxHP;
            int expectedDamage = (int)(maxHP * 0.25f); // 25% for 3 layers

            var actions = EntryHazardProcessor.ProcessHazards(_slot, pokemon, _field, HazardCatalog.GetByType).ToList();

            var damageAction = actions.OfType<DamageAction>().FirstOrDefault();
            Assert.That(damageAction, Is.Not.Null);
            Assert.That(damageAction.Context.FinalDamage, Is.EqualTo(expectedDamage));
        }

        [Test]
        public void ProcessHazards_Spikes_FlyingImmune()
        {
            var hazardData = HazardCatalog.GetByType(HazardType.Spikes);
            _field.EnemySide.AddHazard(hazardData);

            var flyingPokemon = PokemonFactory.Create(PokemonCatalog.Charizard, 50); // Flying type

            var actions = EntryHazardProcessor.ProcessHazards(_slot, flyingPokemon, _field, HazardCatalog.GetByType).ToList();

            // Flying types should not be affected by Spikes
            var damageAction = actions.OfType<DamageAction>().FirstOrDefault();
            Assert.That(damageAction, Is.Null);
        }

        #endregion

        #region Stealth Rock Tests

        [Test]
        public void ProcessHazards_StealthRock_DamageByType()
        {
            var hazardData = HazardCatalog.GetByType(HazardType.StealthRock);
            _field.EnemySide.AddHazard(hazardData);

            var pokemon = PokemonFactory.Create(PokemonCatalog.Pikachu, 50); // Electric type (1x vs Rock)
            int maxHP = pokemon.MaxHP;
            int expectedDamage = (int)(maxHP * 0.125f); // 12.5% base

            var actions = EntryHazardProcessor.ProcessHazards(_slot, pokemon, _field, HazardCatalog.GetByType).ToList();

            var damageAction = actions.OfType<DamageAction>().FirstOrDefault();
            Assert.That(damageAction, Is.Not.Null);
            Assert.That(damageAction.Context.FinalDamage, Is.EqualTo(expectedDamage));
        }

        [Test]
        public void ProcessHazards_StealthRock_FlyingAffected()
        {
            var hazardData = HazardCatalog.GetByType(HazardType.StealthRock);
            _field.EnemySide.AddHazard(hazardData);

            var flyingPokemon = PokemonFactory.Create(PokemonCatalog.Charizard, 50); // Fire/Flying type (Rock is 2x vs Fire and 2x vs Flying = 4x total)
            int maxHP = flyingPokemon.MaxHP;
            int expectedDamage = (int)(maxHP * 0.5f); // 50% (4x effectiveness: 2x * 2x)

            var actions = EntryHazardProcessor.ProcessHazards(_slot, flyingPokemon, _field, HazardCatalog.GetByType).ToList();

            // Flying types ARE affected by Stealth Rock (and Fire/Flying gets 4x damage)
            var damageAction = actions.OfType<DamageAction>().FirstOrDefault();
            Assert.That(damageAction, Is.Not.Null);
            Assert.That(damageAction.Context.FinalDamage, Is.EqualTo(expectedDamage));
        }

        #endregion

        #region Toxic Spikes Tests

        [Test]
        public void ProcessHazards_ToxicSpikes_OneLayer_Poisons()
        {
            var hazardData = HazardCatalog.GetByType(HazardType.ToxicSpikes);
            _field.EnemySide.AddHazard(hazardData);

            var pokemon = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);

            var actions = EntryHazardProcessor.ProcessHazards(_slot, pokemon, _field, HazardCatalog.GetByType).ToList();

            var statusAction = actions.OfType<ApplyStatusAction>().FirstOrDefault();
            Assert.That(statusAction, Is.Not.Null);
            Assert.That(statusAction.Status, Is.EqualTo(PersistentStatus.Poison));
        }

        [Test]
        public void ProcessHazards_ToxicSpikes_TwoLayers_BadlyPoisons()
        {
            var hazardData = HazardCatalog.GetByType(HazardType.ToxicSpikes);
            _field.EnemySide.AddHazard(hazardData);
            _field.EnemySide.AddHazard(hazardData); // 2 layers

            var pokemon = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);

            var actions = EntryHazardProcessor.ProcessHazards(_slot, pokemon, _field, HazardCatalog.GetByType).ToList();

            var statusAction = actions.OfType<ApplyStatusAction>().FirstOrDefault();
            Assert.That(statusAction, Is.Not.Null);
            Assert.That(statusAction.Status, Is.EqualTo(PersistentStatus.BadlyPoisoned));
        }

        [Test]
        public void ProcessHazards_ToxicSpikes_PoisonType_Absorbs()
        {
            var hazardData = HazardCatalog.GetByType(HazardType.ToxicSpikes);
            _field.EnemySide.AddHazard(hazardData);

            var poisonPokemon = PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50); // Grass/Poison type

            var actions = EntryHazardProcessor.ProcessHazards(_slot, poisonPokemon, _field, HazardCatalog.GetByType).ToList();

            // Poison types absorb Toxic Spikes (no status applied, spikes removed)
            var statusAction = actions.OfType<ApplyStatusAction>().FirstOrDefault();
            Assert.That(statusAction, Is.Null);
            
            // Spikes should be removed
            Assert.That(_field.EnemySide.HasHazard(HazardType.ToxicSpikes), Is.False);
        }

        [Test]
        public void ProcessHazards_ToxicSpikes_FlyingImmune()
        {
            var hazardData = HazardCatalog.GetByType(HazardType.ToxicSpikes);
            _field.EnemySide.AddHazard(hazardData);

            var flyingPokemon = PokemonFactory.Create(PokemonCatalog.Charizard, 50); // Flying type

            var actions = EntryHazardProcessor.ProcessHazards(_slot, flyingPokemon, _field, HazardCatalog.GetByType).ToList();

            // Flying types should not be affected by Toxic Spikes
            var statusAction = actions.OfType<ApplyStatusAction>().FirstOrDefault();
            Assert.That(statusAction, Is.Null);
        }

        #endregion

        #region Sticky Web Tests

        [Test]
        public void ProcessHazards_StickyWeb_LowersSpeed()
        {
            var hazardData = HazardCatalog.GetByType(HazardType.StickyWeb);
            _field.EnemySide.AddHazard(hazardData);

            var pokemon = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);

            var actions = EntryHazardProcessor.ProcessHazards(_slot, pokemon, _field, HazardCatalog.GetByType).ToList();

            var statChangeAction = actions.OfType<StatChangeAction>().FirstOrDefault();
            Assert.That(statChangeAction, Is.Not.Null);
            Assert.That(statChangeAction.Stat, Is.EqualTo(Stat.Speed));
            Assert.That(statChangeAction.Change, Is.EqualTo(-1));
        }

        [Test]
        public void ProcessHazards_StickyWeb_FlyingImmune()
        {
            var hazardData = HazardCatalog.GetByType(HazardType.StickyWeb);
            _field.EnemySide.AddHazard(hazardData);

            var flyingPokemon = PokemonFactory.Create(PokemonCatalog.Charizard, 50); // Flying type

            var actions = EntryHazardProcessor.ProcessHazards(_slot, flyingPokemon, _field, HazardCatalog.GetByType).ToList();

            // Flying types should not be affected by Sticky Web
            var statChangeAction = actions.OfType<StatChangeAction>().FirstOrDefault();
            Assert.That(statChangeAction, Is.Null);
        }

        #endregion

        #region Multiple Hazards Tests

        [Test]
        public void ProcessHazards_MultipleHazards_AllProcess()
        {
            var spikesData = HazardCatalog.GetByType(HazardType.Spikes);
            var stealthRockData = HazardCatalog.GetByType(HazardType.StealthRock);
            var stickyWebData = HazardCatalog.GetByType(HazardType.StickyWeb);

            _field.EnemySide.AddHazard(spikesData);
            _field.EnemySide.AddHazard(stealthRockData);
            _field.EnemySide.AddHazard(stickyWebData);

            var pokemon = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);

            var actions = EntryHazardProcessor.ProcessHazards(_slot, pokemon, _field, HazardCatalog.GetByType).ToList();

            // Should have damage from Spikes and Stealth Rock, and Speed reduction from Sticky Web
            var damageActions = actions.OfType<DamageAction>().ToList();
            var statChangeAction = actions.OfType<StatChangeAction>().FirstOrDefault();

            Assert.That(damageActions.Count, Is.GreaterThanOrEqualTo(2)); // Spikes + Stealth Rock
            Assert.That(statChangeAction, Is.Not.Null);
            Assert.That(statChangeAction.Stat, Is.EqualTo(Stat.Speed));
        }

        #endregion

        #region No Hazards Tests

        [Test]
        public void ProcessHazards_NoHazards_NoEffect()
        {
            // No hazards added

            var pokemon = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);

            var actions = EntryHazardProcessor.ProcessHazards(_slot, pokemon, _field, HazardCatalog.GetByType).ToList();

            Assert.That(actions.Count, Is.EqualTo(0));
        }

        #endregion
    }
}

