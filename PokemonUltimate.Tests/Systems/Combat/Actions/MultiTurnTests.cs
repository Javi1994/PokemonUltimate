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
    /// Tests for Multi-Turn move mechanics (charge then attack).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.15: Advanced Move Mechanics
    /// **Documentation**: See `docs/features/2-combat-system/2.15-advanced-move-mechanics/README.md`
    /// </remarks>
    [TestFixture]
    public class MultiTurnTests
    {
        private BattleField _field;
        private BattleSlot _userSlot;
        private BattleSlot _targetSlot;
        private MoveInstance _solarBeamMove;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            _field.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 },
                new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) },
                new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) });

            _userSlot = _field.PlayerSide.Slots[0];
            _targetSlot = _field.EnemySide.Slots[0];

            // Create Solar Beam move (2-turn move)
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
            _solarBeamMove = new MoveInstance(solarBeamMoveData);
        }

        [Test]
        public void UseMoveAction_MultiTurn_FirstTurn_Charges()
        {
            // Arrange
            // User is not charging yet
            
            // Act - First use (charge turn)
            var solarBeamAction = new UseMoveAction(_userSlot, _targetSlot, _solarBeamMove);
            var reactions = solarBeamAction.ExecuteLogic(_field).ToList();

            // Assert
            // Should mark user as charging
            Assert.That(_userSlot.HasVolatileStatus(VolatileStatus.Charging), Is.True);
            Assert.That(_userSlot.ChargingMoveName, Is.EqualTo("Solar Beam"));
            
            // Should show charge message
            Assert.That(reactions.Any(r => r is MessageAction msg && msg.Message.Contains("charging")), Is.True);
            
            // Should NOT deal damage on charge turn
            var damageAction = reactions.OfType<DamageAction>().FirstOrDefault();
            Assert.That(damageAction, Is.Null);
            
            // PP should be deducted
            Assert.That(_solarBeamMove.CurrentPP, Is.LessThan(_solarBeamMove.MaxPP));
        }

        [Test]
        public void UseMoveAction_MultiTurn_SecondTurn_Attacks()
        {
            // Arrange
            // User is already charging
            _userSlot.AddVolatileStatus(VolatileStatus.Charging);
            _userSlot.SetChargingMove("Solar Beam");
            
            // Act - Second use (attack turn)
            var solarBeamAction = new UseMoveAction(_userSlot, _targetSlot, _solarBeamMove);
            var reactions = solarBeamAction.ExecuteLogic(_field).ToList();

            // Assert
            // Should clear charging status
            Assert.That(_userSlot.HasVolatileStatus(VolatileStatus.Charging), Is.False);
            Assert.That(_userSlot.ChargingMoveName, Is.Null);
            
            // Should deal damage
            var damageAction = reactions.OfType<DamageAction>().FirstOrDefault();
            Assert.That(damageAction, Is.Not.Null);
            Assert.That(damageAction.Context.FinalDamage, Is.GreaterThan(0));
            
            // Should show attack message
            Assert.That(reactions.Any(r => r is MessageAction msg && msg.Message.Contains("Solar Beam")), Is.True);
        }

        [Test]
        public void UseMoveAction_MultiTurn_DifferentMove_CancelsCharge()
        {
            // Arrange
            // User is charging Solar Beam
            _userSlot.AddVolatileStatus(VolatileStatus.Charging);
            _userSlot.SetChargingMove("Solar Beam");
            
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
            
            // Act - Use different move
            var tackleAction = new UseMoveAction(_userSlot, _targetSlot, tackleMove);
            tackleAction.ExecuteLogic(_field);

            // Assert
            // Charging should be cancelled
            Assert.That(_userSlot.HasVolatileStatus(VolatileStatus.Charging), Is.False);
            Assert.That(_userSlot.ChargingMoveName, Is.Null);
        }
    }
}

