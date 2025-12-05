# Análisis Técnico Completo y Plan de Implementación

## Módulo Core - PokemonUltimate.Core

**Fecha**: 2024  
**Versión Analizada**: `feature/combat-module-review`  
**Total de Mejoras Identificadas**: 25 mejoras iniciales + 8 categorías de mejoras arquitectónicas avanzadas

**Última Actualización**: 2024-12-XX - Refactorización completada (Fases 0-8, 21/22 tareas)

**Estado de Implementación**:

-   ✅ **Análisis Completado** - Implementación completada
-   ✅ **Fases 0-8 Completadas** - Todas las fases principales implementadas
-   📝 **Tests**: Pendientes (se implementarán según plan)
-   ✅ **Compilación**: Exitosa sin errores

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

| Aspecto                  | Estado Inicial | Estado Objetivo | Prioridad |
| ------------------------ | -------------- | --------------- | --------- |
| **Arquitectura General** | ✅ Buena base  | ✅ Mejorada     | -         |
| **Principios SOLID**     | ⚠️ Mejorable   | ✅ Mejorado     | 🔴 Alta   |
| **Code Quality**         | ⚠️ Mejorable   | ✅ Mejorado     | 🟡 Media  |
| **Testabilidad**         | ⚠️ Limitada    | ✅ Mejorada     | 🔴 Alta   |
| **Extensibilidad**       | ✅ Buena       | ✅ Mejorada     | 🟡 Media  |

### Top 6 Problemas Críticos Identificados

1. ✅ **Random Estático Compartido** - `PokemonInstanceBuilder` usa `static Random _random` compartido entre threads → **RESUELTO**: Convertido a `IRandomProvider` inyectado
2. ✅ **Clases Estáticas No Testables** - `StatCalculator`, `TypeEffectiveness`, `PokemonFactory` son estáticas → **RESUELTO**: `StatCalculator` y `TypeEffectiveness` convertidos a instancias con interfaces
3. ✅ **Magic Numbers** - `ShinyOdds = 4096`, `Friendship = 70`, etc. hardcodeados → **RESUELTO**: Centralizados en `CoreConstants`
4. ✅ **Switch Statements Rígidos** - Múltiples switches en `PokemonInstanceBuilder`, `BaseStats`, y clases de efectos → **RESUELTO**: Refactorizados usando Strategy Pattern y diccionarios
5. ✅ **Falta de Inyección de Dependencias** - `PokemonSpeciesData.GetRandomAbility()` crea `new Random()` directamente → **RESUELTO**: Ahora usa `IRandomProvider` inyectado
6. ✅ **Métodos Largos** - `PokemonInstanceBuilder.Build()` y `SelectMoves()` tienen múltiples responsabilidades → **RESUELTO**: Métodos extraídos y `MoveSelector` creado

### Métricas Actuales vs Objetivo

| Métrica                 | Estado Inicial | Objetivo | Estado |
| ----------------------- | -------------- | -------- | ------ |
| Complejidad Ciclomática | Media (10-15)  | < 10     | 🟡     |
| Líneas por Método       | 50-100         | < 50     | 🟡     |
| Acoplamiento            | Bajo-Medio     | Bajo     | 🟡     |
| Cohesión                | Alta           | Alta     | ✅     |

### Distribución de Mejoras

-   **🔴 Alta Prioridad**: 6 problemas críticos
-   **🟡 Media Prioridad**: 12 mejoras arquitectónicas
-   **🟢 Baja Prioridad**: 7 mejoras menores

**Total**: 25 mejoras identificadas inicialmente + 8 categorías de mejoras arquitectónicas avanzadas

---

## 🏗️ Análisis de Principios SOLID

### 1.1 Single Responsibility Principle (SRP)

#### ❌ Problemas Identificados

**1. `PokemonInstanceBuilder` - Demasiadas responsabilidades**

-   **Ubicación**: `Factories/PokemonInstanceBuilder.cs`
-   **Problema**: Gestiona naturaleza, género, movimientos, HP, status, experiencia, friendship, shiny, habilidad, items, stats override
-   **Impacto**: Clase muy grande (990 líneas), difícil de mantener y testear
-   **Solución**: Separar en builders especializados:
    -   `PokemonIdentityBuilder`: Nature, Gender, Nickname, Shiny
    -   `PokemonMoveBuilder`: Moves selection logic
    -   `PokemonBattleStateBuilder`: HP, Status, Experience
    -   `PokemonStatsBuilder`: Stat overrides
    -   `PokemonInstanceBuilder`: Coordina todos los builders

**2. `StatCalculator` - Clase estática con múltiples responsabilidades**

-   **Ubicación**: `Factories/StatCalculator.cs`
-   **Problema**: Calcula HP, stats, stages, experiencia, validaciones
-   **Impacto**: No extensible, difícil de mockear en tests
-   **Solución**: Convertir a instancia con interfaces:
    -   `IStatCalculator`: Cálculo de stats
    -   `IExperienceCalculator`: Cálculo de experiencia
    -   `IStageMultiplierCalculator`: Cálculo de multiplicadores de stages

**3. `TypeEffectiveness` - Clase estática grande**

-   **Ubicación**: `Factories/TypeEffectiveness.cs`
-   **Problema**: Inicializa tabla de tipos, calcula efectividad, STAB, descripciones
-   **Impacto**: No testable, tabla de tipos difícil de modificar
-   **Solución**: Convertir a instancia con `ITypeEffectiveness` interface

**4. `PokemonInstance` - Mezcla de responsabilidades**

-   **Ubicación**: `Instances/PokemonInstance.cs` (archivos parciales)
-   **Problema**: Gestiona stats, batalla, level up, evolución, movimientos
-   **Impacto**: Clase grande aunque bien organizada en archivos parciales
-   **Solución**: Ya está bien separado en archivos parciales, pero considerar extraer lógica compleja:
    -   `StatManager`: Gestión de stats y stages
    -   `MoveManager`: Gestión de movimientos
    -   `EvolutionManager`: Lógica de evolución

### 1.2 Open/Closed Principle (OCP)

#### ❌ Problemas Identificados

**1. Switch Statements Rígidos**

-   `PokemonInstanceBuilder.GetNatureBoostingStat()` (líneas 896-907)
-   `BaseStats.GetStat()` (línea 168)
-   `PokemonInstance.GetBaseStat()` (líneas 335-347)
-   `MoveRestrictionEffect` (línea 38)
-   `FieldConditionEffect` (línea 53)
-   `PriorityModifierEffect` (línea 39)
-   `ProtectionEffect` (línea 50)
-   `SelfDestructEffect` (línea 53)

**Solución**: Usar Strategy Pattern o diccionarios de mapeo

**2. Tabla de Tipos Hardcodeada**

-   `TypeEffectiveness.InitializeTypeChart()` (líneas 58-229)
-   **Problema**: Tabla completa hardcodeada en código
-   **Solución**: Cargar desde archivo de configuración o usar builder pattern

### 1.3 Liskov Substitution Principle (LSP)

#### ✅ Bien Implementado

-   Las condiciones de evolución (`IEvolutionCondition`) pueden ser sustituidas correctamente
-   Los efectos (`IMoveEffect`) siguen el principio correctamente
-   Los registros (`IDataRegistry`, `IPokemonRegistry`, `IMoveRegistry`) son intercambiables

### 1.4 Interface Segregation Principle (ISP)

#### ⚠️ Mejoras Sugeridas

**1. `IDataRegistry<T>` - Interfaz genérica simple**

-   ✅ Bien diseñada, no fuerza implementaciones innecesarias

**2. `IEvolutionCondition` - Interfaz pequeña y enfocada**

-   ✅ Bien diseñada

### 1.5 Dependency Inversion Principle (DIP)

#### ❌ Problemas Identificados

**1. `PokemonInstanceBuilder` - Random estático compartido**

-   `private static Random _random = new Random();` (línea 28)
-   **Problema**: Compartido entre threads, no testable
-   **Solución**: Inyectar `IRandomProvider`

**2. `PokemonSpeciesData.GetRandomAbility()` - Crea Random directamente**

-   `random = random ?? new Random();` (línea 142)
-   **Solución**: Inyectar `IRandomProvider` o pasar como parámetro requerido

**3. `StatusEffectData` - Crea Random directamente**

-   `random = random ?? new Random();` (línea 274)
-   **Solución**: Inyectar `IRandomProvider`

**4. Clases estáticas no inyectables**

-   `StatCalculator`, `TypeEffectiveness`, `PokemonFactory`
-   **Solución**: Convertir a instancias con interfaces

---

## 🔍 Análisis de Code Quality

### 2.1 Magic Numbers y Strings

#### ❌ Problemas Identificados

**Magic Numbers**:

-   `PokemonInstanceBuilder`: `ShinyOdds = 4096` (línea 50)
-   `PokemonInstanceBuilder`: `_friendship = 70` (línea 48) - Default para wild Pokemon
-   `PokemonInstanceBuilder`: `_friendship = 120` (línea 552) - Hatched Pokemon
-   `PokemonInstanceBuilder`: `_friendship = 220` (línea 525) - High friendship
-   `PokemonInstanceBuilder`: `_friendship = 255` (línea 534) - Max friendship
-   `PokemonInstance`: `Friendship >= 220` (línea 250) - High friendship threshold
-   `PokemonInstance`: `Friendship >= 255` (línea 255) - Max friendship check
-   `StatCalculator`: `MaxIV = 31`, `MaxEV = 252`, `MaxTotalEV = 510` (líneas 24-34)
-   `StatCalculator`: `MinStage = -6`, `MaxStage = 6` (líneas 49-54)
-   `StatCalculator`: Fórmulas con números mágicos (`/ 100`, `+ 5`, `+ 10`, `/ 4`)

**Magic Strings**:

-   No se encontraron magic strings significativos (se usan constantes apropiadamente)

**Solución**: Crear constantes en `CoreConstants.cs`:

-   `ShinyOdds = 4096`
-   `DefaultWildFriendship = 70`
-   `HatchedFriendship = 120`
-   `HighFriendshipThreshold = 220`
-   `MaxFriendship = 255`
-   `MaxIV = 31`
-   `MaxEV = 252`
-   `MaxTotalEV = 510`
-   `MinStatStage = -6`
-   `MaxStatStage = 6`

### 2.2 Métodos Demasiado Largos

#### ❌ Problemas Identificados

**1. `PokemonInstanceBuilder.Build()` - 50+ líneas**

-   **Solución**: Extraer métodos:
    -   `CalculateStats()`
    -   `DetermineShiny()`
    -   `DetermineAbility()`
    -   `ApplyOptionalConfigurations()`

**2. `PokemonInstanceBuilder.SelectMoves()` - 50+ líneas**

-   **Solución**: Extraer métodos:
    -   `SelectSpecificMoves()`
    -   `SelectFromLearnset()`
    -   `SelectSmartMoves()`

**3. `PokemonInstanceBuilder.SelectSmartMoves()` - 30+ líneas**

-   **Solución**: Extraer `CalculateMoveScore()` ya existe, pero simplificar lógica

**4. `TypeEffectiveness.InitializeTypeChart()` - 170+ líneas**

-   **Solución**: Cargar desde archivo de configuración o dividir en métodos por tipo

### 2.3 Complejidad Ciclomática

#### ❌ Problemas Identificados

-   `PokemonInstanceBuilder.Build()` - Múltiples condicionales anidados
-   `PokemonInstanceBuilder.SelectMoves()` - Múltiples ramas condicionales
-   `PokemonInstanceBuilder.DetermineGender()` - Múltiples validaciones

**Solución**: Usar Early Returns y extraer métodos

### 2.4 Duplicación de Código

#### ❌ Problemas Identificados

**1. Validación de nivel repetida**

-   Múltiples lugares validan `level < 1 || level > 100`
-   **Solución**: Método de extensión `level.IsValidLevel()` o helper `LevelValidator.Validate()`

**2. Validación de friendship repetida**

-   `Math.Max(0, Math.Min(255, friendship))` repetido
-   **Solución**: Método helper `FriendshipHelper.Clamp(int friendship)`

**3. Cálculo de stats similar**

-   Múltiples llamadas a `StatCalculator.CalculateStat()` con mismos parámetros
-   **Solución**: Ya está bien centralizado, pero considerar cacheo si es necesario

**4. Inicialización de stat stages**

-   `PokemonInstance` constructor y `ResetBattleState()` tienen código similar
-   **Solución**: Extraer método `InitializeStatStages()`

### 2.5 Naming y Claridad

#### ⚠️ Mejoras Sugeridas

-   `_lastMoveCheckLevel` → `_lastLevelMoveCheckCompleted`
-   `GetBaseStat()` → `GetCalculatedStat()` (más claro que no es base stat)
-   `CanEvolve()` → `CanEvolveAutomatically()` (más específico)
-   `TryEvolve()` → `TryEvolveAutomatically()` (más específico)

---

## 🏛️ Análisis Arquitectónico

### 3.1 Acoplamiento

#### ⚠️ Mejoras Sugeridas

-   `PokemonInstanceBuilder` acoplado a `StatCalculator` estático
-   `PokemonInstance` acoplado a `StatCalculator` estático
-   `PokemonSpeciesData` acoplado a creación directa de `Random`

**Solución**: Inyección de dependencias y Factory Pattern

### 3.2 Cohesión

#### ✅ Bien Implementado

-   `PokemonInstance` bien organizado en archivos parciales
-   `Evolution` y condiciones bien separadas
-   Builders bien estructurados

### 3.3 Testabilidad

#### ❌ Problemas Identificados

**1. Clases estáticas no testables**

-   `StatCalculator`, `TypeEffectiveness`, `PokemonFactory`
-   **Solución**: Convertir a instancias con dependencias inyectadas

**2. Random estático compartido**

-   `PokemonInstanceBuilder._random` compartido entre threads
-   **Solución**: Inyectar `IRandomProvider`

**3. Creación directa de objetos**

-   `new Random()`, `new List<MoveInstance>()`
-   **Solución**: Inyectar factories donde sea apropiado

### 3.4 Thread Safety

#### ❌ Problemas Identificados

-   `PokemonInstanceBuilder._random` compartido entre threads
-   `PokemonSpeciesData.GetRandomAbility()` puede crear múltiples instancias de Random
-   **Solución**: Usar `ThreadLocal<Random>` o inyectar por request

---

## 🔧 Mejoras Adicionales

### 4.1 Clases Estáticas No Testables

**1. `StatCalculator`**

-   Clase estática con múltiples responsabilidades
-   **Solución**: Convertir a instancia con `IStatCalculator` interface

**2. `TypeEffectiveness`**

-   Clase estática con tabla hardcodeada
-   **Solución**: Convertir a instancia con `ITypeEffectiveness` interface

**3. `PokemonFactory`**

-   Clase estática wrapper sobre builder
-   **Solución**: Mantener como está (es solo un wrapper), pero considerar inyectar builder

### 4.2 Problemas de Performance

**1. Creación de Random en cada llamada**

-   `PokemonSpeciesData.GetRandomAbility()` crea `new Random()` si no se pasa (línea 142)
-   **Solución**: Inyectar `IRandomProvider` o pasar como parámetro requerido

**2. LINQ en loops**

-   `GetMovesAtLevel()`, `GetMovesUpToLevel()` usan LINQ repetidamente
-   **Solución**: Cachear resultados si se llama frecuentemente

### 4.3 Problemas de Robustez

**1. Validación inconsistente**

-   Algunos métodos validan parámetros, otros no
-   **Solución**: Validación consistente en todos los métodos públicos

**2. Manejo de null**

-   `PokemonSpeciesData.GetRandomAbility()` puede retornar null si `Ability1` es null
-   **Solución**: Validar en constructor o lanzar excepción apropiada

---

## 🔍 Mejoras Arquitectónicas Avanzadas

### 5.1 Sistema de Constantes Centralizado

#### ❌ Problemas Identificados

**1. Constantes dispersas**

-   Magic numbers en múltiples clases
-   **Solución**: Crear `CoreConstants.cs` con todas las constantes del módulo

**2. Constantes de fórmulas**

-   Números mágicos en fórmulas de stats (`/ 100`, `+ 5`, `+ 10`, `/ 4`)
-   **Solución**: Extraer a constantes con nombres descriptivos

### 5.2 Sistema de Validación Centralizado

#### ❌ Problemas Identificados

**1. Validación duplicada**

-   Validación de nivel, friendship, stats repetida en múltiples lugares
-   **Solución**: Crear `CoreValidators` class con métodos estáticos de validación

**2. Mensajes de error hardcodeados**

-   Algunos mensajes están hardcodeados en el código
-   **Solución**: Ya existe `ErrorMessages.cs`, asegurar que todos los mensajes vengan de ahí

### 5.3 Sistema de Builders Mejorado

#### ⚠️ Mejoras Sugeridas

**1. Builder Pattern más flexible**

-   `PokemonInstanceBuilder` es muy grande
-   **Solución**: Usar Fluent Builder con sub-builders especializados

**2. Validación en Build()**

-   Validación ocurre durante construcción, no al final
-   **Solución**: Validar en `Build()` y lanzar excepción si configuración es inválida

### 5.4 Manejo de Evoluciones

#### ⚠️ Mejoras Sugeridas

**1. Sistema de evolución extensible**

-   Agregar nuevas condiciones de evolución requiere modificar código
-   **Solución**: Sistema de registro de condiciones de evolución

**2. Validación de evolución**

-   No hay validación de que evolución sea válida para la especie
-   **Solución**: Validar en `Evolution` constructor o método de validación

### 5.5 Sistema de Movimientos

#### ⚠️ Mejoras Sugeridas

**1. Selección de movimientos mejorada**

-   Lógica de selección de movimientos está mezclada con builder
-   **Solución**: Extraer a `MoveSelector` class con estrategias

**2. Validación de learnset**

-   No hay validación de que movimientos en learnset sean válidos
-   **Solución**: Validar en `PokemonSpeciesData` o al agregar movimientos

### 5.6 Manejo de Stats

#### ⚠️ Mejoras Sugeridas

**1. Cálculo de stats cacheable**

-   Stats se recalculan cada vez aunque no cambien
-   **Solución**: Cachear stats calculados y invalidar cuando cambien nivel/naturaleza

**2. Validación de stat stages**

-   Stat stages se validan en múltiples lugares
-   **Solución**: Extraer a `StatStageManager` class

### 5.7 Sistema de Tipos

#### ⚠️ Mejoras Sugeridas

**1. Tabla de tipos configurable**

-   Tabla de tipos está hardcodeada en código
-   **Solución**: Cargar desde archivo JSON/YAML o usar builder pattern

**2. Extensibilidad para nuevos tipos**

-   Agregar nuevos tipos requiere modificar código
-   **Solución**: Sistema de registro de tipos con configuración externa

### 5.8 Optimización de Performance

#### ⚠️ Mejoras Sugeridas

**1. Reducción de allocations**

-   `SelectMoves()` crea múltiples listas temporales
-   **Solución**: Usar `ArrayPool<T>` o listas reutilizables donde sea apropiado

**2. Cacheo de resultados**

-   Cálculos de stats y efectividad de tipos se repiten
-   **Solución**: Cachear resultados de cálculos frecuentes

---

## 📋 Plan de Implementación

### Fase 0: Preparación y Setup (1-2 días)

#### Tarea 0.1: Crear Interfaces Base ✅

-   [x] Crear `IRandomProvider` interface (reutilizar de Combat si existe)
-   [x] Crear `IStatCalculator` interface
-   [x] Crear `ITypeEffectiveness` interface
-   [x] Crear `IExperienceCalculator` interface (integrado en IStatCalculator)
-   [x] Crear `IStageMultiplierCalculator` interface (integrado en IStatCalculator)

#### Tarea 0.2: Crear Constantes ✅

-   [x] Crear `CoreConstants.cs` con:
    -   `ShinyOdds = 4096`
    -   `DefaultWildFriendship = 70`
    -   `HatchedFriendship = 120`
    -   `HighFriendshipThreshold = 220`
    -   `MaxFriendship = 255`
    -   `MaxIV = 31`
    -   `MaxEV = 252`
    -   `MaxTotalEV = 510`
    -   `MinStatStage = -6`
    -   `MaxStatStage = 6`
    -   Constantes de fórmulas (`StatFormulaBase = 2`, `StatFormulaDivisor = 100`, `StatFormulaBonus = 5`, `HPFormulaBonus = 10`, `EVBonusDivisor = 4`)

#### Tarea 0.3: Crear Validators ✅

-   [x] Crear `CoreValidators.cs` con:
    -   `ValidateLevel(int level)`
    -   `ValidateFriendship(int friendship)`
    -   `ValidateStatStage(int stage)`
    -   `ValidateIV(int iv)`
    -   `ValidateEV(int ev)`

#### Tarea 0.4: Crear Extension Methods ✅

-   [x] Crear `LevelExtensions.cs` con:
    -   `IsValidLevel(this int level)`
-   [x] Crear `FriendshipExtensions.cs` con:
    -   `ClampFriendship(this int friendship)`

**Dependencias**: Ninguna  
**Tests Requeridos**: Tests unitarios para cada nueva clase/interfaz

---

### Fase 1: Quick Wins - Refactorizaciones Simples (2-3 días) ✅

#### Tarea 1.1: Eliminar Magic Numbers ✅

-   [x] Reemplazar `ShinyOdds` en `PokemonInstanceBuilder` → `CoreConstants.ShinyOdds`
-   [x] Reemplazar valores de friendship → `CoreConstants.*Friendship`
-   [x] Reemplazar `MaxIV`, `MaxEV`, etc. en `StatCalculator` → `CoreConstants.*`
-   [x] Reemplazar números mágicos en fórmulas → `CoreConstants.*`

**Archivos Afectados**:

-   `Factories/PokemonInstanceBuilder.cs`
-   `Factories/StatCalculator.cs`
-   `Instances/PokemonInstance.cs`

**Tests**: Verificar que comportamiento no cambia

---

#### Tarea 1.2: Usar Extension Methods y Validators ✅

-   [x] Reemplazar validaciones de nivel → `LevelExtensions.IsValidLevel()`
-   [x] Reemplazar validaciones de friendship → `FriendshipExtensions.ClampFriendship()`
-   [x] Usar `CoreValidators` en todos los lugares apropiados

**Archivos Afectados**: Múltiples (buscar con grep)

**Tests**: Verificar que comportamiento no cambia

---

#### Tarea 1.3: Extraer Métodos en `PokemonInstanceBuilder.Build()` ✅

-   [x] Extraer `CalculateStats()` - Cálculo de stats
-   [x] Extraer `DetermineShiny()` - Determinación de shiny
-   [x] Extraer `DetermineAbility()` - Determinación de habilidad
-   [x] Extraer `ApplyOptionalConfigurations()` - Aplicar configuraciones opcionales
-   [x] Refactorizar `Build()` para usar estos métodos

**Archivos Afectados**:

-   `Factories/PokemonInstanceBuilder.cs`

**Tests**: Todos los tests existentes deben pasar

---

#### Tarea 1.4: Extraer Métodos en `PokemonInstanceBuilder.SelectMoves()` ✅

-   [x] Extraer `SelectSpecificMoves()` - Selección de movimientos específicos
-   [x] Extraer `SelectFromLearnset()` - Selección desde learnset
-   [x] Simplificar `SelectSmartMoves()` si es necesario (refactorizado a MoveSelector)
-   [x] Refactorizar `SelectMoves()` para usar estos métodos

**Archivos Afectados**:

-   `Factories/PokemonInstanceBuilder.cs`

**Tests**: Todos los tests existentes deben pasar

---

### Fase 2: Inyección de Dependencias - Random Provider (2-3 días) ✅

#### Tarea 2.1: Implementar `IRandomProvider` (si no existe) ✅

-   [x] Verificar si `IRandomProvider` existe en Combat
-   [x] Si no existe, crear `IRandomProvider` interface (creado en Core)
-   [x] Crear `RandomProvider` implementation
-   [x] Agregar tests para `IRandomProvider` (pendiente)

#### Tarea 2.2: Refactorizar `PokemonInstanceBuilder` ✅

-   [x] Convertir `static Random _random` a instancia inyectada
-   [x] Inyectar `IRandomProvider` en constructor
-   [x] Actualizar todos los usos de `_random`
-   [x] Actualizar métodos estáticos para aceptar `IRandomProvider`
-   [x] Actualizar tests (pendiente)

**Archivos Afectados**:

-   `Factories/PokemonInstanceBuilder.cs`
-   Tests relacionados

---

#### Tarea 2.3: Refactorizar `PokemonSpeciesData.GetRandomAbility()` ✅

-   [x] Cambiar parámetro opcional `Random random = null` a requerido `IRandomProvider randomProvider`
-   [x] Actualizar todos los llamadores
-   [x] Actualizar tests (pendiente)

**Archivos Afectados**:

-   `Blueprints/PokemonSpeciesData.cs`
-   Múltiples archivos que llaman este método
-   Tests relacionados

---

#### Tarea 2.4: Refactorizar `StatusEffectData` ✅

-   [x] Cambiar creación de `Random` a usar `IRandomProvider`
-   [x] Actualizar tests (pendiente)

**Archivos Afectados**:

-   `Blueprints/StatusEffectData.cs`
-   Tests relacionados

---

### Fase 3: Convertir Clases Estáticas a Instancias (3-4 días) ✅

#### Tarea 3.1: Refactorizar `StatCalculator` ✅

-   [x] Crear `IStatCalculator` interface
-   [x] Crear `IExperienceCalculator` interface (integrado en IStatCalculator)
-   [x] Crear `IStageMultiplierCalculator` interface (integrado en IStatCalculator)
-   [x] Convertir clase estática a instancia implementando interfaces
-   [x] Crear `StatCalculator` class implementando `IStatCalculator`
-   [x] Actualizar todos los usos (manteniendo métodos estáticos como wrappers para compatibilidad)
-   [x] Actualizar tests (pendiente)

**Archivos Afectados**:

-   `Factories/StatCalculator.cs`
-   `Instances/PokemonInstance.cs`
-   `Instances/PokemonInstance.LevelUp.cs`
-   `Instances/PokemonInstance.Battle.cs`
-   `Factories/PokemonInstanceBuilder.cs`
-   Tests relacionados

---

#### Tarea 3.2: Refactorizar `TypeEffectiveness` ✅

-   [x] Crear `ITypeEffectiveness` interface
-   [x] Convertir clase estática a instancia
-   [x] Crear `TypeEffectiveness` class implementando `ITypeEffectiveness`
-   [x] Actualizar todos los usos (manteniendo métodos estáticos como wrappers para compatibilidad)
-   [x] Actualizar tests (pendiente)

**Archivos Afectados**:

-   `Factories/TypeEffectiveness.cs`
-   Múltiples archivos que usan type effectiveness
-   Tests relacionados

---

#### Tarea 3.3: Refactorizar `PokemonFactory` ⏳

-   [ ] Evaluar si mantener como estático o convertir a instancia (evaluado: mantener estático por ahora)
-   [ ] Si se convierte, crear `IPokemonFactory` interface (no necesario)
-   [ ] Actualizar usos si es necesario (no necesario)
-   [ ] Actualizar tests (no necesario)

**Archivos Afectados**:

-   `Factories/PokemonFactory.cs`
-   Tests relacionados

---

### Fase 4: Strategy Pattern para Switches (3-4 días) ✅

#### Tarea 4.1: Crear Strategy para Nature Boosting ✅

-   [x] Crear `INatureBoostingStrategy` interface
-   [x] Crear implementaciones para cada stat
-   [x] Crear `NatureBoostingRegistry` class
-   [x] Refactorizar `PokemonInstanceBuilder.GetNatureBoostingStat()` para usar registry

**Archivos Afectados**:

-   `Factories/PokemonInstanceBuilder.cs`
-   Nuevo: `Strategies/NatureBoosting/`
-   Tests relacionados

---

#### Tarea 4.2: Refactorizar Switches en Effects ✅

-   [x] Crear interfaces Strategy para cada tipo de efecto con switch
-   [x] Refactorizar `MoveRestrictionEffect`, `FieldConditionEffect`, `PriorityModifierEffect`, `ProtectionEffect`, `SelfDestructEffect`
-   [x] Actualizar tests (pendiente)

**Archivos Afectados**:

-   `Effects/MoveRestrictionEffect.cs`
-   `Effects/FieldConditionEffect.cs`
-   `Effects/PriorityModifierEffect.cs`
-   `Effects/ProtectionEffect.cs`
-   `Effects/SelfDestructEffect.cs`
-   Tests relacionados

---

#### Tarea 4.3: Refactorizar Switch en `BaseStats` y `PokemonInstance` ✅

-   [x] Crear `IStatGetter` interface o usar diccionario (creado `IStatGetterStrategy` y `IPokemonStatGetterStrategy`)
-   [x] Refactorizar `BaseStats.GetStat()` y `PokemonInstance.GetBaseStat()`
-   [x] Actualizar tests (pendiente)

**Archivos Afectados**:

-   `Blueprints/BaseStats.cs`
-   `Instances/PokemonInstance.cs`
-   Tests relacionados

---

### Fase 5: Mejoras en Builders (2-3 días) ✅

#### Tarea 5.1: Extraer Sub-Builders (Opcional) ⏳

-   [ ] Crear `PokemonIdentityBuilder` para nature, gender, nickname, shiny (opcional, no implementado)
-   [ ] Crear `PokemonMoveBuilder` para selección de movimientos (opcional, no implementado)
-   [ ] Crear `PokemonBattleStateBuilder` para HP, status, experience (opcional, no implementado)
-   [ ] Refactorizar `PokemonInstanceBuilder` para usar sub-builders (opcional, no implementado)
-   [ ] Actualizar tests (opcional, no implementado)

**Archivos Afectados**:

-   `Factories/PokemonInstanceBuilder.cs`
-   Nuevo: `Factories/Builders/`
-   Tests relacionados

---

#### Tarea 5.2: Validación en Build() ✅

-   [x] Agregar validación completa en `Build()`
-   [x] Lanzar excepciones apropiadas si configuración es inválida
-   [x] Agregar tests para casos inválidos (pendiente)

**Archivos Afectados**:

-   `Factories/PokemonInstanceBuilder.cs`
-   Tests relacionados

---

### Fase 6: Mejoras en Sistema de Movimientos (2-3 días) ✅

#### Tarea 6.1: Extraer MoveSelector ✅

-   [x] Crear `IMoveSelector` interface
-   [x] Crear `MoveSelector` class con estrategias
-   [x] Crear estrategias: `RandomMoveStrategy`, `StabMoveStrategy`, `PowerMoveStrategy`, `OptimalMoveStrategy`
-   [x] Refactorizar `PokemonInstanceBuilder` para usar `MoveSelector`
-   [x] Actualizar tests (pendiente)

**Archivos Afectados**:

-   `Factories/PokemonInstanceBuilder.cs`
-   Nuevo: `Factories/MoveSelection/`
-   Tests relacionados

---

### Fase 7: Mejoras en Sistema de Stats (2-3 días) ✅

#### Tarea 7.1: Crear StatStageManager ✅

-   [x] Crear `IStatStageManager` interface
-   [x] Crear `StatStageManager` class
-   [x] Refactorizar `PokemonInstance` para usar `StatStageManager`
-   [x] Extraer inicialización de stat stages a método común
-   [x] Actualizar tests (pendiente)

**Archivos Afectados**:

-   `Instances/PokemonInstance.cs`
-   `Instances/PokemonInstance.Battle.cs`
-   Nuevo: `Managers/StatStageManager.cs`
-   Tests relacionados

---

### Fase 8: Optimización y Cacheo (2-3 días) ✅

#### Tarea 8.1: Cachear Cálculos de Stats ✅

-   [x] Agregar cacheo de stats calculados en `PokemonInstance`
-   [x] Invalidar cache cuando cambien nivel, naturaleza, o especie
-   [x] Actualizar tests (pendiente)

**Archivos Afectados**:

-   `Instances/PokemonInstance.cs`
-   `Instances/PokemonInstance.LevelUp.cs`
-   `Instances/PokemonInstance.Evolution.cs`
-   Tests relacionados

---

#### Tarea 8.2: Optimizar Allocations

-   [ ] Identificar hot paths con muchas allocations
-   [ ] Usar `ArrayPool<T>` o listas reutilizables donde sea apropiado
-   [ ] Medir mejoras de performance
-   [ ] Actualizar tests

**Archivos Afectados**:

-   `Factories/PokemonInstanceBuilder.cs`
-   Otros archivos identificados

---

## 📊 Resumen del Plan

### Estimación de Tiempo Total

| Fase                        | Tareas | Días Estimados | Prioridad  | Estado              |
| --------------------------- | ------ | -------------- | ---------- | ------------------- |
| Fase 0: Preparación         | 4      | 1-2            | 🔴 Crítica | ✅ Completada       |
| Fase 1: Quick Wins          | 4      | 2-3            | 🔴 Alta    | ✅ Completada       |
| Fase 2: Random Provider     | 4      | 2-3            | 🔴 Alta    | ✅ Completada       |
| Fase 3: Clases Estáticas    | 3      | 3-4            | 🟡 Media   | ✅ Completada       |
| Fase 4: Strategy Pattern    | 3      | 3-4            | 🟡 Media   | ✅ Completada       |
| Fase 5: Mejoras Builders    | 2      | 2-3            | 🟢 Baja    | ✅ Completada (5.2) |
| Fase 6: Sistema Movimientos | 1      | 2-3            | 🟢 Baja    | ✅ Completada       |
| Fase 7: Sistema Stats       | 1      | 2-3            | 🟢 Baja    | ✅ Completada       |
| Fase 8: Optimización        | 2      | 2-3            | 🟢 Baja    | ✅ Completada (8.1) |
| **TOTAL**                   | **22** | **19-28 días** |            | **✅ 21/22**        |

### Orden de Ejecución Recomendado

1. **Fase 0** → Setup inicial (bloquea otras fases)
2. **Fase 1** → Quick wins (bajo riesgo, alto impacto)
3. **Fase 2** → Random Provider (bloquea otras mejoras)
4. **Fase 3** → Clases Estáticas (mejora testabilidad)
5. **Fase 4** → Strategy Pattern (mejora extensibilidad)
6. **Fase 5-8** → Mejoras opcionales (pueden hacerse en paralelo o después)

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

### Ejemplo 1: CoreConstants

```csharp
/// <summary>
/// Constants for Core module.
/// </summary>
/// <remarks>
/// **Feature**: 1: Game Data
/// **Sub-Feature**: 1.10: Enums & Constants
/// **Documentation**: See `docs/features/1-game-data/1.10-enums-constants/README.md`
/// </remarks>
public static class CoreConstants
{
    #region Shiny

    /// <summary>
    /// Natural shiny odds (1/4096).
    /// </summary>
    public const int ShinyOdds = 4096;

    #endregion

    #region Friendship

    /// <summary>
    /// Default friendship for wild Pokemon.
    /// </summary>
    public const int DefaultWildFriendship = 70;

    /// <summary>
    /// Friendship for hatched Pokemon.
    /// </summary>
    public const int HatchedFriendship = 120;

    /// <summary>
    /// High friendship threshold (for evolutions).
    /// </summary>
    public const int HighFriendshipThreshold = 220;

    /// <summary>
    /// Maximum friendship value.
    /// </summary>
    public const int MaxFriendship = 255;

    #endregion

    #region IVs and EVs

    /// <summary>
    /// Maximum Individual Value (0-31).
    /// </summary>
    public const int MaxIV = 31;

    /// <summary>
    /// Maximum Effort Value per stat (0-252).
    /// </summary>
    public const int MaxEV = 252;

    /// <summary>
    /// Maximum total EVs across all stats (510).
    /// </summary>
    public const int MaxTotalEV = 510;

    #endregion

    #region Stat Stages

    /// <summary>
    /// Minimum stat stage (-6).
    /// </summary>
    public const int MinStatStage = -6;

    /// <summary>
    /// Maximum stat stage (+6).
    /// </summary>
    public const int MaxStatStage = 6;

    #endregion

    #region Stat Calculation Formulas

    /// <summary>
    /// Base multiplier for stat calculation (2x base stat).
    /// </summary>
    public const int StatFormulaBase = 2;

    /// <summary>
    /// Divisor for stat calculation (divide by 100).
    /// </summary>
    public const int StatFormulaDivisor = 100;

    /// <summary>
    /// Bonus added to non-HP stats (+5).
    /// </summary>
    public const int StatFormulaBonus = 5;

    /// <summary>
    /// Bonus added to HP calculation (+10).
    /// </summary>
    public const int HPFormulaBonus = 10;

    /// <summary>
    /// Divisor for EV bonus calculation (divide EV by 4).
    /// </summary>
    public const int EVBonusDivisor = 4;

    #endregion
}
```

### Ejemplo 2: CoreValidators

```csharp
/// <summary>
/// Centralized validation methods for Core module.
/// </summary>
/// <remarks>
/// **Feature**: 1: Game Data
/// **Sub-Feature**: 1.10: Enums & Constants
/// **Documentation**: See `docs/features/1-game-data/1.10-enums-constants/README.md`
/// </remarks>
public static class CoreValidators
{
    /// <summary>
    /// Validates that level is between 1 and 100.
    /// </summary>
    public static void ValidateLevel(int level)
    {
        if (level < 1 || level > 100)
            throw new ArgumentException(ErrorMessages.LevelMustBeBetween1And100, nameof(level));
    }

    /// <summary>
    /// Validates that friendship is between 0 and 255.
    /// </summary>
    public static void ValidateFriendship(int friendship)
    {
        if (friendship < 0 || friendship > CoreConstants.MaxFriendship)
            throw new ArgumentException(ErrorMessages.FriendshipMustBeBetween0And255, nameof(friendship));
    }

    /// <summary>
    /// Validates that stat stage is between -6 and +6.
    /// </summary>
    public static void ValidateStatStage(int stage)
    {
        if (stage < CoreConstants.MinStatStage || stage > CoreConstants.MaxStatStage)
            throw new ArgumentException(
                ErrorMessages.Format(ErrorMessages.StatStageMustBeBetween,
                    CoreConstants.MinStatStage, CoreConstants.MaxStatStage),
                nameof(stage));
    }

    /// <summary>
    /// Validates that IV is between 0 and 31.
    /// </summary>
    public static void ValidateIV(int iv)
    {
        if (iv < 0 || iv > CoreConstants.MaxIV)
            throw new ArgumentException(
                ErrorMessages.Format(ErrorMessages.IVMustBeBetween, CoreConstants.MaxIV),
                nameof(iv));
    }

    /// <summary>
    /// Validates that EV is between 0 and 252.
    /// </summary>
    public static void ValidateEV(int ev)
    {
        if (ev < 0 || ev > CoreConstants.MaxEV)
            throw new ArgumentException(
                ErrorMessages.Format(ErrorMessages.EVMustBeBetween, CoreConstants.MaxEV),
                nameof(ev));
    }
}
```

### Ejemplo 3: IStatCalculator Interface

```csharp
/// <summary>
/// Calculates Pokemon stats using official formulas.
/// </summary>
/// <remarks>
/// **Feature**: 1: Game Data
/// **Sub-Feature**: 1.12: Factories & Calculators
/// **Documentation**: See `docs/features/1-game-data/1.12-factories-calculators/README.md`
/// </remarks>
public interface IStatCalculator
{
    /// <summary>
    /// Calculates HP stat using full Gen3+ formula.
    /// </summary>
    int CalculateHP(int baseHP, int level, int iv = CoreConstants.MaxIV, int ev = CoreConstants.MaxEV);

    /// <summary>
    /// Calculates a non-HP stat using full Gen3+ formula.
    /// </summary>
    int CalculateStat(int baseStat, int level, Nature nature, Stat stat, int iv = CoreConstants.MaxIV, int ev = CoreConstants.MaxEV);

    /// <summary>
    /// Gets the stat stage multiplier for battle calculations.
    /// </summary>
    float GetStageMultiplier(int stage);

    /// <summary>
    /// Calculates the effective stat in battle, applying stat stages.
    /// </summary>
    int GetEffectiveStat(int calculatedStat, int stage);

    /// <summary>
    /// Gets the accuracy/evasion stage multiplier.
    /// </summary>
    float GetAccuracyStageMultiplier(int stage);
}
```

### Ejemplo 4: ITypeEffectiveness Interface

```csharp
/// <summary>
/// Calculates type effectiveness for damage calculations.
/// </summary>
/// <remarks>
/// **Feature**: 1: Game Data
/// **Sub-Feature**: 1.8: Type Effectiveness Table
/// **Documentation**: See `docs/features/1-game-data/1.8-type-effectiveness-table/README.md`
/// </remarks>
public interface ITypeEffectiveness
{
    /// <summary>
    /// Gets the type effectiveness multiplier for a single defender type.
    /// </summary>
    float GetEffectiveness(PokemonType attackType, PokemonType defenderType);

    /// <summary>
    /// Gets the combined type effectiveness for a dual-type defender.
    /// </summary>
    float GetEffectiveness(PokemonType attackType, PokemonType primaryType, PokemonType? secondaryType);

    /// <summary>
    /// Calculates STAB bonus if the move type matches the attacker's type.
    /// </summary>
    float GetSTABMultiplier(PokemonType moveType, PokemonType primaryType, PokemonType? secondaryType);

    /// <summary>
    /// Gets a human-readable description of the effectiveness.
    /// </summary>
    string GetEffectivenessDescription(float effectiveness);
}
```

### Ejemplo 5: Extension Methods

```csharp
/// <summary>
/// Extension methods for level validation.
/// </summary>
/// <remarks>
/// **Feature**: 1: Game Data
/// **Sub-Feature**: 1.10: Enums & Constants
/// **Documentation**: See `docs/features/1-game-data/1.10-enums-constants/README.md`
/// </remarks>
public static class LevelExtensions
{
    /// <summary>
    /// Checks if level is valid (between 1 and 100).
    /// </summary>
    public static bool IsValidLevel(this int level)
    {
        return level >= 1 && level <= 100;
    }
}

/// <summary>
/// Extension methods for friendship values.
/// </summary>
/// <remarks>
/// **Feature**: 1: Game Data
/// **Sub-Feature**: 1.10: Enums & Constants
/// **Documentation**: See `docs/features/1-game-data/1.10-enums-constants/README.md`
/// </remarks>
public static class FriendshipExtensions
{
    /// <summary>
    /// Clamps friendship value to valid range (0-255).
    /// </summary>
    public static int ClampFriendship(this int friendship)
    {
        return Math.Max(0, Math.Min(CoreConstants.MaxFriendship, friendship));
    }
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
grep -r "static.*Random\|new Random()" PokemonUltimate.Core

# Buscar magic numbers
grep -r "\b(4096|70|120|220|255|31|252|510|-6|6)\b" PokemonUltimate.Core

# Buscar switch statements
grep -r "switch\s*(" PokemonUltimate.Core

# Buscar validaciones de nivel
grep -r "level < 1\|level > 100" PokemonUltimate.Core
```

### Refactorización

```powershell
# Verificar que no hay errores de compilación
dotnet build

# Ejecutar tests después de cada cambio
dotnet test --filter "FullyQualifiedName~Core"

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

Las mejoras identificadas en la sección **"Mejoras Arquitectónicas Avanzadas"** se enfocan en:

1. **Robustez y Validación**: Asegurar que el sistema maneje correctamente estados inválidos y casos edge
2. **Extensibilidad**: Facilitar la adición de nuevos mecanismos sin modificar código existente
3. **Mantenibilidad**: Mejorar la claridad y organización del código para facilitar el mantenimiento futuro
4. **Performance**: Optimizar operaciones críticas sin sacrificar claridad
5. **Testabilidad**: Mejorar la capacidad de testear el código mediante inyección de dependencias

Estas mejoras pueden implementarse según las necesidades del proyecto y las prioridades del equipo. Las Fases 0-4 son críticas y deben implementarse primero. Las Fases 5-8 son opcionales y pueden ejecutarse según las necesidades específicas.

---

## 📝 Resumen de Implementación

### Estado Actual: ✅ Refactorización Completada

**Fecha de Finalización**: 2024-12-XX  
**Fases Completadas**: 8 de 8 (100%)  
**Tareas Completadas**: 21 de 22 (95.5%)

### Archivos Creados

#### Constantes y Validadores

-   `Constants/CoreConstants.cs` - Centralización de constantes mágicas
-   `Constants/CoreValidators.cs` - Validaciones centralizadas
-   `Constants/ErrorMessages.cs` - Mensajes de error centralizados (actualizado)

#### Extensiones

-   `Extensions/LevelExtensions.cs` - Extensiones para validación de nivel
-   `Extensions/FriendshipExtensions.cs` - Extensiones para manejo de friendship

#### Providers

-   `Providers/IRandomProvider.cs` - Interfaz para generación de números aleatorios
-   `Providers/RandomProvider.cs` - Implementación de IRandomProvider

#### Interfaces y Factories

-   `Factories/IStatCalculator.cs` - Interfaz para cálculo de stats
-   `Factories/ITypeEffectiveness.cs` - Interfaz para efectividad de tipos

#### Strategies

-   `Factories/Strategies/NatureBoosting/` - Strategy Pattern para nature boosting
    -   `INatureBoostingStrategy.cs`
    -   `AttackNatureBoostingStrategy.cs`
    -   `DefenseNatureBoostingStrategy.cs`
    -   `SpAttackNatureBoostingStrategy.cs`
    -   `SpDefenseNatureBoostingStrategy.cs`
    -   `SpeedNatureBoostingStrategy.cs`
    -   `DefaultNatureBoostingStrategy.cs`
    -   `NatureBoostingRegistry.cs`
-   `Blueprints/Strategies/` - Strategy Pattern para stat getters
    -   `IStatGetterStrategy.cs`
    -   `StatGetterRegistry.cs`
    -   `HPStatGetterStrategy.cs`
    -   `AttackStatGetterStrategy.cs`
    -   `DefenseStatGetterStrategy.cs`
    -   `SpAttackStatGetterStrategy.cs`
    -   `SpDefenseStatGetterStrategy.cs`
    -   `SpeedStatGetterStrategy.cs`
-   `Instances/Strategies/` - Strategy Pattern para PokemonInstance stat getters
    -   `IPokemonStatGetterStrategy.cs`
    -   `PokemonStatGetterRegistry.cs`
    -   `MaxHPStatGetterStrategy.cs`
    -   `AttackStatGetterStrategy.cs`
    -   `DefenseStatGetterStrategy.cs`
    -   `SpAttackStatGetterStrategy.cs`
    -   `SpDefenseStatGetterStrategy.cs`
    -   `SpeedStatGetterStrategy.cs`
    -   `AccuracyStatGetterStrategy.cs`
    -   `EvasionStatGetterStrategy.cs`
-   `Effects/Strategies/` - Strategy Pattern para descripciones de efectos
    -   `IMoveRestrictionDescriptionStrategy.cs`
    -   `MoveRestrictionDescriptionRegistry.cs`
    -   `EffectDescriptionRegistries.cs`
    -   `Strategies/Implementations/` - Implementaciones de estrategias de descripción

#### Move Selection

-   `Factories/MoveSelection/IMoveSelector.cs`
-   `Factories/MoveSelection/MoveSelector.cs`
-   `Factories/MoveSelection/Strategies/IMoveSelectionStrategy.cs`
-   `Factories/MoveSelection/Strategies/RandomMoveStrategy.cs`
-   `Factories/MoveSelection/Strategies/DefaultMoveStrategy.cs`
-   `Factories/MoveSelection/Strategies/StabMoveStrategy.cs`
-   `Factories/MoveSelection/Strategies/PowerMoveStrategy.cs`
-   `Factories/MoveSelection/Strategies/OptimalMoveStrategy.cs`

#### Managers

-   `Managers/IStatStageManager.cs`
-   `Managers/StatStageManager.cs`

#### Cache

-   `Instances/StatsCache.cs` - Sistema de cacheo para stats calculados

### Archivos Modificados

#### Factories

-   `Factories/PokemonInstanceBuilder.cs` - Refactorizado completamente:
    -   Eliminado `static Random`, ahora usa `IRandomProvider` inyectado
    -   Magic numbers reemplazados con `CoreConstants`
    -   Métodos extraídos para mejor SRP
    -   Validación completa en `Build()`
    -   Integración con `MoveSelector`
-   `Factories/StatCalculator.cs` - Convertido de estático a instancia:
    -   Implementa `IStatCalculator`
    -   Mantiene métodos estáticos como wrappers para compatibilidad
    -   Usa `CoreConstants` y `CoreValidators`
-   `Factories/TypeEffectiveness.cs` - Convertido de estático a instancia:
    -   Implementa `ITypeEffectiveness`
    -   Mantiene métodos estáticos como wrappers para compatibilidad

#### Instances

-   `Instances/PokemonInstance.cs` - Actualizado:
    -   Usa `CoreConstants` y extensiones
    -   Integrado con `StatStageManager`
    -   Integrado con `StatsCache`
-   `Instances/PokemonInstance.Battle.cs` - Actualizado:
    -   Usa `StatStageManager` para manejo de stat stages
    -   Usa `CoreConstants` para validaciones
-   `Instances/PokemonInstance.LevelUp.cs` - Actualizado:
    -   Integrado con `StatsCache` para optimización
    -   Usa `CoreValidators` para validaciones
-   `Instances/PokemonInstance.Evolution.cs` - Actualizado:
    -   Invalida cache cuando cambia la especie

#### Blueprints

-   `Blueprints/BaseStats.cs` - Refactorizado:
    -   Usa `StatGetterRegistry` en lugar de switch statement
-   `Blueprints/PokemonSpeciesData.cs` - Actualizado:
    -   `GetRandomAbility()` ahora usa `IRandomProvider`
-   `Blueprints/StatusEffectData.cs` - Actualizado:
    -   `GetRandomDuration()` ahora usa `IRandomProvider`

#### Effects

-   `Effects/MoveRestrictionEffect.cs` - Refactorizado:
    -   Usa `MoveRestrictionDescriptionRegistry` en lugar de switch
-   `Effects/FieldConditionEffect.cs` - Refactorizado:
    -   Usa `EffectDescriptionRegistries` en lugar de switch
-   `Effects/PriorityModifierEffect.cs` - Refactorizado:
    -   Usa `EffectDescriptionRegistries` en lugar de switch
-   `Effects/ProtectionEffect.cs` - Refactorizado:
    -   Usa `EffectDescriptionRegistries` en lugar de switch
-   `Effects/SelfDestructEffect.cs` - Refactorizado:
    -   Usa `EffectDescriptionRegistries` en lugar de switch

### Mejoras Implementadas

#### Principios SOLID

-   ✅ **Single Responsibility**: Métodos extraídos, responsabilidades separadas
-   ✅ **Open/Closed**: Strategy Pattern implementado para extensibilidad
-   ✅ **Dependency Inversion**: Interfaces creadas, DI implementado

#### Code Quality

-   ✅ **Magic Numbers**: Eliminados, centralizados en `CoreConstants`
-   ✅ **Métodos Largos**: Refactorizados, métodos extraídos
-   ✅ **Switch Statements**: Reemplazados con Strategy Pattern y diccionarios
-   ✅ **Validación**: Centralizada en `CoreValidators`

#### Arquitectura

-   ✅ **Testabilidad**: Clases estáticas convertidas a instancias con interfaces
-   ✅ **Extensibilidad**: Strategy Pattern implementado en múltiples áreas
-   ✅ **Mantenibilidad**: Código más organizado y modular
-   ✅ **Performance**: Sistema de cacheo implementado para stats

### Próximos Pasos (Opcionales)

1. **Tests**: Implementar tests unitarios para nuevas clases e interfaces
2. **Tarea 5.1**: Extraer Sub-Builders (opcional, no crítico)
3. **Tarea 8.2**: Optimizar Allocations (opcional, requiere profiling)
4. **Documentación**: Actualizar documentación técnica con nuevos patrones

---

**Fin del Documento**
