using PokemonUltimate.Core.Effects.Strategies;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Core.Effects
{
    /// <summary>
    /// User faints after using the move.
    /// Used by Explosion, Self-Destruct, Memento, Final Gambit, Healing Wish.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.2: Move Data
    /// **Documentation**: See `docs/features/1-game-data/1.2-move-data/architecture.md`
    /// </remarks>
    public class SelfDestructEffect : IMoveEffect
    {
        public EffectType EffectType => EffectType.SelfDestruct;
        public string Description => GetDescription();

        /// <summary>Type of self-destruct effect.</summary>
        public SelfDestructType Type { get; set; } = SelfDestructType.Explosion;

        /// <summary>Whether the move deals damage (Explosion) or has other effects (Memento).</summary>
        public bool DealsDamage { get; set; } = true;

        /// <summary>Whether target's Defense is halved for damage calc (Explosion, Gen 1-4).</summary>
        public bool HalvesTargetDefense { get; set; } = false;

        /// <summary>Stat changes to apply to target before fainting (Memento).</summary>
        public StatChangeEffect[] StatChanges { get; set; }

        /// <summary>Whether the next Pokemon is fully healed (Healing Wish, Lunar Dance).</summary>
        public bool HealsReplacement { get; set; } = false;

        /// <summary>Whether PP is restored to replacement (Lunar Dance).</summary>
        public bool RestoresPP { get; set; } = false;

        /// <summary>For Final Gambit: deals damage equal to user's remaining HP.</summary>
        public bool DamageEqualsHP { get; set; } = false;

        public SelfDestructEffect() { }

        public SelfDestructEffect(SelfDestructType type)
        {
            Type = type;
            DealsDamage = type == SelfDestructType.Explosion || type == SelfDestructType.FinalGambit;
            HealsReplacement = type == SelfDestructType.HealingWish || type == SelfDestructType.LunarDance;
            DamageEqualsHP = type == SelfDestructType.FinalGambit;
            RestoresPP = type == SelfDestructType.LunarDance;
        }

        private string GetDescription()
        {
            return EffectDescriptionRegistries.GetSelfDestructDescription(this);
        }
    }

    /// <summary>
    /// Types of self-destruct moves.
    /// </summary>
    public enum SelfDestructType
    {
        /// <summary>Explosion, Self-Destruct - deals damage.</summary>
        Explosion,
        /// <summary>Memento - lowers target's stats.</summary>
        Memento,
        /// <summary>Final Gambit - damage equals user's HP.</summary>
        FinalGambit,
        /// <summary>Healing Wish - heals replacement.</summary>
        HealingWish,
        /// <summary>Lunar Dance - heals and restores PP.</summary>
        LunarDance,
        /// <summary>Misty Explosion - boosted in Misty Terrain.</summary>
        MistyExplosion,
    }
}

