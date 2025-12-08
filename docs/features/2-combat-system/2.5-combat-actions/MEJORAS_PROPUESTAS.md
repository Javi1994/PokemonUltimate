# Propuestas de Mejora: Sistema de Acciones de Combate

> **Objetivo**: Reducir código por acción y aumentar la seguridad del sistema

**Fecha**: 2025-01-XX  
**Sub-Feature**: 2.5: Combat Actions  
**Estado**: Propuestas

---

## 📋 Problemas Identificados

### 1. **Código Repetitivo**

-   Todas las acciones validan `field == null` manualmente
-   Validaciones de `Target.IsEmpty` duplicadas
-   Checks de `view == null` en cada `ExecuteVisual`
-   Patrones similares de retorno (`Enumerable.Empty<BattleAction>()`)

### 2. **Lógica Compleja Embebida**

-   `DamageAction` tiene lógica de Focus Sash/Sturdy directamente en la acción (80+ líneas)
-   Validaciones de negocio mezcladas con la lógica de la acción
-   Difícil de testear y mantener

### 3. **Falta de Seguridad**

-   No hay validación centralizada de parámetros
-   Construcción de acciones puede fallar en runtime
-   No hay garantías de tipo en tiempo de compilación

### 4. **Manejo Inconsistente de Errores**

-   Algunas acciones retornan `Enumerable.Empty<BattleAction>()` cuando fallan
-   Otras lanzan excepciones
-   No hay patrón claro de "acción fallida"

### 5. **Referencias Hardcodeadas a Contenido**

-   Strings hardcodeados para nombres de habilidades (`"Sturdy"`, `"Contrary"`)
-   Strings hardcodeados para nombres de items (`"Focus Sash"`)
-   Strings hardcodeados para nombres de movimientos (potencial)
-   Dependencia de localización (nombres pueden cambiar)
-   Frágil ante cambios de nombres o traducciones
-   No hay validación en tiempo de compilación

**Ejemplos encontrados:**

-   `DamageAction.cs`: `GetByName("Focus Sash")`, `GetByName("Sturdy")`
-   `SwitchInProcessor.cs`: `GetByName("Contrary")`

---

## 🎯 Propuestas de Mejora

### Propuesta 0: Eliminar Referencias Hardcodeadas (CRÍTICA)

**Objetivo**: Eliminar todas las referencias hardcodeadas a habilidades, items y movimientos usando IDs constantes y verificación por comportamiento.

#### Problema Actual

```csharp
// ❌ MAL: String hardcodeado, frágil ante cambios
var focusSashItem = ItemCatalog.GetByName("Focus Sash");
var sturdyAbility = AbilityCatalog.GetByName("Sturdy");
```

**Problemas:**

-   Depende de nombres que pueden cambiar con localización
-   No hay validación en tiempo de compilación
-   Frágil ante refactorizaciones
-   Difícil de mantener

#### Solución: Sistema de IDs Constantes + Verificación por Comportamiento

**Opción A: Usar Propiedades Estáticas del Catálogo (Recomendada)**

```csharp
namespace PokemonUltimate.Combat.Actions.Constants
{
    /// <summary>
    /// Referencias seguras a contenido del juego usando propiedades estáticas.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// </remarks>
    public static class GameContentReferences
    {
        // Items
        public static ItemData FocusSash => ItemCatalog.FocusSash;
        public static ItemData Leftovers => ItemCatalog.Leftovers;
        public static ItemData LifeOrb => ItemCatalog.LifeOrb;

        // Abilities
        public static AbilityData Sturdy => AbilityCatalog.Sturdy;
        public static AbilityData Contrary => AbilityCatalog.Contrary;
        public static AbilityData Intimidate => AbilityCatalog.Intimidate;

        // Moves (si es necesario)
        // public static MoveData Thunderbolt => MoveCatalog.Thunderbolt;
    }
}
```

**Uso:**

```csharp
// ✅ BIEN: Referencia segura, validada en compilación
var focusSashItem = GameContentReferences.FocusSash;
if (target.Pokemon.HeldItem?.Id == focusSashItem.Id)
{
    // ...
}
```

**Ventajas:**

-   Validación en tiempo de compilación
-   Refactoring seguro (IDE detecta cambios)
-   No depende de strings
-   Fácil de mantener

**Opción B: Extension Methods para Verificación por Comportamiento (Más Flexible)**

```csharp
namespace PokemonUltimate.Combat.Extensions
{
    /// <summary>
    /// Extension methods para verificar comportamientos de items y habilidades.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// </remarks>
    public static class AbilityItemExtensions
    {
        /// <summary>
        /// Verifica si el item previene OHKO (Focus Sash).
        /// </summary>
        public static bool PreventsOHKO(this ItemData item)
        {
            if (item == null) return false;

            // Verificar por ID (más seguro que nombre)
            return item.Id == "focus_sash"; // O usar constante

            // Alternativa: Verificar por comportamiento si está en los datos
            // return item.Triggers.HasFlag(ItemTrigger.OnOHKO) && item.IsConsumable;
        }

        /// <summary>
        /// Verifica si la habilidad previene OHKO (Sturdy).
        /// </summary>
        public static bool PreventsOHKO(this AbilityData ability)
        {
            if (ability == null) return false;

            // Verificar por ID
            return ability.Id == "sturdy";

            // Alternativa: Verificar por comportamiento
            // return ability.Effect == AbilityEffect.PreventsOHKO;
        }

        /// <summary>
        /// Verifica si la habilidad invierte cambios de estadísticas (Contrary).
        /// </summary>
        public static bool ReversesStatChanges(this AbilityData ability)
        {
            if (ability == null) return false;
            return ability.Id == "contrary";
        }

        /// <summary>
        /// Verifica si el Pokemon tiene un item específico por ID.
        /// </summary>
        public static bool HasItem(this PokemonInstance pokemon, ItemData item)
        {
            if (pokemon == null || item == null) return false;
            return pokemon.HeldItem?.Id == item.Id;
        }

        /// <summary>
        /// Verifica si el Pokemon tiene una habilidad específica por ID.
        /// </summary>
        public static bool HasAbility(this PokemonInstance pokemon, AbilityData ability)
        {
            if (pokemon == null || ability == null) return false;
            return pokemon.Ability?.Id == ability.Id;
        }
    }
}
```

**Uso:**

```csharp
// ✅ BIEN: Verificación por comportamiento, sin strings
if (target.Pokemon.HeldItem?.PreventsOHKO() == true)
{
    // Procesar Focus Sash
}

if (target.Pokemon.Ability?.PreventsOHKO() == true)
{
    // Procesar Sturdy
}

// O usando referencias seguras
if (target.Pokemon.HasItem(GameContentReferences.FocusSash))
{
    // ...
}
```

**Opción C: Sistema de Traits/Interfaces (Más Complejo, Más Flexible)**

```csharp
namespace PokemonUltimate.Combat.Traits
{
    /// <summary>
    /// Trait que identifica items/habilidades que previenen OHKO.
    /// </summary>
    public interface IOHKOPreventionTrait
    {
        /// <summary>
        /// Verifica si puede prevenir OHKO en las condiciones dadas.
        /// </summary>
        bool CanPreventOHKO(PokemonInstance pokemon, int damage);

        /// <summary>
        /// Calcula el daño modificado después de prevenir OHKO.
        /// </summary>
        int CalculateModifiedDamage(PokemonInstance pokemon, int originalDamage);

        /// <summary>
        /// Genera acciones de reacción (mensajes, consumo de item).
        /// </summary>
        IEnumerable<BattleAction> GenerateReactions(BattleSlot target, ILocalizationProvider localization);
    }

    /// <summary>
    /// Implementación para Focus Sash.
    /// </summary>
    public class FocusSashTrait : IOHKOPreventionTrait
    {
        private static readonly ItemData FocusSash = ItemCatalog.FocusSash;

        public bool CanPreventOHKO(PokemonInstance pokemon, int damage)
        {
            if (pokemon.HeldItem?.Id != FocusSash.Id) return false;
            return pokemon.CurrentHP >= pokemon.MaxHP; // Solo funciona a HP completo
        }

        public int CalculateModifiedDamage(PokemonInstance pokemon, int originalDamage)
        {
            return Math.Max(0, pokemon.CurrentHP - 1);
        }

        public IEnumerable<BattleAction> GenerateReactions(BattleSlot target, ILocalizationProvider localization)
        {
            target.Pokemon.HeldItem = null; // Consumir item

            var provider = localization ?? LocalizationService.Instance;
            var itemName = FocusSash.GetDisplayName(provider) ?? "Focus Sash";

            yield return new MessageAction(provider.GetString(LocalizationKey.ItemActivated, target.Pokemon.DisplayName, itemName));
            yield return new MessageAction(provider.GetString(LocalizationKey.HeldOnUsingItem, target.Pokemon.DisplayName, itemName));
        }
    }

    /// <summary>
    /// Implementación para Sturdy.
    /// </summary>
    public class SturdyTrait : IOHKOPreventionTrait
    {
        private static readonly AbilityData Sturdy = AbilityCatalog.Sturdy;

        public bool CanPreventOHKO(PokemonInstance pokemon, int damage)
        {
            if (pokemon.Ability?.Id != Sturdy.Id) return false;
            return pokemon.CurrentHP >= pokemon.MaxHP; // Solo funciona a HP completo
        }

        public int CalculateModifiedDamage(PokemonInstance pokemon, int originalDamage)
        {
            return Math.Max(0, pokemon.CurrentHP - 1);
        }

        public IEnumerable<BattleAction> GenerateReactions(BattleSlot target, ILocalizationProvider localization)
        {
            var provider = localization ?? LocalizationService.Instance;
            var abilityName = Sturdy.GetDisplayName(provider) ?? "Sturdy";

            yield return new MessageAction(provider.GetString(LocalizationKey.AbilityActivated, target.Pokemon.DisplayName, abilityName));
            yield return new MessageAction(provider.GetString(LocalizationKey.EnduredHit, target.Pokemon.DisplayName));
        }
    }

    /// <summary>
    /// Registry de traits para evitar hardcoding.
    /// </summary>
    public class TraitRegistry
    {
        private readonly Dictionary<string, IOHKOPreventionTrait> _ohkoTraits;

        public TraitRegistry()
        {
            _ohkoTraits = new Dictionary<string, IOHKOPreventionTrait>
            {
                { ItemCatalog.FocusSash.Id, new FocusSashTrait() },
                { AbilityCatalog.Sturdy.Id, new SturdyTrait() }
            };
        }

        public IOHKOPreventionTrait GetOHKOPreventionTrait(PokemonInstance pokemon)
        {
            // Verificar item primero
            if (pokemon.HeldItem != null && _ohkoTraits.TryGetValue(pokemon.HeldItem.Id, out var itemTrait))
            {
                if (itemTrait.CanPreventOHKO(pokemon, 0))
                    return itemTrait;
            }

            // Verificar habilidad
            if (pokemon.Ability != null && _ohkoTraits.TryGetValue(pokemon.Ability.Id, out var abilityTrait))
            {
                if (abilityTrait.CanPreventOHKO(pokemon, 0))
                    return abilityTrait;
            }

            return null;
        }
    }
}
```

**Uso:**

```csharp
// ✅ BIEN: Sin hardcoding, extensible, testeable
var traitRegistry = new TraitRegistry();
var ohkoTrait = traitRegistry.GetOHKOPreventionTrait(target.Pokemon);

if (ohkoTrait != null)
{
    damage = ohkoTrait.CalculateModifiedDamage(target.Pokemon, damage);
    reactions.AddRange(ohkoTrait.GenerateReactions(target, localization));
}
```

#### Recomendación Final

**Para este proyecto, recomiendo: Opción A + Opción C (Behavior Checker Pattern):**

1. **`GameContentReferences`** para referencias seguras a contenido común
2. **Behavior Checker Pattern** para verificaciones de comportamiento complejas
3. **Uso de IDs** en lugar de nombres en todas las comparaciones

**Razones:**

-   ✅ **Consistente con tu arquitectura**: Similar a `MoveEffectProcessorRegistry` que ya usas
-   ✅ **Separación de responsabilidades**: Cada checker maneja un comportamiento específico
-   ✅ **Fácil de extender**: Agregar nuevos checkers es simple (ej: `TypeImmunityChecker`, `StatusImmunityChecker`)
-   ✅ **Testeable**: Cada checker se puede testear independientemente
-   ✅ **Sin hardcoding**: Usa referencias estáticas del catálogo
-   ✅ **Mantenible**: Cambios en comportamiento se hacen en una sola clase
-   ✅ **Reutilizable**: Los checkers se pueden usar en múltiples lugares

**Comparación con otros patrones:**

| Patrón                | Complejidad     | Consistencia    | Extensibilidad      | Mantenibilidad      |
| --------------------- | --------------- | --------------- | ------------------- | ------------------- |
| **Extension Methods** | ⭐⭐⭐⭐⭐ Baja | ⭐⭐⭐ Media    | ⭐⭐⭐⭐ Alta       | ⭐⭐⭐ Media        |
| **Behavior Checker**  | ⭐⭐⭐⭐ Media  | ⭐⭐⭐⭐⭐ Alta | ⭐⭐⭐⭐⭐ Muy Alta | ⭐⭐⭐⭐⭐ Muy Alta |
| **Traits/Interfaces** | ⭐⭐ Alta       | ⭐⭐⭐ Media    | ⭐⭐⭐⭐⭐ Muy Alta | ⭐⭐⭐ Media        |

**El Behavior Checker Pattern es el mejor balance** porque:

-   Es consistente con tu arquitectura existente (Strategy + Registry)
-   Separa verificaciones complejas en clases dedicadas
-   Es más simple que Traits pero más organizado que Extension Methods
-   Sigue el principio de responsabilidad única
-   **Reduce código significativamente**: `UseMoveAction` puede pasar de ~400 líneas a ~250 líneas (37% reducción)

**Checkers adicionales recomendados para movimientos:**

Además de los checkers básicos (`OHKOPreventionChecker`, `StatChangeReversalChecker`), se recomienda crear:

1. **`FocusPunchBehaviorChecker`** - Maneja focusing state y hit checks
2. **`MultiTurnBehaviorChecker`** - Maneja turnos de carga/ataque
3. **`SemiInvulnerableBehaviorChecker`** - Maneja movimientos semi-invulnerables (Dig, Fly, etc.)
4. **`PursuitBehaviorChecker`** - Maneja efecto Pursuit (doble poder si target cambia)
5. **`SpreadMoveBehaviorChecker`** - Maneja movimientos que golpean múltiples targets

Estos checkers eliminan la necesidad de pasar parámetros booleanos (`hasFocusPunchEffect`, `hasMultiTurnEffect`) por todos los métodos y encapsulan la lógica de comportamiento específica.

**Ejemplo: Refactorización de UseMoveAction con Behavior Checkers:**

**Antes (❌):**

```csharp
// Código largo con lógica específica mezclada
bool hasFocusPunchEffect = Move.Effects.Any(e => e is FocusPunchEffect);
var focusPunchResult = ProcessFocusPunchMove(actions, hasFocusPunchEffect);
if (focusPunchResult != null)
    return focusPunchResult;

// ... más código ...

var protectionResult = CheckProtection(actions, hasFocusPunchEffect);
// ... tiene que pasar hasFocusPunchEffect por todos lados ...

private List<BattleAction> ProcessFocusPunchMove(List<BattleAction> actions, bool hasFocusPunchEffect)
{
    if (!hasFocusPunchEffect)
        return null;

    User.AddVolatileStatus(VolatileStatus.Focusing);
    if (User.WasHitWhileFocusing)
    {
        MoveInstance.Use();
        actions.Add(new MessageAction(...));
        User.RemoveVolatileStatus(VolatileStatus.Focusing);
        return actions;
    }
    return null;
}

private void CleanupVolatileStatusesOnFailure(bool hasFocusPunchEffect, bool hasMultiTurnEffect)
{
    if (hasFocusPunchEffect)
        User.RemoveVolatileStatus(VolatileStatus.Focusing);
    if (hasMultiTurnEffect)
    {
        User.RemoveVolatileStatus(VolatileStatus.Charging);
        User.ClearChargingMove();
    }
}
```

**Después (✅):**

```csharp
// Código limpio usando Behavior Checkers
var behaviorRegistry = new BehaviorCheckerRegistry();
var focusPunchChecker = behaviorRegistry.GetFocusPunchChecker();
var multiTurnChecker = behaviorRegistry.GetMultiTurnChecker();

// Procesar Multi-Turn
if (multiTurnChecker.HasMultiTurnBehavior(Move))
{
    if (multiTurnChecker.ProcessMultiTurn(User, Move, MoveInstance, actions))
        return actions; // Turno de carga
}

// Procesar Focus Punch
if (focusPunchChecker.HasFocusPunchBehavior(Move))
{
    if (focusPunchChecker.ProcessFocusPunchStart(User, actions, MoveInstance))
        return actions; // Focus perdido
}

// ... resto del código sin pasar parámetros booleanos ...

// Si falla en cualquier punto
if (moveFailed)
{
    focusPunchChecker.CleanupOnFailure(User);
    multiTurnChecker.CleanupOnFailure(User);
}
```

**Reducción estimada**: De ~400 líneas a ~250 líneas en `UseMoveAction` (37% reducción).

**Checkers de Movimientos Propuestos:**

```csharp
// FocusPunchBehaviorChecker - Maneja focusing state
public class FocusPunchBehaviorChecker
{
    public bool HasFocusPunchBehavior(MoveData move)
    {
        return move?.Effects.Any(e => e is FocusPunchEffect) == true;
    }

    public bool ProcessFocusPunchStart(BattleSlot user, List<BattleAction> actions, MoveInstance moveInstance)
    {
        user.AddVolatileStatus(VolatileStatus.Focusing);
        if (user.WasHitWhileFocusing)
        {
            moveInstance.Use();
            actions.Add(new MessageAction(...));
            user.RemoveVolatileStatus(VolatileStatus.Focusing);
            return true; // Cancelar movimiento
        }
        return false; // Continuar
    }

    public void CleanupOnFailure(BattleSlot user) { /* ... */ }
    public void CleanupOnSuccess(BattleSlot user) { /* ... */ }
}

// MultiTurnBehaviorChecker - Maneja turnos de carga/ataque
public class MultiTurnBehaviorChecker
{
    public bool HasMultiTurnBehavior(MoveData move)
    {
        return move?.Effects.Any(e => e is MultiTurnEffect) == true;
    }

    public bool ProcessMultiTurn(BattleSlot user, MoveData move, MoveInstance moveInstance, List<BattleAction> actions)
    {
        // Lógica de carga/ataque encapsulada
        // Retorna true si es turno de carga (cancelar ejecución)
    }

    public void CleanupOnFailure(BattleSlot user) { /* ... */ }
}
```

**Beneficios específicos para UseMoveAction:**

-   ✅ **Elimina métodos largos**: `ProcessFocusPunchMove`, `ProcessMultiTurnMove` se simplifican
-   ✅ **Elimina parámetros booleanos**: Ya no necesitas pasar `hasFocusPunchEffect` por todos lados
-   ✅ **Código más limpio**: La lógica de comportamiento está encapsulada en checkers
-   ✅ **Fácil de testear**: Cada checker se testea independientemente
-   ✅ **Fácil de extender**: Agregar nuevos comportamientos es simple

**Implementación en OHKOPreventionProcessor usando Behavior Checker:**

```csharp
public class OHKOPreventionProcessor
{
    private readonly ILocalizationProvider _localization;
    private readonly BehaviorCheckerRegistry _behaviorRegistry;

    public OHKOPreventionProcessor(
        ILocalizationProvider localization,
        BehaviorCheckerRegistry behaviorRegistry = null)
    {
        _localization = localization ?? throw new ArgumentNullException(nameof(localization));
        _behaviorRegistry = behaviorRegistry ?? new BehaviorCheckerRegistry();
    }

    public IEnumerable<BattleAction> ProcessOHKOPrevention(
        BattleSlot target,
        int damage,
        BattleField field)
    {
        if (target.IsEmpty || damage == 0)
            return Enumerable.Empty<BattleAction>();

        var pokemon = target.Pokemon;
        bool wouldFaint = pokemon.CurrentHP <= damage;
        bool wasAtFullHP = pokemon.CurrentHP >= pokemon.MaxHP;

        if (!wouldFaint || !wasAtFullHP)
            return Enumerable.Empty<BattleAction>();

        // ✅ Usar Behavior Checker (sin hardcoding)
        var ohkoChecker = _behaviorRegistry.GetOHKOPreventionChecker();

        if (!ohkoChecker.HasBehavior(pokemon))
            return Enumerable.Empty<BattleAction>();

        var reactions = new List<BattleAction>();
        var preventionType = ohkoChecker.GetPreventionType(pokemon);

        // Modificar daño
        damage = ohkoChecker.CalculateModifiedDamage(pokemon, damage);

        // Generar mensajes según el tipo
        if (preventionType == OHKOPreventionType.Item)
        {
            // Focus Sash
            pokemon.HeldItem = null; // Consumir item
            var itemName = GameContentReferences.FocusSash.GetDisplayName(_localization) ?? "Focus Sash";
            reactions.Add(new MessageAction(_localization.GetString(LocalizationKey.ItemActivated, pokemon.DisplayName, itemName)));
            reactions.Add(new MessageAction(_localization.GetString(LocalizationKey.HeldOnUsingItem, pokemon.DisplayName, itemName)));
        }
        else if (preventionType == OHKOPreventionType.Ability)
        {
            // Sturdy
            var abilityName = GameContentReferences.Sturdy.GetDisplayName(_localization) ?? "Sturdy";
            reactions.Add(new MessageAction(_localization.GetString(LocalizationKey.AbilityActivated, pokemon.DisplayName, abilityName)));
            reactions.Add(new MessageAction(_localization.GetString(LocalizationKey.EnduredHit, pokemon.DisplayName)));
        }

        return reactions;
    }
}
```

**Uso en SwitchInProcessor para Contrary:**

```csharp
public class SwitchInProcessor : IActionGeneratingPhaseProcessor
{
    private readonly BehaviorCheckerRegistry _behaviorRegistry;

    public SwitchInProcessor(
        DamageContextFactory damageContextFactory,
        BehaviorCheckerRegistry behaviorRegistry = null)
    {
        _damageContextFactory = damageContextFactory ?? throw new ArgumentNullException(nameof(damageContextFactory));
        _behaviorRegistry = behaviorRegistry ?? new BehaviorCheckerRegistry();
    }

    private void ProcessHazardStatChanges(BattleSlot slot, HazardData hazardData)
    {
        int stages = hazardData.StatStages;

        // ✅ Usar Behavior Checker (sin hardcoding)
        var reversalChecker = _behaviorRegistry.GetStatChangeReversalChecker();
        if (reversalChecker.HasBehavior(slot.Pokemon))
        {
            stages = reversalChecker.ReverseStatChange(slot.Pokemon, stages);
        }

        // Aplicar cambio de stats
        slot.ModifyStatStage(hazardData.TargetStat, stages);
    }
}
```

**Beneficios:**

-   ✅ Cero strings hardcodeados
-   ✅ Validación en compilación
-   ✅ Fácil de extender (agregar nuevos items/habilidades)
-   ✅ Testeable (mock de `GameContentReferences`)
-   ✅ Mantenible (cambios en catálogo se reflejan automáticamente)

---

## 🎯 Propuestas de Mejora

### Propuesta 1: Action Validators (Validadores Reutilizables)

**Objetivo**: Centralizar validaciones comunes y reducir código repetitivo.

#### Implementación

```csharp
namespace PokemonUltimate.Combat.Actions.Validation
{
    /// <summary>
    /// Validadores reutilizables para acciones de combate.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
    /// </remarks>
    public static class ActionValidators
    {
        /// <summary>
        /// Valida que el campo de batalla no sea null.
        /// </summary>
        /// <exception cref="ArgumentNullException">Si field es null.</exception>
        public static void ValidateField(BattleField field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field), ErrorMessages.FieldCannotBeNull);
        }

        /// <summary>
        /// Valida que el slot objetivo no sea null ni vacío.
        /// </summary>
        /// <returns>True si el slot es válido, false si está vacío.</returns>
        /// <exception cref="ArgumentNullException">Si target es null.</exception>
        public static bool ValidateTarget(BattleSlot target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target), ErrorMessages.PokemonCannotBeNull);

            return !target.IsEmpty;
        }

        /// <summary>
        /// Valida que el slot objetivo esté activo.
        /// </summary>
        /// <returns>True si el slot está activo.</returns>
        public static bool ValidateActiveTarget(BattleSlot target)
        {
            if (!ValidateTarget(target))
                return false;

            return target.IsActive();
        }

        /// <summary>
        /// Valida que la vista no sea null.
        /// </summary>
        /// <exception cref="ArgumentNullException">Si view es null.</exception>
        public static void ValidateView(IBattleView view)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));
        }

        /// <summary>
        /// Valida múltiples condiciones y retorna si la acción debe ejecutarse.
        /// </summary>
        public static bool ShouldExecute(BattleField field, BattleSlot target, bool checkActive = false)
        {
            ValidateField(field);

            if (checkActive)
                return ValidateActiveTarget(target);

            return ValidateTarget(target);
        }
    }
}
```

#### Uso en Acciones

**Antes:**

```csharp
public override IEnumerable<BattleAction> ExecuteLogic(BattleField field)
{
    if (field == null)
        throw new ArgumentNullException(nameof(field));

    if (Target.IsEmpty)
        return Enumerable.Empty<BattleAction>();

    // Lógica...
}
```

**Después:**

```csharp
public override IEnumerable<BattleAction> ExecuteLogic(BattleField field)
{
    if (!ActionValidators.ShouldExecute(field, Target))
        return Enumerable.Empty<BattleAction>();

    // Lógica...
}
```

**Reducción**: De ~5 líneas a 2 líneas por acción.

---

### Propuesta 2: Template Method Pattern en BattleAction

**Objetivo**: Reducir código repetitivo usando el patrón Template Method.

#### Implementación

```csharp
public abstract class BattleAction
{
    // ... propiedades existentes ...

    /// <summary>
    /// Template method que ejecuta la lógica con validaciones comunes.
    /// </summary>
    public IEnumerable<BattleAction> ExecuteLogic(BattleField field)
    {
        ActionValidators.ValidateField(field);

        // Validación específica por tipo de acción
        if (!CanExecute(field))
            return Enumerable.Empty<BattleAction>();

        // Ejecutar lógica específica
        return ExecuteLogicCore(field);
    }

    /// <summary>
    /// Determina si la acción puede ejecutarse.
    /// Por defecto retorna true, las acciones pueden sobrescribir.
    /// </summary>
    protected virtual bool CanExecute(BattleField field)
    {
        return true;
    }

    /// <summary>
    /// Lógica específica de la acción (sin validaciones comunes).
    /// </summary>
    protected abstract IEnumerable<BattleAction> ExecuteLogicCore(BattleField field);

    /// <summary>
    /// Template method para la ejecución visual.
    /// </summary>
    public Task ExecuteVisual(IBattleView view)
    {
        ActionValidators.ValidateView(view);

        if (!ShouldShowVisual(view))
            return Task.CompletedTask;

        return ExecuteVisualCore(view);
    }

    /// <summary>
    /// Determina si se debe mostrar la visualización.
    /// </summary>
    protected virtual bool ShouldShowVisual(IBattleView view)
    {
        return true;
    }

    /// <summary>
    /// Lógica visual específica de la acción.
    /// </summary>
    protected abstract Task ExecuteVisualCore(IBattleView view);
}
```

#### Uso en Acciones

**Antes:**

```csharp
public override IEnumerable<BattleAction> ExecuteLogic(BattleField field)
{
    if (field == null)
        throw new ArgumentNullException(nameof(field));

    if (Target.IsEmpty || Amount == 0)
        return Enumerable.Empty<BattleAction>();

    Target.Pokemon.Heal(Amount);
    return Enumerable.Empty<BattleAction>();
}

public override Task ExecuteVisual(IBattleView view)
{
    if (view == null)
        throw new ArgumentNullException(nameof(view));

    if (Target.IsEmpty || Amount == 0)
        return Task.CompletedTask;

    return view.UpdateHPBar(Target);
}
```

**Después:**

```csharp
protected override bool CanExecute(BattleField field)
{
    return ActionValidators.ValidateTarget(Target) && Amount > 0;
}

protected override IEnumerable<BattleAction> ExecuteLogicCore(BattleField field)
{
    Target.Pokemon.Heal(Amount);
    return Enumerable.Empty<BattleAction>();
}

protected override bool ShouldShowVisual(IBattleView view)
{
    return ActionValidators.ValidateTarget(Target) && Amount > 0;
}

protected override Task ExecuteVisualCore(IBattleView view)
{
    return view.UpdateHPBar(Target);
}
```

**Reducción**: Elimina ~8 líneas de validación por acción.

---

### Propuesta 3: Action Builders (Construcción Segura)

**Objetivo**: Garantizar construcción válida de acciones en tiempo de compilación.

#### Implementación

```csharp
namespace PokemonUltimate.Combat.Actions.Builders
{
    /// <summary>
    /// Builder para crear acciones de forma segura y fluida.
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// </remarks>
    public class ActionBuilder
    {
        private BattleSlot _user;
        private BattleSlot _target;
        private BattleField _field; // Para validaciones

        public ActionBuilder WithUser(BattleSlot user)
        {
            _user = user;
            return this;
        }

        public ActionBuilder WithTarget(BattleSlot target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target), ErrorMessages.PokemonCannotBeNull);

            _target = target;
            return this;
        }

        public ActionBuilder ValidateAgainst(BattleField field)
        {
            _field = field;
            return this;
        }

        /// <summary>
        /// Construye una acción de daño.
        /// </summary>
        public DamageAction BuildDamage(DamageContext context)
        {
            if (_target == null)
                throw new InvalidOperationException("Target must be set before building DamageAction");

            if (context == null)
                throw new ArgumentNullException(nameof(context));

            // Validación adicional si hay campo
            if (_field != null && _target.IsEmpty)
                throw new InvalidOperationException("Cannot create DamageAction for empty slot");

            return new DamageAction(_user, _target, context);
        }

        /// <summary>
        /// Construye una acción de curación.
        /// </summary>
        public HealAction BuildHeal(int amount)
        {
            if (_target == null)
                throw new InvalidOperationException("Target must be set before building HealAction");

            if (amount < 0)
                throw new ArgumentException(ErrorMessages.AmountCannotBeNegative, nameof(amount));

            return new HealAction(_user, _target, amount);
        }

        // ... más métodos Build* para cada tipo de acción ...
    }
}
```

#### Uso

**Antes:**

```csharp
var action = new DamageAction(user, target, context);
// Puede fallar en runtime si target es null o está vacío
```

**Después:**

```csharp
var action = new ActionBuilder()
    .WithUser(user)
    .WithTarget(target)
    .ValidateAgainst(field)
    .BuildDamage(context);
// Falla en construcción si hay problemas, más seguro
```

---

### Propuesta 4: Action Result Pattern

**Objetivo**: Manejo explícito de éxito/fallo de acciones.

#### Implementación

```csharp
namespace PokemonUltimate.Combat.Actions.Results
{
    /// <summary>
    /// Resultado de la ejecución de una acción.
    /// </summary>
    public class ActionExecutionResult
    {
        /// <summary>
        /// Indica si la acción se ejecutó exitosamente.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Razón del fallo, si aplica.
        /// </summary>
        public string FailureReason { get; }

        /// <summary>
        /// Acciones de reacción generadas.
        /// </summary>
        public IEnumerable<BattleAction> Reactions { get; }

        private ActionExecutionResult(bool success, string failureReason, IEnumerable<BattleAction> reactions)
        {
            Success = success;
            FailureReason = failureReason;
            Reactions = reactions ?? Enumerable.Empty<BattleAction>();
        }

        public static ActionExecutionResult Successful(IEnumerable<BattleAction> reactions = null)
        {
            return new ActionExecutionResult(true, null, reactions);
        }

        public static ActionExecutionResult Failed(string reason)
        {
            return new ActionExecutionResult(false, reason, Enumerable.Empty<BattleAction>());
        }

        public static ActionExecutionResult Skipped(string reason = "Action skipped")
        {
            return new ActionExecutionResult(false, reason, Enumerable.Empty<BattleAction>());
        }
    }
}
```

#### Uso en BattleAction

```csharp
public abstract class BattleAction
{
    // Método público que retorna resultado explícito
    public ActionExecutionResult ExecuteLogicSafe(BattleField field)
    {
        try
        {
            ActionValidators.ValidateField(field);

            if (!CanExecute(field))
                return ActionExecutionResult.Skipped("Action cannot execute");

            var reactions = ExecuteLogicCore(field);
            return ActionExecutionResult.Successful(reactions);
        }
        catch (Exception ex)
        {
            return ActionExecutionResult.Failed(ex.Message);
        }
    }

    // Método legacy para compatibilidad
    public IEnumerable<BattleAction> ExecuteLogic(BattleField field)
    {
        var result = ExecuteLogicSafe(field);
        return result.Reactions;
    }
}
```

**Ventajas**:

-   Manejo explícito de errores
-   Mejor para logging y debugging
-   Permite decisiones basadas en éxito/fallo

---

### Propuesta 5: Separar Lógica de Negocio (Focus Sash/Sturdy)

**Objetivo**: Mover lógica compleja fuera de las acciones.

#### Implementación

```csharp
namespace PokemonUltimate.Combat.Actions.Processors
{
    /// <summary>
    /// Procesa efectos de OHKO prevention (Focus Sash, Sturdy).
    /// </summary>
    /// <remarks>
    /// **Feature**: 2: Combat System
    /// **Sub-Feature**: 2.5: Combat Actions
    /// </remarks>
    public class OHKOPreventionProcessor
    {
        private readonly ILocalizationProvider _localization;

        public OHKOPreventionProcessor(ILocalizationProvider localization)
        {
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
        }

        /// <summary>
        /// Procesa prevención de OHKO y modifica el daño si es necesario.
        /// </summary>
        /// <returns>Acciones de reacción generadas (mensajes, consumo de items).</returns>
        public IEnumerable<BattleAction> ProcessOHKOPrevention(
            BattleSlot target,
            int damage,
            BattleField field)
        {
            if (target.IsEmpty || damage == 0)
                return Enumerable.Empty<BattleAction>();

            var pokemon = target.Pokemon;
            bool wouldFaint = pokemon.CurrentHP <= damage;
            bool wasAtFullHP = pokemon.CurrentHP >= pokemon.MaxHP;

            if (!wouldFaint || !wasAtFullHP)
                return Enumerable.Empty<BattleAction>();

            var reactions = new List<BattleAction>();

            // Check Focus Sash
            if (TryProcessFocusSash(target, damage, out int newDamage, out var sashReactions))
            {
                reactions.AddRange(sashReactions);
                return reactions; // Focus Sash tiene prioridad
            }

            // Check Sturdy
            if (TryProcessSturdy(target, damage, out newDamage, out var sturdyReactions))
            {
                reactions.AddRange(sturdyReactions);
                return reactions;
            }

            return Enumerable.Empty<BattleAction>();
        }

        private bool TryProcessFocusSash(
            BattleSlot target,
            int damage,
            out int modifiedDamage,
            out IEnumerable<BattleAction> reactions)
        {
            modifiedDamage = damage;
            reactions = Enumerable.Empty<BattleAction>();

            var focusSashItem = ItemCatalog.GetByName("Focus Sash");
            if (focusSashItem == null)
                return false;

            bool hasFocusSash = target.Pokemon.HeldItem != null &&
                               target.Pokemon.HeldItem.Name.Equals(focusSashItem.Name, StringComparison.OrdinalIgnoreCase);

            if (!hasFocusSash)
                return false;

            // Reducir daño a dejar 1 HP
            modifiedDamage = Math.Max(0, target.Pokemon.CurrentHP - 1);

            // Consumir item
            target.Pokemon.HeldItem = null;

            // Generar mensajes
            var provider = LocalizationService.Instance;
            var itemName = focusSashItem.GetDisplayName(provider) ?? "Focus Sash";
            reactions = new[]
            {
                new MessageAction(provider.GetString(LocalizationKey.ItemActivated, target.Pokemon.DisplayName, itemName)),
                new MessageAction(provider.GetString(LocalizationKey.HeldOnUsingItem, target.Pokemon.DisplayName, itemName))
            };

            return true;
        }

        private bool TryProcessSturdy(
            BattleSlot target,
            int damage,
            out int modifiedDamage,
            out IEnumerable<BattleAction> reactions)
        {
            modifiedDamage = damage;
            reactions = Enumerable.Empty<BattleAction>();

            var sturdyAbility = AbilityCatalog.GetByName("Sturdy");
            if (sturdyAbility == null)
                return false;

            bool hasSturdy = target.Pokemon.Ability != null &&
                           target.Pokemon.Ability.Name.Equals(sturdyAbility.Name, StringComparison.OrdinalIgnoreCase);

            if (!hasSturdy)
                return false;

            // Reducir daño a dejar 1 HP
            modifiedDamage = Math.Max(0, target.Pokemon.CurrentHP - 1);

            // Generar mensajes
            var provider = LocalizationService.Instance;
            var abilityName = sturdyAbility.GetDisplayName(provider) ?? "Sturdy";
            reactions = new[]
            {
                new MessageAction(provider.GetString(LocalizationKey.AbilityActivated, target.Pokemon.DisplayName, abilityName)),
                new MessageAction(provider.GetString(LocalizationKey.EnduredHit, target.Pokemon.DisplayName))
            };

            return true;
        }
    }
}
```

#### Uso en DamageAction

**Antes:** (80+ líneas con lógica embebida)

**Después:**

```csharp
public class DamageAction : BattleAction
{
    private readonly OHKOPreventionProcessor _ohkoProcessor;

    public DamageAction(
        BattleSlot user,
        BattleSlot target,
        DamageContext context,
        OHKOPreventionProcessor ohkoProcessor = null) : base(user)
    {
        Target = target ?? throw new ArgumentNullException(nameof(target));
        Context = context ?? throw new ArgumentNullException(nameof(context));
        _ohkoProcessor = ohkoProcessor ?? new OHKOPreventionProcessor(LocalizationService.Instance);
    }

    protected override IEnumerable<BattleAction> ExecuteLogicCore(BattleField field)
    {
        int damage = Context.FinalDamage;

        if (damage == 0)
            return Enumerable.Empty<BattleAction>();

        var reactions = new List<BattleAction>();

        // Procesar prevención de OHKO (Focus Sash, Sturdy)
        var ohkoReactions = _ohkoProcessor.ProcessOHKOPrevention(Target, damage, field);
        reactions.AddRange(ohkoReactions);

        // Aplicar daño (puede haber sido modificado por el procesador)
        int actualDamage = Target.Pokemon.TakeDamage(damage);

        // Registrar daño para Counter/Mirror Coat
        if (actualDamage > 0)
        {
            RecordDamage(actualDamage);
        }

        // Verificar desmayo
        if (Target.Pokemon.IsFainted)
        {
            reactions.Add(new FaintAction(User, Target));
        }

        return reactions;
    }
}
```

**Reducción**: De ~150 líneas a ~40 líneas en `DamageAction`.

---

## 📊 Resumen de Beneficios

| Propuesta                | Reducción de Código         | Mejora de Seguridad | Complejidad |
| ------------------------ | --------------------------- | ------------------- | ----------- |
| **1. Action Validators** | ~3 líneas/acción            | ⭐⭐⭐ Alta         | Baja        |
| **2. Template Method**   | ~8 líneas/acción            | ⭐⭐ Media          | Media       |
| **3. Action Builders**   | 0 líneas (nuevo código)     | ⭐⭐⭐ Alta         | Media       |
| **4. Action Result**     | 0 líneas (nuevo código)     | ⭐⭐ Media          | Baja        |
| **5. OHKO Processor**    | ~110 líneas en DamageAction | ⭐⭐⭐ Alta         | Media       |
| **0. Sin Hardcoding**    | Elimina fragilidad          | ⭐⭐⭐ Alta         | Baja        |

**Total estimado**: Reducción de ~15-20 líneas por acción + separación de lógica compleja + eliminación de referencias hardcodeadas.

---

## 🎯 Plan de Implementación Recomendado

### Fase 0: Eliminar Hardcoding (CRÍTICA - Hacer primero)

1. Crear `GameContentReferences` con referencias estáticas
2. Crear extension methods (`HasItem`, `HasAbility`, etc.)
3. Refactorizar `DamageAction` para usar referencias seguras
4. Refactorizar `SwitchInProcessor` para usar referencias seguras
5. Buscar y eliminar todos los `GetByName()` hardcodeados
6. Validar que no queden strings hardcodeados

**Criterio de éxito**: Cero `GetByName()` con strings literales en código de combate.

### Fase 1: Validadores (Bajo riesgo, alto impacto)

1. Crear `ActionValidators`
2. Refactorizar 2-3 acciones como prueba
3. Validar que tests pasen
4. Refactorizar resto de acciones

### Fase 2: Template Method (Medio riesgo)

1. Modificar `BattleAction` base
2. Refactorizar acciones para usar nuevos métodos
3. Mantener compatibilidad con código existente

### Fase 3: Separar Lógica Compleja (Alto impacto)

1. Crear `OHKOPreventionProcessor`
2. Refactorizar `DamageAction`
3. Tests de integración

### Fase 4: Builders y Result Pattern (Opcional)

1. Implementar `ActionBuilder`
2. Implementar `ActionExecutionResult`
3. Migración gradual

---

## ⚠️ Consideraciones

### Compatibilidad

-   Mantener métodos públicos existentes durante transición
-   Usar métodos `protected` nuevos para evitar breaking changes

### Testing

-   Todos los cambios deben mantener tests existentes
-   Agregar tests para nuevos componentes (Validators, Processors)

### Performance

-   Validadores son métodos estáticos (sin overhead)
-   Template Method tiene overhead mínimo (una llamada extra)

---

## 📝 Próximos Pasos

1. **Revisar propuestas** con el equipo
2. **Priorizar** qué propuestas implementar primero
3. **⚠️ CRÍTICO**: Implementar Fase 0 (Eliminar Hardcoding) primero
4. **Crear issues** para cada fase
5. **Implementar** siguiendo TDD

## 🔍 Búsqueda de Hardcoding

Para encontrar todas las referencias hardcodeadas:

```powershell
# Buscar GetByName con strings literales
grep -r "GetByName(\"" PokemonUltimate.Combat/

# Buscar comparaciones con nombres
grep -r "\.Name.*==.*\"" PokemonUltimate.Combat/
grep -r "\.Name.*Equals.*\"" PokemonUltimate.Combat/

# Buscar strings de contenido común
grep -r "\"Focus Sash\"" PokemonUltimate.Combat/
grep -r "\"Sturdy\"" PokemonUltimate.Combat/
grep -r "\"Contrary\"" PokemonUltimate.Combat/
```

---

**Última actualización**: 2025-01-XX
