using NUnit.Framework;
using PokemonUltimate.Core.Models;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Effects;
using System.Collections.Generic;

namespace PokemonUltimate.Tests.Models
{
    /// <summary>
    /// Tests for MoveData model: property values, defaults, effects helpers, and edge cases
    /// </summary>
    [TestFixture]
    public class MoveDataTests
    {
        #region Property Tests

        [Test]
        public void Test_MoveData_Properties_Are_Set_Correctly()
        {
            var move = new MoveData
            {
                Name = "Thunderbolt",
                Description = "A strong electric blast.",
                Type = PokemonType.Electric,
                Category = MoveCategory.Special,
                Power = 90,
                Accuracy = 100,
                MaxPP = 15,
                Priority = 0,
                TargetScope = TargetScope.SingleEnemy
            };

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo("Thunderbolt"));
                Assert.That(move.Description, Is.EqualTo("A strong electric blast."));
                Assert.That(move.Type, Is.EqualTo(PokemonType.Electric));
                Assert.That(move.Category, Is.EqualTo(MoveCategory.Special));
                Assert.That(move.Power, Is.EqualTo(90));
                Assert.That(move.Accuracy, Is.EqualTo(100));
                Assert.That(move.MaxPP, Is.EqualTo(15));
                Assert.That(move.Priority, Is.EqualTo(0));
                Assert.That(move.TargetScope, Is.EqualTo(TargetScope.SingleEnemy));
            });
        }

        [Test]
        public void Test_Id_Returns_Name()
        {
            var move = new MoveData { Name = "Ice Beam" };

            Assert.That(move.Id, Is.EqualTo("Ice Beam"));
        }

        [Test]
        public void Test_Default_Values()
        {
            var move = new MoveData();

            Assert.Multiple(() =>
            {
                Assert.That(move.Name, Is.EqualTo(string.Empty));
                Assert.That(move.Description, Is.EqualTo(string.Empty));
                Assert.That(move.Power, Is.EqualTo(0));
                Assert.That(move.Accuracy, Is.EqualTo(0));
                Assert.That(move.MaxPP, Is.EqualTo(0));
                Assert.That(move.Priority, Is.EqualTo(0));
            });
        }

        #endregion

        #region Status Move Tests

        [Test]
        public void Test_Status_Move_Has_Zero_Power()
        {
            var thunderWave = new MoveData
            {
                Name = "Thunder Wave",
                Type = PokemonType.Electric,
                Category = MoveCategory.Status,
                Power = 0,
                Accuracy = 90
            };

            Assert.Multiple(() =>
            {
                Assert.That(thunderWave.Category, Is.EqualTo(MoveCategory.Status));
                Assert.That(thunderWave.Power, Is.EqualTo(0));
            });
        }

        #endregion

        #region Priority Move Tests

        [Test]
        public void Test_Priority_Move_Has_Positive_Priority()
        {
            var quickAttack = new MoveData
            {
                Name = "Quick Attack",
                Type = PokemonType.Normal,
                Category = MoveCategory.Physical,
                Power = 40,
                Accuracy = 100,
                Priority = 1
            };

            Assert.That(quickAttack.Priority, Is.EqualTo(1));
        }

        [Test]
        public void Test_Negative_Priority_Move()
        {
            var roar = new MoveData
            {
                Name = "Roar",
                Type = PokemonType.Normal,
                Category = MoveCategory.Status,
                Power = 0,
                Priority = -6
            };

            Assert.That(roar.Priority, Is.EqualTo(-6));
        }

        #endregion

        #region TargetScope Tests

        [Test]
        public void Test_Self_Targeting_Move()
        {
            var swordsDance = new MoveData
            {
                Name = "Swords Dance",
                Type = PokemonType.Normal,
                Category = MoveCategory.Status,
                TargetScope = TargetScope.Self
            };

            Assert.That(swordsDance.TargetScope, Is.EqualTo(TargetScope.Self));
        }

        [Test]
        public void Test_AllEnemies_Move()
        {
            var earthquake = new MoveData
            {
                Name = "Earthquake",
                Type = PokemonType.Ground,
                Category = MoveCategory.Physical,
                Power = 100,
                TargetScope = TargetScope.AllEnemies
            };

            Assert.That(earthquake.TargetScope, Is.EqualTo(TargetScope.AllEnemies));
        }

        [Test]
        public void Test_Field_Effect_Move()
        {
            var stealthRock = new MoveData
            {
                Name = "Stealth Rock",
                Type = PokemonType.Rock,
                Category = MoveCategory.Status,
                TargetScope = TargetScope.Field
            };

            Assert.That(stealthRock.TargetScope, Is.EqualTo(TargetScope.Field));
        }

        #endregion

        #region Effect Helper Tests

        [Test]
        public void Test_HasEffect_Returns_True_When_Effect_Exists()
        {
            var move = new MoveData
            {
                Name = "Flamethrower",
                Effects = new List<IMoveEffect>
                {
                    new DamageEffect(),
                    new StatusEffect(PersistentStatus.Burn, 10)
                }
            };

            Assert.Multiple(() =>
            {
                Assert.That(move.HasEffect<DamageEffect>(), Is.True);
                Assert.That(move.HasEffect<StatusEffect>(), Is.True);
            });
        }

        [Test]
        public void Test_HasEffect_Returns_False_When_Effect_Not_Exists()
        {
            var move = new MoveData
            {
                Name = "Tackle",
                Effects = new List<IMoveEffect> { new DamageEffect() }
            };

            Assert.Multiple(() =>
            {
                Assert.That(move.HasEffect<StatusEffect>(), Is.False);
                Assert.That(move.HasEffect<HealEffect>(), Is.False);
                Assert.That(move.HasEffect<RecoilEffect>(), Is.False);
            });
        }

        [Test]
        public void Test_HasEffect_Returns_False_When_No_Effects()
        {
            var move = new MoveData { Name = "Empty Move" };

            Assert.That(move.HasEffect<DamageEffect>(), Is.False);
        }

        [Test]
        public void Test_GetEffect_Returns_Effect_When_Exists()
        {
            var damageEffect = new DamageEffect { CritStages = 2 };
            var move = new MoveData
            {
                Name = "Slash",
                Effects = new List<IMoveEffect> { damageEffect }
            };

            var result = move.GetEffect<DamageEffect>();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.EqualTo(damageEffect));
                Assert.That(result.CritStages, Is.EqualTo(2));
            });
        }

        [Test]
        public void Test_GetEffect_Returns_Null_When_Not_Exists()
        {
            var move = new MoveData
            {
                Name = "Tackle",
                Effects = new List<IMoveEffect> { new DamageEffect() }
            };

            Assert.That(move.GetEffect<StatusEffect>(), Is.Null);
        }

        [Test]
        public void Test_GetEffect_Returns_First_Match()
        {
            var effect1 = new StatChangeEffect(Stat.Attack, 1);
            var effect2 = new StatChangeEffect(Stat.Defense, -1);
            var move = new MoveData
            {
                Name = "Growth",
                Effects = new List<IMoveEffect> { effect1, effect2 }
            };

            var result = move.GetEffect<StatChangeEffect>();

            Assert.That(result, Is.EqualTo(effect1));
        }

        #endregion

        #region All Pokemon Types Test

        [Test]
        [TestCase(PokemonType.Normal)]
        [TestCase(PokemonType.Fire)]
        [TestCase(PokemonType.Water)]
        [TestCase(PokemonType.Grass)]
        [TestCase(PokemonType.Electric)]
        [TestCase(PokemonType.Ice)]
        [TestCase(PokemonType.Fighting)]
        [TestCase(PokemonType.Poison)]
        [TestCase(PokemonType.Ground)]
        [TestCase(PokemonType.Flying)]
        [TestCase(PokemonType.Psychic)]
        [TestCase(PokemonType.Bug)]
        [TestCase(PokemonType.Rock)]
        [TestCase(PokemonType.Ghost)]
        [TestCase(PokemonType.Dragon)]
        [TestCase(PokemonType.Dark)]
        [TestCase(PokemonType.Steel)]
        [TestCase(PokemonType.Fairy)]
        public void Test_Move_Can_Have_Any_Pokemon_Type(PokemonType type)
        {
            var move = new MoveData { Name = $"Test_{type}", Type = type };

            Assert.That(move.Type, Is.EqualTo(type));
        }

        #endregion
    }
}

