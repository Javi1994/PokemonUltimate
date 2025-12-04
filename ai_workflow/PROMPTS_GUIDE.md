# Guía de Prompts - Cómo Pedir Cosas al Sistema

> **Guía rápida de comandos y prompts para interactuar con el sistema AI Workflow**  
> **Útil para**: Usuarios y IA

## 🎯 Comandos Principales

### 1. Definir el Juego ⭐ **PRIMERO**

**Cuándo usar**: Al empezar un proyecto nuevo o cuando el juego no está definido.

**Prompts que funcionan**:
```
"Define game"
"What is this game?"
"Define [nombre del juego]"
"Set up game definition"
"Create game definition"
```

**Qué hace**:
- Crea `docs/GAME_DEFINITION.yaml`
- Genera `docs/features_master_list.md`
- Genera `docs/features_master_list_detailed.md`
- Establece el contexto del proyecto

**Ejemplo**:
```
"Define game: Es un simulador de ecosistema realista con animales que se comportan de forma realista"
```

---

### 1.5. Redefinir/Mejorar Game Definition ⭐ **PARA MEJORAR DEFINICIÓN EXISTENTE**

**Cuándo usar**: Cuando quieres mejorar o expandir una definición existente.

**Prompts que funcionan**:
```
"Redefine game definition"
"Improve game definition"
"Review game definition"
"Update game definition"
"Enhance game definition"
"Redefine game definition: [nueva información]"
```

**Qué hace**:
1. Lee la definición existente
2. Analiza completitud y mejores prácticas
3. Identifica mejoras y conceptos faltantes
4. Sugiere mejoras específicas
5. Actualiza la definición con mejoras
6. Regenera las master lists

**Ejemplo**:
```
"Redefine game definition: Agregar sistema de migración estacional para animales"
"Improve game definition: Asegurar que todas las features sean granulares"
"Review game definition: Verificar que no falten conceptos importantes"
```

**Beneficios**:
- Mejora iterativa de la definición
- Identifica gaps automáticamente
- Sugiere mejoras basadas en mejores prácticas
- Mantiene consistencia y completitud

---

### 2. Implementar Features

**Cuándo usar**: Cuando quieres desarrollar una funcionalidad nueva.

**Prompts que funcionan**:
```
"Implement [nombre de feature]"
"Add [funcionalidad]"
"Create [componente]"
"Build [sistema]"
"Develop [feature]"
```

**Qué hace**:
1. Busca si la feature ya existe (Feature Discovery)
2. Si no existe, la crea automáticamente
3. Escribe tests primero (TDD)
4. Implementa el código
5. Valida y documenta

**Ejemplos**:
```
"Implement animal behavior system"
"Add predator-prey interaction"
"Create entity base class"
"Build ecosystem simulation engine"
```

---

### 3. Escribir Tests

**Cuándo usar**: Cuando quieres agregar tests específicos o seguir TDD manualmente.

**Prompts que funcionan**:
```
"Write tests for [componente]"
"Add tests for [feature]"
"Create test for [método]"
"Test [funcionalidad]"
```

**Qué hace**:
- Usa templates apropiados (functional/edgecases/integration)
- Sigue convenciones de naming
- Incluye referencias a features

**Ejemplos**:
```
"Write tests for Animal class"
"Add edge case tests for EntityManager"
"Create integration test for BehaviorSystem"
```

---

### 4. Refactorizar

**Cuándo usar**: Cuando quieres mejorar código existente sin cambiar comportamiento.

**Prompts que funcionan**:
```
"Refactor [componente]"
"Improve [código]"
"Clean up [archivo]"
"Optimize [sistema]"
```

**Qué hace**:
- Mejora código manteniendo tests pasando
- Sigue patrones del proyecto
- Actualiza documentación si es necesario

**Ejemplos**:
```
"Refactor Animal class to use better patterns"
"Improve EntityManager performance"
"Clean up BehaviorSystem code"
```

---

### 5. Buscar Features

**Cuándo usar**: Cuando quieres saber qué features existen o encontrar una específica.

**Prompts que funcionan**:
```
"What features exist?"
"List all features"
"Find feature for [funcionalidad]"
"Search features related to [tema]"
```

**Qué hace**:
- Lee `docs/features_master_list.md`
- Busca semánticamente features relacionadas
- Muestra información relevante

**Ejemplos**:
```
"What features exist for animal behavior?"
"Find feature related to ecosystem simulation"
"List all features in phase 1"
```

---

### 6. Actualizar Documentación

**Cuándo usar**: Cuando quieres actualizar documentación de features.

**Prompts que funcionan**:
```
"Update documentation for [feature]"
"Document [componente]"
"Update [feature] docs"
```

**Qué hace**:
- Actualiza `roadmap.md`, `architecture.md`, `use_cases.md`
- Actualiza `code_location.md` y `testing.md`
- Mantiene consistencia

**Ejemplos**:
```
"Update documentation for Animal feature"
"Document EntityManager architecture"
```

---

## 📋 Patrones de Prompts Efectivos

### ✅ Buenos Prompts (Específicos y Claros)

```
"Implement animal movement system with pathfinding"
"Add tests for EntityManager.AddEntity method"
"Refactor BehaviorSystem to use state pattern"
"Create integration test for Animal-Ecosystem interaction"
```

### ❌ Prompts Vagos (Evitar)

```
"Make it work"
"Do the thing"
"Fix stuff"
"Add something"
```

### 💡 Tips para Prompts Efectivos

1. **Sé específico**: "Implement animal hunting behavior" > "Add behavior"
2. **Menciona contexto**: "Add tests for EntityManager in feature 1.2"
3. **Indica prioridad**: "Implement animal movement (high priority)"
4. **Especifica detalles**: "Create Animal class with health, age, and position properties"

---

## 🔄 Flujo de Trabajo Recomendado

### Para Proyecto Nuevo

1. **Definir juego**: `"Define game: [descripción]"`
2. **Implementar primera feature**: `"Implement [feature básica]"`
3. **Continuar desarrollo**: `"Implement [siguiente feature]"`

### Para Feature Nueva

1. **Verificar si existe**: `"What features exist for [tema]?"`
2. **Implementar**: `"Implement [feature]"` (el sistema busca automáticamente)
3. **Verificar**: El sistema valida y documenta automáticamente

### Para Mejorar Código Existente

1. **Identificar**: `"Find feature for [componente]"`
2. **Refactorizar**: `"Refactor [componente]"`
3. **Validar**: El sistema ejecuta tests automáticamente

---

## 🎯 Comandos Especiales

### Validación Manual

```
"Validate test structure"
"Check FDD compliance"
"Run validation scripts"
```

### Información del Proyecto

```
"What is the current project state?"
"Show me the game definition"
"What features are in phase 1?"
```

### Troubleshooting

```
"Why are tests failing?"
"Check why [componente] doesn't work"
"Validate [archivo] structure"
```

---

## 📚 Referencias Rápidas

- **Game Definition**: `"Define game"`
- **Implementar**: `"Implement [nombre]"`
- **Tests**: `"Write tests for [componente]"`
- **Refactorizar**: `"Refactor [componente]"`
- **Buscar**: `"What features exist for [tema]?"`

---

## 💬 Ejemplos Completos

### Ejemplo 1: Nueva Feature Completa

```
Usuario: "Implement animal reproduction system"

Sistema ejecuta:
1. Feature Discovery → Busca "reproduction" en master list
2. Si no existe → Crea feature automáticamente
3. TDD → Escribe tests primero
4. Implementa → Código mínimo para pasar tests
5. Edge cases → Tests de límites
6. Documenta → Actualiza toda la documentación
```

### Ejemplo 2: Agregar Tests a Feature Existente

```
Usuario: "Add edge case tests for Animal.Health property"

Sistema ejecuta:
1. Busca feature de Animal
2. Lee tests existentes
3. Usa template edgecases
4. Escribe tests para límites (0, negativo, máximo)
5. Implementa validaciones si faltan
```

### Ejemplo 3: Refactorización

```
Usuario: "Refactor EntityManager to use dependency injection"

Sistema ejecuta:
1. Lee código existente
2. Identifica patrones actuales
3. Refactoriza manteniendo tests pasando
4. Actualiza documentación
5. Valida estructura
```

---

**Última Actualización**: 2025-01-XX

