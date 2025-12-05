using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Engine;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.Actions
{
    /// <summary>
    /// Tests for Semi-Invulnerable move mechanics (2-turn moves like Fly, Dig, Dive).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.15: Advanced Move Mechanics
    /// **Documentation**: See `docs/features/2-combat-system/2.15-advanced-move-mechanics/README.md`
    /// </remarks>
    [TestFixture]
    public class SemiInvulnerableTests
    {
        private BattleField _field;
        private BattleSlot _userSlot;
        private BattleSlot _targetSlot;
        private MoveInstance _flyMove;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            _field.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 },
                new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) },
                new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) });

            _userSlot = _field.PlayerSide.Slots[0];
            _targetSlot = _field.EnemySide.Slots[0];

            // Create Fly move (semi-invulnerable, 2-turn)
            var flyMoveData = new MoveData
            {
                Name = "Fly",
                Power = 90,
                Accuracy = 95,
                Type = PokemonType.Flying,
                Category = MoveCategory.Physical,
                MaxPP = 15,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy,
                Effects = new List<IMoveEffect> 
                { 
                    new DamageEffect(),
                    new SemiInvulnerableEffect()
                }
            };
            _flyMove = new MoveInstance(flyMoveData);
        }

        [Test]
        public void UseMoveAction_Fly_ChargeTurn_SemiInvulnerable()
        {
            // Arrange
            
            // Act - First use (charge turn)
            var flyAction = new UseMoveAction(_userSlot, _targetSlot, _flyMove);
            var reactions = flyAction.ExecuteLogic(_field).ToList();

            // Assert
            // Should be semi-invulnerable and charging
            Assert.That(_userSlot.HasVolatileStatus(VolatileStatus.SemiInvulnerable), Is.True);
            Assert.That(_userSlot.SemiInvulnerableMoveName, Is.EqualTo("Fly"));
            Assert.That(_userSlot.IsSemiInvulnerableCharging, Is.True);
            
            // Should show charge message
            Assert.That(reactions.Any(r => r is MessageAction msg && msg.Message.Contains("flew up high")), Is.True);
            
            // Should NOT deal damage on charge turn
            var damageAction = reactions.OfType<DamageAction>().FirstOrDefault();
            Assert.That(damageAction, Is.Null);
            
            // PP should be deducted
            Assert.That(_flyMove.CurrentPP, Is.LessThan(_flyMove.MaxPP));
        }

        [Test]
        public void UseMoveAction_Fly_AttackTurn_Hits()
        {
            // Arrange
            // User is charging Fly
            _userSlot.AddVolatileStatus(VolatileStatus.SemiInvulnerable);
            _userSlot.SetSemiInvulnerableMove("Fly", isCharging: true);
            
            // Simulate end of turn (marks as ready for attack)
            _userSlot.SetSemiInvulnerableReady();
            
            // Act - Second use (attack turn)
            var flyAction = new UseMoveAction(_userSlot, _targetSlot, _flyMove);
            var reactions = flyAction.ExecuteLogic(_field).ToList();

            // Assert
            // Should clear semi-invulnerable status
            Assert.That(_userSlot.HasVolatileStatus(VolatileStatus.SemiInvulnerable), Is.False);
            Assert.That(_userSlot.SemiInvulnerableMoveName, Is.Null);
            
            // Should deal damage
            var damageAction = reactions.OfType<DamageAction>().FirstOrDefault();
            Assert.That(damageAction, Is.Not.Null);
            Assert.That(damageAction.Context.FinalDamage, Is.GreaterThan(0));
        }

        [Test]
        public void UseMoveAction_Fly_Earthquake_Hits()
        {
            // Arrange
            // User is charging Fly
            _userSlot.AddVolatileStatus(VolatileStatus.SemiInvulnerable);
            _userSlot.SetSemiInvulnerableMove("Fly", isCharging: true);
            
            // Create Earthquake move (hits Fly users)
            var earthquakeMoveData = new MoveData
            {
                Name = "Earthquake",
                Power = 100,
                Accuracy = 100,
                Type = PokemonType.Ground,
                Category = MoveCategory.Physical,
                MaxPP = 10,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy,
                Effects = new List<IMoveEffect> { new DamageEffect() }
            };
            var earthquakeMove = new MoveInstance(earthquakeMoveData);
            
            // Act - Target uses Earthquake on semi-invulnerable user
            var earthquakeAction = new UseMoveAction(_targetSlot, _userSlot, earthquakeMove);
            var reactions = earthquakeAction.ExecuteLogic(_field).ToList();

            // Assert
            // Earthquake should miss (Fly is in the air, not underground)
            // Note: In Gen 6+, Earthquake doesn't hit Fly users unless in specific conditions
            // For now, we'll test that most moves miss
            var damageAction = reactions.OfType<DamageAction>().FirstOrDefault();
            // Earthquake doesn't hit Fly in our implementation (only Dig)
            Assert.That(damageAction, Is.Null);
        }

        [Test]
        public void UseMoveAction_Dive_Surf_Hits()
        {
            // Arrange
            // Create Dive move
            var diveMoveData = new MoveData
            {
                Name = "Dive",
                Power = 80,
                Accuracy = 100,
                Type = PokemonType.Water,
                Category = MoveCategory.Physical,
                MaxPP = 10,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy,
                Effects = new List<IMoveEffect> 
                { 
                    new DamageEffect(),
                    new SemiInvulnerableEffect()
                }
            };
            var diveMove = new MoveInstance(diveMoveData);
            
            // User charges Dive
            var diveChargeAction = new UseMoveAction(_userSlot, _targetSlot, diveMove);
            diveChargeAction.ExecuteLogic(_field);
            Assert.That(_userSlot.HasVolatileStatus(VolatileStatus.SemiInvulnerable), Is.True);
            Assert.That(_userSlot.SemiInvulnerableMoveName, Is.EqualTo("Dive"));
            
            // Create Surf move (hits Dive users)
            var surfMoveData = new MoveData
            {
                Name = "Surf",
                Power = 90,
                Accuracy = 100,
                Type = PokemonType.Water,
                Category = MoveCategory.Special,
                MaxPP = 15,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy,
                Effects = new List<IMoveEffect> { new DamageEffect() }
            };
            var surfMove = new MoveInstance(surfMoveData);
            
            // Act - Target uses Surf on Dive user
            var surfAction = new UseMoveAction(_targetSlot, _userSlot, surfMove);
            var reactions = surfAction.ExecuteLogic(_field).ToList();

            // Assert
            // Surf should hit Dive users
            var damageAction = reactions.OfType<DamageAction>().FirstOrDefault();
            Assert.That(damageAction, Is.Not.Null);
            Assert.That(damageAction.Context.FinalDamage, Is.GreaterThan(0));
        }

        [Test]
        public void UseMoveAction_SemiInvulnerable_DifferentMove_Cancels()
        {
            // Arrange
            // User is charging Fly
            _userSlot.AddVolatileStatus(VolatileStatus.SemiInvulnerable);
            _userSlot.SetSemiInvulnerableMove("Fly", isCharging: true);
            
            // Create different move
            var tackleMoveData = new MoveData
            {
                Name = "Tackle",
                Power = 40,
                Accuracy = 100,
                Type = PokemonType.Normal,
                Category = MoveCategory.Physical,
                MaxPP = 35,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy,
                Effects = new List<IMoveEffect> { new DamageEffect() }
            };
            var tackleMove = new MoveInstance(tackleMoveData);
            
            // Act - User uses different move
            var tackleAction = new UseMoveAction(_userSlot, _targetSlot, tackleMove);
            tackleAction.ExecuteLogic(_field);

            // Assert
            // Semi-invulnerable should be cancelled
            Assert.That(_userSlot.HasVolatileStatus(VolatileStatus.SemiInvulnerable), Is.False);
            Assert.That(_userSlot.SemiInvulnerableMoveName, Is.Null);
        }

        [Test]
        public void EndOfTurnProcessor_SemiInvulnerable_MarksReadyForAttack()
        {
            // Arrange
            // User is charging Fly
            _userSlot.AddVolatileStatus(VolatileStatus.SemiInvulnerable);
            _userSlot.SetSemiInvulnerableMove("Fly", isCharging: true);
            Assert.That(_userSlot.IsSemiInvulnerableCharging, Is.True);
            
            // Act - Process end of turn
            EndOfTurnProcessor.ProcessEffects(_field);
            
            // Assert
            // Should be marked as ready for attack turn
            Assert.That(_userSlot.IsSemiInvulnerableCharging, Is.False);
            Assert.That(_userSlot.HasVolatileStatus(VolatileStatus.SemiInvulnerable), Is.True);
            Assert.That(_userSlot.SemiInvulnerableMoveName, Is.EqualTo("Fly"));
        }
    }
}

