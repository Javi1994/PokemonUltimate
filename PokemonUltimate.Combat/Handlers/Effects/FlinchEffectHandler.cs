using System.Collections.Generic;
using PokemonUltimate.Combat.Field;
using PokemonUltimate.Combat.Handlers.Definition;
using PokemonUltimate.Combat.Infrastructure.Providers.Definition;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Effects;
using PokemonUltimate.Core.Data.Effects.Definition;
using PokemonUltimate.Core.Data.Enums;

namespace PokemonUltimate.Combat.Handlers.Effects
{
    /// <summary>
    /// Handler para efectos de flinch (aturdimiento).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/PROCESSOR_REFACTOR_PROPOSAL.md`
    /// </remarks>
    public class FlinchEffectHandler : IMoveEffectHandler
    {
        private readonly IRandomProvider _randomProvider;

        /// <summary>
        /// El tipo de efecto que maneja este handler.
        /// </summary>
        public System.Type EffectType => typeof(FlinchEffect);

        /// <summary>
        /// Crea una nueva instancia del handler de efectos de flinch.
        /// </summary>
        /// <param name="randomProvider">El proveedor de números aleatorios. Si es null, crea uno temporal.</param>
        public FlinchEffectHandler(Infrastructure.Providers.Definition.IRandomProvider randomProvider = null)
        {
            _randomProvider = randomProvider ?? new Infrastructure.Providers.RandomProvider();
        }

        /// <summary>
        /// Verifica si el efecto de flinch puede aplicarse.
        /// </summary>
        public bool CanApply(IMoveEffect effect, BattleSlot user, BattleSlot target, BattleField field)
        {
            if (!(effect is FlinchEffect flinchEffect))
                return false;

            // Verificar probabilidad
            if (_randomProvider.Next(100) >= flinchEffect.ChancePercent)
                return false;

            // El objetivo debe estar activo y no tener Inner Focus
            if (target == null || target.IsEmpty || target.Pokemon.IsFainted)
                return false;

            // Verificar si el objetivo tiene Inner Focus (inmune a flinch)
            // Nota: Esto requeriría acceso al registry, pero por ahora lo dejamos simple
            // En una implementación completa, se verificaría aquí

            return true;
        }

        /// <summary>
        /// Procesa el efecto de flinch y genera acciones.
        /// </summary>
        public List<Actions.BattleAction> Process(
            IMoveEffect effect,
            BattleSlot user,
            BattleSlot target,
            MoveData move,
            BattleField field,
            int damageDealt)
        {
            var actions = new List<Actions.BattleAction>();

            if (!(effect is FlinchEffect flinchEffect))
                return actions;

            // Verificar si puede aplicarse (incluye verificación de probabilidad)
            if (!CanApply(effect, user, target, field))
                return actions;

            // Aplicar flinch directamente al slot (no genera acción, se aplica como estado volátil)
            target.AddVolatileStatus(VolatileStatus.Flinch);

            return actions;
        }

        /// <summary>
        /// Modifica un valor basado en el comportamiento del efecto.
        /// Los efectos de flinch no modifican valores directamente.
        /// </summary>
        public int? ModifyValue(
            IMoveEffect effect,
            int originalValue,
            string valueType,
            BattleSlot user = null,
            BattleSlot target = null)
        {
            // Los efectos de flinch no modifican valores
            return null;
        }
    }
}
