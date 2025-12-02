using NUnit.Framework;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Evolution;
using PokemonUltimate.Core.Evolution.Conditions;
using MoveCatalog = PokemonUltimate.Content.Catalogs.Moves.MoveCatalog;

namespace PokemonUltimate.Tests.Systems.Core.Evolution
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

        #region TimeOfDay All Values Tests

        [Test]
        [TestCase(TimeOfDay.Morning)]
        [TestCase(TimeOfDay.Day)]
        [TestCase(TimeOfDay.Evening)]
        [TestCase(TimeOfDay.Night)]
        public void TimeOfDayCondition_Should_Work_For_All_Times(TimeOfDay time)
        {
            var condition = new TimeOfDayCondition(time);

            Assert.Multiple(() =>
            {
                Assert.That(condition.RequiredTime, Is.EqualTo(time));
                Assert.That(condition.Description, Does.Contain(time.ToString()));
            });
        }

        #endregion

        #region Edge Cases

        [Test]
        public void LevelCondition_Should_Accept_Level_One()
        {
            var condition = new LevelCondition(1);

            Assert.That(condition.MinLevel, Is.EqualTo(1));
        }

        [Test]
        public void LevelCondition_Should_Accept_High_Level()
        {
            var condition = new LevelCondition(100);

            Assert.That(condition.MinLevel, Is.EqualTo(100));
        }

        [Test]
        public void FriendshipCondition_Description_Should_Include_Value()
        {
            var condition = new FriendshipCondition(180);

            Assert.That(condition.Description, Does.Contain("180"));
        }

        [Test]
        public void ItemCondition_Should_Work_With_Different_Items()
        {
            var stones = new[] { "Fire Stone", "Water Stone", "Leaf Stone", "Moon Stone" };

            foreach (var stone in stones)
            {
                var condition = new ItemCondition(stone);
                Assert.That(condition.ItemName, Is.EqualTo(stone));
                Assert.That(condition.Description, Does.Contain(stone));
            }
        }

        #endregion
    }
}

