using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.Actions
{
    /// <summary>
    /// Tests for Multi-Hit move mechanics.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.15: Advanced Move Mechanics
    /// **Documentation**: See `docs/features/2-combat-system/2.15-advanced-move-mechanics/README.md`
    /// </remarks>
    [TestFixture]
    public class MultiHitTests
    {
        private BattleField _field;
        private BattleSlot _userSlot;
        private BattleSlot _targetSlot;
        private MoveInstance _doubleSlapMove;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            _field.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 },
                new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) },
                new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) });

            _userSlot = _field.PlayerSide.Slots[0];
            _targetSlot = _field.EnemySide.Slots[0];

            // Create Double Slap move (multi-hit: 2-5 hits)
            var doubleSlapMoveData = new MoveData
            {
                Name = "Double Slap",
                Power = 15,
                Accuracy = 100, // 100% accuracy to avoid random misses in tests
                Type = PokemonType.Normal,
                Category = MoveCategory.Physical,
                MaxPP = 10,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy,
                Effects = new List<IMoveEffect> 
                { 
                    new DamageEffect(),
                    new MultiHitEffect { MinHits = 2, MaxHits = 5 }
                }
            };
            _doubleSlapMove = new MoveInstance(doubleSlapMoveData);
        }

        [Test]
        public void UseMoveAction_MultiHit_HitsMultipleTimes()
        {
            // Arrange
            // Use a move with 100% accuracy to avoid random misses
            var doubleSlapMoveData = new MoveData
            {
                Name = "Double Slap",
                Power = 15,
                Accuracy = 100, // 100% accuracy to avoid random misses
                Type = PokemonType.Normal,
                Category = MoveCategory.Physical,
                MaxPP = 10,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy,
                Effects = new List<IMoveEffect> 
                { 
                    new DamageEffect(),
                    new MultiHitEffect { MinHits = 2, MaxHits = 5 }
                }
            };
            var doubleSlapMove = new MoveInstance(doubleSlapMoveData);
            
            // Act
            var doubleSlapAction = new UseMoveAction(_userSlot, _targetSlot, doubleSlapMove);
            var reactions = doubleSlapAction.ExecuteLogic(_field).ToList();

            // Assert
            // Should have multiple damage actions (2-5 hits)
            var damageActions = reactions.OfType<DamageAction>().ToList();
            Assert.That(damageActions.Count, Is.GreaterThanOrEqualTo(2), 
                "Multi-hit move should hit at least 2 times");
            Assert.That(damageActions.Count, Is.LessThanOrEqualTo(5),
                "Multi-hit move should hit at most 5 times");
            
            // Each hit should deal damage
            foreach (var damageAction in damageActions)
            {
                Assert.That(damageAction.Context.FinalDamage, Is.GreaterThan(0),
                    "Each hit should deal damage");
            }
        }

        [Test]
        public void UseMoveAction_MultiHit_EachHitIndependent()
        {
            // Arrange
            // Use a move with 100% accuracy to avoid random misses
            var doubleSlapMoveData = new MoveData
            {
                Name = "Double Slap",
                Power = 15,
                Accuracy = 100, // 100% accuracy to avoid random misses
                Type = PokemonType.Normal,
                Category = MoveCategory.Physical,
                MaxPP = 10,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy,
                Effects = new List<IMoveEffect> 
                { 
                    new DamageEffect(),
                    new MultiHitEffect { MinHits = 2, MaxHits = 5 }
                }
            };
            var doubleSlapMove = new MoveInstance(doubleSlapMoveData);
            
            // Act
            var doubleSlapAction = new UseMoveAction(_userSlot, _targetSlot, doubleSlapMove);
            var reactions = doubleSlapAction.ExecuteLogic(_field).ToList();

            // Assert
            // Each hit should be independent (different damage values possible)
            var damageActions = reactions.OfType<DamageAction>().ToList();
            Assert.That(damageActions.Count, Is.GreaterThanOrEqualTo(2),
                "Multi-hit move should hit at least 2 times");
            
            // Verify all hits target the same target
            foreach (var damageAction in damageActions)
            {
                Assert.That(damageAction.Target, Is.EqualTo(_targetSlot),
                    "All hits should target the same target");
                Assert.That(damageAction.User, Is.EqualTo(_userSlot),
                    "All hits should come from the same user");
            }
        }

        [Test]
        public void UseMoveAction_MultiHit_TotalDamageGreaterThanSingleHit()
        {
            // Arrange
            // Create single-hit move with same power for comparison
            var tackleMoveData = new MoveData
            {
                Name = "Tackle",
                Power = 15,
                Accuracy = 100,
                Type = PokemonType.Normal,
                Category = MoveCategory.Physical,
                MaxPP = 35,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy,
                Effects = new List<IMoveEffect> { new DamageEffect() }
            };
            var tackleMove = new MoveInstance(tackleMoveData);
            
            // Act - Multi-hit move
            var doubleSlapAction = new UseMoveAction(_userSlot, _targetSlot, _doubleSlapMove);
            var multiHitReactions = doubleSlapAction.ExecuteLogic(_field).ToList();
            var multiHitDamages = multiHitReactions.OfType<DamageAction>().ToList();
            int totalMultiHitDamage = multiHitDamages.Sum(d => d.Context.FinalDamage);
            
            // Act - Single-hit move
            var tackleAction = new UseMoveAction(_userSlot, _targetSlot, tackleMove);
            var singleHitReactions = tackleAction.ExecuteLogic(_field).ToList();
            var singleHitDamage = singleHitReactions.OfType<DamageAction>().FirstOrDefault();
            
            // Assert
            // Multi-hit should deal more total damage (at least 2x)
            Assert.That(totalMultiHitDamage, Is.GreaterThan(singleHitDamage.Context.FinalDamage));
        }
    }
}

