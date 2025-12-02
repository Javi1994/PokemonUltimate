using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Engine;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Combat.Integration
{
    /// <summary>
    /// Integration tests for HealAction - verifies healing integrates with CombatEngine and battle flow.
    /// </summary>
    [TestFixture]
    public class HealActionIntegrationTests
    {
        private BattleField _field;
        private BattleSlot _targetSlot;
        private PokemonInstance _target;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            _field.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 },
                new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) },
                new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) });

            _targetSlot = _field.PlayerSide.Slots[0];
            _target = _targetSlot.Pokemon;
        }

        #region HealAction -> CombatEngine Integration

        [Test]
        public async Task CombatEngine_HealAction_RestoresHP()
        {
            // Arrange
            var engine = new CombatEngine();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            var view = new NullBattleView();
            
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            
            engine.Initialize(rules, playerParty, enemyParty, 
                new TestActionProvider(new MessageAction("Pass")), 
                new TestActionProvider(new MessageAction("Pass")), 
                view);

            // Damage the Pokemon after initialization
            var targetSlot = engine.Field.PlayerSide.Slots[0];
            targetSlot.Pokemon.TakeDamage(50);
            int hpBeforeHeal = targetSlot.Pokemon.CurrentHP;
            Assert.That(hpBeforeHeal, Is.LessThan(targetSlot.Pokemon.MaxHP));

            // Create heal action
            var healAction = new HealAction(null, targetSlot, 30);
            var provider = new TestActionProvider(healAction);
            targetSlot.ActionProvider = provider;

            // Act - Process heal action through CombatEngine
            await engine.RunTurn();

            // Assert - HP should be restored
            int hpAfterHeal = targetSlot.Pokemon.CurrentHP;
            Assert.That(hpAfterHeal, Is.GreaterThan(hpBeforeHeal));
        }

        [Test]
        public void HealAction_ThroughBattleQueue_ProcessesCorrectly()
        {
            // Arrange
            var queue = new BattleQueue();
            int initialHP = _target.CurrentHP;
            
            // Damage the Pokemon
            _target.TakeDamage(50);
            int damagedHP = _target.CurrentHP;
            Assert.That(damagedHP, Is.LessThan(initialHP));

            var healAction = new HealAction(null, _targetSlot, 30);

            // Act - Process through queue
            queue.Enqueue(healAction);
            queue.ProcessQueue(_field, new NullBattleView()).Wait();

            // Assert - HP should be restored (but not exceed MaxHP)
            Assert.That(_target.CurrentHP, Is.GreaterThan(damagedHP));
            Assert.That(_target.CurrentHP, Is.LessThanOrEqualTo(_target.MaxHP));
        }

        [Test]
        public void HealAction_Overheal_PreventsExceedingMaxHP()
        {
            // Arrange
            int maxHP = _target.MaxHP;
            int currentHP = _target.CurrentHP;
            
            // Heal more than needed
            int healAmount = maxHP + 100;
            var healAction = new HealAction(null, _targetSlot, healAmount);

            // Act
            healAction.ExecuteLogic(_field);

            // Assert - HP should not exceed MaxHP
            Assert.That(_target.CurrentHP, Is.EqualTo(maxHP));
        }

        [Test]
        public async Task CombatEngine_HealAction_AfterDamage_RestoresHP()
        {
            // Arrange
            var engine = new CombatEngine();
            var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
            var view = new NullBattleView();
            
            var playerParty = new[] { PokemonFactory.Create(PokemonCatalog.Pikachu, 50) };
            var enemyParty = new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) };
            
            engine.Initialize(rules, playerParty, enemyParty, 
                new TestActionProvider(new MessageAction("Pass")), 
                new TestActionProvider(new MessageAction("Pass")), 
                view);

            var targetSlot = engine.Field.PlayerSide.Slots[0];
            int maxHP = targetSlot.Pokemon.MaxHP;
            
            // Damage the Pokemon
            targetSlot.Pokemon.TakeDamage(50);
            int damagedHP = targetSlot.Pokemon.CurrentHP;
            Assert.That(damagedHP, Is.LessThan(maxHP));

            // Create heal action
            var healAction = new HealAction(null, targetSlot, 30);
            var healProvider = new TestActionProvider(healAction);
            targetSlot.ActionProvider = healProvider;

            // Act - Process heal through CombatEngine
            await engine.RunTurn();

            // Assert - HP should be restored
            Assert.That(targetSlot.Pokemon.CurrentHP, Is.GreaterThan(damagedHP));
        }

        #endregion
    }
}

