using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.AI;
using PokemonUltimate.Combat.Validation;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Tests.Systems.Combat.Engine;

namespace PokemonUltimate.Tests.Systems.Combat.Integration.System
{
    /// <summary>
    /// Tests for doubles battle switching coordination to prevent duplicate Pokemon selections.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
    /// </remarks>
    [TestFixture]
    public class DoublesBattleSwitchingTests
    {
        private CombatEngine _engine;
        private BattleRules _rules;
        private NullBattleView _view;
        private IBattleStateValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _engine = CombatEngineTestHelper.CreateCombatEngine();
            _rules = new BattleRules { PlayerSlots = 2, EnemySlots = 2 }; // Doubles battle
            _view = new NullBattleView();
            _validator = new BattleStateValidator();
        }

        #region Duplicate Prevention Tests

        [Test]
        public async Task HandleFaintedPokemonSwitching_DoublesBattle_NoDuplicatePokemon()
        {
            // Arrange: Create a doubles battle where both player slots need switching
            var playerParty = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50),
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50)
            };
            var enemyParty = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };

            var playerAI = new TeamBattleAI();
            var enemyAI = new TeamBattleAI();

            _engine.Initialize(_rules, playerParty, enemyParty, playerAI, enemyAI, _view);

            // Faint both active Pokemon
            _engine.Field.PlayerSide.Slots[0].Pokemon.TakeDamage(_engine.Field.PlayerSide.Slots[0].Pokemon.MaxHP);
            _engine.Field.PlayerSide.Slots[1].Pokemon.TakeDamage(_engine.Field.PlayerSide.Slots[1].Pokemon.MaxHP);

            // Act: Use reflection to call HandleFaintedPokemonSwitching
            var method = typeof(CombatEngine).GetMethod("HandleFaintedPokemonSwitching",
                BindingFlags.NonPublic | BindingFlags.Instance);
            await (Task)method.Invoke(_engine, null);

            // Assert: No duplicate Pokemon in active slots
            var activePokemon = _engine.Field.PlayerSide.Slots
                .Where(s => !s.IsEmpty && s.Pokemon != null)
                .Select(s => s.Pokemon)
                .ToList();

            Assert.That(activePokemon.Count, Is.EqualTo(2), "Should have 2 active Pokemon after switching");
            Assert.That(activePokemon.Distinct().Count(), Is.EqualTo(activePokemon.Count),
                "No duplicate Pokemon should be in active slots");

            // Validate battle state
            var errors = _validator.ValidateField(_engine.Field);
            var duplicateErrors = errors.Where(e => e.Contains("Duplicate")).ToList();
            Assert.That(duplicateErrors, Is.Empty,
                $"Battle state should be valid. Errors: {string.Join("; ", errors)}");
        }

        [Test]
        public async Task HandleFaintedPokemonSwitching_DoublesBattle_BothSides_NoDuplicates()
        {
            // Arrange: Both sides have Pokemon that need switching
            var playerParty = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50),
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50)
            };
            var enemyParty = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50),
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50)
            };

            var playerAI = new TeamBattleAI();
            var enemyAI = new TeamBattleAI();

            _engine.Initialize(_rules, playerParty, enemyParty, playerAI, enemyAI, _view);

            // Faint all active Pokemon
            _engine.Field.PlayerSide.Slots[0].Pokemon.TakeDamage(_engine.Field.PlayerSide.Slots[0].Pokemon.MaxHP);
            _engine.Field.PlayerSide.Slots[1].Pokemon.TakeDamage(_engine.Field.PlayerSide.Slots[1].Pokemon.MaxHP);
            _engine.Field.EnemySide.Slots[0].Pokemon.TakeDamage(_engine.Field.EnemySide.Slots[0].Pokemon.MaxHP);
            _engine.Field.EnemySide.Slots[1].Pokemon.TakeDamage(_engine.Field.EnemySide.Slots[1].Pokemon.MaxHP);

            // Act: Use reflection to call HandleFaintedPokemonSwitching
            var method = typeof(CombatEngine).GetMethod("HandleFaintedPokemonSwitching",
                BindingFlags.NonPublic | BindingFlags.Instance);
            await (Task)method.Invoke(_engine, null);

            // Assert: No duplicates on either side
            var playerActive = _engine.Field.PlayerSide.Slots
                .Where(s => !s.IsEmpty && s.Pokemon != null)
                .Select(s => s.Pokemon)
                .ToList();

            var enemyActive = _engine.Field.EnemySide.Slots
                .Where(s => !s.IsEmpty && s.Pokemon != null)
                .Select(s => s.Pokemon)
                .ToList();

            Assert.That(playerActive.Distinct().Count(), Is.EqualTo(playerActive.Count),
                "Player side should have no duplicate Pokemon");
            Assert.That(enemyActive.Distinct().Count(), Is.EqualTo(enemyActive.Count),
                "Enemy side should have no duplicate Pokemon");

            // Validate battle state
            var errors = _validator.ValidateField(_engine.Field);
            var duplicateErrors = errors.Where(e => e.Contains("Duplicate")).ToList();
            Assert.That(duplicateErrors, Is.Empty,
                $"Battle state should be valid. Errors: {string.Join("; ", errors)}");
        }

        [Test]
        public async Task HandleFaintedPokemonSwitching_DoublesBattle_ValidatesBattleState()
        {
            // Arrange: Create doubles battle with multiple Pokemon
            var playerParty = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50),
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50),
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50)
            };
            var enemyParty = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };

            var playerAI = new TeamBattleAI();
            var enemyAI = new TeamBattleAI();

            _engine.Initialize(_rules, playerParty, enemyParty, playerAI, enemyAI, _view);

            // Faint both active Pokemon
            _engine.Field.PlayerSide.Slots[0].Pokemon.TakeDamage(_engine.Field.PlayerSide.Slots[0].Pokemon.MaxHP);
            _engine.Field.PlayerSide.Slots[1].Pokemon.TakeDamage(_engine.Field.PlayerSide.Slots[1].Pokemon.MaxHP);

            // Act: Use reflection to call HandleFaintedPokemonSwitching
            var method = typeof(CombatEngine).GetMethod("HandleFaintedPokemonSwitching",
                BindingFlags.NonPublic | BindingFlags.Instance);
            await (Task)method.Invoke(_engine, null);

            // Assert: Battle state should be valid (no duplicates)
            var errors = _validator.ValidateField(_engine.Field);
            var duplicateErrors = errors.Where(e => e.Contains("Duplicate")).ToList();

            Assert.That(duplicateErrors, Is.Empty,
                $"Battle state should be valid after switching. Errors: {string.Join("; ", errors)}");
        }

        [Test]
        public async Task HandleFaintedPokemonSwitching_DoublesBattle_InsufficientPokemon_HandlesGracefully()
        {
            // Arrange: Only 2 Pokemon available, both slots need switching
            var playerParty = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
                PokemonFactory.Create(PokemonCatalog.Charmander, 50)
            };
            var enemyParty = new[]
            {
                PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50),
                PokemonFactory.Create(PokemonCatalog.Squirtle, 50)
            };

            var playerAI = new TeamBattleAI();
            var enemyAI = new TeamBattleAI();

            _engine.Initialize(_rules, playerParty, enemyParty, playerAI, enemyAI, _view);

            // Faint both active Pokemon
            _engine.Field.PlayerSide.Slots[0].Pokemon.TakeDamage(_engine.Field.PlayerSide.Slots[0].Pokemon.MaxHP);
            _engine.Field.PlayerSide.Slots[1].Pokemon.TakeDamage(_engine.Field.PlayerSide.Slots[1].Pokemon.MaxHP);

            // Act: Use reflection to call HandleFaintedPokemonSwitching
            var method = typeof(CombatEngine).GetMethod("HandleFaintedPokemonSwitching",
                BindingFlags.NonPublic | BindingFlags.Instance);
            await (Task)method.Invoke(_engine, null);

            // Assert: Should handle gracefully (may have fewer switches than slots needing them)
            // Since both Pokemon are already active, no switches should be possible
            var activeCount = _engine.Field.PlayerSide.Slots
                .Count(s => !s.IsEmpty && s.Pokemon != null && !s.Pokemon.IsFainted);

            Assert.That(activeCount, Is.LessThanOrEqualTo(2),
                "Should handle case where insufficient Pokemon available");

            // Battle state should still be valid
            var errors = _validator.ValidateField(_engine.Field);
            var duplicateErrors = errors.Where(e => e.Contains("Duplicate")).ToList();
            Assert.That(duplicateErrors, Is.Empty,
                $"Battle state should be valid even with insufficient Pokemon. Errors: {string.Join("; ", errors)}");
        }

        #endregion
    }
}
