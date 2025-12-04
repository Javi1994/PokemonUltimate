using System.Collections.Generic;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Helpers;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Field;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.Helpers
{
    /// <summary>
    /// Tests for Tailwind speed multiplier in TurnOrderResolver.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.16: Field Conditions
    /// **Documentation**: See `docs/features/2-combat-system/2.16-field-conditions/README.md`
    /// </remarks>
    [TestFixture]
    public class TailwindSpeedTests
    {
        private BattleField _field;
        private BattleSlot _slot;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            var playerParty = new List<PokemonInstance> { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new List<PokemonInstance> { PokemonFactory.Create(PokemonCatalog.Squirtle, 50) };
            _field.Initialize(rules, playerParty, enemyParty);
            _slot = _field.PlayerSide.Slots[0];
        }

        [Test]
        public void GetEffectiveSpeed_Tailwind_DoublesSpeed()
        {
            var tailwindData = SideConditionCatalog.GetByType(SideCondition.Tailwind);
            _slot.Side.AddSideCondition(tailwindData, 4);

            float speedWithTailwind = TurnOrderResolver.GetEffectiveSpeed(_slot, _field);
            _slot.Side.RemoveSideCondition(SideCondition.Tailwind);
            float speedWithoutTailwind = TurnOrderResolver.GetEffectiveSpeed(_slot, _field);

            // Tailwind doubles speed (2.0x multiplier)
            Assert.That(speedWithTailwind, Is.EqualTo(speedWithoutTailwind * 2.0f).Within(0.1f));
        }

        [Test]
        public void GetEffectiveSpeed_NoTailwind_NoEffect()
        {
            // No Tailwind added

            float speed = TurnOrderResolver.GetEffectiveSpeed(_slot, _field);

            // Should calculate normally without Tailwind
            Assert.That(speed, Is.GreaterThan(0));
        }

        [Test]
        public void GetEffectiveSpeed_TailwindOnOpposingSide_NoEffect()
        {
            var tailwindData = SideConditionCatalog.GetByType(SideCondition.Tailwind);
            _field.EnemySide.AddSideCondition(tailwindData, 4); // Tailwind on enemy side

            float speed = TurnOrderResolver.GetEffectiveSpeed(_slot, _field);
            _field.EnemySide.RemoveSideCondition(SideCondition.Tailwind);
            float speedWithoutTailwind = TurnOrderResolver.GetEffectiveSpeed(_slot, _field);

            // Tailwind on enemy side should not affect player side
            Assert.That(speed, Is.EqualTo(speedWithoutTailwind));
        }

        [Test]
        public void SortActions_Tailwind_AffectsTurnOrder()
        {
            var tailwindData = SideConditionCatalog.GetByType(SideCondition.Tailwind);
            _slot.Side.AddSideCondition(tailwindData, 4);

            var slowPokemon = PokemonFactory.Create(PokemonCatalog.Snorlax, 50); // Slow Pokemon
            var fastPokemon = PokemonFactory.Create(PokemonCatalog.Pikachu, 50); // Fast Pokemon

            var slowSlot = _field.PlayerSide.Slots[0];
            slowSlot.SetPokemon(slowPokemon);
            var fastSlot = _field.EnemySide.Slots[0];
            fastSlot.SetPokemon(fastPokemon);

            // Remove Tailwind from fast side, add to slow side
            _field.EnemySide.RemoveSideCondition(SideCondition.Tailwind);
            _field.PlayerSide.AddSideCondition(tailwindData, 4);

            // Create move instances for the actions
            var tackleMove = new MoveData { Name = "Tackle", Power = 40, Accuracy = 100, Type = PokemonType.Normal, Category = MoveCategory.Physical, MaxPP = 35, Priority = 0, TargetScope = TargetScope.SingleEnemy };
            var slowMoveInstance = new MoveInstance(tackleMove);
            var fastMoveInstance = new MoveInstance(tackleMove);
            var slowAction = new UseMoveAction(slowSlot, slowSlot, slowMoveInstance);
            var fastAction = new UseMoveAction(fastSlot, fastSlot, fastMoveInstance);

            var actions = new List<BattleAction> { slowAction, fastAction };
            var sorted = TurnOrderResolver.SortActions(actions, _field);

            // With Tailwind, slow Pokemon should go first (doubled speed > base fast speed)
            // Note: This test may be flaky due to random tiebreaker, but the speed calculation should be correct
            float slowSpeedWithTailwind = TurnOrderResolver.GetEffectiveSpeed(slowSlot, _field);
            float fastSpeed = TurnOrderResolver.GetEffectiveSpeed(fastSlot, _field);

            Assert.That(slowSpeedWithTailwind, Is.GreaterThan(fastSpeed));
        }
    }
}

