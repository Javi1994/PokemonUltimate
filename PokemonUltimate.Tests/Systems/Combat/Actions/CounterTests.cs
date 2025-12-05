using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.Actions
{
    /// <summary>
    /// Tests for Counter/Mirror Coat move mechanics.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.15: Advanced Move Mechanics
    /// **Documentation**: See `docs/features/2-combat-system/2.15-advanced-move-mechanics/README.md`
    /// </remarks>
    [TestFixture]
    public class CounterTests
    {
        private BattleField _field;
        private BattleSlot _userSlot;
        private BattleSlot _targetSlot;
        private MoveInstance _counterMove;
        private MoveInstance _mirrorCoatMove;
        private MoveInstance _physicalAttackMove;
        private MoveInstance _specialAttackMove;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            _field.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 },
                new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) },
                new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) });

            _userSlot = _field.PlayerSide.Slots[0];
            _targetSlot = _field.EnemySide.Slots[0];

            // Create Counter move
            var counterMoveData = new MoveData
            {
                Name = "Counter",
                Power = 0,
                Accuracy = 100,
                Type = PokemonType.Fighting,
                Category = MoveCategory.Physical,
                MaxPP = 20,
                Priority = -5,
                TargetScope = TargetScope.SingleEnemy,
                Effects = new List<IMoveEffect> { new CounterEffect { IsPhysicalCounter = true } }
            };
            _counterMove = new MoveInstance(counterMoveData);

            // Create Mirror Coat move
            var mirrorCoatMoveData = new MoveData
            {
                Name = "Mirror Coat",
                Power = 0,
                Accuracy = 100,
                Type = PokemonType.Psychic,
                Category = MoveCategory.Special,
                MaxPP = 20,
                Priority = -5,
                TargetScope = TargetScope.SingleEnemy,
                Effects = new List<IMoveEffect> { new CounterEffect { IsPhysicalCounter = false } }
            };
            _mirrorCoatMove = new MoveInstance(mirrorCoatMoveData);

            // Create physical attack move
            _physicalAttackMove = _userSlot.Pokemon.Moves.First(m => m.Move.Category == MoveCategory.Physical);

            // Create special attack move
            _specialAttackMove = _userSlot.Pokemon.Moves.FirstOrDefault(m => m.Move.Category == MoveCategory.Special);
            if (_specialAttackMove == null)
            {
                var specialMoveData = new MoveData
                {
                    Name = "Thunder Shock",
                    Power = 40,
                    Accuracy = 100,
                    Type = PokemonType.Electric,
                    Category = MoveCategory.Special,
                    MaxPP = 30,
                    Priority = 0,
                    TargetScope = TargetScope.SingleEnemy,
                    Effects = new List<IMoveEffect> { new DamageEffect() }
                };
                _specialAttackMove = new MoveInstance(specialMoveData);
            }
        }

        [Test]
        public void UseMoveAction_Counter_Returns2xPhysicalDamage()
        {
            // Arrange
            // First, deal physical damage to target
            var attackAction = new UseMoveAction(_userSlot, _targetSlot, _physicalAttackMove);
            var attackReactions = attackAction.ExecuteLogic(_field).ToList();
            var damageAction = attackReactions.OfType<DamageAction>().FirstOrDefault();
            Assert.That(damageAction, Is.Not.Null);
            
            int damageTaken = damageAction.Context.FinalDamage;
            Assert.That(damageTaken, Is.GreaterThan(0));

            // Execute damage action to register damage
            damageAction.ExecuteLogic(_field);
            
            // Verify damage was recorded
            Assert.That(_targetSlot.PhysicalDamageTakenThisTurn, Is.EqualTo(damageTaken));

            // Act - Use Counter
            var counterAction = new UseMoveAction(_targetSlot, _userSlot, _counterMove);
            var counterReactions = counterAction.ExecuteLogic(_field).ToList();

            // Assert
            // Counter should return 2x physical damage
            var counterDamageAction = counterReactions.OfType<DamageAction>().FirstOrDefault();
            Assert.That(counterDamageAction, Is.Not.Null, $"Counter reactions: {string.Join(", ", counterReactions.Select(r => r.GetType().Name))}");
            Assert.That(counterDamageAction.Context.FinalDamage, Is.EqualTo(damageTaken * 2));
        }

        [Test]
        public void UseMoveAction_Counter_SpecialDamage_Fails()
        {
            // Arrange
            // Deal special damage to target
            var attackAction = new UseMoveAction(_userSlot, _targetSlot, _specialAttackMove);
            attackAction.ExecuteLogic(_field);

            // Act - Try to use Counter (should fail - Counter only works on physical)
            var counterAction = new UseMoveAction(_targetSlot, _userSlot, _counterMove);
            var counterReactions = counterAction.ExecuteLogic(_field).ToList();

            // Assert
            // Counter should fail (no damage returned)
            Assert.That(counterReactions.OfType<DamageAction>().Any(), Is.False);
        }

        [Test]
        public void UseMoveAction_MirrorCoat_Returns2xSpecialDamage()
        {
            // Arrange
            // First, deal special damage to target
            var attackAction = new UseMoveAction(_userSlot, _targetSlot, _specialAttackMove);
            var attackReactions = attackAction.ExecuteLogic(_field).ToList();
            var damageAction = attackReactions.OfType<DamageAction>().FirstOrDefault();
            Assert.That(damageAction, Is.Not.Null);
            
            int damageTaken = damageAction.Context.FinalDamage;
            Assert.That(damageTaken, Is.GreaterThan(0));

            // Execute damage action to register damage
            damageAction.ExecuteLogic(_field);

            // Act - Use Mirror Coat
            var mirrorCoatAction = new UseMoveAction(_targetSlot, _userSlot, _mirrorCoatMove);
            var mirrorCoatReactions = mirrorCoatAction.ExecuteLogic(_field).ToList();

            // Assert
            // Mirror Coat should return 2x special damage
            var mirrorCoatDamageAction = mirrorCoatReactions.OfType<DamageAction>().FirstOrDefault();
            Assert.That(mirrorCoatDamageAction, Is.Not.Null);
            Assert.That(mirrorCoatDamageAction.Context.FinalDamage, Is.EqualTo(damageTaken * 2));
        }

        [Test]
        public void UseMoveAction_Counter_Priority_Minus5()
        {
            // Arrange
            // Counter move is already created in SetUp with Priority = -5
            
            // Act
            var counterAction = new UseMoveAction(_targetSlot, _userSlot, _counterMove);
            
            // Assert
            Assert.That(counterAction.Priority, Is.EqualTo(-5));
        }
    }
}

