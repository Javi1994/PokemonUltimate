using NUnit.Framework;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Instances;

namespace PokemonUltimate.Tests.Systems.Core.Instances
{
    /// <summary>
    /// Tests for MoveInstance - runtime move with PP tracking.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Pokemon Data
    /// **Sub-Feature**: 1.3: PokemonInstance
    /// **Documentation**: See `docs/features/1-pokemon-data/architecture.md`
    /// </remarks>
    [TestFixture]
    public class MoveInstanceTests
    {
        private MoveData _testMove;

        [SetUp]
        public void SetUp()
        {
            _testMove = new MoveData
            {
                Name = "Test Move",
                Type = PokemonType.Normal,
                Category = MoveCategory.Physical,
                Power = 50,
                Accuracy = 100,
                MaxPP = 10
            };
        }

        #region Constructor Tests

        [Test]
        public void Constructor_Should_Set_Move_Reference()
        {
            var instance = new MoveInstance(_testMove);

            Assert.That(instance.Move, Is.EqualTo(_testMove));
        }

        [Test]
        public void Constructor_Should_Set_MaxPP_From_Blueprint()
        {
            var instance = new MoveInstance(_testMove);

            Assert.That(instance.MaxPP, Is.EqualTo(10));
        }

        [Test]
        public void Constructor_Should_Set_CurrentPP_To_MaxPP()
        {
            var instance = new MoveInstance(_testMove);

            Assert.That(instance.CurrentPP, Is.EqualTo(instance.MaxPP));
        }

        [Test]
        public void Constructor_Should_Throw_When_Move_Is_Null()
        {
            Assert.That(() => new MoveInstance(null), Throws.ArgumentNullException);
        }

        #endregion

        #region HasPP Tests

        [Test]
        public void HasPP_Should_Return_True_When_PP_Remaining()
        {
            var instance = new MoveInstance(_testMove);

            Assert.That(instance.HasPP, Is.True);
        }

        [Test]
        public void HasPP_Should_Return_False_When_No_PP()
        {
            var instance = new MoveInstance(_testMove);
            
            // Use all PP
            for (int i = 0; i < 10; i++)
                instance.Use();

            Assert.That(instance.HasPP, Is.False);
        }

        #endregion

        #region Use Tests

        [Test]
        public void Use_Should_Decrease_CurrentPP_By_One()
        {
            var instance = new MoveInstance(_testMove);
            int initialPP = instance.CurrentPP;

            instance.Use();

            Assert.That(instance.CurrentPP, Is.EqualTo(initialPP - 1));
        }

        [Test]
        public void Use_Should_Return_True_When_PP_Available()
        {
            var instance = new MoveInstance(_testMove);

            bool result = instance.Use();

            Assert.That(result, Is.True);
        }

        [Test]
        public void Use_Should_Return_False_When_No_PP()
        {
            var instance = new MoveInstance(_testMove);
            
            // Use all PP
            for (int i = 0; i < 10; i++)
                instance.Use();

            bool result = instance.Use();

            Assert.That(result, Is.False);
        }

        [Test]
        public void Use_Should_Not_Go_Below_Zero()
        {
            var instance = new MoveInstance(_testMove);
            
            // Try to use more than available
            for (int i = 0; i < 15; i++)
                instance.Use();

            Assert.That(instance.CurrentPP, Is.EqualTo(0));
        }

        #endregion

        #region Restore Tests

        [Test]
        public void Restore_Should_Increase_CurrentPP()
        {
            var instance = new MoveInstance(_testMove);
            instance.Use();
            instance.Use();
            instance.Use();

            instance.Restore(2);

            Assert.That(instance.CurrentPP, Is.EqualTo(9));
        }

        [Test]
        public void Restore_Should_Not_Exceed_MaxPP()
        {
            var instance = new MoveInstance(_testMove);
            instance.Use();

            instance.Restore(100);

            Assert.That(instance.CurrentPP, Is.EqualTo(instance.MaxPP));
        }

        [Test]
        public void Restore_Should_Throw_When_Amount_Negative()
        {
            var instance = new MoveInstance(_testMove);

            Assert.That(() => instance.Restore(-1), Throws.ArgumentException);
        }

        [Test]
        public void Restore_Zero_Should_Not_Change_PP()
        {
            var instance = new MoveInstance(_testMove);
            instance.Use();
            int ppBefore = instance.CurrentPP;

            instance.Restore(0);

            Assert.That(instance.CurrentPP, Is.EqualTo(ppBefore));
        }

        #endregion

        #region RestoreFully Tests

        [Test]
        public void RestoreFully_Should_Set_CurrentPP_To_MaxPP()
        {
            var instance = new MoveInstance(_testMove);
            
            // Use some PP
            for (int i = 0; i < 5; i++)
                instance.Use();

            instance.RestoreFully();

            Assert.That(instance.CurrentPP, Is.EqualTo(instance.MaxPP));
        }

        [Test]
        public void RestoreFully_Should_Work_When_Already_Full()
        {
            var instance = new MoveInstance(_testMove);

            instance.RestoreFully();

            Assert.That(instance.CurrentPP, Is.EqualTo(instance.MaxPP));
        }

        [Test]
        public void RestoreFully_Should_Work_When_Empty()
        {
            var instance = new MoveInstance(_testMove);
            
            // Use all PP
            for (int i = 0; i < 10; i++)
                instance.Use();

            instance.RestoreFully();

            Assert.That(instance.CurrentPP, Is.EqualTo(instance.MaxPP));
        }

        #endregion

        #region Integration Tests

        [Test]
        public void Move_Properties_Should_Be_Accessible_Through_Instance()
        {
            var instance = new MoveInstance(_testMove);

            Assert.That(instance.Move.Name, Is.EqualTo("Test Move"));
            Assert.That(instance.Move.Power, Is.EqualTo(50));
            Assert.That(instance.Move.Type, Is.EqualTo(PokemonType.Normal));
        }

        [Test]
        public void Different_PP_Values_Should_Work()
        {
            var lowPPMove = new MoveData { Name = "Low PP", MaxPP = 5 };
            var highPPMove = new MoveData { Name = "High PP", MaxPP = 40 };

            var lowInstance = new MoveInstance(lowPPMove);
            var highInstance = new MoveInstance(highPPMove);

            Assert.That(lowInstance.MaxPP, Is.EqualTo(5));
            Assert.That(highInstance.MaxPP, Is.EqualTo(40));
        }

        #endregion
    }
}

