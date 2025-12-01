using NUnit.Framework;
using PokemonUltimate.Content.Catalogs.Field;
using PokemonUltimate.Core.Builders;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Tests.Blueprints
{
    /// <summary>
    /// Tests for FieldEffectData and FieldEffectBuilder.
    /// </summary>
    [TestFixture]
    public class FieldEffectDataTests
    {
        #region Builder Tests

        [Test]
        public void Define_WithName_SetsNameAndId()
        {
            var effect = Room.Define("Test Room").Build();

            Assert.That(effect.Name, Is.EqualTo("Test Room"));
            Assert.That(effect.Id, Is.EqualTo("test-room"));
        }

        [Test]
        public void Duration_SetsCorrectly()
        {
            var effect = Room.Define("Test")
                .Duration(5)
                .Build();

            Assert.That(effect.DefaultDuration, Is.EqualTo(5));
        }

        [Test]
        public void Toggle_SetsCorrectly()
        {
            var effect = Room.Define("Test")
                .Toggle()
                .Build();

            Assert.That(effect.IsToggle, Is.True);
        }

        [Test]
        public void ReversesSpeedOrder_SetsCorrectly()
        {
            var effect = Room.Define("Test")
                .ReversesSpeedOrder()
                .Build();

            Assert.That(effect.ReversesSpeedOrder, Is.True);
        }

        [Test]
        public void DisablesItems_SetsCorrectly()
        {
            var effect = Room.Define("Test")
                .DisablesItems()
                .Build();

            Assert.That(effect.DisablesItems, Is.True);
        }

        [Test]
        public void SwapsDefenses_SetsCorrectly()
        {
            var effect = Room.Define("Test")
                .SwapsDefenses()
                .Build();

            Assert.That(effect.SwapsDefenses, Is.True);
        }

        [Test]
        public void GroundsAllPokemon_SetsCorrectly()
        {
            var effect = Room.Define("Test")
                .GroundsAllPokemon()
                .Build();

            Assert.That(effect.GroundsAllPokemon, Is.True);
            Assert.That(effect.GroundHitsFlying, Is.True);
        }

        [Test]
        public void DisablesMoves_SetsCorrectly()
        {
            var effect = Room.Define("Test")
                .DisablesMoves("Fly", "Bounce")
                .Build();

            Assert.That(effect.DisablesMoves, Is.True);
            Assert.That(effect.IsMoveDisabled("Fly"), Is.True);
            Assert.That(effect.IsMoveDisabled("Bounce"), Is.True);
            Assert.That(effect.IsMoveDisabled("Surf"), Is.False);
        }

        [Test]
        public void ChangesMoveType_SetsCorrectly()
        {
            var effect = Room.Define("Test")
                .ChangesMoveType(PokemonType.Normal, PokemonType.Electric)
                .Build();

            Assert.That(effect.ChangesMoveTypes, Is.True);
            Assert.That(effect.ChangesMovesFromType, Is.EqualTo(PokemonType.Normal));
            Assert.That(effect.ChangesMovesToType, Is.EqualTo(PokemonType.Electric));
        }

        [Test]
        public void ReducesTypePower_SetsCorrectly()
        {
            var effect = Room.Define("Test")
                .ReducesTypePower(PokemonType.Electric, 0.33f)
                .Build();

            Assert.That(effect.ReducesMovePower, Is.True);
            Assert.That(effect.ReducesPowerOfType, Is.EqualTo(PokemonType.Electric));
            Assert.That(effect.PowerMultiplier, Is.EqualTo(0.33f));
        }

        #endregion

        #region Helper Methods Tests

        [Test]
        public void IsMoveDisabled_CaseInsensitive()
        {
            var effect = Room.Define("Test")
                .DisablesMoves("Fly")
                .Build();

            Assert.That(effect.IsMoveDisabled("FLY"), Is.True);
            Assert.That(effect.IsMoveDisabled("fly"), Is.True);
        }

        [Test]
        public void GetPowerMultiplier_AffectedType_ReturnsMultiplier()
        {
            var effect = Room.Define("Test")
                .ReducesTypePower(PokemonType.Electric, 0.33f)
                .Build();

            Assert.That(effect.GetPowerMultiplier(PokemonType.Electric), Is.EqualTo(0.33f));
            Assert.That(effect.GetPowerMultiplier(PokemonType.Fire), Is.EqualTo(1.0f));
        }

        [Test]
        public void GetEffectiveMoveType_WithChange_ReturnsNewType()
        {
            var effect = Room.Define("Test")
                .ChangesMoveType(PokemonType.Normal, PokemonType.Electric)
                .Build();

            Assert.That(effect.GetEffectiveMoveType(PokemonType.Normal), Is.EqualTo(PokemonType.Electric));
            Assert.That(effect.GetEffectiveMoveType(PokemonType.Fire), Is.EqualTo(PokemonType.Fire));
        }

        #endregion

        #region Catalog Tests

        [Test]
        public void Catalog_TrickRoom_HasCorrectProperties()
        {
            var tr = FieldEffectCatalog.TrickRoom;

            Assert.That(tr.Type, Is.EqualTo(FieldEffect.TrickRoom));
            Assert.That(tr.DefaultDuration, Is.EqualTo(5));
            Assert.That(tr.IsToggle, Is.True);
            Assert.That(tr.ReversesSpeedOrder, Is.True);
        }

        [Test]
        public void Catalog_MagicRoom_HasCorrectProperties()
        {
            var mr = FieldEffectCatalog.MagicRoom;

            Assert.That(mr.Type, Is.EqualTo(FieldEffect.MagicRoom));
            Assert.That(mr.DisablesItems, Is.True);
            Assert.That(mr.IsToggle, Is.True);
        }

        [Test]
        public void Catalog_WonderRoom_HasCorrectProperties()
        {
            var wr = FieldEffectCatalog.WonderRoom;

            Assert.That(wr.Type, Is.EqualTo(FieldEffect.WonderRoom));
            Assert.That(wr.SwapsDefenses, Is.True);
        }

        [Test]
        public void Catalog_Gravity_HasCorrectProperties()
        {
            var gravity = FieldEffectCatalog.Gravity;

            Assert.That(gravity.Type, Is.EqualTo(FieldEffect.Gravity));
            Assert.That(gravity.GroundsAllPokemon, Is.True);
            Assert.That(gravity.DisablesMoves, Is.True);
            Assert.That(gravity.IsMoveDisabled("Fly"), Is.True);
            Assert.That(gravity.IsMoveDisabled("Bounce"), Is.True);
        }

        [Test]
        public void Catalog_IonDeluge_HasCorrectProperties()
        {
            var ion = FieldEffectCatalog.IonDeluge;

            Assert.That(ion.Type, Is.EqualTo(FieldEffect.IonDeluge));
            Assert.That(ion.DefaultDuration, Is.EqualTo(1));
            Assert.That(ion.GetEffectiveMoveType(PokemonType.Normal), Is.EqualTo(PokemonType.Electric));
        }

        [Test]
        public void Catalog_MudSport_ReducesElectric()
        {
            var ms = FieldEffectCatalog.MudSport;

            Assert.That(ms.ReducesPowerOfType, Is.EqualTo(PokemonType.Electric));
            Assert.That(ms.GetPowerMultiplier(PokemonType.Electric), Is.EqualTo(0.33f));
        }

        [Test]
        public void Catalog_WaterSport_ReducesFire()
        {
            var ws = FieldEffectCatalog.WaterSport;

            Assert.That(ws.ReducesPowerOfType, Is.EqualTo(PokemonType.Fire));
            Assert.That(ws.GetPowerMultiplier(PokemonType.Fire), Is.EqualTo(0.33f));
        }

        [Test]
        public void Catalog_All_Contains8Effects()
        {
            Assert.That(FieldEffectCatalog.All.Count, Is.EqualTo(8));
        }

        [Test]
        public void Catalog_GetByType_ReturnsCorrectEffect()
        {
            Assert.That(FieldEffectCatalog.GetByType(FieldEffect.TrickRoom), Is.EqualTo(FieldEffectCatalog.TrickRoom));
            Assert.That(FieldEffectCatalog.GetByType(FieldEffect.None), Is.Null);
        }

        [Test]
        public void Catalog_GetRooms_Returns3Rooms()
        {
            var rooms = new System.Collections.Generic.List<Core.Blueprints.FieldEffectData>(
                FieldEffectCatalog.GetRooms());

            Assert.That(rooms.Count, Is.EqualTo(3));
            Assert.That(rooms, Contains.Item(FieldEffectCatalog.TrickRoom));
            Assert.That(rooms, Contains.Item(FieldEffectCatalog.MagicRoom));
            Assert.That(rooms, Contains.Item(FieldEffectCatalog.WonderRoom));
        }

        [Test]
        public void Catalog_GetToggleable_ReturnsToggleEffects()
        {
            var toggleable = new System.Collections.Generic.List<Core.Blueprints.FieldEffectData>(
                FieldEffectCatalog.GetToggleable());

            Assert.That(toggleable, Contains.Item(FieldEffectCatalog.TrickRoom));
            Assert.That(toggleable, Does.Not.Contain(FieldEffectCatalog.Gravity));
        }

        #endregion
    }
}

