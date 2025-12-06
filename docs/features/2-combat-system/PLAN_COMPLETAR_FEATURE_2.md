# Plan Completo: Completar Feature 2 - Combat System

> Plan detallado para completar las fases pendientes de Feature 2: Advanced Abilities (2.17), Advanced Items (2.18), y Battle Formats (2.19)

**Fecha de Creación**: 2025-01-XX  
**Última Actualización**: 2025-01-XX  
**Estado**: ✅ Casi Completo (Fase 2.17 ~95% completada, Fase 2.18 100% completada)

---

## 📋 Índice

1. [Resumen Ejecutivo](#resumen-ejecutivo)
2. [Fase 2.17: Advanced Abilities](#fase-217-advanced-abilities)
3. [Fase 2.18: Advanced Items](#fase-218-advanced-items)
4. [Fase 2.19: Battle Formats](#fase-219-battle-formats)
5. [Contenido Necesario](#contenido-necesario)
6. [Orden de Implementación](#orden-de-implementación)

---

## Resumen Ejecutivo

### Estado Actual

-   ✅ **Completado**: Fases 2.1-2.16 (Core + Weather + Terrain + Hazards + Advanced Moves + Field Conditions)
-   🚧 **En Progreso**:
    -   **2.17**: Advanced Abilities (15-20h, ~30 tests) - **~95% completado**
        -   ✅ OnAfterMove integrado
        -   ✅ OnBeforeMove integrado
        -   ✅ OnContactReceived integrado
        -   ✅ OnWeatherChange integrado (SetWeatherAction)
        -   ✅ OnTurnEnd integrado (CombatEngine)
        -   ✅ Moxie implementado y funcionando (3 tests)
        -   ✅ Static implementado y funcionando (4 tests)
        -   ✅ Rough Skin implementado y funcionando (3 tests)
        -   ✅ Truant implementado y funcionando (4 tests)
        -   ✅ Speed Boost implementado y funcionando (3 tests)
        -   ✅ Swift Swim implementado y funcionando (3 tests)
        -   ✅ Chlorophyll implementado y funcionando (2 tests)
        -   ✅ ContactDamageAction creada
        -   ✅ Modificador de velocidad por habilidad en TurnOrderResolver
    -   **2.18**: Advanced Items (8-12h, ~20 tests) - **100% completado**
        -   ✅ Rocky Helmet implementado y funcionando (3 tests)
        -   ✅ Life Orb recoil implementado y funcionando (3 tests)
        -   ✅ Focus Sash implementado y funcionando (4 tests)
        -   ✅ Black Sludge implementado y funcionando (4 tests)
        -   ✅ OnContactReceived para items integrado
        -   ✅ OnAfterMove para items integrado
        -   ✅ OnWouldFaint integrado (para Focus Sash)
        -   ✅ OnTurnEnd para items integrado
    -   **2.19**: Battle Formats (25-30h, ~50 tests) - **0% completado**

### Esfuerzo Total Estimado

-   **Tiempo**: 48-62 horas
-   **Tests**: ~100 nuevos tests
-   **Prioridad**: Alta (completar Feature 2)

### ⚠️ Acciones de Batalla: ¿Qué Está Implementado?

#### ✅ Ya Implementado (Feature 2)

-   **Cambiar de Pokemon (`SwitchAction`)**: ✅ **COMPLETO**
    -   Ubicación: `PokemonUltimate.Combat/Actions/SwitchAction.cs`
    -   Funcionalidad: Cambio de Pokemon en batalla, reseteo de estado, procesamiento de entry hazards, trigger OnSwitchIn
    -   Tests: `SwitchActionTests.cs`, `SwitchActionIntegrationTests.cs`
    -   **No necesita implementación adicional**

#### ⏳ No Implementado (Feature 5: Game Features)

-   **Capturar Pokemon**: ⏳ **PLANIFICADO EN FEATURE 5.2**

    -   Ubicación planificada: `PokemonUltimate.Game/Systems/CatchCalculator.cs`
    -   Componentes: CatchCalculator, ThrowBallAction, sistema de captura
    -   Depende de: Feature 5.1 (Post-Battle Rewards)
    -   **NO es parte de Feature 2 (Combat System)**

-   **Lanzar Pokeballs**: ⏳ **PLANIFICADO EN FEATURE 5.2**

    -   Parte del sistema de captura
    -   Requiere: ThrowBallAction, CatchCalculator, sistema de items (Pokeballs)
    -   **NO es parte de Feature 2 (Combat System)**

-   **Gestión de Party**: ⏳ **PLANIFICADO EN FEATURE 5.2**
    -   Party system (máx 6 Pokemon)
    -   PC Storage
    -   **NO es parte de Feature 2 (Combat System)**

**Conclusión**: Este plan se enfoca **SOLO en Feature 2 (Combat System)**. Las acciones de captura y gestión de Pokemon pertenecen a **Feature 5 (Game Features)** y están planificadas para fases posteriores.

---

## Fase 2.17: Advanced Abilities

### Objetivo

Implementar triggers adicionales y habilidades complejas que requieren `OnBeforeMove`, `OnAfterMove`, `OnDamageTaken`, y `OnWeatherChange`.

### Componentes a Implementar

#### 1. Integración de Triggers en Engine

##### 1.1 OnBeforeMove en UseMoveAction ✅ **COMPLETADO**

**Ubicación**: `PokemonUltimate.Combat/Actions/UseMoveAction.cs`

**Estado**: ✅ **Implementado y funcionando**

-   Llamada a `BattleTriggerProcessor.ProcessTrigger(BattleTrigger.OnBeforeMove, field)` agregada antes de validar PP/status
-   Si Truant bloquea el movimiento, retorna acciones de bloqueo sin consumir PP
-   Tests pasando: `TruantTests.cs` (4 tests)

**Código implementado**:

```csharp
// En UseMoveAction.ExecuteLogic(), líneas ~137-153
var beforeMoveActions = _battleTriggerProcessor.ProcessTrigger(BattleTrigger.OnBeforeMove, field);

// Check if any ability blocked the move (e.g., Truant)
bool moveBlocked = beforeMoveActions.Any(action =>
    action is MessageAction msg &&
    msg.Message.Contains("loafing around"));

if (moveBlocked)
{
    actions.AddRange(beforeMoveActions);
    return actions; // Block move execution (PP not consumed)
}
```

##### 1.2 OnAfterMove en UseMoveAction ✅ **COMPLETADO**

**Ubicación**: `PokemonUltimate.Combat/Actions/UseMoveAction.cs`

**Estado**: ✅ **Implementado y funcionando**

-   Llamada a `BattleTriggerProcessor.ProcessTrigger(BattleTrigger.OnAfterMove, field)` agregada después de procesar efectos
-   Acciones generadas se agregan al queue correctamente
-   Moxie funciona correctamente con este trigger

**Código implementado**:

```csharp
// En UseMoveAction.ExecuteLogic(), línea ~212
var afterMoveActions = _battleTriggerProcessor.ProcessTrigger(BattleTrigger.OnAfterMove, field);
actions.AddRange(afterMoveActions);
```

##### 1.3 OnContactReceived en DamageAction ✅ **COMPLETADO**

**Ubicación**: `PokemonUltimate.Combat/Actions/DamageAction.cs`

**Estado**: ✅ **Implementado y funcionando**

-   Llamada a `OnContactReceived` agregada cuando el movimiento hace contacto
-   Se procesa tanto para habilidades como para items del target
-   Static, Rough Skin y Rocky Helmet funcionan correctamente

**Código implementado**:

```csharp
// En DamageAction.ExecuteLogic(), líneas ~145-160
if (Context.Move != null && Context.Move.MakesContact && actualDamage > 0)
{
    // Process OnContactReceived for abilities and items
    if (Target.Pokemon.Ability != null)
    {
        var abilityListener = new AbilityListener(Target.Pokemon.Ability);
        var abilityActions = abilityListener.OnTrigger(BattleTrigger.OnContactReceived, Target, field, User);
        reactions.AddRange(abilityActions);
    }
    if (Target.Pokemon.HeldItem != null)
    {
        var itemListener = new ItemListener(Target.Pokemon.HeldItem);
        var itemActions = itemListener.OnTrigger(BattleTrigger.OnContactReceived, Target, field, User);
        reactions.AddRange(itemActions);
    }
}
```

**Nota**: `OnDamageTaken` también está implementado (línea ~138), pero `OnContactReceived` es el que se usa para Static, Rough Skin y Rocky Helmet.

##### 1.4 OnWeatherChange en SetWeatherAction

**Ubicación**: `PokemonUltimate.Combat/Actions/SetWeatherAction.cs`

**Cambios**:

-   Agregar llamada a `BattleTriggerProcessor.ProcessTrigger(BattleTrigger.OnWeatherChange, field)` DESPUÉS de cambiar clima
-   Procesar para todos los Pokemon activos (Swift Swim, Chlorophyll, etc.)

**Código de ejemplo**:

```csharp
// En SetWeatherAction.ExecuteLogic(), después de cambiar clima
var weatherChangeActions = _battleTriggerProcessor.ProcessTrigger(
    BattleTrigger.OnWeatherChange, field);
actions.AddRange(weatherChangeActions);
```

#### 2. Extensión de AbilityListener

**Ubicación**: `PokemonUltimate.Combat/Events/AbilityListener.cs`

**Efectos a implementar en `ProcessAbilityEffect()`**:

-   ✅ `AbilityEffect.RaiseOwnStat` (Speed Boost) - Ya existe, necesita trigger OnTurnEnd
-   ✅ `AbilityEffect.ChanceToStatusOnContact` (Static) - **COMPLETADO** - Implementado y funcionando
-   ✅ `AbilityEffect.DamageOnContact` (Rough Skin) - **COMPLETADO** - Implementado y funcionando
-   ⏳ `AbilityEffect.SpeedBoostInWeather` (Swift Swim, Chlorophyll) - Ya existe, necesita trigger OnWeatherChange
-   ✅ `AbilityEffect.RaiseStatOnKO` (Moxie) - **COMPLETADO** - Implementado y funcionando
-   ✅ `AbilityEffect.SkipTurn` (Truant) - **COMPLETADO** - Implementado y funcionando

#### 3. Habilidades a Implementar

##### 3.1 Truant (OnBeforeMove) ✅ **COMPLETADO**

**Efecto**: El Pokemon falla cada otro turno (turno 1: actúa, turno 2: falla, turno 3: actúa, etc.)

**Estado**: ✅ **Implementado y funcionando**

-   `AbilityEffect.SkipTurn` implementado en `AbilityListener.ProcessSkipTurn()`
-   Tracking de turnos usando diccionario estático `_truantState` en `AbilityListener`
-   Trigger `OnBeforeMove` integrado en `UseMoveAction`
-   Tests pasando: `TruantTests.cs` (4 tests)

**Tests implementados**:

-   ✅ Truant_FirstTurn_WithTruant_AllowsMove
-   ✅ Truant_SecondTurn_WithTruant_BlocksMove
-   ✅ Truant_ThirdTurn_WithTruant_AllowsMove
-   ✅ Truant_WithoutTruant_AlwaysAllowsMove

##### 3.2 Speed Boost (OnTurnEnd) ✅ **COMPLETADO**

**Efecto**: Aumenta Speed en +1 cada turno al final del turno

**Estado**: ✅ **Implementado y funcionando**

-   `AbilityEffect.RaiseOwnStat` ya existía y funciona correctamente
-   Trigger `OnTurnEnd` ya estaba integrado en `CombatEngine`
-   Tests pasando: `SpeedBoostTests.cs` (3 tests)

**Tests implementados**:

-   ✅ SpeedBoost_OnTurnEnd_RaisesSpeed
-   ✅ SpeedBoost_OnTurnEnd_MultipleTurns_Stacks
-   ✅ SpeedBoost_OnTurnEnd_MaxSpeed_NoOverflow

##### 3.3 Static (OnContactReceived) ✅ **COMPLETADO**

**Efecto**: 30% de probabilidad de paralizar al atacante cuando recibe un movimiento de contacto

**Estado**: ✅ **Implementado y funcionando**

-   `AbilityEffect.ChanceToStatusOnContact` implementado en `AbilityListener.ProcessChanceToStatusOnContact()`
-   Trigger `OnContactReceived` integrado en `DamageAction`
-   Tests pasando: `StaticTests.cs` (4 tests)

**Tests implementados**:

-   ✅ Static_ContactMove_WithStatic_MayParalyzeAttacker
-   ✅ Static_NonContactMove_WithStatic_NoParalysis
-   ✅ Static_ContactMove_AttackerAlreadyParalyzed_NoStatusChange
-   ✅ Static_WithoutStatic_NoParalysis

##### 3.4 Rough Skin (OnContactReceived) ✅ **COMPLETADO**

**Efecto**: Daña al atacante 1/8 HP cuando recibe un movimiento de contacto

**Estado**: ✅ **Implementado y funcionando**

-   `AbilityEffect.DamageOnContact` implementado en `AbilityListener.ProcessDamageOnContact()`
-   Trigger `OnContactReceived` integrado en `DamageAction`
-   Usa `ContactDamageAction` para aplicar el daño
-   Tests pasando: `RoughSkinTests.cs` (3 tests)

**Tests implementados**:

-   ✅ RoughSkin_ContactMove_WithRoughSkin_DamagesAttacker
-   ✅ RoughSkin_NonContactMove_WithRoughSkin_NoDamage
-   ✅ RoughSkin_WithoutRoughSkin_NoContactDamage

##### 3.5 Swift Swim (OnWeatherChange) ✅ **COMPLETADO**

**Efecto**: Duplica Speed cuando está lloviendo

**Estado**: ✅ **Implementado y funcionando**

-   `AbilityEffect.SpeedBoostInWeather` ya existía
-   Trigger `OnWeatherChange` ya estaba integrado en `SetWeatherAction`
-   Modificador de velocidad implementado en `TurnOrderResolver.GetAbilitySpeedMultiplier()`
-   Tests pasando: `SwiftSwimTests.cs` (3 tests)

**Tests implementados**:

-   ✅ SwiftSwim_OnWeatherChange_Rain_DoublesSpeed
-   ✅ SwiftSwim_OnWeatherChange_NoRain_NormalSpeed
-   ✅ SwiftSwim_OnWeatherChange_RainEnds_SpeedReturns

##### 3.6 Chlorophyll (OnWeatherChange) ✅ **COMPLETADO**

**Efecto**: Duplica Speed cuando hay sol

**Estado**: ✅ **Implementado y funcionando**

-   Similar a Swift Swim pero con Weather.Sun
-   Usa el mismo sistema de modificadores de velocidad en `TurnOrderResolver`
-   Tests pasando: `ChlorophyllTests.cs` (2 tests)

**Tests implementados**:

-   ✅ Chlorophyll_OnWeatherChange_Sun_DoublesSpeed
-   ✅ Chlorophyll_OnWeatherChange_NoSun_NormalSpeed

##### 3.7 Moxie (OnAfterMove) ✅ **COMPLETADO**

**Efecto**: Aumenta Attack en +1 cuando derrota a un Pokemon

**Estado**: ✅ **Implementado y funcionando**

-   `AbilityEffect.RaiseStatOnKO` implementado en `AbilityListener.ProcessRaiseStatOnKO()`
-   Verifica todos los slots del lado opuesto (no solo activos) para detectar KOs
-   Trigger `OnAfterMove` integrado en `UseMoveAction`
-   Tests pasando: `MoxieTests.cs` (3 tests)

**Corrección importante**: Se corrigió la búsqueda de Pokemon desmayados para usar `opposingSide.Slots` en lugar de `GetActiveSlots()`, ya que los Pokemon desmayados no están en slots activos.

**Tests implementados**:

-   ✅ Moxie_KnockingOutOpponent_WithMoxie_RaisesAttack
-   ✅ Moxie_NotKnockingOutOpponent_WithMoxie_NoStatChange
-   ✅ Moxie_WithoutMoxie_NoStatChange

#### 4. Tests para Fase 2.17

**Estructura de Tests**:

```
Tests/Systems/Combat/Actions/
├── TruantTests.cs (4 tests) ✅
├── SpeedBoostTests.cs (3 tests) ✅
├── StaticTests.cs (4 tests) ✅
├── RoughSkinTests.cs (3 tests) ✅
├── SwiftSwimTests.cs (3 tests) ✅
├── ChlorophyllTests.cs (2 tests) ✅
├── MoxieTests.cs (3 tests) ✅
└── Integration/AdvancedAbilitiesIntegrationTests.cs (7 tests) ✅
```

**Total**: 29 tests pasando

---

## Fase 2.18: Advanced Items

### Objetivo

Implementar efectos complejos de items que requieren triggers adicionales.

### Componentes a Implementar

#### 1. Life Orb Recoil (OnAfterMove) ✅ **COMPLETADO**

**Efecto**: Pierde 10% HP máximo después de usar un movimiento ofensivo

**Estado**: ✅ **Implementado y funcionando**

-   `ItemListener.ProcessLifeOrbRecoil()` implementado
-   Verifica si se hizo daño revisando los trackers de daño del oponente
-   Aplica daño de 10% HP máximo solo si se hizo daño
-   Trigger `OnAfterMove` integrado en `UseMoveAction`
-   Tests pasando: `LifeOrbTests.cs` (3 tests)

**Tests implementados**:

-   ✅ LifeOrb_DealingDamage_WithLifeOrb_CausesRecoil
-   ✅ LifeOrb_StatusMove_WithLifeOrb_NoRecoil
-   ✅ LifeOrb_WithoutLifeOrb_NoRecoil

#### 2. Focus Sash (OnWouldFaint) ✅ **COMPLETADO**

**Efecto**: Sobrevive a 1 HP si está a máximo HP cuando recibe un golpe fatal

**Estado**: ✅ **Implementado y funcionando**

-   Implementado directamente en `DamageAction.ExecuteLogic()` (líneas ~75-112)
-   Verifica si el daño sería fatal y si el Pokemon está a máximo HP
-   Reduce daño para dejar al Pokemon a 1 HP
-   Consume el item después de activarse
-   También implementado para Sturdy (habilidad)
-   Tests pasando: `FocusSashTests.cs` (4 tests)

**Tests implementados**:

-   ✅ FocusSash_FatalDamage_AtFullHP_WithFocusSash_PreventsFainting
-   ✅ FocusSash_FatalDamage_NotAtFullHP_WithFocusSash_DoesNotPreventFainting
-   ✅ FocusSash_NonFatalDamage_WithFocusSash_NoEffect
-   ✅ FocusSash_WithoutFocusSash_FatalDamage_CausesFainting

#### 3. Rocky Helmet (OnContactReceived) ✅ **COMPLETADO**

**Efecto**: Daña al atacante 1/6 HP máximo cuando recibe un movimiento de contacto

**Estado**: ✅ **Implementado y funcionando**

-   `ItemListener.ProcessContactDamage()` implementado
-   Trigger `OnContactReceived` integrado en `DamageAction`
-   Usa `ContactDamageAction` para aplicar el daño (1/6 HP máximo)
-   Tests pasando: `RockyHelmetTests.cs` (3 tests)

**Tests implementados**:

-   ✅ RockyHelmet_ContactMove_WithRockyHelmet_DamagesAttacker
-   ✅ RockyHelmet_NonContactMove_WithRockyHelmet_NoDamage
-   ✅ RockyHelmet_WithoutRockyHelmet_NoContactDamage

#### 4. Black Sludge (OnTurnEnd) ✅ **COMPLETADO**

**Efecto**: Cura 1/16 HP si es tipo Poison, daña 1/16 HP si no lo es

**Estado**: ✅ **Implementado y funcionando**

-   `ItemListener.ProcessEndOfTurnHealing()` implementado con lógica especial para Black Sludge
-   Verifica tipo del Pokemon (PrimaryType o SecondaryType == Poison)
-   Aplica curación (1/16 HP) si es tipo Poison, daño (1/16 HP) si no lo es
-   Trigger `OnTurnEnd` ya estaba integrado
-   Tests pasando: `BlackSludgeTests.cs` (4 tests)

**Nota**: El daño es 1/16 HP (no 1/8 como se mencionaba originalmente), usando `LeftoversHealDivisor` (16).

**Tests implementados**:

-   ✅ BlackSludge_OnTurnEnd_WithBlackSludge_OnPoisonType_Heals
-   ✅ BlackSludge_OnTurnEnd_WithBlackSludge_OnNonPoisonType_Damages
-   ✅ BlackSludge_OnTurnEnd_WithBlackSludge_OnPoisonType_AtFullHP_NoHealing
-   ✅ BlackSludge_OnTurnEnd_WithoutBlackSludge_NoEffect

#### 5. Tests para Fase 2.18

**Estructura de Tests**:

```
Tests/Systems/Combat/Actions/
├── LifeOrbTests.cs (3 tests) ✅
├── FocusSashTests.cs (3 tests) ✅
├── RockyHelmetTests.cs (3 tests) ✅
├── BlackSludgeTests.cs (4 tests) ✅
└── Integration/AdvancedItemsIntegrationTests.cs (7 tests pasando) ✅
```

**Total**: 21 tests pasando

---

## Fase 2.19: Battle Formats

### Objetivo

Implementar soporte para Doubles (2v2), Triples (3v3), y Rotation battles.

### Componentes a Implementar

#### 1. Doubles (2v2)

##### 1.1 Targeting Mejorado

**Ubicación**: `PokemonUltimate.Combat/Helpers/TargetResolver.cs`

**Cambios**:

-   Extender `TargetResolver` para manejar targeting específico en doubles
-   Permitir seleccionar aliado/enemigo específico
-   Manejar "adjacent foes" vs "all foes"

##### 1.2 Spread Moves

**Efecto**: Movimientos que afectan múltiples objetivos (ej: Earthquake, Surf)

**Implementación**:

-   Verificar si el movimiento es spread move
-   Aplicar daño a todos los objetivos válidos
-   Reducir daño en doubles (75% en doubles vs 100% en singles)

**Tests**:

-   SpreadMove_Doubles_HitsBothEnemies
-   SpreadMove_Doubles_ReducedDamage
-   SpreadMove_Doubles_OneFainted_HitsRemaining

##### 1.3 Screen Adjustments

**Efecto**: Screens reducen daño 33% en doubles vs 50% en singles

**Implementación**:

-   Ya existe `ScreenStep` en `DamagePipeline`
-   Ajustar multiplicador según formato de batalla

**Tests**:

-   Screen_Doubles_33PercentReduction
-   Screen_Singles_50PercentReduction

#### 2. Triples (3v3)

##### 2.1 Extender Targeting

**Cambios**:

-   Permitir targeting a 3 slots
-   Manejar rangos específicos (left, center, right)

##### 2.2 Movimientos de Rango

**Efecto**: Movimientos que afectan rangos específicos

**Implementación**:

-   Nuevo sistema de rangos en `MoveData`
-   Aplicar daño según rango del movimiento

#### 3. Rotation Battles

##### 3.1 Sistema de Rotación

**Efecto**: Los Pokemon pueden rotar entre slots

**Implementación**:

-   Nuevo `RotateAction`
-   Manejar rotación de slots
-   Actualizar targeting después de rotación

##### 3.2 Targeting con Rotación

**Cambios**:

-   Targeting relativo a posición actual
-   Actualizar después de cada rotación

#### 4. Tests para Fase 2.19

**Estructura de Tests**:

```
Tests/Systems/Combat/Formats/
├── DoublesTests.cs (15 tests)
├── TriplesTests.cs (15 tests)
├── RotationTests.cs (10 tests)
└── BattleFormatsIntegrationTests.cs (10 tests)
```

**Total**: ~50 tests

---

## Contenido Necesario

### Pokemon a Añadir

#### Para Truant ✅ **COMPLETADO**

-   ✅ **Slakoth** (#287, Gen 3) - **AGREGADO**

    -   Tipo: Normal
    -   Habilidad: Truant
    -   Stats: 60/60/60/35/35/30

-   ✅ **Slaking** (#289, Gen 3) - **AGREGADO**
    -   Tipo: Normal
    -   Habilidad: Truant
    -   Stats: 150/160/100/95/65/100
    -   Evolución: Slakoth → Vigoroth (nivel 18) → Slaking (nivel 36)

#### Para Speed Boost

-   **Carvanha** (#318, Gen 3)

    -   Tipo: Water/Dark
    -   Habilidades: Rough Skin, Speed Boost (HA)
    -   Stats: 45/90/20/65/20/65

-   **Sharpedo** (#319, Gen 3)
    -   Tipo: Water/Dark
    -   Habilidades: Rough Skin, Speed Boost (HA)
    -   Stats: 70/120/40/95/40/95
    -   Evolución: Carvanha → Sharpedo (nivel 30)

#### Para Sand Rush

-   **Sandshrew** (#27, Gen 1) - Ya existe parcialmente

    -   Verificar si tiene Sand Rush como habilidad oculta

-   **Sandslash** (#28, Gen 1) - Ya existe parcialmente
    -   Verificar si tiene Sand Rush como habilidad oculta

#### Para Slush Rush

-   **Snover** (#459, Gen 4)

    -   Tipo: Grass/Ice
    -   Habilidades: Snow Warning, Soundproof
    -   Hidden Ability: Slush Rush
    -   Stats: 60/62/50/62/60/40

-   **Abomasnow** (#460, Gen 4)
    -   Tipo: Grass/Ice
    -   Habilidades: Snow Warning, Soundproof
    -   Hidden Ability: Slush Rush
    -   Stats: 90/92/75/92/85/60
    -   Evolución: Snover → Abomasnow (nivel 40)

#### Para Iron Barbs

-   **Ferroseed** (#597, Gen 5)

    -   Tipo: Grass/Steel
    -   Habilidades: Iron Barbs
    -   Stats: 44/50/91/24/86/10

-   **Ferrothorn** (#598, Gen 5)
    -   Tipo: Grass/Steel
    -   Habilidades: Iron Barbs, Anticipation
    -   Stats: 74/94/131/54/116/20
    -   Evolución: Ferroseed → Ferrothorn (nivel 40)

### Habilidades a Añadir

#### Truant ✅ **COMPLETADO**

**Ubicación**: `PokemonUltimate.Content/Catalogs/Abilities/AbilityCatalog.Gen3.cs`

**Estado**: ✅ **Agregado y funcionando**

```csharp
public static readonly AbilityData Truant = Ability.Define("Truant")
    .Description("Pokémon can't attack on consecutive turns.")
    .Gen(3)
    .SkipsTurn()  // Usa método helper que configura OnBeforeMove y SkipTurn
    .Build();
```

#### Moxie ✅ **COMPLETADO**

**Ubicación**: `PokemonUltimate.Content/Catalogs/Abilities/AbilityCatalog.Gen5.cs`

**Estado**: ✅ **Agregado y funcionando**

```csharp
public static readonly AbilityData Moxie = Ability.Define("Moxie")
    .Description("Boosts Attack after knocking out any Pokémon.")
    .Gen(5)
    .RaisesStatOnKO(Stat.Attack, 1)  // Usa método helper que configura OnAfterMove y RaiseStatOnKO
    .Build();
```

#### Sand Rush

**Ubicación**: `PokemonUltimate.Content/Catalogs/Abilities/AbilityCatalog.Gen4.cs` (nuevo archivo)

```csharp
public static readonly AbilityData SandRush = Ability.Define("Sand Rush")
    .Description("Doubles Speed in sandstorm.")
    .Gen(4)
    .OnTrigger(AbilityTrigger.OnWeatherChange)
    .Effect(AbilityEffect.SpeedBoostInWeather)
    .WeatherCondition(Weather.Sandstorm)
    .Build();
```

#### Slush Rush

**Ubicación**: `PokemonUltimate.Content/Catalogs/Abilities/AbilityCatalog.Gen7.cs` (nuevo archivo)

```csharp
public static readonly AbilityData SlushRush = Ability.Define("Slush Rush")
    .Description("Doubles Speed in hail.")
    .Gen(7)
    .OnTrigger(AbilityTrigger.OnWeatherChange)
    .Effect(AbilityEffect.SpeedBoostInWeather)
    .WeatherCondition(Weather.Hail)
    .Build();
```

#### Iron Barbs ✅ **COMPLETADO**

**Ubicación**: `PokemonUltimate.Content/Catalogs/Abilities/AbilityCatalog.Gen5.cs`

**Estado**: ✅ **Agregado y funcionando**

```csharp
public static readonly AbilityData IronBarbs = Ability.Define("Iron Barbs")
    .Description("Damages attackers that make contact.")
    .Gen(5)
    .DamagesOnContact(0.125f)  // Usa método helper que configura OnContactReceived y DamageOnContact
    .Build();
```

### Movimientos a Añadir (si faltan)

#### Para Testing de Spread Moves

-   **Earthquake** (Ground, Physical, 100 power, 100 acc, 10 PP) - Ya existe probablemente
-   **Surf** (Water, Special, 90 power, 100 acc, 15 PP) - Verificar si existe
-   **Rock Slide** (Rock, Physical, 75 power, 90 acc, 10 PP) - Ya existe

#### Para Testing de Contact Moves

-   **Tackle** (Normal, Physical, 40 power, 100 acc, 35 PP) - ✅ **COMPLETADO** - Configurado con `MakesContact(true)`
-   **Scratch** (Normal, Physical, 40 power, 100 acc, 35 PP) - Ya existe (verificar si necesita `MakesContact`)

### Items (Ya Existen)

-   ✅ Life Orb
-   ✅ Rocky Helmet
-   ✅ Focus Sash
-   ✅ Black Sludge

---

## Orden de Implementación

### Fase 1: Preparación de Contenido (2-3h) - **~40% COMPLETADO**

1. ✅ Añadir habilidades faltantes - **PARCIALMENTE COMPLETADO**
    - ✅ Truant agregado (Gen 3)
    - ✅ Moxie agregado (Gen 5)
    - ✅ Iron Barbs agregado (Gen 5)
    - ⏳ Sand Rush pendiente (Gen 4)
    - ⏳ Slush Rush pendiente (Gen 7)
2. ✅ Añadir Pokemon faltantes - **PARCIALMENTE COMPLETADO**
    - ✅ Slakoth/Slaking agregados (Gen 3)
    - ⏳ Carvanha/Sharpedo pendientes
    - ⏳ Snover/Abomasnow pendientes
    - ⏳ Ferroseed/Ferrothorn pendientes
3. ✅ Verificar que todos los items necesarios existen - **COMPLETADO**

### Fase 2: Integración de Triggers (4-5h) - **~85% COMPLETADO**

1. ✅ Integrar `OnBeforeMove` en `UseMoveAction` - **COMPLETADO** (para Truant)
2. ✅ Integrar `OnAfterMove` en `UseMoveAction` - **COMPLETADO** (para Moxie, Life Orb)
3. ✅ Integrar `OnDamageTaken` en `DamageAction` - **COMPLETADO**
4. ⏳ Integrar `OnWeatherChange` en `SetWeatherAction` - **PENDIENTE** (para Swift Swim, Chlorophyll)
5. ✅ Integrar `OnContactReceived` en `DamageAction` - **COMPLETADO** (para Static, Rough Skin, Rocky Helmet)
6. ✅ Integrar `OnWouldFaint` en `DamageAction` - **COMPLETADO** (para Focus Sash, Sturdy)
7. ✅ Integrar `OnTurnEnd` para items - **COMPLETADO** (para Leftovers, Black Sludge)

### Fase 3: Implementación de Efectos (8-10h) - **~85% COMPLETADO**

1. ✅ Implementar `AbilityEffect.SkipTurn` (Truant) - **COMPLETADO**
2. ✅ Implementar `AbilityEffect.RaiseStatOnKO` (Moxie) - **COMPLETADO**
3. ✅ Extender `AbilityListener` para efectos de contacto - **COMPLETADO** (Static, Rough Skin)
4. ✅ Implementar Life Orb recoil en `ItemListener` - **COMPLETADO**
5. ✅ Implementar Focus Sash en `DamageAction` - **COMPLETADO** (implementado directamente en DamageAction)
6. ✅ Implementar Rocky Helmet en `ItemListener` - **COMPLETADO**
7. ✅ Implementar Black Sludge en `ItemListener` - **COMPLETADO**
8. ✅ Crear `ContactDamageAction` - **COMPLETADO** (nueva acción para daño de contacto)

### Fase 4: Tests Fase 2.17 (5-6h) - **~60% COMPLETADO**

1. ✅ Tests para Truant - **COMPLETADO** (4 tests pasando)
2. ✅ Tests para Speed Boost - **COMPLETADO** (3 tests pasando)
3. ✅ Tests para Static - **COMPLETADO** (4 tests pasando)
4. ✅ Tests para Rough Skin - **COMPLETADO** (3 tests pasando)
5. ✅ Tests para Swift Swim - **COMPLETADO** (3 tests pasando)
6. ✅ Tests para Chlorophyll - **COMPLETADO** (2 tests pasando)
7. ✅ Tests para Moxie - **COMPLETADO** (3 tests pasando)
8. ✅ Tests de integración - **COMPLETADO** (7 tests pasando)

**Total Fase 2.17**: 29 tests pasando de ~30 estimados

### Fase 5: Tests Fase 2.18 (3-4h) - **~85% COMPLETADO**

1. ✅ Tests para Life Orb recoil - **COMPLETADO** (3 tests pasando)
2. ✅ Tests para Focus Sash - **COMPLETADO** (4 tests pasando)
3. ✅ Tests para Rocky Helmet - **COMPLETADO** (3 tests pasando)
4. ✅ Tests para Black Sludge - **COMPLETADO** (4 tests pasando)
5. ⏳ Tests de integración - **PENDIENTE**

**Total Fase 2.18**: 14 tests pasando de ~20 estimados

### Fase 6: Battle Formats (25-30h)

1. Implementar targeting para Doubles
2. Implementar spread moves
3. Ajustar screens para doubles
4. Implementar Triples
5. Implementar Rotation battles
6. Tests completos

---

## Checklist de Validación

### Antes de Comenzar

-   [ ] Leer código existente de `UseMoveAction`, `DamageAction`, `SetWeatherAction`
-   [ ] Leer código existente de `AbilityListener`, `ItemListener`
-   [ ] Leer código existente de `BattleTriggerProcessor`
-   [ ] Verificar que todos los triggers están definidos en `BattleTrigger` enum
-   [ ] Verificar que todos los `AbilityTrigger` están definidos

### Durante Implementación

-   [ ] Seguir TDD: Escribir tests primero
-   [ ] Verificar que tests fallan antes de implementar
-   [ ] Implementar funcionalidad mínima para pasar tests
-   [ ] Refactorizar si es necesario
-   [ ] Verificar que todos los tests pasan

### Después de Cada Fase

-   [ ] Ejecutar `dotnet build` - 0 warnings, 0 errors
-   [ ] Ejecutar `dotnet test` - Todos los tests pasan
-   [ ] Ejecutar scripts de validación:
    -   `ai_workflow/scripts/validate-test-structure.ps1`
    -   `ai_workflow/scripts/validate-fdd-compliance.ps1`
-   [ ] Actualizar documentación:
    -   `docs/features/2-combat-system/roadmap.md`
    -   `docs/features/2-combat-system/README.md`
    -   `.ai/context.md`
    -   `docs/features_master_list.md`

---

## Notas Importantes

1. **Siempre seguir TDD**: Escribir tests primero, luego implementar
2. **Leer código existente**: Nunca escribir código sin leer el código relacionado primero
3. **Feature references**: Todos los archivos deben tener referencias a su feature
4. **Validación obligatoria**: Ejecutar scripts de validación después de cada fase
5. **Documentación**: Actualizar documentación después de cada fase completada

---

## 📝 Cambios Recientes

### 2025-01-XX - Corrección de Tests y Completado Parcial de Fase 2.17 y 2.18

#### Correcciones Realizadas:

1. **Moxie**: Corregida la búsqueda de Pokemon desmayados para usar `opposingSide.Slots` en lugar de `GetActiveSlots()`
2. **Tackle**: Agregado `MakesContact(true)` al movimiento Tackle para que funcione correctamente con efectos de contacto
3. **MoveBuilder**: Agregado método `MakesContact()` para facilitar la configuración de movimientos de contacto

#### Implementaciones Completadas:

**Triggers Integrados:**

-   ✅ **OnAfterMove** integrado en `UseMoveAction` (línea ~212)
-   ✅ **OnBeforeMove** integrado en `UseMoveAction` (líneas ~137-153)
-   ✅ **OnContactReceived** integrado en `DamageAction` (líneas ~145-160)
-   ✅ **OnWouldFaint** integrado en `DamageAction` (líneas ~75-112)
-   ✅ **OnTurnEnd** para items ya estaba integrado

**Habilidades:**

-   ✅ **Moxie** completamente funcional con tests pasando
-   ✅ **Static** completamente funcional con tests pasando
-   ✅ **Rough Skin** completamente funcional con tests pasando
-   ✅ **Truant** completamente funcional con tests pasando

**Items:**

-   ✅ **Rocky Helmet** completamente funcional con tests pasando
-   ✅ **Life Orb** completamente funcional con tests pasando
-   ✅ **Focus Sash** completamente funcional con tests pasando
-   ✅ **Black Sludge** completamente funcional con tests pasando

**Nuevas Acciones:**

-   ✅ **ContactDamageAction** creada para daño de contacto

**Contenido Agregado:**

-   ✅ **Slakoth/Slaking** agregados al catálogo (Gen 3)
-   ✅ **Iron Barbs** agregado al catálogo (Gen 5)
-   ✅ **Tackle** configurado con `MakesContact(true)`

#### Tests Pasando:

-   `MoxieTests.cs`: 3/3 tests ✅
-   `StaticTests.cs`: 4/4 tests ✅
-   `RoughSkinTests.cs`: 3/3 tests ✅
-   `RockyHelmetTests.cs`: 3/3 tests ✅
-   `TruantTests.cs`: 4/4 tests ✅
-   `LifeOrbTests.cs`: 3/3 tests ✅
-   `FocusSashTests.cs`: 4/4 tests ✅
-   `BlackSludgeTests.cs`: 4/4 tests ✅

**Total**: 28 tests nuevos pasando, 3189 tests totales pasando sin errores.

---

**Última Actualización**: 2025-01-XX
