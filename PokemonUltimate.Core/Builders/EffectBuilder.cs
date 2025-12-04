using System.Collections.Generic;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.Content.Builders
{
    /// <summary>
    /// Fluent builder for composing move effects.
    /// Used within MoveBuilder.WithEffects().
    /// </summary>
    /// <remarks>
    /// **Feature**: 3: Content Expansion
    /// **Sub-Feature**: 3.9: Builders
    /// **Documentation**: See `docs/features/3-content-expansion/3.9-builders/README.md`
    /// </remarks>
    public class EffectBuilder
    {
        private readonly List<IMoveEffect> _effects = new List<IMoveEffect>();

        #region Damage Effects

        /// <summary>
        /// Add standard damage effect.
        /// </summary>
        public EffectBuilder Damage()
        {
            _effects.Add(new DamageEffect());
            return this;
        }

        /// <summary>
        /// Add damage effect with high crit ratio.
        /// </summary>
        public EffectBuilder DamageHighCrit(int critStages = 1)
        {
            _effects.Add(new DamageEffect { CritStages = critStages });
            return this;
        }

        /// <summary>
        /// Add fixed damage effect.
        /// </summary>
        public EffectBuilder FixedDamage(int amount)
        {
            _effects.Add(new FixedDamageEffect(amount));
            return this;
        }

        /// <summary>
        /// Add level-based damage effect (like Seismic Toss).
        /// </summary>
        public EffectBuilder LevelDamage()
        {
            _effects.Add(new FixedDamageEffect { UseLevelAsDamage = true });
            return this;
        }

        #endregion

        #region Status Effects

        /// <summary>
        /// May inflict burn on target.
        /// </summary>
        public EffectBuilder MayBurn(int chancePercent = 100)
        {
            _effects.Add(new StatusEffect(PersistentStatus.Burn, chancePercent));
            return this;
        }

        /// <summary>
        /// May inflict paralysis on target.
        /// </summary>
        public EffectBuilder MayParalyze(int chancePercent = 100)
        {
            _effects.Add(new StatusEffect(PersistentStatus.Paralysis, chancePercent));
            return this;
        }

        /// <summary>
        /// May inflict poison on target.
        /// </summary>
        public EffectBuilder MayPoison(int chancePercent = 100)
        {
            _effects.Add(new StatusEffect(PersistentStatus.Poison, chancePercent));
            return this;
        }

        /// <summary>
        /// May inflict bad poison (toxic) on target.
        /// </summary>
        public EffectBuilder MayBadlyPoison(int chancePercent = 100)
        {
            _effects.Add(new StatusEffect(PersistentStatus.BadlyPoisoned, chancePercent));
            return this;
        }

        /// <summary>
        /// May inflict sleep on target.
        /// </summary>
        public EffectBuilder MaySleep(int chancePercent = 100)
        {
            _effects.Add(new StatusEffect(PersistentStatus.Sleep, chancePercent));
            return this;
        }

        /// <summary>
        /// May freeze the target.
        /// </summary>
        public EffectBuilder MayFreeze(int chancePercent = 100)
        {
            _effects.Add(new StatusEffect(PersistentStatus.Freeze, chancePercent));
            return this;
        }

        /// <summary>
        /// Inflict status on self (like Rest).
        /// </summary>
        public EffectBuilder SelfStatus(PersistentStatus status)
        {
            _effects.Add(new StatusEffect { Status = status, TargetSelf = true });
            return this;
        }

        #endregion

        #region Stat Change Effects

        /// <summary>
        /// Raise user's Attack.
        /// </summary>
        public EffectBuilder RaiseAttack(int stages = 1, int chancePercent = 100)
        {
            _effects.Add(new StatChangeEffect(Stat.Attack, stages, true, chancePercent));
            return this;
        }

        /// <summary>
        /// Raise user's Defense.
        /// </summary>
        public EffectBuilder RaiseDefense(int stages = 1, int chancePercent = 100)
        {
            _effects.Add(new StatChangeEffect(Stat.Defense, stages, true, chancePercent));
            return this;
        }

        /// <summary>
        /// Raise user's Special Attack.
        /// </summary>
        public EffectBuilder RaiseSpAttack(int stages = 1, int chancePercent = 100)
        {
            _effects.Add(new StatChangeEffect(Stat.SpAttack, stages, true, chancePercent));
            return this;
        }

        /// <summary>
        /// Raise user's Special Defense.
        /// </summary>
        public EffectBuilder RaiseSpDefense(int stages = 1, int chancePercent = 100)
        {
            _effects.Add(new StatChangeEffect(Stat.SpDefense, stages, true, chancePercent));
            return this;
        }

        /// <summary>
        /// Raise user's Speed.
        /// </summary>
        public EffectBuilder RaiseSpeed(int stages = 1, int chancePercent = 100)
        {
            _effects.Add(new StatChangeEffect(Stat.Speed, stages, true, chancePercent));
            return this;
        }

        /// <summary>
        /// Lower target's Attack.
        /// </summary>
        public EffectBuilder LowerAttack(int stages = 1, int chancePercent = 100)
        {
            _effects.Add(new StatChangeEffect(Stat.Attack, -stages, false, chancePercent));
            return this;
        }

        /// <summary>
        /// Lower target's Defense.
        /// </summary>
        public EffectBuilder LowerDefense(int stages = 1, int chancePercent = 100)
        {
            _effects.Add(new StatChangeEffect(Stat.Defense, -stages, false, chancePercent));
            return this;
        }

        /// <summary>
        /// Lower target's Special Attack.
        /// </summary>
        public EffectBuilder LowerSpAttack(int stages = 1, int chancePercent = 100)
        {
            _effects.Add(new StatChangeEffect(Stat.SpAttack, -stages, false, chancePercent));
            return this;
        }

        /// <summary>
        /// Lower target's Special Defense.
        /// </summary>
        public EffectBuilder LowerSpDefense(int stages = 1, int chancePercent = 100)
        {
            _effects.Add(new StatChangeEffect(Stat.SpDefense, -stages, false, chancePercent));
            return this;
        }

        /// <summary>
        /// Lower target's Speed.
        /// </summary>
        public EffectBuilder LowerSpeed(int stages = 1, int chancePercent = 100)
        {
            _effects.Add(new StatChangeEffect(Stat.Speed, -stages, false, chancePercent));
            return this;
        }

        /// <summary>
        /// Lower target's Accuracy.
        /// </summary>
        public EffectBuilder LowerAccuracy(int stages = 1, int chancePercent = 100)
        {
            _effects.Add(new StatChangeEffect(Stat.Accuracy, -stages, false, chancePercent));
            return this;
        }

        /// <summary>
        /// Raise user's Evasion.
        /// </summary>
        public EffectBuilder RaiseEvasion(int stages = 1, int chancePercent = 100)
        {
            _effects.Add(new StatChangeEffect(Stat.Evasion, stages, true, chancePercent));
            return this;
        }

        #endregion

        #region Recoil & Drain Effects

        /// <summary>
        /// User takes recoil damage (percentage of damage dealt).
        /// </summary>
        public EffectBuilder Recoil(int percent)
        {
            _effects.Add(new RecoilEffect(percent));
            return this;
        }

        /// <summary>
        /// User heals from damage dealt.
        /// </summary>
        public EffectBuilder Drain(int percent)
        {
            _effects.Add(new DrainEffect(percent));
            return this;
        }

        #endregion

        #region Heal Effects

        /// <summary>
        /// Heal user by percentage of max HP.
        /// </summary>
        public EffectBuilder Heal(int percent)
        {
            _effects.Add(new HealEffect(percent));
            return this;
        }

        #endregion

        #region Other Effects

        /// <summary>
        /// May cause target to flinch.
        /// </summary>
        public EffectBuilder MayFlinch(int chancePercent)
        {
            _effects.Add(new FlinchEffect(chancePercent));
            return this;
        }

        /// <summary>
        /// Hit multiple times (2-5 by default).
        /// </summary>
        public EffectBuilder MultiHit(int minHits = 2, int maxHits = 5)
        {
            _effects.Add(new MultiHitEffect(minHits, maxHits));
            return this;
        }

        /// <summary>
        /// Hit exactly N times.
        /// </summary>
        public EffectBuilder HitsNTimes(int times)
        {
            _effects.Add(new MultiHitEffect(times));
            return this;
        }

        #endregion

        /// <summary>
        /// Build the list of effects.
        /// </summary>
        public List<IMoveEffect> Build()
        {
            return _effects;
        }
    }
}

