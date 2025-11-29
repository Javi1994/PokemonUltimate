using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Evolution;
using PokemonUltimate.Core.Evolution.Conditions;
using PokemonUltimate.Core.Catalogs;

namespace PokemonUltimate.Tests.Evolution
{
    [TestFixture]
    public class EvolutionConditionTests
    {
        [Test]
        public void LevelCondition_Should_Store_MinLevel()
        {
            var condition = new LevelCondition(16);

            Assert.Multiple(() =>
            {
                Assert.That(condition.ConditionType, Is.EqualTo(EvolutionConditionType.Level));
                Assert.That(condition.MinLevel, Is.EqualTo(16));
                Assert.That(condition.Description, Is.EqualTo("Reach level 16"));
            });
        }

        [Test]
        public void ItemCondition_Should_Store_ItemName()
        {
            var condition = new ItemCondition("Thunder Stone");

            Assert.Multiple(() =>
            {
                Assert.That(condition.ConditionType, Is.EqualTo(EvolutionConditionType.UseItem));
                Assert.That(condition.ItemName, Is.EqualTo("Thunder Stone"));
                Assert.That(condition.Description, Is.EqualTo("Use Thunder Stone"));
            });
        }

        [Test]
        public void FriendshipCondition_Should_Have_Default_Value()
        {
            var condition = new FriendshipCondition();

            Assert.Multiple(() =>
            {
                Assert.That(condition.ConditionType, Is.EqualTo(EvolutionConditionType.Friendship));
                Assert.That(condition.MinFriendship, Is.EqualTo(220));
            });
        }

        [Test]
        public void FriendshipCondition_Should_Accept_Custom_Value()
        {
            var condition = new FriendshipCondition(160);

            Assert.That(condition.MinFriendship, Is.EqualTo(160));
        }

        [Test]
        public void TimeOfDayCondition_Should_Store_RequiredTime()
        {
            var condition = new TimeOfDayCondition(TimeOfDay.Night);

            Assert.Multiple(() =>
            {
                Assert.That(condition.ConditionType, Is.EqualTo(EvolutionConditionType.TimeOfDay));
                Assert.That(condition.RequiredTime, Is.EqualTo(TimeOfDay.Night));
                Assert.That(condition.Description, Is.EqualTo("During Night"));
            });
        }

        [Test]
        public void TradeCondition_Should_Have_Correct_Type()
        {
            var condition = new TradeCondition();

            Assert.Multiple(() =>
            {
                Assert.That(condition.ConditionType, Is.EqualTo(EvolutionConditionType.Trade));
                Assert.That(condition.Description, Is.EqualTo("Trade"));
            });
        }

        [Test]
        public void KnowsMoveCondition_Should_Store_Move_Reference()
        {
            var condition = new KnowsMoveCondition(MoveCatalog.Psychic);

            Assert.Multiple(() =>
            {
                Assert.That(condition.ConditionType, Is.EqualTo(EvolutionConditionType.KnowsMove));
                Assert.That(condition.RequiredMove, Is.EqualTo(MoveCatalog.Psychic));
                Assert.That(condition.Description, Does.Contain("Psychic"));
            });
        }
    }
}

