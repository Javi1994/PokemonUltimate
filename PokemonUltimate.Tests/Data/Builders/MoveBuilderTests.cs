using NUnit.Framework;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;
using System.Linq;
using PokemonUltimate.Content.Builders;

namespace PokemonUltimate.Tests.Data.Builders
{
    [TestFixture]
    public class MoveBuilderTests
    {
        #region Basic Properties

        [Test]
        public void Define_Should_Set_Name()
        {
            var move = Move.Define("Thunderbolt").Build();

            Assert.That(move.Name, Is.EqualTo("Thunderbolt"));
        }

        [Test]
        public void Type_Should_Set_Move_Type()
        {
            var move = Move.Define("Thunderbolt")
                .Type(PokemonType.Electric)
                .Build();

            Assert.That(move.Type, Is.EqualTo(PokemonType.Electric));
        }

        [Test]
        public void Description_Should_Set_Description()
        {
            var move = Move.Define("Thunderbolt")
                .Description("A strong electric blast.")
                .Build();

            Assert.That(move.Description, Is.EqualTo("A strong electric blast."));
        }

        #endregion

        #region Category Methods

        [Test]
        public void Physical_Should_Set_Category_And_Stats()
        {
            var move = Move.Define("Earthquake")
                .Type(PokemonType.Ground)
                .Physical(100, 100, 10)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Physical));
                Assert.That(move.Power, Is.EqualTo(100));
                Assert.That(move.Accuracy, Is.EqualTo(100));
                Assert.That(move.MaxPP, Is.EqualTo(10));
            });
        }

        [Test]
        public void Special_Should_Set_Category_And_Stats()
        {
            var move = Move.Define("Flamethrower")
                .Type(PokemonType.Fire)
                .Special(90, 100, 15)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Special));
                Assert.That(move.Power, Is.EqualTo(90));
                Assert.That(move.Accuracy, Is.EqualTo(100));
                Assert.That(move.MaxPP, Is.EqualTo(15));
            });
        }

        [Test]
        public void Status_Should_Set_Category_With_Zero_Power()
        {
            var move = Move.Define("Thunder Wave")
                .Type(PokemonType.Electric)
                .Status(90, 20)
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Status));
                Assert.That(move.Power, Is.EqualTo(0));
                Assert.That(move.Accuracy, Is.EqualTo(90));
                Assert.That(move.MaxPP, Is.EqualTo(20));
            });
        }

        #endregion

        #region Priority and Target

        [Test]
        public void Priority_Should_Set_Move_Priority()
        {
            var move = Move.Define("Quick Attack")
                .Type(PokemonType.Normal)
                .Physical(40, 100, 30)
                .Priority(1)
                .Build();

            Assert.That(move.Priority, Is.EqualTo(1));
        }

        [Test]
        public void Target_Should_Set_TargetScope()
        {
            var move = Move.Define("Earthquake")
                .Target(TargetScope.AllAdjacent)
                .Build();

            Assert.That(move.TargetScope, Is.EqualTo(TargetScope.AllAdjacent));
        }

        [Test]
        public void TargetSelf_Should_Set_Self_Scope()
        {
            var move = Move.Define("Swords Dance")
                .TargetSelf()
                .Build();

            Assert.That(move.TargetScope, Is.EqualTo(TargetScope.Self));
        }

        [Test]
        public void TargetAllEnemies_Should_Set_AllEnemies_Scope()
        {
            var move = Move.Define("Surf")
                .TargetAllEnemies()
                .Build();

            Assert.That(move.TargetScope, Is.EqualTo(TargetScope.AllEnemies));
        }

        [Test]
        public void Default_Target_Is_SingleEnemy()
        {
            var move = Move.Define("Tackle").Build();

            Assert.That(move.TargetScope, Is.EqualTo(TargetScope.SingleEnemy));
        }

        [Test]
        public void TargetAllAdjacent_Should_Set_AllAdjacent_Scope()
        {
            var move = Move.Define("Earthquake")
                .TargetAllAdjacent()
                .Build();

            Assert.That(move.TargetScope, Is.EqualTo(TargetScope.AllAdjacent));
        }

        [Test]
        public void TargetAllAdjacentEnemies_Should_Set_AllAdjacentEnemies_Scope()
        {
            var move = Move.Define("Heat Wave")
                .TargetAllAdjacentEnemies()
                .Build();

            Assert.That(move.TargetScope, Is.EqualTo(TargetScope.AllAdjacentEnemies));
        }

        #endregion

        #region Effects Integration

        [Test]
        public void WithEffects_Should_Add_Effects()
        {
            var move = Move.Define("Flamethrower")
                .Type(PokemonType.Fire)
                .Special(90, 100, 15)
                .WithEffects(e => e
                    .Damage()
                    .MayBurn(10))
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(move.Effects, Has.Count.EqualTo(2));
                Assert.That(move.HasEffect<DamageEffect>(), Is.True);
                Assert.That(move.HasEffect<StatusEffect>(), Is.True);
            });
        }

        [Test]
        public void Move_Without_Effects_Has_Empty_List()
        {
            var move = Move.Define("Splash").Build();

            Assert.That(move.Effects, Is.Empty);
        }

        #endregion

        #region Full Move Examples

        [Test]
        public void Full_Physical_Move_Definition()
        {
            var move = Move.Define("Earthquake")
                .Type(PokemonType.Ground)
                .Physical(100, 100, 10)
                .Target(TargetScope.AllAdjacent)
                .WithEffects(e => e.Damage())
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Earthquake"));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Ground));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Physical));
                Assert.That(move.Power, Is.EqualTo(100));
                Assert.That(move.TargetScope, Is.EqualTo(TargetScope.AllAdjacent));
                Assert.That(move.HasEffect<DamageEffect>(), Is.True);
            });
        }

        [Test]
        public void Full_Special_Move_With_Secondary_Effect()
        {
            var move = Move.Define("Ice Beam")
                .Type(PokemonType.Ice)
                .Special(90, 100, 10)
                .WithEffects(e => e
                    .Damage()
                    .MayFreeze(10))
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Ice Beam"));
                Assert.That(move.Effects, Has.Count.EqualTo(2));
                
                var freezeEffect = move.GetEffect<StatusEffect>();
                Assert.That(freezeEffect.Status, Is.EqualTo(PersistentStatus.Freeze));
                Assert.That(freezeEffect.ChancePercent, Is.EqualTo(10));
            });
        }

        [Test]
        public void Full_Status_Move_Definition()
        {
            var move = Move.Define("Swords Dance")
                .Type(PokemonType.Normal)
                .Status(0, 20) // Never misses
                .TargetSelf()
                .WithEffects(e => e.RaiseAttack(2))
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Status));
                Assert.That(move.TargetScope, Is.EqualTo(TargetScope.Self));
                
                var statEffect = move.GetEffect<StatChangeEffect>();
                Assert.That(statEffect.TargetStat, Is.EqualTo(Stat.Attack));
                Assert.That(statEffect.Stages, Is.EqualTo(2));
                Assert.That(statEffect.TargetSelf, Is.True);
            });
        }

        [Test]
        public void Priority_Move_Example()
        {
            var move = Move.Define("Quick Attack")
                .Type(PokemonType.Normal)
                .Physical(40, 100, 30)
                .Priority(1)
                .WithEffects(e => e.Damage())
                .Build();

            Assert.Multiple(() =>
            {
                Assert.That(move.Priority, Is.EqualTo(1));
                Assert.That(move.Power, Is.EqualTo(40));
            });
        }

        #endregion
    }
}

