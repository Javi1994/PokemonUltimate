using System.Linq;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Items;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Blueprints;

namespace PokemonUltimate.Tests.Systems.Combat.Actions
{
    /// <summary>
    /// Functional tests for Focus Sash item - prevents fainting at full HP.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.18: Advanced Items
    /// **Documentation**: See `docs/features/2-combat-system/2.18-advanced-items/README.md`
    /// </remarks>
    [TestFixture]
    public class FocusSashTests
    {
        private BattleField _field;
        private BattleSlot _userSlot;
        private BattleSlot _targetSlot;
        private PokemonInstance _user;
        private PokemonInstance _target;
        private MoveInstance _moveInstance;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            _field.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 },
                new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) },
                new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) });

            _userSlot = _field.PlayerSide.Slots[0];
            _targetSlot = _field.EnemySide.Slots[0];
            _user = _userSlot.Pokemon;
            _target = _targetSlot.Pokemon;
            
            // Set Focus Sash on target
            _target.HeldItem = ItemCatalog.FocusSash;
            _moveInstance = _user.Moves[0];
        }

        #region OnWouldFaint Tests

        [Test]
        public void ExecuteLogic_FatalDamage_AtFullHP_WithFocusSash_PreventsFainting()
        {
            // Arrange - Target at full HP, damage that would KO
            _target.CurrentHP = _target.MaxHP;
            
            // Create damage context that would KO
            var context = new DamageContext(_userSlot, _targetSlot, MoveCatalog.Tackle, _field);
            context.BaseDamage = _target.MaxHP + 100; // More than enough to KO
            context.Multiplier = 1.0f;

            // Act
            var damageAction = new DamageAction(_userSlot, _targetSlot, context);
            var reactions = damageAction.ExecuteLogic(_field).ToList();

            // Assert - Focus Sash should prevent fainting, leave at 1 HP
            Assert.That(_target.CurrentHP, Is.EqualTo(1), "Focus Sash should leave Pokemon at 1 HP");
            Assert.That(_target.IsFainted, Is.False, "Focus Sash should prevent fainting");
            Assert.That(_target.HeldItem, Is.Null, "Focus Sash should be consumed");
        }

        [Test]
        public void ExecuteLogic_FatalDamage_NotAtFullHP_WithFocusSash_DoesNotPreventFainting()
        {
            // Arrange - Target NOT at full HP
            _target.CurrentHP = _target.MaxHP - 10; // Not at full HP
            
            // Create damage context that would KO
            var context = new DamageContext(_userSlot, _targetSlot, MoveCatalog.Tackle, _field);
            context.BaseDamage = _target.MaxHP + 100;
            context.Multiplier = 1.0f;

            // Act
            var damageAction = new DamageAction(_userSlot, _targetSlot, context);
            var reactions = damageAction.ExecuteLogic(_field).ToList();

            // Assert - Focus Sash should NOT prevent fainting if not at full HP
            Assert.That(_target.IsFainted, Is.True, "Focus Sash should not prevent fainting if not at full HP");
        }

        [Test]
        public void ExecuteLogic_NonFatalDamage_WithFocusSash_NoEffect()
        {
            // Arrange - Damage that won't KO
            _target.CurrentHP = _target.MaxHP;
            int initialHP = _target.CurrentHP;
            
            var context = new DamageContext(_userSlot, _targetSlot, MoveCatalog.Tackle, _field);
            context.BaseDamage = 10; // Small damage
            context.Multiplier = 1.0f;

            // Act
            var damageAction = new DamageAction(_userSlot, _targetSlot, context);
            var reactions = damageAction.ExecuteLogic(_field).ToList();

            // Assert - Focus Sash should not activate for non-fatal damage
            Assert.That(_target.CurrentHP, Is.LessThan(initialHP), "Damage should be applied");
            Assert.That(_target.HeldItem, Is.EqualTo(ItemCatalog.FocusSash), "Focus Sash should not be consumed");
        }

        [Test]
        public void ExecuteLogic_WithoutFocusSash_FatalDamage_CausesFainting()
        {
            // Arrange - Remove Focus Sash
            _target.HeldItem = null;
            _target.CurrentHP = _target.MaxHP;
            
            var context = new DamageContext(_userSlot, _targetSlot, MoveCatalog.Tackle, _field);
            context.BaseDamage = _target.MaxHP + 100;
            context.Multiplier = 1.0f;

            // Act
            var damageAction = new DamageAction(_userSlot, _targetSlot, context);
            var reactions = damageAction.ExecuteLogic(_field).ToList();

            // Assert - Should faint without Focus Sash
            Assert.That(_target.IsFainted, Is.True, "Should faint without Focus Sash");
        }

        #endregion
    }
}

