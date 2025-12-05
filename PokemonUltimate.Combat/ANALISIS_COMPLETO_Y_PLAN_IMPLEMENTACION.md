# Análisis Técnico Completo y Plan de Implementación

## Sistema de Combate - PokemonUltimate.Combat

**Fecha**: 2024  
**Versión Analizada**: `feature/2.15-advanced-moves`  
**Total de Mejoras Identificadas**: 29 mejoras iniciales + 10 categorías de mejoras arquitectónicas avanzadas

**Última Actualización**: Revisión completa considerando todos los casos del combate Pokémon

---

## 📋 Tabla de Contenidos

1. [Resumen Ejecutivo](#resumen-ejecutivo)
2. [Análisis de Principios SOLID](#análisis-de-principios-solid)
3. [Análisis de Code Quality](#análisis-de-code-quality)
4. [Análisis Arquitectónico](#análisis-arquitectónico)
5. [Mejoras Adicionales](#mejoras-adicionales)
6. [Mejoras Arquitectónicas Avanzadas](#mejoras-arquitectónicas-avanzadas)
7. [Plan de Implementación](#plan-de-implementación)
8. [Ejemplos de Código](#ejemplos-de-código)

---

## 📊 Resumen Ejecutivo

### Estado General

| Aspecto                  | Estado        | Prioridad |
| ------------------------ | ------------- | --------- |
| **Arquitectura General** | ✅ Buena base | -         |
| **Principios SOLID**     | ⚠️ Mejorable  | 🔴 Alta   |
| **Code Quality**         | ⚠️ Mejorable  | 🟡 Media  |
| **Testabilidad**         | ⚠️ Limitada   | 🔴 Alta   |
| **Extensibilidad**       | ⚠️ Limitada   | 🟡 Media  |

### Top 6 Problemas Críticos

1. **🔴 Métodos Demasiado Largos** - `UseMoveAction.ExecuteLogic()` (150+ líneas)
2. **🔴 Creación Directa de Objetos** - `CombatEngine`, `UseMoveAction`
3. **🔴 Random Estático Compartido** - `TurnOrderResolver`, `AccuracyChecker`, y más
4. **🔴 Switch Statements Rígidos** - Múltiples ubicaciones
5. **🔴 Magic Numbers y Strings** - Múltiples archivos
6. **🔴 Múltiples Random Estáticos** - 5+ clases afectadas

### Métricas Actuales vs Objetivo

| Métrica                 | Actual     | Objetivo | Estado |
| ----------------------- | ---------- | -------- | ------ |
| Complejidad Ciclomática | Alta (>15) | < 10     | 🔴     |
| Líneas por Método       | 150+       | < 50     | 🔴     |
| Acoplamiento            | Medio-Alto | Bajo     | 🟡     |
| Cohesión                | Media      | Alta     | 🟡     |

### Distribución de Mejoras

-   **🔴 Alta Prioridad**: 6 problemas críticos
-   **🟡 Media Prioridad**: 14 mejoras arquitectónicas
-   **🟢 Baja Prioridad**: 9 mejoras menores

**Total**: 29 mejoras identificadas inicialmente + 10 categorías de mejoras arquitectónicas avanzadas

---

## 🏗️ Análisis de Principios SOLID

### 1.1 Single Responsibility Principle (SRP)

#### ❌ Problemas Identificados

**1. `CombatEngine` - Demasiadas responsabilidades**

-   **Ubicación**: `Engine/CombatEngine.cs`
-   **Problema**: Orquesta combate completo, gestiona providers, queue, field, ejecuta turnos
-   **Impacto**: Difícil de testear, modificar y extender
-   **Solución**: Separar en:
    -   `BattleOrchestrator`: Coordina flujo general
    -   `TurnExecutor`: Ejecuta turno individual
    -   `BattleInitializer`: Inicializa campo de batalla

**2. `UseMoveAction` - Método `ExecuteLogic` demasiado largo**

-   **Ubicación**: `Actions/UseMoveAction.cs` (líneas 71-223)
-   **Problema**: 150+ líneas con múltiples responsabilidades
-   **Impacto**: Difícil de mantener y testear
-   **Solución**: Extraer a clases especializadas:
    -   `MoveValidator`: Valida PP, estados, protección
    -   `MoveEffectProcessor`: Procesa efectos del movimiento
    -   `MoveDamageCalculator`: Coordina cálculo de daño

**3. `EndOfTurnProcessor` - Clase estática con múltiples responsabilidades**

-   **Ubicación**: `Engine/EndOfTurnProcessor.cs`
-   **Problema**: Procesa status, weather, terrain, limpia estados
-   **Impacto**: No extensible, difícil de mockear en tests
-   **Solución**: Convertir a instancia con estrategias:
    -   `IEndOfTurnEffectProcessor` con implementaciones específicas
    -   `StatusEffectProcessor`, `WeatherEffectProcessor`, `TerrainEffectProcessor`

**4. `BattleField` - Mezcla de responsabilidades**

-   **Ubicación**: `Field/BattleField.cs`
-   **Problema**: Gestiona weather, terrain, sides, slots
-   **Impacto**: Clase grande con múltiples razones para cambiar
-   **Solución**: Extraer a Value Objects:
    -   `WeatherState`: Gestiona weather y duración
    -   `TerrainState`: Gestiona terrain y duración

### 1.2 Open/Closed Principle (OCP)

#### ❌ Problemas Identificados

**1. Switch Statements Rígidos**

-   `BattleTriggerProcessor` - `AbilityListener.ShouldRespondToTrigger()` (líneas 59-75)
-   `EndOfTurnProcessor.ProcessStatusEffects` (líneas 120-142)
-   `UseMoveAction.ProcessEffects` (líneas 456-572)
-   `ItemListener.ShouldRespondToTrigger()` (líneas 59-75)
-   `TargetResolver.GetValidTargets()` (líneas 40-100)

**Solución**: Usar Strategy Pattern o diccionarios de mapeo

### 1.3 Liskov Substitution Principle (LSP)

#### ✅ Bien Implementado

-   Las acciones (`BattleAction`) pueden ser sustituidas correctamente
-   Los listeners (`IBattleListener`) siguen el principio correctamente

### 1.4 Interface Segregation Principle (ISP)

#### ✅ Bien Implementado

-   `IBattleListener` tiene interfaz pequeña y enfocada
-   `IDamageStep` tiene única responsabilidad

### 1.5 Dependency Inversion Principle (DIP)

#### ❌ Problemas Identificados

**1. `CombatEngine` - Dependencia directa de implementaciones**

-   Crea instancias directamente (`new BattleField()`, `new BattleQueue()`)
-   **Solución**: Inyectar dependencias

**2. `UseMoveAction` - Crea `DamagePipeline` directamente**

-   `new DamagePipeline()` dentro del método (líneas 380, 394)
-   **Solución**: Inyectar `IDamagePipeline`

**3. Múltiples Random estáticos compartidos**

-   `TurnOrderResolver` (línea 21)
-   `AccuracyChecker` (línea 19)
-   `RandomFactorStep` (línea 19)
-   `CriticalHitStep` (línea 19)
-   `UseMoveAction` crea `new Random()` en métodos (líneas 301, 318)
-   **Solución**: Crear `IRandomProvider` e inyectarlo

**4. Creación de `MoveData` dummy**

-   `EndOfTurnProcessor.CreateStatusDamageMove()` (líneas 41-54)
-   `EntryHazardProcessor.CreateHazardDamageAction()` (líneas 206-216)
-   **Solución**: Crear `DamageContextFactory` o `SystemDamageContext`

---

## 🔍 Análisis de Code Quality

### 2.1 Magic Numbers y Strings

#### ❌ Problemas Identificados

**Magic Numbers**:

-   `CombatEngine`: `const int maxTurns = 1000;` (línea 107)
-   `BattleQueue`: `private const int MaxIterations = 1000;` (línea 21)
-   `TurnOrderResolver`: `return 0.5f;` para Paralysis (línea 121)
-   `ItemListener`: `healAmount = pokemon.MaxHP / 16;` (línea 119)

**Magic Strings**:

-   `UseMoveAction`: Comparaciones hardcodeadas (`"dig"`, `"dive"`, etc.) (líneas 260-280)

**Solución**: Crear constantes en `BattleConstants`, `StatusConstants`, `ItemConstants`

### 2.2 Métodos Demasiado Largos

#### ❌ Problemas Identificados

**1. `UseMoveAction.ExecuteLogic` - 150+ líneas**

-   **Solución**: Dividir en métodos más pequeños:
    -   `ValidateMoveExecution()`
    -   `ProcessMultiTurnMove()`
    -   `ProcessSemiInvulnerableMove()`
    -   `ProcessFocusPunchMove()`
    -   `CheckProtection()`
    -   `CheckAccuracy()`

**2. `UseMoveAction.ProcessEffects` - 200+ líneas**

-   **Solución**: Extraer cada caso a método privado o clase Strategy

**3. `EndOfTurnProcessor.ProcessEffects` - 50+ líneas**

-   **Solución**: Usar patrón Chain of Responsibility o Composite

### 2.3 Complejidad Ciclomática

#### ❌ Problemas Identificados

-   `UseMoveAction.ExecuteLogic` - Alta complejidad (múltiples if/switch anidados)
-   `UseMoveAction.ProcessEffects` - Alta complejidad (switch extenso)

**Solución**: Usar Early Returns y extraer métodos

### 2.4 Duplicación de Código

#### ❌ Problemas Identificados

**1. Creación de `DamageContext` duplicada**

-   Múltiples lugares (`UseMoveAction.cs`, `EndOfTurnProcessor.cs`, `EntryHazardProcessor.cs`)
-   **Solución**: Crear `DamageContextFactory`

**2. Validación de slots activos**

-   `slot.IsEmpty || slot.HasFainted` repetido
-   **Solución**: Método de extensión `slot.IsActive()`

**3. Cálculo de daño mínimo**

-   `Math.Max(EndOfTurnConstants.MinimumDamage, damage)` repetido
-   **Solución**: Método helper `EnsureMinimumDamage(int damage)`

### 2.5 Naming y Claridad

#### ⚠️ Mejoras Sugeridas

-   `moveForDamage` → `effectiveMove` o `damageMove`
-   `hasMultiHitEffect` → `isMultiHitMove`
-   `ProcessEffects` → `ProcessMoveEffects` o `ApplyMoveEffects`

---

## 🏛️ Análisis Arquitectónico

### 3.1 Acoplamiento

#### ❌ Problemas Identificados

-   `CombatEngine` acoplado a detalles de implementación
-   `UseMoveAction` acoplado a `DamagePipeline`
-   `EndOfTurnProcessor` acoplado a tipos específicos de status
-   Múltiples clases crean objetos directamente

**Solución**: Inyección de dependencias y Factory Pattern

### 3.2 Cohesión

#### ⚠️ Mejoras Sugeridas

**1. `BattleSlot` - Demasiados campos**

-   10+ campos privados con diferentes propósitos
-   **Solución**: Agrupar en Value Objects:
    -   `StatStages`: Gestiona stages de stats
    -   `VolatileStatusTracker`: Gestiona estados volátiles
    -   `DamageTracker`: Gestiona daño para Counter/Mirror Coat

**2. `BattleSide` - Mezcla de responsabilidades**

-   Gestiona slots, party, hazards, side conditions
-   **Solución**: Separar en managers especializados

### 3.3 Testabilidad

#### ❌ Problemas Identificados

**1. Métodos estáticos no testables**

-   `BattleTriggerProcessor`, `EndOfTurnProcessor`, `TurnOrderResolver`
-   `AccuracyChecker`, `TargetResolver`, `EntryHazardProcessor`
-   **Solución**: Convertir a instancias con dependencias inyectadas

**2. Creación directa de objetos**

-   `new DamagePipeline()`, `new Random()`, `new MoveData()`
-   **Solución**: Inyectar dependencias

**3. Random estático compartido**

-   Múltiples clases afectadas
-   **Solución**: Inyectar `IRandomProvider`

### 3.4 Thread Safety

#### ❌ Problemas Identificados

-   `TurnOrderResolver._random` compartido entre threads
-   `AccuracyChecker._random` compartido
-   `RandomFactorStep._random` compartido
-   `CriticalHitStep._random` compartido
-   **Solución**: Usar `ThreadLocal<Random>` o inyectar por request

---

## 🔧 Mejoras Adicionales

### 4.1 Clases Estáticas No Testables

**1. `EntryHazardProcessor`**

-   Clase estática con creación directa de objetos
-   **Solución**: Convertir a instancia con `IHazardProcessor`

**2. `AccuracyChecker`**

-   Clase estática con Random compartido
-   Dos métodos sobrecargados (confuso)
-   **Solución**: Convertir a instancia con `IAccuracyChecker`

**3. `TargetResolver`**

-   Clase estática con switch extenso
-   TODO pendiente sobre redirección (línea 107)
-   **Solución**: Convertir a instancia, implementar redirección

### 4.2 Problemas de Performance

**1. Creación de Random en cada llamada**

-   `UseMoveAction.ProcessEffects()` crea `new Random()` (líneas 301, 318)
-   **Solución**: Inyectar `IRandomProvider`

**2. LINQ en loops calientes**

-   Validaciones redundantes después de `GetAllActiveSlots()`
-   **Solución**: Confiar en el método o documentar claramente

### 4.3 Problemas de Robustez

**1. `SwitchAction` funcionalidad incompleta**

-   Comentario indica funcionalidad incompleta (línea 93)
-   **Solución**: Implementar correctamente o documentar

**2. Manejo de null en `DamageAction`**

-   `Context.Move != null` check (línea 69)
-   **Solución**: Validar en constructor o documentar

---

## 🔍 Mejoras Arquitectónicas Avanzadas

### 5.1 Validación de Invariantes del Estado de Batalla

#### ❌ Problemas Identificados

**1. Falta de validación de consistencia de estado**

-   No hay validación de que los slots activos coincidan con los Pokemon en el party
-   No hay validación de que un Pokemon no esté en múltiples slots simultáneamente
-   No hay validación de que los Pokemon en slots pertenezcan al party correspondiente
-   No hay validación de que los stat stages estén en rango válido (-6 a +6)
-   No hay validación de que los contadores de estado (Badly Poisoned, Protect) sean consistentes

**Solución**: Crear `IBattleStateValidator` con métodos:

-   `ValidateSlotConsistency(BattleField field)`
-   `ValidateStatStages(BattleSlot slot)`
-   `ValidateStatusCounters(BattleSlot slot)`
-   `ValidatePartySlotConsistency(BattleSide side)`

**2. Falta de validación de precondiciones robusta**

-   Algunos métodos no validan completamente sus precondiciones antes de ejecutar
-   No hay validación de que los efectos de movimientos sean consistentes con los datos
-   No hay validación de que las habilidades/items sean válidos para el Pokemon

**Solución**: Crear métodos de validación centralizados y usar Contract.Assert o validaciones explícitas

### 5.2 Manejo de Efectos Complejos y Acumulativos

#### ❌ Problemas Identificados

**1. Creación de `MoveData` temporal para Pursuit**

-   `UseMoveAction.ProcessEffects()` crea un `MoveData` temporal con poder duplicado (líneas 327-338)
-   Esto viola el principio de inmutabilidad y puede causar confusión
-   **Solución**: Crear `MoveModifier` o `EffectiveMoveData` que encapsule modificaciones temporales

**2. Manejo de efectos acumulativos**

-   Badly Poisoned counter se incrementa directamente en `EndOfTurnProcessor` (línea 192)
-   No hay un sistema claro para manejar efectos que se acumulan en múltiples turnos
-   **Solución**: Crear `IAccumulativeEffect` interface y `AccumulativeEffectTracker` class

**3. Manejo de efectos que se cancelan**

-   No hay un sistema claro para manejar efectos que se cancelan entre sí (ej: Paralysis cancela Focus Punch)
-   **Solución**: Crear `EffectCancellationRegistry` que defina qué efectos cancelan otros

### 5.3 Sistema de Mensajes y Logging

#### ❌ Problemas Identificados

**1. Mensajes hardcodeados en múltiples lugares**

-   Mensajes como `"{Target.Pokemon.DisplayName} is switching out!"` están hardcodeados (línea 339 de `UseMoveAction`)
-   No hay un sistema centralizado para mensajes de batalla
-   **Solución**: Crear `IBattleMessageFormatter` y `BattleMessageTemplates` class

**2. Falta de sistema de logging estructurado**

-   No hay logging de eventos importantes de batalla para debugging
-   No hay forma de rastrear el flujo de ejecución de acciones
-   **Solución**: Crear `IBattleLogger` interface con niveles de log (Debug, Info, Warning, Error)

**3. Falta de sistema de eventos estructurado**

-   El sistema de triggers (`BattleTriggerProcessor`) es básico y no extensible
-   No hay un sistema claro para eventos que ocurren en momentos específicos
-   **Solución**: Crear `IBattleEventBus` con sistema de suscripción/desuscripción

### 5.4 Manejo de Redirección y Targeting Avanzado

#### ❌ Problemas Identificados

**1. TODO pendiente sobre redirección**

-   `TargetResolver.GetValidTargets()` tiene un TODO sobre redirección (línea 107)
-   No hay implementación de Follow Me, Rage Powder, Lightning Rod, Storm Drain
-   **Solución**: Crear `ITargetRedirectionResolver` interface y implementaciones específicas

**2. Manejo de targeting complejo**

-   No hay manejo claro de movimientos que cambian de target durante la ejecución
-   No hay manejo de movimientos que afectan múltiples targets de forma diferente
-   **Solución**: Crear `ITargetingStrategy` interface con implementaciones específicas

### 5.5 Validación de Consistencia de Datos

#### ❌ Problemas Identificados

**1. Falta de validación de datos de movimientos**

-   No hay validación de que los efectos de movimientos sean consistentes con el tipo de movimiento
-   No hay validación de que los valores de poder/precisión sean válidos
-   **Solución**: Crear `MoveDataValidator` class con validaciones específicas

**2. Falta de validación de habilidades/items**

-   No hay validación de que las habilidades/items sean válidos para el Pokemon
-   No hay validación de que los efectos de habilidades/items sean consistentes
-   **Solución**: Crear `AbilityValidator` y `ItemValidator` classes

### 5.6 Manejo de Efectos Multi-Turno y Estados Complejos

#### ❌ Problemas Identificados

**1. Manejo de estados de movimientos multi-turno**

-   `BattleSlot` tiene múltiples campos para manejar estados de movimientos (`_chargingMoveName`, `_semiInvulnerableMoveName`, `_isSemiInvulnerableCharging`)
-   Esto viola SRP y hace difícil agregar nuevos tipos de movimientos multi-turno
-   **Solución**: Crear `MoveStateTracker` Value Object que encapsule todos los estados de movimientos

**2. Manejo de efectos que dependen de otros efectos**

-   No hay un sistema claro para manejar dependencias entre efectos (ej: Solar Beam depende de Sun)
-   **Solución**: Crear `IEffectDependencyResolver` interface

### 5.7 Manejo de Batallas Dobles/Triples

#### ⚠️ Mejoras Sugeridas

**1. Optimización para batallas multi-slot**

-   Aunque el sistema soporta batallas dobles/triples, algunas operaciones podrían optimizarse
-   `GetAllActiveSlots()` itera sobre todos los slots sin considerar el formato de batalla
-   **Solución**: Crear métodos optimizados específicos para singles/doubles/triples

**2. Manejo de efectos que afectan múltiples slots**

-   Algunos efectos (como Spread moves) podrían beneficiarse de un manejo más estructurado
-   **Solución**: Crear `ISpreadEffectProcessor` interface

### 5.8 Manejo de Errores y Casos Edge

#### ❌ Problemas Identificados

**1. Manejo inconsistente de casos edge**

-   Algunos métodos manejan casos edge explícitamente, otros no
-   No hay un patrón consistente para manejar situaciones inesperadas
-   **Solución**: Crear `BattleException` hierarchy y usar consistentemente

**2. Falta de validación de casos imposibles**

-   No hay validación de que ciertos estados sean imposibles (ej: Pokemon con HP negativo)
-   **Solución**: Agregar validaciones de invariantes en puntos críticos

### 5.9 Optimización de Performance

#### ⚠️ Mejoras Sugeridas

**1. Reducción de allocations en hot paths**

-   `UseMoveAction.ProcessEffects()` crea múltiples listas y objetos temporales
-   `EndOfTurnProcessor.ProcessEffects()` crea listas para cada slot
-   **Solución**: Usar `ArrayPool<T>` o `List<T>` reutilizables donde sea apropiado

**2. Optimización de búsquedas**

-   Búsquedas de efectos en `Move.Effects` se hacen múltiples veces con LINQ
-   **Solución**: Cachear resultados de búsquedas o usar diccionarios indexados

### 5.10 Extensibilidad para Nuevos Mecanismos

#### ⚠️ Mejoras Sugeridas

**1. Sistema de plugins para efectos**

-   No hay un sistema claro para agregar nuevos tipos de efectos sin modificar código existente
-   **Solución**: Crear sistema de registro de efectos con `IEffectRegistry`

**2. Sistema de configuración de batalla**

-   `BattleRules` es básico y no extensible para nuevas reglas
-   **Solución**: Crear `IBattleRule` interface y sistema de reglas composables

---

## 📋 Plan de Implementación

### Fase 0: Preparación y Setup (1-2 días)

#### Tarea 0.1: Crear Interfaces Base

-   [ ] Crear `IRandomProvider` interface
-   [ ] Crear `IDamagePipeline` interface
-   [ ] Crear `IAccuracyChecker` interface
-   [ ] Crear `IHazardProcessor` interface
-   [ ] Crear `ITargetResolver` interface

#### Tarea 0.2: Crear Constantes

-   [ ] Crear `BattleConstants.cs` con:
    -   `MaxTurns = 1000`
    -   `MaxQueueIterations = 1000`
-   [ ] Crear `StatusConstants.cs` con:
    -   `ParalysisSpeedMultiplier = 0.5f`
    -   `ParalysisFullParalysisChance = 25`
-   [ ] Crear `ItemConstants.cs` con:
    -   `LeftoversHealDivisor = 16`
-   [ ] Crear `MoveConstants.cs` con nombres de movimientos semi-invulnerables

#### Tarea 0.3: Crear Extension Methods

-   [ ] Crear `BattleSlotExtensions.cs` con:
    -   `IsActive(this BattleSlot slot)`
-   [ ] Crear `DamageCalculationExtensions.cs` con:
    -   `EnsureMinimumDamage(int damage)`

#### Tarea 0.4: Crear Factories

-   [ ] Crear `DamageContextFactory.cs`
-   [ ] Crear `RandomProvider.cs` (implementación de `IRandomProvider`)
-   [ ] Crear `ThreadSafeRandomProvider.cs` (si se necesita)

**Dependencias**: Ninguna  
**Tests Requeridos**: Tests unitarios para cada nueva clase/interfaz

---

### Fase 1: Quick Wins - Refactorizaciones Simples (3-5 días)

#### Tarea 1.1: Eliminar Magic Numbers y Strings

-   [ ] Reemplazar `maxTurns` en `CombatEngine` → `BattleConstants.MaxTurns`
-   [ ] Reemplazar `MaxIterations` en `BattleQueue` → `BattleConstants.MaxQueueIterations`
-   [ ] Reemplazar `0.5f` en `TurnOrderResolver` → `StatusConstants.ParalysisSpeedMultiplier`
-   [ ] Reemplazar `/ 16` en `ItemListener` → `ItemConstants.LeftoversHealDivisor`
-   [ ] Reemplazar strings hardcodeados en `UseMoveAction` → `MoveConstants`

**Archivos Afectados**:

-   `Engine/CombatEngine.cs`
-   `Engine/BattleQueue.cs`
-   `Helpers/TurnOrderResolver.cs`
-   `Events/ItemListener.cs`
-   `Actions/UseMoveAction.cs`

**Tests**: Verificar que comportamiento no cambia

---

#### Tarea 1.2: Usar Extension Methods

-   [ ] Reemplazar `slot.IsEmpty || slot.HasFainted` → `slot.IsActive()`
-   [ ] Reemplazar `Math.Max(EndOfTurnConstants.MinimumDamage, damage)` → `damage.EnsureMinimumDamage()`

**Archivos Afectados**: Múltiples (buscar con grep)

**Tests**: Verificar que comportamiento no cambia

---

#### Tarea 1.3: Extraer Métodos en `UseMoveAction.ExecuteLogic`

-   [ ] Extraer `ValidateMoveExecution()` - Validaciones iniciales (PP, Flinch, Status)
-   [ ] Extraer `ProcessMultiTurnMove()` - Lógica de movimientos multi-turno
-   [ ] Extraer `ProcessSemiInvulnerableMove()` - Lógica de movimientos semi-invulnerables
-   [ ] Extraer `ProcessFocusPunchMove()` - Lógica de Focus Punch
-   [ ] Extraer `CheckProtection()` - Verificación de protección
-   [ ] Extraer `CheckSemiInvulnerable()` - Verificación de semi-invulnerable
-   [ ] Extraer `CheckAccuracy()` - Verificación de precisión
-   [ ] Refactorizar `ExecuteLogic()` para usar estos métodos

**Archivos Afectados**:

-   `Actions/UseMoveAction.cs`

**Tests**: Todos los tests existentes deben pasar

---

### Fase 2: Inyección de Dependencias - Random Provider (2-3 días)

#### Tarea 2.1: Implementar `IRandomProvider`

-   [ ] Crear `RandomProvider.cs` implementando `IRandomProvider`
-   [ ] Crear `ThreadSafeRandomProvider.cs` si se necesita
-   [ ] Agregar tests para `IRandomProvider`

#### Tarea 2.2: Refactorizar `TurnOrderResolver`

-   [ ] Convertir de estático a instancia
-   [ ] Inyectar `IRandomProvider` en constructor
-   [ ] Actualizar llamadas en `CombatEngine`
-   [ ] Actualizar tests

**Archivos Afectados**:

-   `Helpers/TurnOrderResolver.cs`
-   `Engine/CombatEngine.cs`
-   Tests relacionados

---

#### Tarea 2.3: Refactorizar `AccuracyChecker`

-   [ ] Convertir de estático a instancia
-   [ ] Crear `IAccuracyChecker` interface
-   [ ] Inyectar `IRandomProvider` en constructor
-   [ ] Unificar métodos sobrecargados
-   [ ] Actualizar llamadas en `UseMoveAction`
-   [ ] Actualizar tests

**Archivos Afectados**:

-   `Helpers/AccuracyChecker.cs`
-   `Actions/UseMoveAction.cs`
-   Tests relacionados

---

#### Tarea 2.4: Refactorizar `RandomFactorStep`

-   [ ] Inyectar `IRandomProvider` en constructor
-   [ ] Actualizar `DamagePipeline` para pasar `IRandomProvider`
-   [ ] Actualizar tests

**Archivos Afectados**:

-   `Damage/Steps/RandomFactorStep.cs`
-   `Damage/DamagePipeline.cs`
-   Tests relacionados

---

#### Tarea 2.5: Refactorizar `CriticalHitStep`

-   [ ] Inyectar `IRandomProvider` en constructor
-   [ ] Actualizar `DamagePipeline` para pasar `IRandomProvider`
-   [ ] Actualizar tests

**Archivos Afectados**:

-   `Damage/Steps/CriticalHitStep.cs`
-   `Damage/DamagePipeline.cs`
-   Tests relacionados

---

#### Tarea 2.6: Refactorizar `UseMoveAction`

-   [ ] Inyectar `IRandomProvider` en constructor
-   [ ] Reemplazar `new Random()` con `_randomProvider`
-   [ ] Actualizar tests

**Archivos Afectados**:

-   `Actions/UseMoveAction.cs`
-   Tests relacionados

---

### Fase 3: Inyección de Dependencias - Damage Pipeline (2-3 días)

#### Tarea 3.1: Crear `IDamagePipeline` Interface

-   [ ] Extraer interface de `DamagePipeline`
-   [ ] Crear `IDamagePipeline` con método `Calculate()`
-   [ ] Hacer `DamagePipeline` implementar `IDamagePipeline`

#### Tarea 3.2: Refactorizar `UseMoveAction`

-   [ ] Inyectar `IDamagePipeline` en constructor
-   [ ] Reemplazar `new DamagePipeline()` con `_damagePipeline`
-   [ ] Actualizar tests

**Archivos Afectados**:

-   `Actions/UseMoveAction.cs`
-   Tests relacionados

---

### Fase 4: Factory Pattern - DamageContext (2-3 días)

#### Tarea 4.1: Crear `DamageContextFactory`

-   [ ] Crear `DamageContextFactory.cs` con métodos:
    -   `CreateForMove(BattleSlot attacker, BattleSlot defender, MoveData move, BattleField field)`
    -   `CreateForStatusDamage(BattleSlot slot, int damage, BattleField field)`
    -   `CreateForHazardDamage(BattleSlot slot, int damage, BattleField field)`
    -   `CreateForRecoil(BattleSlot slot, int damage, MoveData move, BattleField field)`
    -   `CreateForCounter(BattleSlot attacker, BattleSlot defender, int damage, MoveData move, BattleField field)`

#### Tarea 4.2: Refactorizar `EndOfTurnProcessor`

-   [ ] Inyectar `DamageContextFactory` en constructor (cuando se convierta a instancia)
-   [ ] Reemplazar creación de `MoveData` dummy y `DamageContext` con factory
-   [ ] Actualizar tests

**Archivos Afectados**:

-   `Engine/EndOfTurnProcessor.cs`
-   Tests relacionados

---

#### Tarea 4.3: Refactorizar `EntryHazardProcessor`

-   [ ] Inyectar `DamageContextFactory` en constructor (cuando se convierta a instancia)
-   [ ] Reemplazar creación de `MoveData` dummy y `DamageContext` con factory
-   [ ] Actualizar tests

**Archivos Afectados**:

-   `Engine/EntryHazardProcessor.cs`
-   Tests relacionados

---

#### Tarea 4.4: Refactorizar `UseMoveAction`

-   [ ] Inyectar `DamageContextFactory` en constructor
-   [ ] Reemplazar creación directa de `DamageContext` con factory
-   [ ] Actualizar tests

**Archivos Afectados**:

-   `Actions/UseMoveAction.cs`
-   Tests relacionados

---

### Fase 5: Convertir Clases Estáticas a Instancias (3-4 días)

#### Tarea 5.1: Refactorizar `EndOfTurnProcessor`

-   [ ] Crear `IEndOfTurnProcessor` interface
-   [ ] Convertir clase estática a instancia
-   [ ] Crear `StatusEffectProcessor`, `WeatherEffectProcessor`, `TerrainEffectProcessor`
-   [ ] Inyectar dependencias (`DamageContextFactory`, `IRandomProvider`)
-   [ ] Actualizar `CombatEngine` para crear instancia
-   [ ] Actualizar tests

**Archivos Afectados**:

-   `Engine/EndOfTurnProcessor.cs`
-   `Engine/CombatEngine.cs`
-   Tests relacionados

---

#### Tarea 5.2: Refactorizar `EntryHazardProcessor`

-   [ ] Crear `IHazardProcessor` interface
-   [ ] Convertir clase estática a instancia
-   [ ] Crear estrategias para cada tipo de hazard (opcional)
-   [ ] Inyectar dependencias (`DamageContextFactory`)
-   [ ] Actualizar `SwitchAction` para usar instancia
-   [ ] Actualizar tests

**Archivos Afectados**:

-   `Engine/EntryHazardProcessor.cs`
-   `Actions/SwitchAction.cs`
-   Tests relacionados

---

#### Tarea 5.3: Refactorizar `TargetResolver`

-   [ ] Crear `ITargetResolver` interface
-   [ ] Convertir clase estática a instancia
-   [ ] Implementar redirección (resolver TODO línea 107)
-   [ ] Considerar Strategy Pattern para diferentes scopes
-   [ ] Actualizar llamadas (si las hay)
-   [ ] Actualizar tests

**Archivos Afectados**:

-   `Helpers/TargetResolver.cs`
-   Tests relacionados

---

#### Tarea 5.4: Refactorizar `BattleTriggerProcessor`

-   [ ] Crear `IBattleTriggerProcessor` interface
-   [ ] Convertir clase estática a instancia (si es necesario)
-   [ ] Actualizar llamadas
-   [ ] Actualizar tests

**Archivos Afectados**:

-   `Events/BattleTriggerProcessor.cs`
-   Tests relacionados

---

### Fase 6: Strategy Pattern para Efectos (4-5 días)

#### Tarea 6.1: Crear Interfaces y Base Classes

-   [ ] Crear `IMoveEffectProcessor` interface
-   [ ] Crear `MoveEffectProcessorRegistry` class
-   [ ] Crear implementaciones base:
    -   `StatusEffectProcessor`
    -   `StatChangeEffectProcessor`
    -   `RecoilEffectProcessor`
    -   `DrainEffectProcessor`
    -   `FlinchEffectProcessor`
    -   `ProtectEffectProcessor`
    -   `CounterEffectProcessor`
    -   `HealEffectProcessor`

#### Tarea 6.2: Refactorizar `UseMoveAction.ProcessEffects`

-   [ ] Reemplazar switch statement con `MoveEffectProcessorRegistry`
-   [ ] Inyectar `MoveEffectProcessorRegistry` en constructor
-   [ ] Actualizar tests

**Archivos Afectados**:

-   `Actions/UseMoveAction.cs`
-   Tests relacionados

---

#### Tarea 6.3: Refactorizar `AbilityListener` y `ItemListener`

-   [ ] Reemplazar switch en `AbilityListener.ShouldRespondToTrigger()` con diccionario
-   [ ] Reemplazar switch en `ItemListener.ShouldRespondToTrigger()` con diccionario
-   [ ] Actualizar tests

**Archivos Afectados**:

-   `Events/AbilityListener.cs`
-   `Events/ItemListener.cs`
-   Tests relacionados

---

### Fase 7: Inyección de Dependencias en CombatEngine (2-3 días)

#### Tarea 7.1: Crear Factories para BattleField y BattleQueue

-   [ ] Crear `IBattleFieldFactory` interface
-   [ ] Crear `BattleFieldFactory` implementation
-   [ ] Crear `IBattleQueueFactory` interface (opcional, puede ser simple)
-   [ ] Crear `BattleQueueFactory` implementation

#### Tarea 7.2: Refactorizar `CombatEngine`

-   [ ] Inyectar `IBattleFieldFactory` en constructor
-   [ ] Inyectar `IBattleQueueFactory` en constructor (o crear directamente)
-   [ ] Inyectar `IEndOfTurnProcessor` en constructor
-   [ ] Inyectar `IRandomProvider` en constructor (para pasarlo a otros)
-   [ ] Actualizar `Initialize()` para usar factories
-   [ ] Actualizar tests

**Archivos Afectados**:

-   `Engine/CombatEngine.cs`
-   Tests relacionados

---

### Fase 8: Value Objects para BattleSlot (3-4 días)

#### Tarea 8.1: Crear Value Objects

-   [ ] Crear `StatStages.cs` Value Object
-   [ ] Crear `VolatileStatusFlags.cs` Value Object
-   [ ] Crear `DamageTracker.cs` Value Object
-   [ ] Crear `ProtectTracker.cs` Value Object
-   [ ] Crear `SemiInvulnerableState.cs` Value Object
-   [ ] Crear `ChargingMoveState.cs` Value Object

#### Tarea 8.2: Refactorizar `BattleSlot`

-   [ ] Reemplazar campos individuales con Value Objects
-   [ ] Actualizar métodos para usar Value Objects
-   [ ] Actualizar tests

**Archivos Afectados**:

-   `Field/BattleSlot.cs`
-   Tests relacionados

---

### Fase 9: Separar Responsabilidades de BattleField (2-3 días)

#### Tarea 9.1: Crear Value Objects para Weather y Terrain

-   [ ] Crear `WeatherState.cs` Value Object
-   [ ] Crear `TerrainState.cs` Value Object

#### Tarea 9.2: Refactorizar `BattleField`

-   [ ] Reemplazar campos de weather/terrain con Value Objects
-   [ ] Actualizar métodos para usar Value Objects
-   [ ] Actualizar tests

**Archivos Afectados**:

-   `Field/BattleField.cs`
-   Tests relacionados

---

### Fase 10: Completar Funcionalidad Pendiente (2-3 días)

#### Tarea 10.1: Completar `SwitchAction`

-   [ ] Implementar correctamente manejo de party
-   [ ] O documentar claramente comportamiento esperado
-   [ ] Actualizar tests

**Archivos Afectados**:

-   `Actions/SwitchAction.cs`
-   Tests relacionados

---

#### Tarea 10.2: Resolver Manejo de Null en `DamageAction`

-   [ ] Validar `Context.Move` en constructor de `DamageContext`
-   [ ] O documentar cuándo puede ser null
-   [ ] Remover check redundante si no es necesario
-   [ ] Actualizar tests

**Archivos Afectados**:

-   `Actions/DamageAction.cs`
-   `Damage/DamageContext.cs`
-   Tests relacionados

---

### Fase 11: Validación de Invariantes y Robustez (3-4 días)

#### Tarea 11.1: Crear Sistema de Validación de Estado

-   [ ] Crear `IBattleStateValidator` interface
-   [ ] Crear `BattleStateValidator` implementation
-   [ ] Agregar validaciones de consistencia de slots/party
-   [ ] Agregar validaciones de stat stages
-   [ ] Agregar validaciones de contadores de estado
-   [ ] Integrar validaciones en puntos críticos del flujo de batalla

**Archivos Afectados**:

-   Nuevo: `Validation/IBattleStateValidator.cs`
-   Nuevo: `Validation/BattleStateValidator.cs`
-   `Engine/CombatEngine.cs`
-   `Field/BattleField.cs`
-   Tests relacionados

---

#### Tarea 11.2: Crear Sistema de Mensajes Centralizado

-   [ ] Crear `IBattleMessageFormatter` interface
-   [ ] Crear `BattleMessageTemplates` class
-   [ ] Refactorizar mensajes hardcodeados en `UseMoveAction`
-   [ ] Refactorizar mensajes en otros lugares
-   [ ] Actualizar tests

**Archivos Afectados**:

-   Nuevo: `Messages/IBattleMessageFormatter.cs`
-   Nuevo: `Messages/BattleMessageTemplates.cs`
-   `Actions/UseMoveAction.cs`
-   Múltiples archivos con mensajes hardcodeados
-   Tests relacionados

---

#### Tarea 11.3: Implementar Redirección de Targets

-   [ ] Crear `ITargetRedirectionResolver` interface
-   [ ] Crear implementaciones: `FollowMeResolver`, `LightningRodResolver`, etc.
-   [ ] Integrar en `TargetResolver`
-   [ ] Resolver TODO pendiente (línea 107)
-   [ ] Actualizar tests

**Archivos Afectados**:

-   Nuevo: `Helpers/ITargetRedirectionResolver.cs`
-   Nuevo: `Helpers/TargetRedirectionResolvers/`
-   `Helpers/TargetResolver.cs`
-   Tests relacionados

---

### Fase 12: Manejo de Efectos Complejos (4-5 días)

#### Tarea 12.1: Crear Sistema de Modificadores de Movimientos

-   [ ] Crear `IMoveModifier` interface
-   [ ] Crear `MoveModifier` class para encapsular modificaciones temporales
-   [ ] Refactorizar creación de `MoveData` temporal en `UseMoveAction`
-   [ ] Actualizar tests

**Archivos Afectados**:

-   Nuevo: `Effects/IMoveModifier.cs`
-   Nuevo: `Effects/MoveModifier.cs`
-   `Actions/UseMoveAction.cs`
-   Tests relacionados

---

#### Tarea 12.2: Crear Sistema de Efectos Acumulativos

-   [ ] Crear `IAccumulativeEffect` interface
-   [ ] Crear `AccumulativeEffectTracker` class
-   [ ] Refactorizar manejo de Badly Poisoned counter
-   [ ] Extender para otros efectos acumulativos
-   [ ] Actualizar tests

**Archivos Afectados**:

-   Nuevo: `Effects/IAccumulativeEffect.cs`
-   Nuevo: `Effects/AccumulativeEffectTracker.cs`
-   `Engine/EndOfTurnProcessor.cs`
-   `Core/Effects/BadlyPoisonedEffect.cs` (si existe)
-   Tests relacionados

---

#### Tarea 12.3: Crear Value Object para Estados de Movimientos

-   [ ] Crear `MoveStateTracker` Value Object
-   [ ] Refactorizar `BattleSlot` para usar `MoveStateTracker`
-   [ ] Actualizar todos los usos de campos individuales
-   [ ] Actualizar tests

**Archivos Afectados**:

-   Nuevo: `Field/MoveStateTracker.cs`
-   `Field/BattleSlot.cs`
-   Múltiples archivos que usan estados de movimientos
-   Tests relacionados

---

### Fase 13: Sistema de Logging y Eventos (3-4 días)

#### Tarea 13.1: Crear Sistema de Logging

-   [ ] Crear `IBattleLogger` interface
-   [ ] Crear `BattleLogger` implementation
-   [ ] Agregar logging en puntos críticos
-   [ ] Crear `NullBattleLogger` para tests
-   [ ] Actualizar tests

**Archivos Afectados**:

-   Nuevo: `Logging/IBattleLogger.cs`
-   Nuevo: `Logging/BattleLogger.cs`
-   Nuevo: `Logging/NullBattleLogger.cs`
-   `Engine/CombatEngine.cs`
-   `Engine/BattleQueue.cs`
-   Tests relacionados

---

#### Tarea 13.2: Mejorar Sistema de Eventos

-   [ ] Crear `IBattleEventBus` interface
-   [ ] Crear `BattleEventBus` implementation
-   [ ] Refactorizar `BattleTriggerProcessor` para usar event bus
-   [ ] Agregar sistema de suscripción/desuscripción
-   [ ] Actualizar tests

**Archivos Afectados**:

-   Nuevo: `Events/IBattleEventBus.cs`
-   Nuevo: `Events/BattleEventBus.cs`
-   `Events/BattleTriggerProcessor.cs`
-   Tests relacionados

---

### Fase 14: Optimización y Performance (2-3 días)

#### Tarea 14.1: Optimizar Allocations en Hot Paths

-   [ ] Identificar hot paths con muchas allocations
-   [ ] Usar `ArrayPool<T>` o listas reutilizables donde sea apropiado
-   [ ] Medir mejoras de performance
-   [ ] Actualizar tests

**Archivos Afectados**:

-   `Actions/UseMoveAction.cs`
-   `Engine/EndOfTurnProcessor.cs`
-   Otros archivos identificados

---

#### Tarea 14.2: Cachear Búsquedas de Efectos

-   [ ] Identificar búsquedas repetidas de efectos
-   [ ] Cachear resultados en `UseMoveAction`
-   [ ] Medir mejoras de performance
-   [ ] Actualizar tests

**Archivos Afectados**:

-   `Actions/UseMoveAction.cs`
-   Tests relacionados

---

## 📊 Resumen del Plan

### Estimación de Tiempo Total

| Fase                           | Tareas        | Días Estimados | Prioridad  |
| ------------------------------ | ------------- | -------------- | ---------- |
| Fase 0: Preparación            | 4             | 1-2            | 🔴 Crítica |
| Fase 1: Quick Wins             | 3             | 3-5            | 🔴 Alta    |
| Fase 2: Random Provider        | 6             | 2-3            | 🔴 Alta    |
| Fase 3: Damage Pipeline        | 2             | 2-3            | 🔴 Alta    |
| Fase 4: DamageContext Factory  | 4             | 2-3            | 🟡 Media   |
| Fase 5: Clases Estáticas       | 4             | 3-4            | 🟡 Media   |
| Fase 6: Strategy Pattern       | 3             | 4-5            | 🟡 Media   |
| Fase 7: CombatEngine DI        | 2             | 2-3            | 🟡 Media   |
| Fase 8: Value Objects Slot     | 2             | 3-4            | 🟢 Baja    |
| Fase 9: Value Objects Field    | 2             | 2-3            | 🟢 Baja    |
| Fase 10: Completar Pendientes  | 2             | 2-3            | 🟡 Media   |
| Fase 11: Validación y Robustez | 3             | 3-4            | 🟡 Media   |
| Fase 12: Efectos Complejos     | 3             | 4-5            | 🟡 Media   |
| Fase 13: Logging y Eventos     | 2             | 3-4            | 🟢 Baja    |
| Fase 14: Optimización          | 2             | 2-3            | 🟢 Baja    |
| **TOTAL**                      | **44 tareas** | **39-55 días** |            |

### Orden de Ejecución Recomendado

1. **Fase 0** → Setup inicial (bloquea otras fases)
2. **Fase 1** → Quick wins (bajo riesgo, alto impacto)
3. **Fase 2** → Random Provider (bloquea Fase 3)
4. **Fase 3** → Damage Pipeline (bloquea Fase 4)
5. **Fase 4** → Factory Pattern (bloquea Fase 5 y Fase 12)
6. **Fase 5** → Clases Estáticas (puede hacerse en paralelo con Fase 6)
7. **Fase 6** → Strategy Pattern (puede hacerse en paralelo con Fase 5)
8. **Fase 7** → CombatEngine DI (depende de Fases 2-5)
9. **Fase 8-9** → Value Objects (mejoras de organización)
10. **Fase 10** → Completar pendientes
11. **Fase 11** → Validación y robustez (puede hacerse en paralelo con Fase 12)
12. **Fase 12** → Efectos complejos (depende de Fase 4)
13. **Fase 13** → Logging y eventos (mejoras opcionales, puede hacerse después)
14. **Fase 14** → Optimización (mejoras opcionales, hacer después de todas las demás)

### Criterios de Éxito por Fase

**Cada fase debe cumplir**:

-   ✅ Todos los tests existentes pasan
-   ✅ Nuevos tests escritos para nueva funcionalidad
-   ✅ Código compila sin warnings
-   ✅ Validación de scripts pasa (`validate-test-structure.ps1`, `validate-fdd-compliance.ps1`)
-   ✅ Documentación actualizada

### Riesgos y Mitigaciones

| Riesgo                          | Impacto | Mitigación                                     |
| ------------------------------- | ------- | ---------------------------------------------- |
| Romper tests existentes         | Alto    | Ejecutar tests después de cada tarea           |
| Cambios breaking en API pública | Alto    | Revisar todos los usos antes de cambiar        |
| Complejidad de refactorización  | Medio   | Hacer cambios incrementales, una fase a la vez |
| Tiempo subestimado              | Medio   | Agregar buffer del 20% al tiempo estimado      |

---

## 💻 Ejemplos de Código Clave

### Ejemplo 1: IRandomProvider

```csharp
/// <summary>
/// Provides random number generation for battle calculations.
/// </summary>
/// <remarks>
/// **Feature**: 2: Combat System
/// **Sub-Feature**: 2.3: Turn Order Resolution
/// **Documentation**: See `docs/features/2-combat-system/2.3-turn-order-resolution/architecture.md`
/// </remarks>
public interface IRandomProvider
{
    /// <summary>
    /// Returns a random integer between 0 and maxValue (exclusive).
    /// </summary>
    int Next(int maxValue);

    /// <summary>
    /// Returns a random integer between minValue (inclusive) and maxValue (exclusive).
    /// </summary>
    int Next(int minValue, int maxValue);

    /// <summary>
    /// Returns a random float between 0.0 and 1.0.
    /// </summary>
    float NextFloat();
}

public class RandomProvider : IRandomProvider
{
    private readonly Random _random;

    public RandomProvider(int? seed = null)
    {
        _random = seed.HasValue ? new Random(seed.Value) : new Random();
    }

    public int Next(int maxValue) => _random.Next(maxValue);
    public int Next(int minValue, int maxValue) => _random.Next(minValue, maxValue);
    public float NextFloat() => (float)_random.NextDouble();
}
```

### Ejemplo 2: Extension Methods

```csharp
/// <summary>
/// Extension methods for BattleSlot.
/// </summary>
/// <remarks>
/// **Feature**: 2: Combat System
/// **Sub-Feature**: 2.1: Battle Foundation
/// **Documentation**: See `docs/features/2-combat-system/2.1-battle-foundation/architecture.md`
/// </remarks>
public static class BattleSlotExtensions
{
    /// <summary>
    /// Checks if a slot has an active (non-empty, non-fainted) Pokemon.
    /// </summary>
    public static bool IsActive(this BattleSlot slot)
    {
        return slot != null && !slot.IsEmpty && !slot.HasFainted;
    }
}

/// <summary>
/// Extension methods for damage calculations.
/// </summary>
public static class DamageCalculationExtensions
{
    /// <summary>
    /// Ensures damage is at least the minimum required.
    /// </summary>
    public static int EnsureMinimumDamage(this int damage)
    {
        return Math.Max(EndOfTurnConstants.MinimumDamage, damage);
    }
}
```

### Ejemplo 3: DamageContextFactory

```csharp
/// <summary>
/// Factory for creating DamageContext instances.
/// </summary>
/// <remarks>
/// **Feature**: 2: Combat System
/// **Sub-Feature**: 2.4: Damage Calculation Pipeline
/// **Documentation**: See `docs/features/2-combat-system/2.4-damage-calculation-pipeline/architecture.md`
/// </remarks>
public class DamageContextFactory
{
    /// <summary>
    /// Creates a DamageContext for a move attack.
    /// </summary>
    public DamageContext CreateForMove(
        BattleSlot attacker,
        BattleSlot defender,
        MoveData move,
        BattleField field,
        bool forceCritical = false,
        float? fixedRandomValue = null)
    {
        return new DamageContext(attacker, defender, move, field, forceCritical, fixedRandomValue);
    }

    /// <summary>
    /// Creates a DamageContext for status damage (burn, poison, etc.).
    /// </summary>
    public DamageContext CreateForStatusDamage(
        BattleSlot slot,
        int damage,
        BattleField field)
    {
        var dummyMove = CreateStatusDamageMove();
        var context = new DamageContext(slot, slot, dummyMove, field);
        context.BaseDamage = damage;
        context.Multiplier = 1.0f;
        context.TypeEffectiveness = 1.0f;
        return context;
    }

    /// <summary>
    /// Creates a DamageContext for hazard damage.
    /// </summary>
    public DamageContext CreateForHazardDamage(
        BattleSlot slot,
        int damage,
        BattleField field)
    {
        var dummyMove = CreateHazardDamageMove();
        var context = new DamageContext(slot, slot, dummyMove, field);
        context.BaseDamage = damage;
        context.Multiplier = 1.0f;
        context.TypeEffectiveness = 1.0f;
        return context;
    }

    private MoveData CreateStatusDamageMove()
    {
        return new MoveData
        {
            Name = "Status Damage",
            Power = 0,
            Accuracy = 100,
            Type = PokemonType.Normal,
            Category = MoveCategory.Status,
            MaxPP = 0,
            Priority = 0,
            TargetScope = TargetScope.Self
        };
    }

    private MoveData CreateHazardDamageMove()
    {
        return new MoveData
        {
            Name = "Entry Hazard",
            Power = 1,
            Accuracy = 100,
            Type = PokemonType.Normal,
            Category = MoveCategory.Physical,
            MaxPP = 0,
            Priority = 0,
            TargetScope = TargetScope.Self
        };
    }
}
```

### Ejemplo 4: Strategy Pattern para Efectos de Movimientos

```csharp
/// <summary>
/// Processes a specific type of move effect.
/// </summary>
/// <remarks>
/// **Feature**: 2: Combat System
/// **Sub-Feature**: 2.5: Combat Actions
/// **Documentation**: See `docs/features/2-combat-system/2.5-combat-actions/architecture.md`
/// </remarks>
public interface IMoveEffectProcessor
{
    /// <summary>
    /// Checks if this processor can handle the given effect.
    /// </summary>
    bool CanProcess(IMoveEffect effect);

    /// <summary>
    /// Processes the effect and returns actions to execute.
    /// </summary>
    IEnumerable<BattleAction> Process(IMoveEffect effect, UseMoveAction context);
}

/// <summary>
/// Processes StatusEffect move effects.
/// </summary>
public class StatusEffectProcessor : IMoveEffectProcessor
{
    private readonly IRandomProvider _random;

    public StatusEffectProcessor(IRandomProvider random)
    {
        _random = random ?? throw new ArgumentNullException(nameof(random));
    }

    public bool CanProcess(IMoveEffect effect) => effect is StatusEffect;

    public IEnumerable<BattleAction> Process(IMoveEffect effect, UseMoveAction context)
    {
        var statusEffect = (StatusEffect)effect;

        if (_random.Next(100) < statusEffect.ChancePercent)
        {
            var targetSlot = statusEffect.TargetSelf ? context.User : context.Target;
            yield return new ApplyStatusAction(context.User, targetSlot, statusEffect.Status);
        }
    }
}

/// <summary>
/// Registry for move effect processors.
/// </summary>
public class MoveEffectProcessorRegistry
{
    private readonly List<IMoveEffectProcessor> _processors;

    public MoveEffectProcessorRegistry(IEnumerable<IMoveEffectProcessor> processors)
    {
        _processors = new List<IMoveEffectProcessor>(processors);
    }

    public IEnumerable<BattleAction> ProcessEffect(IMoveEffect effect, UseMoveAction context)
    {
        var processor = _processors.FirstOrDefault(p => p.CanProcess(effect));
        if (processor != null)
        {
            return processor.Process(effect, context);
        }
        return Enumerable.Empty<BattleAction>();
    }
}
```

### Ejemplo 5: Value Objects para BattleSlot

```csharp
/// <summary>
/// Manages stat stage modifications for a battle slot.
/// </summary>
/// <remarks>
/// **Feature**: 2: Combat System
/// **Sub-Feature**: 2.1: Battle Foundation
/// **Documentation**: See `docs/features/2-combat-system/2.1-battle-foundation/architecture.md`
/// </remarks>
public class StatStages
{
    private const int MinStatStage = -6;
    private const int MaxStatStage = 6;

    private readonly Dictionary<Stat, int> _stages;

    public StatStages()
    {
        _stages = new Dictionary<Stat, int>
        {
            { Stat.Attack, 0 },
            { Stat.Defense, 0 },
            { Stat.SpAttack, 0 },
            { Stat.SpDefense, 0 },
            { Stat.Speed, 0 },
            { Stat.Accuracy, 0 },
            { Stat.Evasion, 0 }
        };
    }

    public int GetStage(Stat stat)
    {
        if (stat == Stat.HP)
            return 0;
        return _stages.TryGetValue(stat, out var stage) ? stage : 0;
    }

    public int ModifyStage(Stat stat, int change)
    {
        if (stat == Stat.HP)
            throw new ArgumentException(ErrorMessages.CannotModifyHPStatStage, nameof(stat));

        if (!_stages.ContainsKey(stat))
            return 0;

        var oldStage = _stages[stat];
        var newStage = Math.Max(MinStatStage, Math.Min(MaxStatStage, oldStage + change));
        _stages[stat] = newStage;

        return newStage - oldStage;
    }

    public void Reset()
    {
        foreach (var key in _stages.Keys.ToList())
        {
            _stages[key] = 0;
        }
    }
}

/// <summary>
/// Tracks damage taken for Counter/Mirror Coat calculations.
/// </summary>
public class DamageTracker
{
    public int PhysicalDamageTakenThisTurn { get; private set; }
    public int SpecialDamageTakenThisTurn { get; private set; }
    public bool WasHitWhileFocusing { get; private set; }

    public void RecordPhysicalDamage(int damage)
    {
        PhysicalDamageTakenThisTurn += damage;
    }

    public void RecordSpecialDamage(int damage)
    {
        SpecialDamageTakenThisTurn += damage;
    }

    public void MarkHitWhileFocusing()
    {
        WasHitWhileFocusing = true;
    }

    public void Reset()
    {
        PhysicalDamageTakenThisTurn = 0;
        SpecialDamageTakenThisTurn = 0;
        WasHitWhileFocusing = false;
    }
}
```

### Ejemplo 6: Constantes

```csharp
/// <summary>
/// Constants for battle system limits and thresholds.
/// </summary>
/// <remarks>
/// **Feature**: 2: Combat System
/// **Sub-Feature**: 2.6: Combat Engine
/// **Documentation**: See `docs/features/2-combat-system/2.6-combat-engine/architecture.md`
/// </remarks>
public static class BattleConstants
{
    /// <summary>
    /// Maximum number of turns before battle is considered infinite loop.
    /// </summary>
    public const int MaxTurns = 1000;

    /// <summary>
    /// Maximum number of queue iterations before considering infinite loop.
    /// </summary>
    public const int MaxQueueIterations = 1000;
}

/// <summary>
/// Constants for status condition effects.
/// </summary>
/// <remarks>
/// **Feature**: 2: Combat System
/// **Sub-Feature**: 2.7: Status Conditions
/// **Documentation**: See `docs/features/2-combat-system/2.7-status-conditions/architecture.md`
/// </remarks>
public static class StatusConstants
{
    /// <summary>
    /// Speed multiplier when paralyzed (50% speed).
    /// </summary>
    public const float ParalysisSpeedMultiplier = 0.5f;

    /// <summary>
    /// Chance to be fully paralyzed when attempting to move (25%).
    /// </summary>
    public const int ParalysisFullParalysisChance = 25;
}

/// <summary>
/// Constants for item effects.
/// </summary>
/// <remarks>
/// **Feature**: 2: Combat System
/// **Sub-Feature**: 2.9: Abilities & Items
/// **Documentation**: See `docs/features/2-combat-system/2.9-abilities-items/architecture.md`
/// </remarks>
public static class ItemConstants
{
    /// <summary>
    /// Divisor for Leftovers-style healing (1/16 of Max HP).
    /// </summary>
    public const int LeftoversHealDivisor = 16;
}
```

---

## 🛠️ Comandos Útiles para la Implementación

### Validación y Testing

```powershell
# Ejecutar todos los tests
dotnet test

# Ejecutar tests de un proyecto específico
dotnet test PokemonUltimate.Tests

# Ejecutar validación de estructura de tests
.\ai_workflow\scripts\validate-test-structure.ps1 -TestDir PokemonUltimate.Tests

# Ejecutar validación de cumplimiento FDD
.\ai_workflow\scripts\validate-fdd-compliance.ps1 -CodeDir . -FeaturesDir docs/features -MasterList docs/features_master_list.md

# Compilar y verificar warnings
dotnet build --no-restore
```

### Búsqueda de Código

```powershell
# Buscar todos los usos de Random estático
grep -r "static.*Random" PokemonUltimate.Combat

# Buscar creación de DamagePipeline
grep -r "new DamagePipeline" PokemonUltimate.Combat

# Buscar magic numbers
grep -r "\b(1000|0\.5f|16)\b" PokemonUltimate.Combat

# Buscar validaciones de slots
grep -r "IsEmpty.*HasFainted\|HasFainted.*IsEmpty" PokemonUltimate.Combat
```

### Refactorización

```powershell
# Verificar que no hay errores de compilación
dotnet build

# Ejecutar tests después de cada cambio
dotnet test --filter "FullyQualifiedName~Combat"

# Ver cobertura de tests (si está configurada)
dotnet test /p:CollectCoverage=true
```

---

## ✅ Checklist Final

### Antes de Comenzar

-   [ ] Leer todo el plan de implementación
-   [ ] Revisar tests existentes
-   [ ] Crear branch para refactorización
-   [ ] Asegurar que todos los tests pasan antes de comenzar

### Durante la Implementación

-   [ ] Ejecutar tests después de cada tarea
-   [ ] Ejecutar validación de scripts después de cada fase
-   [ ] Actualizar documentación cuando sea necesario
-   [ ] Hacer commits frecuentes y descriptivos

### Después de Cada Fase

-   [ ] Todos los tests pasan
-   [ ] Validación de scripts pasa
-   [ ] Código compila sin warnings
-   [ ] Documentación actualizada
-   [ ] Code review (si aplica)

---

## 📝 Notas sobre las Mejoras Arquitectónicas Avanzadas

Las mejoras identificadas en la sección **"Mejoras Arquitectónicas Avanzadas"** fueron descubiertas durante una revisión exhaustiva del sistema de combate considerando todos los casos posibles del combate Pokémon. Estas mejoras se enfocan en:

1. **Robustez y Validación**: Asegurar que el sistema maneje correctamente estados inválidos y casos edge
2. **Extensibilidad**: Facilitar la adición de nuevos mecanismos sin modificar código existente
3. **Mantenibilidad**: Mejorar la claridad y organización del código para facilitar el mantenimiento futuro
4. **Performance**: Optimizar operaciones críticas sin sacrificar claridad
5. **Completitud**: Implementar funcionalidades pendientes (como redirección de targets)

Estas mejoras son **opcionales** y pueden implementarse después de completar las fases críticas (Fases 0-10). Las Fases 11-14 pueden ejecutarse según las necesidades del proyecto y las prioridades del equipo.

---

**Fin del Documento**
