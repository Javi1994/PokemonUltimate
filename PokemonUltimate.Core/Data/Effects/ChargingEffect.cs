using PokemonUltimate.Core.Data.Effects.Definition;
using PokemonUltimate.Core.Data.Enums;


namespace PokemonUltimate.Core.Data.Effects
{
    /// <summary>
    /// Two-turn move with a charging/preparation phase.
    /// Used by Solar Beam, Fly, Dig, Skull Bash, etc.
    /// </summary>
    /// <remarks>
    /// **Feature**: 1: Game Data
    /// **Sub-Feature**: 1.2: Move Data
    /// **Documentation**: See `docs/features/1-game-data/1.2-move-data/architecture.md`
    /// </remarks>
    public class ChargingEffect : IMoveEffect
    {
        public EffectType EffectType => EffectType.Charging;
        public string Description => GetDescription();

        /// <summary>Message shown during charge turn.</summary>
        public string ChargeMessage { get; set; } = "{0} is charging power!";

        /// <summary>Whether user is semi-invulnerable during charge (Fly, Dig, Dive).</summary>
        public bool SemiInvulnerable { get; set; } = false;

        /// <summary>Location during semi-invulnerable state.</summary>
        public SemiInvulnerableState InvulnerableState { get; set; } = SemiInvulnerableState.None;

        /// <summary>Weather that skips the charge turn.</summary>
        public Weather? SkipChargeInWeather { get; set; }

        /// <summary>Item that skips the charge turn (Power Herb).</summary>
        public string SkipChargeWithItem { get; set; } = "Power Herb";

        /// <summary>Whether user gets a Defense boost during charge (Skull Bash).</summary>
        public bool BoostsDefenseOnCharge { get; set; } = false;

        /// <summary>Whether move requires recharge AFTER attacking (Hyper Beam).</summary>
        public bool IsRechargeMove { get; set; } = false;

        public ChargingEffect() { }

        private string GetDescription()
        {
            if (IsRechargeMove)
                return "User must recharge on the next turn.";
            if (SemiInvulnerable)
                return $"User becomes semi-invulnerable, then attacks.";
            return "User charges on first turn, attacks on second.";
        }
    }

    /// <summary>
    /// States for semi-invulnerable moves.
    /// </summary>
    public enum SemiInvulnerableState
    {
        None,
        /// <summary>In the air (Fly, Bounce, Sky Drop).</summary>
        Flying,
        /// <summary>Underground (Dig).</summary>
        Underground,
        /// <summary>Underwater (Dive).</summary>
        Underwater,
        /// <summary>Vanished (Shadow Force, Phantom Force).</summary>
        Vanished,
    }
}

