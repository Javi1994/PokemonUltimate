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

namespace PokemonUltimate.Tests.Systems.Combat.Actions.Integration
{
    /// <summary>
    /// Integration tests for Advanced Move Mechanics feature.
    /// Tests combinations of effects and interactions between different mechanics.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.15: Advanced Move Mechanics
    /// **Documentation**: See `docs/features/2-combat-system/2.15-advanced-move-mechanics/README.md`
    /// </remarks>
    [TestFixture]
    public class AdvancedMoveMechanicsIntegrationTests
    {
        private BattleField _field;
        private BattleSlot _userSlot;
        private BattleSlot _targetSlot;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            _field.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 },
                new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) },
                new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) });

            _userSlot = _field.PlayerSide.Slots[0];
            _targetSlot = _field.EnemySide.Slots[0];
        }

        [Test]
        public void Protect_ThenCounter_BlocksAttack_NoCounter()
        {
            // Arrange
            // Create Protect move
            var protectMoveData = new MoveData
            {
                Name = "Protect",
                Power = 0,
                Accuracy = 100,
                Type = PokemonType.Normal,
                Category = MoveCategory.Status,
                MaxPP = 10,
                Priority = 4,
                TargetScope = TargetScope.Self,
                Effects = new List<IMoveEffect> { new ProtectEffect() }
            };
            var protectMove = new MoveInstance(protectMoveData);

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
                Effects = new List<IMoveEffect> 
                { 
                    new CounterEffect { IsPhysicalCounter = true }
                }
            };
            var counterMove = new MoveInstance(counterMoveData);

            // Create attack move
            var attackMoveData = new MoveData
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
            var attackMove = new MoveInstance(attackMoveData);

            // Act
            // Target uses Protect
            var protectAction = new UseMoveAction(_targetSlot, _targetSlot, protectMove);
            protectAction.ExecuteLogic(_field);
            Assert.That(_targetSlot.HasVolatileStatus(VolatileStatus.Protected), Is.True);

            // User attacks protected target
            var attackAction = new UseMoveAction(_userSlot, _targetSlot, attackMove);
            var attackReactions = attackAction.ExecuteLogic(_field).ToList();

            // Target uses Counter
            var counterAction = new UseMoveAction(_targetSlot, _userSlot, counterMove);
            var counterReactions = counterAction.ExecuteLogic(_field).ToList();

            // Assert
            // Attack should be blocked
            Assert.That(attackReactions.Any(r => r is MessageAction msg && msg.Message.Contains("protected")), Is.True);
            Assert.That(attackReactions.Any(r => r is DamageAction), Is.False);

            // Counter should fail (no damage was taken because attack was blocked)
            var counterDamage = counterReactions.OfType<DamageAction>().FirstOrDefault();
            Assert.That(counterDamage, Is.Null);
        }

        [Test]
        public void Pursuit_OnSwitchingTarget_DoublesPower()
        {
            // Arrange
            // Create Pursuit move
            var pursuitMoveData = new MoveData
            {
                Name = "Pursuit",
                Power = 40,
                Accuracy = 100,
                Type = PokemonType.Dark,
                Category = MoveCategory.Physical,
                MaxPP = 20,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy,
                Effects = new List<IMoveEffect> 
                { 
                    new DamageEffect(),
                    new PursuitEffect()
                }
            };
            var pursuitMove = new MoveInstance(pursuitMoveData);

            // Create normal move for comparison
            var normalMoveData = new MoveData
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
            var normalMove = new MoveInstance(normalMoveData);

            // Act
            // Mark target as switching
            _targetSlot.AddVolatileStatus(VolatileStatus.SwitchingOut);

            // Use Pursuit on switching target
            var pursuitAction = new UseMoveAction(_userSlot, _targetSlot, pursuitMove);
            var pursuitReactions = pursuitAction.ExecuteLogic(_field).ToList();
            var pursuitDamage = pursuitReactions.OfType<DamageAction>().FirstOrDefault();

            // Use normal move for comparison
            _targetSlot.RemoveVolatileStatus(VolatileStatus.SwitchingOut);
            var normalAction = new UseMoveAction(_userSlot, _targetSlot, normalMove);
            var normalReactions = normalAction.ExecuteLogic(_field).ToList();
            var normalDamage = normalReactions.OfType<DamageAction>().FirstOrDefault();

            // Assert
            Assert.That(pursuitDamage, Is.Not.Null);
            Assert.That(normalDamage, Is.Not.Null);
            // Pursuit should deal significantly more damage (2x power)
            Assert.That(pursuitDamage.Context.FinalDamage, Is.GreaterThan(normalDamage.Context.FinalDamage));
        }

        [Test]
        public void FocusPunch_HitBeforeMoving_Fails()
        {
            // Arrange
            // Create Focus Punch move
            var focusPunchMoveData = new MoveData
            {
                Name = "Focus Punch",
                Power = 150,
                Accuracy = 100,
                Type = PokemonType.Fighting,
                Category = MoveCategory.Physical,
                MaxPP = 20,
                Priority = -3,
                TargetScope = TargetScope.SingleEnemy,
                Effects = new List<IMoveEffect> 
                { 
                    new DamageEffect(),
                    new FocusPunchEffect()
                }
            };
            var focusPunchMove = new MoveInstance(focusPunchMoveData);

            // Create attack move
            var attackMoveData = new MoveData
            {
                Name = "Quick Attack",
                Power = 40,
                Accuracy = 100,
                Type = PokemonType.Normal,
                Category = MoveCategory.Physical,
                MaxPP = 30,
                Priority = 1,
                TargetScope = TargetScope.SingleEnemy,
                Effects = new List<IMoveEffect> { new DamageEffect() }
            };
            var attackMove = new MoveInstance(attackMoveData);

            // Act
            // User starts focusing (would happen at start of turn)
            _userSlot.AddVolatileStatus(VolatileStatus.Focusing);

            // Target hits user before Focus Punch executes (higher priority)
            var attackAction = new UseMoveAction(_targetSlot, _userSlot, attackMove);
            var attackReactions = attackAction.ExecuteLogic(_field).ToList();
            var damageAction = attackReactions.OfType<DamageAction>().FirstOrDefault();
            if (damageAction != null)
            {
                damageAction.ExecuteLogic(_field);
            }

            // User tries to use Focus Punch (should fail)
            var focusPunchAction = new UseMoveAction(_userSlot, _targetSlot, focusPunchMove);
            var focusPunchReactions = focusPunchAction.ExecuteLogic(_field).ToList();

            // Assert
            // Focus Punch should fail
            var focusPunchDamage = focusPunchReactions.OfType<DamageAction>().FirstOrDefault();
            Assert.That(focusPunchDamage, Is.Null);
            Assert.That(focusPunchReactions.Any(r => r is MessageAction msg && msg.Message.Contains("lost its focus")), Is.True);
        }

        [Test]
        public void MultiHit_WithCounter_EachHitCanBeCountered()
        {
            // Arrange
            // Create multi-hit move
            var multiHitMoveData = new MoveData
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
                    new MultiHitEffect { MinHits = 2, MaxHits = 2 }
                }
            };
            var multiHitMove = new MoveInstance(multiHitMoveData);

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
                Effects = new List<IMoveEffect> 
                { 
                    new CounterEffect { IsPhysicalCounter = true }
                }
            };
            var counterMove = new MoveInstance(counterMoveData);

            // Act
            // User attacks with multi-hit move
            var multiHitAction = new UseMoveAction(_userSlot, _targetSlot, multiHitMove);
            var multiHitReactions = multiHitAction.ExecuteLogic(_field).ToList();
            var multiHitDamages = multiHitReactions.OfType<DamageAction>().ToList();

            // Execute damage actions to record damage
            foreach (var damageAction in multiHitDamages)
            {
                damageAction.ExecuteLogic(_field);
            }

            // Target uses Counter
            var counterAction = new UseMoveAction(_targetSlot, _userSlot, counterMove);
            var counterReactions = counterAction.ExecuteLogic(_field).ToList();

            // Assert
            // Multi-hit should hit multiple times
            Assert.That(multiHitDamages.Count, Is.EqualTo(2));

            // Counter should return 2x the total physical damage taken
            var counterDamage = counterReactions.OfType<DamageAction>().FirstOrDefault();
            Assert.That(counterDamage, Is.Not.Null);
            int totalDamageTaken = multiHitDamages.Sum(d => d.Context.FinalDamage);
            Assert.That(counterDamage.Context.BaseDamage, Is.EqualTo(totalDamageTaken * 2));
        }

        [Test]
        public void MultiTurn_ThenDifferentMove_CancelsCharge()
        {
            // Arrange
            // Create Solar Beam (multi-turn move)
            var solarBeamMoveData = new MoveData
            {
                Name = "Solar Beam",
                Power = 120,
                Accuracy = 100,
                Type = PokemonType.Grass,
                Category = MoveCategory.Special,
                MaxPP = 10,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy,
                Effects = new List<IMoveEffect> 
                { 
                    new DamageEffect(),
                    new MultiTurnEffect { ChargeMessage = "is charging sunlight!" }
                }
            };
            var solarBeamMove = new MoveInstance(solarBeamMoveData);

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

            // Act
            // User starts charging Solar Beam
            var chargeAction = new UseMoveAction(_userSlot, _targetSlot, solarBeamMove);
            chargeAction.ExecuteLogic(_field);
            Assert.That(_userSlot.HasVolatileStatus(VolatileStatus.Charging), Is.True);
            Assert.That(_userSlot.ChargingMoveName, Is.EqualTo("Solar Beam"));

            // User uses different move (should cancel charge)
            var tackleAction = new UseMoveAction(_userSlot, _targetSlot, tackleMove);
            tackleAction.ExecuteLogic(_field);

            // Assert
            // Charging should be cancelled
            Assert.That(_userSlot.HasVolatileStatus(VolatileStatus.Charging), Is.False);
            Assert.That(_userSlot.ChargingMoveName, Is.Null);
        }

        [Test]
        public void SemiInvulnerable_WithEarthquake_Hits()
        {
            // Arrange
            // Create Dig move (semi-invulnerable)
            var digMoveData = new MoveData
            {
                Name = "Dig",
                Power = 80,
                Accuracy = 100,
                Type = PokemonType.Ground,
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
            var digMove = new MoveInstance(digMoveData);

            // Create Earthquake move (hits Dig users)
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

            // Act
            // Target uses Dig (becomes semi-invulnerable)
            var digAction = new UseMoveAction(_targetSlot, _userSlot, digMove);
            digAction.ExecuteLogic(_field);
            Assert.That(_targetSlot.HasVolatileStatus(VolatileStatus.SemiInvulnerable), Is.True);
            Assert.That(_targetSlot.SemiInvulnerableMoveName, Is.EqualTo("Dig"));

            // User uses Earthquake (should hit)
            var earthquakeAction = new UseMoveAction(_userSlot, _targetSlot, earthquakeMove);
            var earthquakeReactions = earthquakeAction.ExecuteLogic(_field).ToList();

            // Assert
            // Earthquake should hit semi-invulnerable target
            var earthquakeDamage = earthquakeReactions.OfType<DamageAction>().FirstOrDefault();
            Assert.That(earthquakeDamage, Is.Not.Null);
            Assert.That(earthquakeDamage.Context.FinalDamage, Is.GreaterThan(0));
        }
    }
}

