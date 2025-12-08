using System.Collections.Generic;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Handlers.Definition;
using PokemonUltimate.Combat.Infrastructure.Providers.Definition;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Effects;
using PokemonUltimate.Core.Data.Effects.Definition;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Localization.Constants;
using PokemonUltimate.Localization.Services;

namespace PokemonUltimate.Combat.Handlers.Effects
{
    /// <summary>
    /// Handler para efectos de protección (Protect, Detect, etc.).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/PROCESSOR_REFACTOR_PROPOSAL.md`
    /// </remarks>
    public class ProtectEffectHandler : IMoveEffectHandler
    {
        private readonly IRandomProvider _randomProvider;

        /// <summary>
        /// El tipo de efecto que maneja este handler.
        /// </summary>
        public System.Type EffectType => typeof(ProtectEffect);

        /// <summary>
        /// Crea una nueva instancia del handler de efectos de protección.
        /// </summary>
        /// <param name="randomProvider">El proveedor de números aleatorios. Si es null, crea uno temporal.</param>
        public ProtectEffectHandler(Infrastructure.Providers.Definition.IRandomProvider randomProvider = null)
        {
            _randomProvider = randomProvider ?? new Infrastructure.Providers.RandomProvider();
        }

        /// <summary>
        /// Verifica si el efecto de protección puede aplicarse.
        /// </summary>
        public bool CanApply(IMoveEffect effect, BattleSlot user, BattleSlot target, BattleField field)
        {
            if (!(effect is ProtectEffect))
                return false;

            // El usuario debe estar activo
            return user != null && !user.IsEmpty && !user.Pokemon.IsFainted;
        }

        /// <summary>
        /// Procesa el efecto de protección y genera acciones.
        /// </summary>
        public List<BattleAction> Process(
            IMoveEffect effect,
            BattleSlot user,
            BattleSlot target,
            MoveData move,
            BattleField field,
            int damageDealt)
        {
            var actions = new List<BattleAction>();

            if (!(effect is ProtectEffect protectEffect))
                return actions;

            // Verificar si puede aplicarse
            if (!CanApply(effect, user, target, field))
                return actions;

            // Calcular tasa de éxito (se reduce a la mitad con cada uso consecutivo)
            int consecutiveUses = user.ProtectConsecutiveUses;
            int successRate = 100;
            for (int i = 0; i < consecutiveUses; i++)
            {
                successRate /= 2; // Reducir a la mitad cada vez
            }

            // Incrementar contador antes de verificar éxito (rastrea usos consecutivos)
            user.IncrementProtectUses();

            var provider = LocalizationService.Instance;
            if (_randomProvider.Next(100) < successRate)
            {
                // Éxito: aplicar protección
                user.AddVolatileStatus(VolatileStatus.Protected);
                actions.Add(new MessageAction(
                    provider.GetString(LocalizationKey.BattleProtected, user.Pokemon.DisplayName)));
            }
            else
            {
                // Falla: protección no se activa
                actions.Add(new MessageAction(
                    provider.GetString(LocalizationKey.MoveProtectFailed, user.Pokemon.DisplayName)));
            }

            return actions;
        }

        /// <summary>
        /// Modifica un valor basado en el comportamiento del efecto.
        /// Los efectos de protección no modifican valores directamente.
        /// </summary>
        public int? ModifyValue(
            IMoveEffect effect,
            int originalValue,
            string valueType,
            BattleSlot user = null,
            BattleSlot target = null)
        {
            // Los efectos de protección no modifican valores
            return null;
        }
    }
}
