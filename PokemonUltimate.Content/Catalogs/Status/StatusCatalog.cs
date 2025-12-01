using System.Collections.Generic;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Builders;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Content.Catalogs.Status
{
    /// <summary>
    /// Central catalog of all status effect definitions.
    /// </summary>
    public static class StatusCatalog
    {
        private static readonly List<StatusEffectData> _all = new List<StatusEffectData>();

        /// <summary>
        /// Gets all registered status effects.
        /// </summary>
        public static IReadOnlyList<StatusEffectData> All => _all;

        #region Persistent Status - Major Status Conditions

        /// <summary>
        /// Burn: 1/16 HP damage per turn, halves physical attack.
        /// Fire types are immune.
        /// </summary>
        public static readonly StatusEffectData Burn = Core.Builders.Status.Define("Burn")
            .Description("The Pokémon is burned. It takes damage each turn and its Attack is halved.")
            .Persistent(PersistentStatus.Burn)
            .Indefinite()
            .DealsDamagePerTurn(1f / 16f)
            .HalvesPhysicalAttack()
            .ImmuneTypes(PokemonType.Fire)
            .Build();

        /// <summary>
        /// Paralysis: 25% chance can't move, Speed halved.
        /// Electric types are immune (Gen 6+), Ground types were never immune.
        /// </summary>
        public static readonly StatusEffectData Paralysis = Core.Builders.Status.Define("Paralysis")
            .Description("The Pokémon is paralyzed. It may be unable to move and its Speed is halved.")
            .Persistent(PersistentStatus.Paralysis)
            .Indefinite()
            .FailsToMove(0.25f)
            .HalvesSpeed()
            .ImmuneTypes(PokemonType.Electric)
            .Build();

        /// <summary>
        /// Sleep: Cannot move for 1-3 turns.
        /// </summary>
        public static readonly StatusEffectData Sleep = Core.Builders.Status.Define("Sleep")
            .Description("The Pokémon is asleep and cannot move.")
            .Persistent(PersistentStatus.Sleep)
            .LastsTurns(1, 3)
            .PreventsAction()
            .Build();

        /// <summary>
        /// Poison: 1/8 HP damage per turn.
        /// Poison and Steel types are immune.
        /// </summary>
        public static readonly StatusEffectData Poison = Core.Builders.Status.Define("Poison")
            .Description("The Pokémon is poisoned and takes damage each turn.")
            .Persistent(PersistentStatus.Poison)
            .Indefinite()
            .DealsDamagePerTurn(1f / 8f)
            .ImmuneTypes(PokemonType.Poison, PokemonType.Steel)
            .Build();

        /// <summary>
        /// Badly Poisoned (Toxic): Escalating damage (1/16, 2/16, 3/16...).
        /// Poison and Steel types are immune.
        /// </summary>
        public static readonly StatusEffectData BadlyPoisoned = Core.Builders.Status.Define("Badly Poisoned")
            .Description("The Pokémon is badly poisoned. Damage increases each turn.")
            .Persistent(PersistentStatus.BadlyPoisoned)
            .Indefinite()
            .DealsEscalatingDamage(1f / 16f, 1)
            .ImmuneTypes(PokemonType.Poison, PokemonType.Steel)
            .Build();

        /// <summary>
        /// Freeze: Cannot move, 20% chance to thaw each turn.
        /// Fire moves thaw the frozen Pokémon. Ice types are immune.
        /// </summary>
        public static readonly StatusEffectData Freeze = Core.Builders.Status.Define("Freeze")
            .Description("The Pokémon is frozen solid and cannot move.")
            .Persistent(PersistentStatus.Freeze)
            .Indefinite()
            .PreventsAction()
            .RecoveryChance(0.20f)
            .CuredByMoveTypes(PokemonType.Fire)
            .ImmuneTypes(PokemonType.Ice)
            .Build();

        #endregion

        #region Volatile Status - Temporary Conditions

        /// <summary>
        /// Confusion: 33% chance to hit self for 40 power typeless damage.
        /// Lasts 2-5 turns.
        /// </summary>
        public static readonly StatusEffectData Confusion = Core.Builders.Status.Define("Confusion")
            .Description("The Pokémon is confused and may hurt itself.")
            .Volatile(VolatileStatus.Confusion)
            .LastsTurns(2, 5)
            .SelfHitChance(0.33f, 40)
            .Build();

        /// <summary>
        /// Attract/Infatuation: 50% chance can't move due to love.
        /// Only works on opposite gender.
        /// </summary>
        public static readonly StatusEffectData Attract = Core.Builders.Status.Define("Attract")
            .Description("The Pokémon is infatuated and may be unable to attack.")
            .Volatile(VolatileStatus.Attract)
            .Indefinite() // Until target switches out
            .FailsToMove(0.50f)
            .Build();

        /// <summary>
        /// Flinch: Cannot move this turn.
        /// Only lasts 1 turn, applied by certain moves.
        /// </summary>
        public static readonly StatusEffectData Flinch = Core.Builders.Status.Define("Flinch")
            .Description("The Pokémon flinched and couldn't move.")
            .Volatile(VolatileStatus.Flinch)
            .LastsTurns(1)
            .PreventsAction()
            .Build();

        /// <summary>
        /// Leech Seed: Drains 1/8 HP to opponent each turn.
        /// Grass types are immune.
        /// </summary>
        public static readonly StatusEffectData LeechSeed = Core.Builders.Status.Define("Leech Seed")
            .Description("The Pokémon's HP is drained to the opponent each turn.")
            .Volatile(VolatileStatus.LeechSeed)
            .Indefinite()
            .DrainsToOpponent(1f / 8f)
            .ImmuneTypes(PokemonType.Grass)
            .Build();

        /// <summary>
        /// Curse (Ghost): Loses 1/4 HP each turn.
        /// Applied by Ghost-type Curse.
        /// </summary>
        public static readonly StatusEffectData Curse = Core.Builders.Status.Define("Curse")
            .Description("The Pokémon is cursed and loses HP each turn.")
            .Volatile(VolatileStatus.Curse)
            .Indefinite()
            .DealsDamagePerTurn(0.25f)
            .Build();

        /// <summary>
        /// Encore: Forced to repeat the last move for 3 turns.
        /// </summary>
        public static readonly StatusEffectData Encore = Core.Builders.Status.Define("Encore")
            .Description("The Pokémon is forced to repeat its last move.")
            .Volatile(VolatileStatus.Encore)
            .LastsTurns(3)
            .ForcesMove()
            .Build();

        /// <summary>
        /// Taunt: Cannot use status moves for 3 turns.
        /// </summary>
        public static readonly StatusEffectData Taunt = Core.Builders.Status.Define("Taunt")
            .Description("The Pokémon cannot use status moves.")
            .Volatile(VolatileStatus.Taunt)
            .LastsTurns(3)
            .RestrictsMoveCategory(MoveCategory.Status)
            .Build();

        /// <summary>
        /// Torment: Cannot use the same move twice in a row.
        /// </summary>
        public static readonly StatusEffectData Torment = Core.Builders.Status.Define("Torment")
            .Description("The Pokémon cannot use the same move consecutively.")
            .Volatile(VolatileStatus.Torment)
            .Indefinite()
            .Build();

        /// <summary>
        /// Disable: One move is disabled for 4 turns.
        /// </summary>
        public static readonly StatusEffectData Disable = Core.Builders.Status.Define("Disable")
            .Description("One of the Pokémon's moves is disabled.")
            .Volatile(VolatileStatus.Disable)
            .LastsTurns(4)
            .Build();

        #endregion

        #region Lookup Methods

        /// <summary>
        /// Gets status effect data by persistent status enum.
        /// </summary>
        public static StatusEffectData GetByStatus(PersistentStatus status)
        {
            switch (status)
            {
                case PersistentStatus.Burn: return Burn;
                case PersistentStatus.Paralysis: return Paralysis;
                case PersistentStatus.Sleep: return Sleep;
                case PersistentStatus.Poison: return Poison;
                case PersistentStatus.BadlyPoisoned: return BadlyPoisoned;
                case PersistentStatus.Freeze: return Freeze;
                default: return null;
            }
        }

        /// <summary>
        /// Gets status effect data by volatile status enum.
        /// </summary>
        public static StatusEffectData GetByStatus(VolatileStatus status)
        {
            switch (status)
            {
                case VolatileStatus.Confusion: return Confusion;
                case VolatileStatus.Attract: return Attract;
                case VolatileStatus.Flinch: return Flinch;
                case VolatileStatus.LeechSeed: return LeechSeed;
                case VolatileStatus.Curse: return Curse;
                case VolatileStatus.Encore: return Encore;
                case VolatileStatus.Taunt: return Taunt;
                case VolatileStatus.Torment: return Torment;
                case VolatileStatus.Disable: return Disable;
                default: return null;
            }
        }

        /// <summary>
        /// Gets status effect data by name.
        /// </summary>
        public static StatusEffectData GetByName(string name)
        {
            return _all.Find(s => s.Name == name);
        }

        #endregion

        #region Static Constructor

        static StatusCatalog()
        {
            // Persistent statuses
            _all.Add(Burn);
            _all.Add(Paralysis);
            _all.Add(Sleep);
            _all.Add(Poison);
            _all.Add(BadlyPoisoned);
            _all.Add(Freeze);

            // Volatile statuses
            _all.Add(Confusion);
            _all.Add(Attract);
            _all.Add(Flinch);
            _all.Add(LeechSeed);
            _all.Add(Curse);
            _all.Add(Encore);
            _all.Add(Taunt);
            _all.Add(Torment);
            _all.Add(Disable);
        }

        #endregion
    }
}

