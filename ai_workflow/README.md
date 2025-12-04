# AI Workflow Optimization System

> **Sistema completo de optimización para desarrollo con IA: rápido, eficiente y eficaz**

**Específico para**: C# .NET SDKs integrados con Unity

## 🎯 Objetivo

Este sistema optimiza el desarrollo con IA enfocándose en:
- **Game Definition** - Definir juego y generar features automáticamente
- **TDD (Test-Driven Development)** - Tests primero, decisiones claras (NUnit)
- **Feature-Driven Development** - Asignación automática de features
- **Validación Automática** - Detección temprana de errores
- **Workflow Optimizado** - Menos pasos, más eficiencia
- **Unity Integration** - SDKs diseñados para integración con Unity

---

## 📁 Estructura

```
ai_workflow/
├── README.md                    # Este archivo
├── START_HERE.md                # ⭐ Guía paso a paso para proyectos nuevos
├── INDEX.md                     # Índice de todos los archivos
│
├── schemas/                     # Schemas YAML para validación
│   ├── game-definition-schema.yaml
│   ├── feature-schema.yaml
│   └── test-structure-schema.yaml
│
├── decision-trees/              # Decision trees estructurados
│   ├── game-definition.yaml     # Definir juego y generar features
│   ├── tdd-workflow.yaml        # Flujo TDD paso a paso
│   └── feature-discovery.yaml   # Feature discovery optimizado
│
├── templates/                    # Templates con ejemplos completos
│   ├── game-definition-template.yaml
│   ├── gitignore-template
│   └── tests/
│       ├── functional-template.[ext]
│       ├── edgecases-template.[ext]
│       └── integration-template.[ext]
│
├── scripts/                      # Scripts de setup y validación
│   ├── setup-project.sh / .ps1   # Setup inicial del proyecto
│   ├── validate-test-structure.sh / .ps1
│   └── validate-fdd-compliance.sh / .ps1
│
└── docs/                         # Documentación detallada
    ├── GAME_DEFINITION_GUIDE.md
    ├── TDD_GUIDE.md
    ├── FDD_GUIDE.md
    └── SMOKE_TESTS_GUIDE.md

**Nota**: El workflow también genera automáticamente `docs/CODE_ORGANIZATION.md` en proyectos con muchas features (5+ features o 20+ sub-features) para definir la estrategia de organización de código.
```

---

## 🚀 Quick Start

### Para Proyecto Nuevo (Desde Cero) ⭐ **START HERE**
1. **Lee** [`START_HERE.md`](START_HERE.md) - Guía paso a paso completa
2. **Define el juego** usando Game Definition workflow
3. **Implementa** empezando con Feature Discovery y TDD

### Para Proyecto Existente
1. **Lee** [`START_HERE.md`](START_HERE.md) para setup rápido
2. **Revisa** [`INDEX.md`](INDEX.md) para ver todos los componentes
3. **Define el juego** si no está definido (Game Definition)
4. **Implementa** empezando con Feature Discovery y TDD

---

## 📋 Fases de Implementación

### ⭐ Fase -1: Game Definition (CRÍTICA - HACER PRIMERO)
- Decision tree Game Definition
- Template de definición de juego
- Generación automática de features
- Schema de validación

### ⭐ Fase 0: TDD + FDD Optimization (CRÍTICA - HACER PRIMERO)
- Decision trees TDD
- Templates de tests completos
- Feature discovery optimizado
- Validación FDD compliance

### Fase 1: Estructura de Datos Parseable
- Schemas YAML
- Metadatos en frontmatter
- Decision trees generales

### Fase 2: Ejemplos Concretos
- Proyectos completos de ejemplo
- Ejemplos inline en templates
- Eliminación de placeholders

### Fase 3-7: Optimizaciones Adicionales
- Validación automática
- Referencias verificables
- Metadatos estructurados
- Prompt templates

---

## 🎯 Principios de Diseño

1. **Parseable First**: Estructura > Narrativa
2. **Ejemplos > Instrucciones**: Mostrar > Decir
3. **Validación Automática**: Detectar > Prevenir
4. **Decisiones Binarias**: Sí/No > Tal vez
5. **Metadatos Explícitos**: Estructurado > Libre

---

## 📚 Documentación

- [`START_HERE.md`](START_HERE.md) ⭐ **Para proyectos nuevos - Leer primero**
- [`PORTABILITY_GUIDE.md`](PORTABILITY_GUIDE.md) ⭐ **Para portar a otro proyecto - Guía completa**
- [`PROMPTS_GUIDE.md`](PROMPTS_GUIDE.md) ⭐ **Cómo pedir cosas - Guía de prompts**
- [`AI_QUICK_REFERENCE.md`](AI_QUICK_REFERENCE.md) ⭐ **Guía rápida para IA - Referencia de ejecución**
- [`INDEX.md`](INDEX.md) - Índice completo
- [`docs/GAME_DEFINITION_GUIDE.md`](docs/GAME_DEFINITION_GUIDE.md) - Guía de definición de juegos
- [`docs/TDD_GUIDE.md`](docs/TDD_GUIDE.md) - Guía TDD detallada
- [`docs/FDD_GUIDE.md`](docs/FDD_GUIDE.md) - Guía FDD detallada

---

**Última Actualización**: 2025-01-XX

