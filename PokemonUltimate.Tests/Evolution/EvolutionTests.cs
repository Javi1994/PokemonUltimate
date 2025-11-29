using System.Collections.Generic;
using NUnit.Framework;
using PokemonUltimate.Core.Data;
using PokemonUltimate.Core.Evolution;
using PokemonUltimate.Core.Evolution.Conditions;

namespace PokemonUltimate.Tests.Evolution
{
    [TestFixture]
    public class EvolutionTests
    {
        private PokemonSpeciesData _target;

        [SetUp]
        public void Setup()
        {
            _target = new PokemonSpeciesData { Name = "Charmeleon", PokedexNumber = 5 };
        }

        [Test]
        public void Evolution_Should_Store_Target()
        {
            var evolution = new Core.Evolution.Evolution
            {
                Target = _target
            };

            Assert.That(evolution.Target, Is.EqualTo(_target));
        }

        [Test]
        public void Evolution_Should_Have_Empty_Conditions_By_Default()
        {
            var evolution = new Core.Evolution.Evolution();

            Assert.That(evolution.Conditions, Is.Empty);
        }

        [Test]
        public void Evolution_HasCondition_Should_Return_True_When_Condition_Exists()
        {
            var evolution = new Core.Evolution.Evolution
            {
                Target = _target,
                Conditions = new List<IEvolutionCondition>
                {
                    new LevelCondition(16),
                    new FriendshipCondition()
                }
            };

            Assert.Multiple(() =>
            {
                Assert.That(evolution.HasCondition<LevelCondition>(), Is.True);
                Assert.That(evolution.HasCondition<FriendshipCondition>(), Is.True);
                Assert.That(evolution.HasCondition<TradeCondition>(), Is.False);
            });
        }

        [Test]
        public void Evolution_GetCondition_Should_Return_Condition_When_Exists()
        {
            var levelCondition = new LevelCondition(36);
            var evolution = new Core.Evolution.Evolution
            {
                Target = _target,
                Conditions = new List<IEvolutionCondition> { levelCondition }
            };

            var result = evolution.GetCondition<LevelCondition>();

            Assert.That(result, Is.EqualTo(levelCondition));
            Assert.That(result.MinLevel, Is.EqualTo(36));
        }

        [Test]
        public void Evolution_GetCondition_Should_Return_Null_When_Not_Exists()
        {
            var evolution = new Core.Evolution.Evolution
            {
                Target = _target,
                Conditions = new List<IEvolutionCondition> { new LevelCondition(16) }
            };

            var result = evolution.GetCondition<TradeCondition>();

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Evolution_Description_Should_Combine_All_Conditions()
        {
            var evolution = new Core.Evolution.Evolution
            {
                Target = _target,
                Conditions = new List<IEvolutionCondition>
                {
                    new LevelCondition(20),
                    new FriendshipCondition()
                }
            };

            Assert.That(evolution.Description, Does.Contain("Reach level 20"));
            Assert.That(evolution.Description, Does.Contain("+"));
            Assert.That(evolution.Description, Does.Contain("Friendship"));
        }
    }
}

