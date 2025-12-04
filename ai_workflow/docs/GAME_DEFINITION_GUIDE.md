# Game Definition Guide

> **Guía completa para definir juegos y generar features automáticamente**  
> **Específico para**: C# .NET SDKs integrados con Unity

## 🎯 Overview

El sistema de **Game Definition** permite definir un juego de forma estructurada y generar automáticamente todas las features necesarias. Esto es especialmente útil para desarrollo con IA porque:

- **Estructura clara**: La IA entiende exactamente qué necesita hacer
- **Generación automática**: Las features se generan automáticamente del juego
- **Dependencias mapeadas**: Las dependencias se identifican automáticamente
- **Parseable**: Todo está en YAML estructurado
- **Unity Integration**: Considera que el SDK proporciona lógica, Unity maneja presentación

---

## 📋 Workflow

### Paso 1: Iniciar Definición de Juego

**Trigger**: Usuario dice "Define game" o "What is this game?"

**Proceso**:
1. Leer `ai_workflow/decision-trees/game-definition.yaml`
2. Verificar si `docs/GAME_DEFINITION.yaml` existe
3. Si no existe → Iniciar definición
4. Si existe → Validar completitud

---

### Paso 2: Definir Básicos del Juego

**Campos requeridos**:
- **Nombre**: Nombre del juego
- **Tipo**: Tipo de juego (Simulation, Strategy, etc.)
- **Descripción**: 1-2 párrafos describiendo el juego
- **Plataforma objetivo**: Unity, Unreal, Web, etc.
- **Propósito del SDK**: Qué proporciona el SDK

**Template**: `ai_workflow/templates/game-definition-template.yaml`

---

### Paso 3: Identificar Conceptos Core

**Conceptos a identificar**:

1. **Entidades**: ¿Qué objetos/entidades existen en el juego?
   - Ejemplo: Animales, Plantas, Recursos, Edificios
   - Para cada entidad: propiedades y comportamientos

2. **Entorno**: ¿Cuál es el ambiente/mundo del juego?
   - Ejemplo: Ecosistema, Ciudad, Espacio
   - Componentes y condiciones del entorno

3. **Interacciones**: ¿Cómo interactúan las entidades?
   - Ejemplo: Depredación, Reproducción, Combate
   - Participantes y tipo de interacción

4. **Sistemas**: ¿Qué sistemas necesita el juego?
   - Ejemplo: Comportamiento/AI, Física, Economía
   - Componentes y dependencias

---

### Paso 4: Generar Categorías de Features

**Mapeo automático**:
- Entidades → Sistema de Entidades
- Entorno → Sistema de Entorno
- Interacciones → Sistema de Interacciones
- Sistemas → Sistemas específicos

**Resultado**: Lista inicial de features principales

---

### Paso 5: Granularizar Features

**Patrón de granularización**:
1. **Base primero**: Clases/interfaces base
2. **Propiedades segundo**: Atributos/propiedades
3. **Comportamiento tercero**: Acciones/comportamientos
4. **Interacciones cuarto**: Interacciones con otros
5. **Sistemas último**: Integración de sistemas

**Resultado**: Lista completa de sub-features con dependencias

---

### Paso 6: Crear Master List

**Archivos generados**:
- `docs/GAME_DEFINITION.yaml` - Definición completa del juego
- `docs/features_master_list.md` - Lista principal de features
- `docs/features_master_list_detailed.md` - Lista detallada con sub-features

**Estructura**:
- Numeración secuencial
- Dependencias mapeadas
- Fases de desarrollo asignadas

---

### Paso 6.5: Crear Estrategia de Organización de Código (Automático) 

**Cuándo se crea**: Automáticamente si hay **5+ features principales** o **20+ sub-features**

**Archivo generado**:
- `docs/CODE_ORGANIZATION.md` - Estrategia de organización de código para proyectos grandes

**Contenido**:
- Principios de organización (código por sistema técnico, docs por feature)
- Estructura propuesta (`Core/[System]/[SubFolder]/`)
- Mapeo Feature → Código
- Ejemplos prácticos

**Propósito**: Definir cómo organizar el código cuando hay muchas features para mantener escalabilidad y navegación fácil.

**Nota**: Este paso es automático y opcional. Si no se crea automáticamente, puedes crearlo manualmente siguiendo el ejemplo en `docs/CODE_ORGANIZATION.md`.

---

### Paso 7: Validar Definición

**Validaciones**:
- ✓ Todos los campos básicos completos
- ✓ Al menos 3 conceptos core identificados
- ✓ Al menos 5 features generadas
- ✓ Dependencias mapeadas
- ✓ Fases asignadas
- ✓ CODE_ORGANIZATION.md creado (si hay 5+ features o 20+ sub-features)

**Si incompleto**: Solicitar información faltante
**Si completo**: Proceder a Feature Discovery (Step 0)

---

### Paso 8: Revisar y Mejorar Definición Existente

**Cuándo usar**: Cuando quieres mejorar o expandir una definición existente.

**Triggers**:
- "Redefine game definition"
- "Improve game definition"
- "Review game definition"
- "Update game definition"
- "Enhance game definition"

**Proceso**:

1. **Lee definición existente**:
   - `docs/GAME_DEFINITION.yaml`
   - `docs/features_master_list.md`
   - `docs/features_master_list_detailed.md`

2. **Analiza completitud**:
   - Verifica entidades faltantes
   - Verifica interacciones faltantes
   - Verifica sistemas faltantes
   - Verifica granularidad de features
   - Verifica mapeo de dependencias

3. **Compara con mejores prácticas**:
   - Entidades típicas para el tipo de juego
   - Interacciones comunes
   - Sistemas estándar de simulación
   - Granularidad recomendada

4. **Identifica mejoras**:
   - Conceptos faltantes
   - Features que deberían agregarse
   - Features que pueden ser más granulares
   - Dependencias faltantes

5. **Sugiere mejoras**:
   - Presenta hallazgos específicos
   - Sugiere mejoras concretas
   - Pide confirmación del usuario

6. **Aplica mejoras**:
   - Actualiza `GAME_DEFINITION.yaml`
   - Regenera listas de features
   - Actualiza master lists
   - Mantiene features existentes

**Ejemplo**:
```
Usuario: "Redefine game definition: Agregar sistema de migración estacional"

Sistema:
1. Lee definición actual
2. Identifica: Sistema de migración faltante
3. Sugiere:
   - Agregar "Migration" como comportamiento animal
   - Agregar "Migration System" como nuevo sistema
   - Agregar features: 3.12 Migration Behavior, 9.1 Migration System
4. Usuario confirma
5. Actualiza definición
6. Regenera feature lists
```

**Beneficios**:
- Mejora iterativa de la definición
- Identifica gaps automáticamente
- Sugiere mejoras basadas en mejores prácticas
- Mantiene consistencia y completitud

---

## 🔄 Integración con Feature Discovery

El sistema de Game Definition es un **prerequisito** para Feature Discovery:

```yaml
feature_discovery:
  prerequisite: "docs/GAME_DEFINITION.yaml must exist"
  
  uses:
    - game_definition: "docs/GAME_DEFINITION.yaml"
    - master_list: "docs/features_master_list.md"
    - detailed_list: "docs/features_master_list_detailed.md"
```

**Flujo completo**:
```
Game Definition (Step -1)
    ↓
Feature Discovery (Step 0)
    ↓
TDD Workflow (Step 1+)
```

---

## 📝 Ejemplo Completo

### Input del Usuario
```
"Define EcosystemGame - it's a realistic ecosystem simulation game
where animals behave realistically. Players can observe animals.
The SDK provides logic for Unity integration."
```

### Proceso Automático

1. **Extrae básicos**:
   - Name: EcosystemGame
   - Type: Simulation
   - Description: [extraído]
   - Platform: Unity
   - SDK Purpose: Logic layer

2. **Identifica conceptos**:
   - Entities: Animal, Plant, Resource
   - Environment: Ecosystem
   - Interactions: Predation, Reproduction
   - Systems: Behavior, Simulation, Observation

3. **Genera features**:
   - Feature 1: Sistema de Entidades
   - Feature 2: Sistema de Entorno
   - Feature 3: Sistema de Datos
   - Feature 4: Sistema de Comportamiento Animal
   - Feature 5: Sistema de Observación
   - Feature 6: Sistema de Simulación
   - Feature 7: Sistema de Eventos
   - Feature 8: Sistema de Validación
   - Feature 9: Sistema de Configuración

4. **Granulariza**:
   - Feature 1 → 14 sub-features
   - Feature 2 → 9 sub-features
   - ... (84 sub-features totales)

5. **Crea master list**:
   - `docs/GAME_DEFINITION.yaml` ✓
   - `docs/features_master_list.md` ✓
   - `docs/features_master_list_detailed.md` ✓
   - `docs/CODE_ORGANIZATION.md` ✓ (si hay 5+ features o 20+ sub-features)

---

## 🔄 Flujo Iterativo de Mejora

El sistema permite mejorar la definición iterativamente:

```
Definición Inicial
    ↓
Desarrollo de Features
    ↓
Nueva Información / Mejores Prácticas
    ↓
"Redefine game definition"
    ↓
Análisis y Sugerencias
    ↓
Aplicar Mejoras
    ↓
Definición Mejorada
    ↓
Continuar Desarrollo
```

**Cuándo usar revisión**:
- Después de implementar varias features
- Cuando descubres conceptos faltantes
- Cuando quieres asegurar granularidad
- Cuando quieres comparar con mejores prácticas

---

## 🎯 Ventajas para Desarrollo con IA

1. **Contexto completo**: La IA entiende el juego completo antes de empezar
2. **Features automáticas**: No necesita adivinar qué features crear
3. **Dependencias claras**: Sabe qué necesita antes de implementar
4. **Estructura consistente**: Todo sigue el mismo patrón
5. **Validación temprana**: Detecta problemas antes de implementar
6. **Mejora iterativa**: Puede mejorar la definición basándose en nueva información

---

## 📚 Referencias

- **Decision Tree**: `decision-trees/game-definition.yaml`
- **Template**: `templates/game-definition-template.yaml`
- **Schema**: `schemas/game-definition-schema.yaml`
- **Feature Discovery**: `decision-trees/feature-discovery.yaml`

---

**Última Actualización**: 2025-01-XX

