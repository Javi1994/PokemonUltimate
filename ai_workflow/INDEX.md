# AI Workflow Optimization - Index

> **Índice completo de todos los componentes del sistema**  
> **Específico para**: C# .NET SDKs integrados con Unity

## 📚 Documentación Principal

- [`README.md`](README.md) - Overview y estructura
- [`START_HERE.md`](START_HERE.md) ⭐ **Para proyectos nuevos - Leer primero** (incluye checklist)
- [`PORTABILITY_GUIDE.md`](PORTABILITY_GUIDE.md) ⭐ **Para portar a otro proyecto - Guía completa de portabilidad**
- [`PROMPTS_GUIDE.md`](PROMPTS_GUIDE.md) ⭐ **Cómo pedir cosas - Guía de prompts para usuarios**
- [`AI_QUICK_REFERENCE.md`](AI_QUICK_REFERENCE.md) ⭐ **Guía rápida para IA - Referencia de ejecución**
- [`INDEX.md`](INDEX.md) - Este archivo

---

## 🔧 Schemas (`schemas/`)

Schemas YAML para validación automática:

- `game-definition-schema.yaml` - Estructura de definición de juego
- `feature-schema.yaml` - Estructura de feature
- `test-structure-schema.yaml` - Estructura de tests

**Uso**: Validar estructura automáticamente

---

## 🌳 Decision Trees (`decision-trees/`)

Decision trees estructurados en YAML:

- `game-definition.yaml` ⭐ **CRÍTICO** - Definir juego y generar features automáticamente
- `tdd-workflow.yaml` ⭐ **CRÍTICO** - Flujo TDD paso a paso
- `feature-discovery.yaml` ⭐ **CRÍTICO** - Feature discovery optimizado

**Uso**: Seguir flujo ejecutable paso a paso

---

## 📝 Templates (`templates/`)

Templates con ejemplos completos:

**Tests** (`templates/tests/`):
- `functional-template.[ext]` - Tests funcionales con ejemplo completo
- `edgecases-template.[ext]` - Tests edge cases con ejemplo completo
- `integration-template.[ext]` - Tests integración con ejemplo completo
- `smoke-template.md` - Smoke tests para verificar features principales funcionan juntas

**Game Definition** (`templates/`):
- `game-definition-template.yaml` - Template para definir juegos

**Project Setup** (`templates/`):
- `gitignore-template` - Template de .gitignore para C# .NET + Unity

**Uso**: Copiar y adaptar para escribir tests, definir juegos o configurar proyecto rápidamente

---

## 🔨 Scripts (`scripts/`)

Scripts de validación automática (Bash + PowerShell):

- `setup-project.sh` / `.ps1` ⭐ **Para proyectos nuevos** - Crear estructura inicial
- `validate-test-structure.sh` / `.ps1` - Validar estructura de tests
- `validate-fdd-compliance.sh` / `.ps1` - Validar compliance FDD

**Uso**: 
```bash
# Bash
./ai_workflow/scripts/validate-test-structure.sh Tests/
./ai_workflow/scripts/validate-fdd-compliance.sh

# PowerShell
ai_workflow/scripts/validate-test-structure.ps1 -TestDir Tests
ai_workflow/scripts/validate-fdd-compliance.ps1
```

---

## 📖 Documentación (`docs/`)

Guías detalladas:

- `GAME_DEFINITION_GUIDE.md` - Guía completa de definición de juegos
- `TDD_GUIDE.md` - Guía completa de TDD
- `FDD_GUIDE.md` - Guía completa de FDD
- `SMOKE_TESTS_GUIDE.md` - Guía completa de smoke tests

**Documentos Generados por el Workflow** (en `docs/` del proyecto):
- `GAME_DEFINITION.yaml` - Definición completa del juego
- `features_master_list.md` - Lista principal de features
- `features_master_list_detailed.md` - Lista detallada con sub-features
- `CODE_ORGANIZATION.md` - Estrategia de organización de código (se crea automáticamente si hay 5+ features o 20+ sub-features)

## ⚙️ Configuración

- `cursorrules-template.md` - Template de .cursorrules optimizado con TDD + FDD integrados

---

## 🎯 Flujo de Uso Recomendado

### Para Nuevo Proyecto (Game Definition) ⭐ **START HERE**
1. Leer `decision-trees/game-definition.yaml`
2. Usar template `templates/game-definition-template.yaml`
3. Definir juego → Features se generan automáticamente
4. Validar con `schemas/game-definition-schema.yaml`

### Para Integrar en Proyecto
1. Leer `START_HERE.md`
2. Copiar estructura a proyecto
3. Actualizar `.cursorrules`
4. Definir juego primero (Game Definition)
5. Probar con primer test/feature

### Para Escribir Tests (TDD)
1. Leer `decision-trees/tdd-workflow.yaml`
2. Usar template de `templates/tests/`
3. Validar con `scripts/validate-test-structure.sh`

### Para Asignar Features (FDD)
1. Leer `decision-trees/feature-discovery.yaml`
2. Seguir flujo de búsqueda/creación
3. Validar con `scripts/validate-fdd-compliance.sh`

---

## 📊 Componentes por Prioridad

### ⭐ Crítico (Hacer Primero)
- `decision-trees/game-definition.yaml` ⭐ **START HERE**
- `decision-trees/tdd-workflow.yaml`
- `decision-trees/feature-discovery.yaml`
- `templates/game-definition-template.yaml`
- `templates/tests/functional-template.[ext]`
- `scripts/validate-fdd-compliance.sh`

### Alta Prioridad
- `schemas/test-structure-schema.yaml`
- `templates/tests/edgecases-template.[ext]`
- `scripts/validate-test-structure.sh`

### Media Prioridad
- `schemas/feature-schema.yaml`
- `schemas/game-definition-schema.yaml`
- `templates/tests/integration-template.[ext]`

---

## 🔄 Actualizaciones

**Última Actualización**: 2025-01-XX

**Versión**: 1.0.0 (Fase 0 completa)

---

**Ver**: [`README.md`](README.md) para overview completo

