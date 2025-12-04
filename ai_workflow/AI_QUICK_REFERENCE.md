# AI Quick Reference - Guía Rápida para IA

> **Referencia rápida para que la IA ejecute el workflow eficientemente**

## 🎯 Triggers Principales

| Trigger del Usuario | Workflow a Ejecutar | Decision Tree |
|---------------------|---------------------|---------------|
| "Define game" / "What is this game?" | Game Definition (nuevo) | `decision-trees/game-definition.yaml` |
| "Redefine game definition" / "Improve game definition" | Review & Improve | `decision-trees/game-definition.yaml` (Step -1.8) |
| "Implement X" / "Add X" | Feature Discovery + TDD | `decision-trees/feature-discovery.yaml` → `tdd-workflow.yaml` |
| "Write tests for X" | TDD Directo | `decision-trees/tdd-workflow.yaml` |
| "Create smoke tests" / "Add smoke tests" | Smoke Tests Creation | `decision-trees/tdd-workflow.yaml` (Step 9) |
| "Refactor X" | Refactoring | Ver código existente + tests |

## 🔧 Herramientas por Paso

### Game Definition
- **read_file**: `docs/GAME_DEFINITION.yaml`
- **write_file**: Crear `docs/GAME_DEFINITION.yaml`
- **read_file**: `templates/game-definition-template.yaml`
- **codebase_search**: Buscar conceptos relacionados

### Feature Discovery
- **read_file**: `docs/features_master_list_index.md` ⭐ **OPTIMIZED FOR AI - START HERE**
  - Ver sección "NEXT AVAILABLE FEATURES" primero
  - Quick Reference para todas las features
- **codebase_search**: Buscar features similares
- **read_file**: Feature docs si existe
- **write_file**: Crear feature docs si no existe
- **search_replace**: Actualizar `features_master_list_index.md` después de completar feature

### TDD
- **read_file**: `templates/tests/[type]-template.md`
- **read_file**: Feature `use_cases.md`
- **read_file**: Feature `code_location.md`
- **write_file**: Crear test file
- **run_terminal_cmd**: `dotnet test`

## 📋 Patrones de Ejecución

### Patrón 1: Nueva Feature Completa
```
1. codebase_search("features related to [user_request]")
2. Si no existe → read_file("docs/features_master_list.md") → determinar número
3. write_file("docs/features/[N]-[name]/README.md")
4. read_file("decision-trees/tdd-workflow.yaml")
5. read_file("templates/tests/functional-template.md")
6. write_file("Tests/[Feature]/[Component]Tests.cs")
7. run_terminal_cmd("dotnet test")
8. write_file("[Feature]/[Component].cs")  # Implementación mínima
9. run_terminal_cmd("dotnet test")  # Verificar pasa
```

### Patrón 2: Feature Existente
```
1. codebase_search("features related to [user_request]")
2. read_file("docs/features/[N]-[name]/README.md")
3. read_file("docs/features/[N]-[name]/code_location.md")
4. read_file("[code_location]")  # Código existente
5. read_file("decision-trees/tdd-workflow.yaml")
6. write_file("Tests/[Feature]/[NewComponent]Tests.cs")
7. [continuar con TDD]
```

### Patrón 3: Game Definition (Nuevo)
```
1. read_file("docs/GAME_DEFINITION.yaml")  # Verificar si existe
2. Si no existe:
   a. read_file("templates/game-definition-template.yaml")
   b. Extraer info del usuario
   c. write_file("docs/GAME_DEFINITION.yaml")
   d. Generar features automáticamente
   e. write_file("docs/features_master_list.md")
   f. write_file("docs/features_master_list_detailed.md")
   g. Si features_count > 5 OR sub_features_count > 20:
      write_file("docs/CODE_ORGANIZATION.md")  # Estrategia de organización
   h. Validar definición completa
```

### Patrón 4: Review & Improve Game Definition
```
1. read_file("docs/GAME_DEFINITION.yaml")  # Leer existente
2. read_file("docs/features_master_list.md")
3. read_file("docs/features_master_list_detailed.md")
4. Analizar completitud:
   - codebase_search("typical ecosystem entities")
   - codebase_search("typical ecosystem interactions")
   - Comparar con definición actual
5. Identificar mejoras:
   - Conceptos faltantes
   - Features que faltan
   - Granularidad mejorable
6. Presentar sugerencias al usuario
7. Si usuario confirma:
   a. Actualizar GAME_DEFINITION.yaml
   b. Regenerar feature lists
   c. Actualizar master lists
```

### Patrón 4: Smoke Tests Creation (Automático)
```
1. Después de completar feature:
   a. Contar features completadas (main features, no sub-features)
   b. Verificar si fase completa está terminada
   c. Si features_completed >= 5 OR phase_completed:
      - Verificar si proyecto smoke tests existe
      - Si no → Crear Tests/EcosystemGameSdk.SmokeTests/
      - Crear smoke tests usando template
      - Verificar happy path funciona
      - Ejecutar smoke tests
```

## ⚠️ Errores Comunes y Soluciones

### Error: "Feature no encontrada"
**Solución**: 
1. Verificar `docs/GAME_DEFINITION.yaml` existe
2. Si no existe → Ejecutar Game Definition primero
3. Buscar semánticamente en `docs/features_master_list.md`

### Error: "Test pasa en red phase"
**Solución**:
1. Verificar que implementación no existe
2. Verificar que test está probando lo correcto
3. Asegurar que test falla por razón correcta

### Error: "No sé qué template usar"
**Solución**:
1. Leer `decision-trees/tdd-workflow.yaml` → Step 1
2. Determinar tipo de test basado en user request
3. Usar template correspondiente

## 🎯 Decisiones Binarias Clave

| Pregunta | Sí → | No → |
|----------|------|------|
| ¿Existe `docs/GAME_DEFINITION.yaml`? | Validar completitud | Ejecutar Game Definition |
| ¿Feature existe en master list? | Asignar a existente | Crear nueva feature |
| ¿Test falla en red phase? | Continuar a green | Revisar test |
| ¿Test pasa en green phase? | Continuar a edge cases | Arreglar implementación |
| ¿Feature interactúa con otros sistemas? | Escribir integration tests | Solo functional + edge |

## 📚 Archivos Críticos a Leer

**Siempre leer primero**:
1. `docs/GAME_DEFINITION.yaml` (si existe)
2. `docs/features_master_list.md`
3. Feature `README.md` (si existe)
4. Feature `code_location.md` (si existe)

**Antes de escribir código**:
1. Leer código existente relacionado
2. Leer `code_location.md` para ver estructura
3. Leer tests existentes para ver patrones

## 🔄 Flujo de Validación

Después de cada cambio:
1. `run_terminal_cmd("dotnet build")` → 0 warnings
2. `run_terminal_cmd("dotnet test")` → Todos pasan
3. `run_terminal_cmd("ai_workflow/scripts/validate-test-structure.ps1")`
4. `run_terminal_cmd("ai_workflow/scripts/validate-fdd-compliance.ps1")`

## 💡 Tips de Optimización

1. **Leer antes de escribir**: Siempre leer código existente primero
2. **Usar codebase_search**: Para encontrar código relacionado rápidamente
3. **Seguir decision trees**: No saltar pasos
4. **Validar temprano**: Ejecutar tests frecuentemente
5. **Documentar mientras**: Actualizar docs inmediatamente después de cambios

---

**Última Actualización**: 2025-01-XX

