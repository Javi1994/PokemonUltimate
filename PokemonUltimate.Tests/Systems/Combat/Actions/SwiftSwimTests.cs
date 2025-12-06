using NUnit.Framework;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Helpers;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Combat.Providers;
using PokemonUltimate.Content.Catalogs.Abilities;
using PokemonUltimate.Content.Catalogs.Pokemon;

namespace PokemonUltimate.Tests.Systems.Combat.Actions
{
    /// <summary>
    /// Functional tests for Swift Swim ability - doubles Speed in rain.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.17: Advanced Abilities
    /// **Documentation**: See `docs/features/2-combat-system/PLAN_COMPLETAR_FEATURE_2.md`
    /// </remarks>
    [TestFixture]
    public class SwiftSwimTests
    {
        private BattleField _field;
        private BattleSlot _userSlot;
        private PokemonInstance _user;
        private TurnOrderResolver _turnOrderResolver;

        [SetUp]
        public void SetUp()
        {
            _field = new BattleField();
            var userPokemon = PokemonFactory.Create(PokemonCatalog.Gyarados, 50);
            userPokemon.SetAbility(AbilityCatalog.SwiftSwim);
            
            _field.Initialize(new BattleRules { PlayerSlots = 1, EnemySlots = 1 },
                new[] { userPokemon },
                new[] { PokemonFactory.Create(PokemonCatalog.Charmander, 50) });

            _userSlot = _field.PlayerSide.Slots[0];
            _user = _userSlot.Pokemon;
            _turnOrderResolver = new TurnOrderResolver(new RandomProvider());
        }

        #region Speed Modifier Tests

        [Test]
        public void GetEffectiveSpeed_OnWeatherChange_Rain_WithSwiftSwim_DoublesSpeed()
        {
            // Arrange - Set rain weather
            var setWeatherAction = new SetWeatherAction(null, Weather.Rain, 5);
            setWeatherAction.ExecuteLogic(_field).ToList(); // Execute to set weather

            // Get base speed (without weather modifier)
            float baseSpeed = _user.Speed;

            // Act - Get effective speed with Swift Swim in rain
            float effectiveSpeed = _turnOrderResolver.GetEffectiveSpeed(_userSlot, _field);

            // Assert - Speed should be doubled (2.0x multiplier)
            Assert.That(effectiveSpeed, Is.EqualTo(baseSpeed * 2.0f).Within(0.01f), 
                "Swift Swim should double Speed in rain");
        }

        [Test]
        public void GetEffectiveSpeed_OnWeatherChange_NoRain_WithSwiftSwim_NormalSpeed()
        {
            // Arrange - No weather (or different weather)
            var setWeatherAction = new SetWeatherAction(null, Weather.None, 0);
            setWeatherAction.ExecuteLogic(_field).ToList(); // Clear weather

            // Get base speed
            float baseSpeed = _user.Speed;

            // Act - Get effective speed without rain
            float effectiveSpeed = _turnOrderResolver.GetEffectiveSpeed(_userSlot, _field);

            // Assert - Speed should be normal (no multiplier)
            Assert.That(effectiveSpeed, Is.EqualTo(baseSpeed).Within(0.01f), 
                "Swift Swim should not affect Speed without rain");
        }

        [Test]
        public void GetEffectiveSpeed_OnWeatherChange_RainEnds_WithSwiftSwim_SpeedReturns()
        {
            // Arrange - Set rain first
            var setRainAction = new SetWeatherAction(null, Weather.Rain, 5);
            setRainAction.ExecuteLogic(_field).ToList();
            
            float speedInRain = _turnOrderResolver.GetEffectiveSpeed(_userSlot, _field);
            float baseSpeed = _user.Speed;
            
            Assert.That(speedInRain, Is.EqualTo(baseSpeed * 2.0f).Within(0.01f), 
                "Speed should be doubled in rain");

            // Act - Clear weather
            var clearWeatherAction = new SetWeatherAction(null, Weather.None, 0);
            clearWeatherAction.ExecuteLogic(_field).ToList();

            // Assert - Speed should return to normal
            float speedAfterRain = _turnOrderResolver.GetEffectiveSpeed(_userSlot, _field);
            Assert.That(speedAfterRain, Is.EqualTo(baseSpeed).Within(0.01f), 
                "Speed should return to normal when rain ends");
        }

        #endregion
    }
}

