using System.Collections.Generic;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Infrastructure.Builders;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Content.Catalogs.Field
{
    /// <summary>
    /// Central catalog of all field effect definitions.
    /// </summary>
    /// <remarks>
    /// **Feature**: 3: Content Expansion
    /// **Sub-Feature**: 3.6: Field Conditions Expansion
    /// **Documentation**: See `docs/features/3-content-expansion/3.6-field-conditions-expansion/README.md`
    /// </remarks>
    public static class FieldEffectCatalog
    {
        private static readonly List<FieldEffectData> _all = new List<FieldEffectData>();

        /// <summary>
        /// Gets all registered field effects.
        /// </summary>
        public static IReadOnlyList<FieldEffectData> All => _all;

        #region Room Effects

        /// <summary>
        /// Trick Room - Slower Pokemon move first.
        /// Duration: 5 turns. Using again ends it early.
        /// </summary>
        public static readonly FieldEffectData TrickRoom = Room.Define("Trick Room")
            .Description("A bizarre area where slower Pokemon move first.")
            .Type(FieldEffect.TrickRoom)
            .Duration(5)
            .Toggle()
            .ReversesSpeedOrder()
            .Build();

        /// <summary>
        /// Magic Room - Held items have no effect.
        /// Duration: 5 turns.
        /// </summary>
        public static readonly FieldEffectData MagicRoom = Room.Define("Magic Room")
            .Description("A bizarre area where held items lose their effects.")
            .Type(FieldEffect.MagicRoom)
            .Duration(5)
            .Toggle()
            .DisablesItems()
            .Build();

        /// <summary>
        /// Wonder Room - Swaps Defense and Special Defense.
        /// Duration: 5 turns.
        /// </summary>
        public static readonly FieldEffectData WonderRoom = Room.Define("Wonder Room")
            .Description("A bizarre area where Defense and Sp. Def are swapped.")
            .Type(FieldEffect.WonderRoom)
            .Duration(5)
            .Toggle()
            .SwapsDefenses()
            .Build();

        #endregion

        #region Field Modifiers

        /// <summary>
        /// Gravity - Grounds all Pokemon, disables flying moves.
        /// Duration: 5 turns.
        /// </summary>
        public static readonly FieldEffectData Gravity = Room.Define("Gravity")
            .Description("Intense gravity grounds all Pokemon and prevents flying moves.")
            .Type(FieldEffect.Gravity)
            .Duration(5)
            .GroundsAllPokemon()
            .DisablesMoves(
                "Fly", "Bounce", "Sky Drop", "High Jump Kick", "Jump Kick",
                "Magnet Rise", "Telekinesis", "Flying Press", "Splash")
            .Build();

        /// <summary>
        /// Ion Deluge - Normal moves become Electric type.
        /// Duration: 1 turn (for that turn only).
        /// </summary>
        public static readonly FieldEffectData IonDeluge = Room.Define("Ion Deluge")
            .Description("Electric charge changes Normal moves to Electric type.")
            .Type(FieldEffect.IonDeluge)
            .Duration(1)
            .ChangesMoveType(PokemonType.Normal, PokemonType.Electric)
            .Build();

        /// <summary>
        /// Fairy Lock - No Pokemon can flee or switch next turn.
        /// Duration: 1 turn.
        /// </summary>
        public static readonly FieldEffectData FairyLock = Room.Define("Fairy Lock")
            .Description("Locks down the battlefield, preventing escape.")
            .Type(FieldEffect.FairyLock)
            .Duration(1)
            .PreventsSwitching()
            .Build();

        #endregion

        #region Type Weakeners (Legacy)

        /// <summary>
        /// Mud Sport - Reduces Electric move power by 67%.
        /// Duration: 5 turns.
        /// </summary>
        public static readonly FieldEffectData MudSport = Room.Define("Mud Sport")
            .Description("Weakens Electric-type moves while in effect.")
            .Type(FieldEffect.MudSport)
            .Duration(5)
            .ReducesTypePower(PokemonType.Electric, 0.33f)
            .Build();

        /// <summary>
        /// Water Sport - Reduces Fire move power by 67%.
        /// Duration: 5 turns.
        /// </summary>
        public static readonly FieldEffectData WaterSport = Room.Define("Water Sport")
            .Description("Weakens Fire-type moves while in effect.")
            .Type(FieldEffect.WaterSport)
            .Duration(5)
            .ReducesTypePower(PokemonType.Fire, 0.33f)
            .Build();

        #endregion

        #region Lookup Methods

        /// <summary>
        /// Gets field effect data by type.
        /// </summary>
        public static FieldEffectData GetByType(FieldEffect type)
        {
            switch (type)
            {
                case FieldEffect.TrickRoom: return TrickRoom;
                case FieldEffect.MagicRoom: return MagicRoom;
                case FieldEffect.WonderRoom: return WonderRoom;
                case FieldEffect.Gravity: return Gravity;
                case FieldEffect.IonDeluge: return IonDeluge;
                case FieldEffect.FairyLock: return FairyLock;
                case FieldEffect.MudSport: return MudSport;
                case FieldEffect.WaterSport: return WaterSport;
                default: return null;
            }
        }

        /// <summary>
        /// Gets field effect data by name.
        /// </summary>
        public static FieldEffectData GetByName(string name)
        {
            return _all.Find(e => e.Name == name);
        }

        /// <summary>
        /// Gets all room-type effects.
        /// </summary>
        public static IEnumerable<FieldEffectData> GetRooms()
        {
            yield return TrickRoom;
            yield return MagicRoom;
            yield return WonderRoom;
        }

        /// <summary>
        /// Gets all toggleable effects.
        /// </summary>
        public static IEnumerable<FieldEffectData> GetToggleable()
        {
            foreach (var effect in _all)
            {
                if (effect.IsToggle) yield return effect;
            }
        }

        #endregion

        #region Static Constructor

        static FieldEffectCatalog()
        {
            // Rooms
            _all.Add(TrickRoom);
            _all.Add(MagicRoom);
            _all.Add(WonderRoom);

            // Field Modifiers
            _all.Add(Gravity);
            _all.Add(IonDeluge);
            _all.Add(FairyLock);

            // Type Weakeners
            _all.Add(MudSport);
            _all.Add(WaterSport);
        }

        #endregion
    }
}

