using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonUltimate.Combat.Actions;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Combat.Damage.Definition;
using PokemonUltimate.Combat.Engine.TurnFlow.Definition;
using PokemonUltimate.Core.Data.Effects;

namespace PokemonUltimate.Combat.Engine.TurnFlow.Steps
{
    /// <summary>
    /// Step que calcula el daño de movimientos usando DamagePipeline explícitamente.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.6: Combat Engine
    /// **Documentation**: See `DECOUPLED_STEPS_PROPOSAL.md`
    /// </remarks>
    public class MoveDamageCalculationStep : ITurnStep
    {
        private readonly IDamagePipeline _damagePipeline;

        public string StepName => "Move Damage Calculation";
        public bool ExecuteEvenIfFainted => false;

        public MoveDamageCalculationStep(IDamagePipeline damagePipeline)
        {
            _damagePipeline = damagePipeline ?? throw new ArgumentNullException(nameof(damagePipeline));
        }

        public async Task<bool> ExecuteAsync(TurnContext context)
        {
            if (context.DamageCalculations == null)
                context.DamageCalculations = new System.Collections.Generic.Dictionary<UseMoveAction, DamageContext>();

            var moveActions = context.SortedActions.OfType<UseMoveAction>()
                .Where(ma => context.MoveValidations?.GetValueOrDefault(ma, true) == true &&
                             !context.ProtectionChecks?.GetValueOrDefault(ma, false) == true &&
                             context.AccuracyChecks?.GetValueOrDefault(ma, true) == true &&
                             ma.Move.Effects.Any(e => e is DamageEffect));

            foreach (var moveAction in moveActions)
            {
                context.Logger?.LogDebug(
                    $"Calculating damage for {moveAction.Move.Name} " +
                    $"({moveAction.User.Pokemon?.DisplayName} → {moveAction.Target.Pokemon?.DisplayName})");

                var damageContext = _damagePipeline.Calculate(
                    moveAction.User,
                    moveAction.Target,
                    moveAction.Move,
                    context.Field);

                context.DamageCalculations[moveAction] = damageContext;

                context.Logger?.LogDebug(
                    $"Damage calculated: {damageContext.FinalDamage} " +
                    $"(Base: {damageContext.BaseDamage}, Multiplier: {damageContext.Multiplier:F2}x, " +
                    $"Type: {damageContext.TypeEffectiveness:F2}x)");
            }

            return await Task.FromResult(true);
        }
    }
}
