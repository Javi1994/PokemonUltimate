# FDD Guide - Complete Reference

> **Guía completa de Feature-Driven Development optimizada para IA**

## 🎯 Overview

Esta guía explica cómo usar el sistema FDD optimizado para desarrollo con IA.

**⚠️ Prerequisito**: Para proyectos nuevos, primero ejecuta **Game Definition** workflow (`decision-trees/game-definition.yaml`) para definir el juego y generar features automáticamente.

---

## 📋 Workflow FDD

### Paso 0: Feature Discovery & Assignment ⭐ **OBLIGATORIO PRIMERO**

**Nunca escribir código sin asignar feature primero**

**Nota**: Si `docs/GAME_DEFINITION.yaml` no existe, ejecuta primero Game Definition workflow.

---

### Paso 0.1: Leer Master List

```markdown
1. Leer `docs/GAME_DEFINITION.yaml` (si existe) para contexto del juego
2. Leer `docs/features_master_list_index.md` ⭐ **OPTIMIZED FOR AI** (o `docs/features_master_list.md` como fallback)
3. Extraer todos los features existentes
4. Ver sección "NEXT AVAILABLE FEATURES" para identificar qué puede implementarse ahora
5. Comparar con request del usuario
```

---

### Paso 0.2: Búsqueda Semántica

**Usar decision tree**: `decision-trees/feature-discovery.yaml`

**Proceso**:
1. Buscar keywords del request
2. Buscar features semánticamente similares
3. Comparar descripciones

---

### Paso 0.3: Verificar Fit

**Decisión binaria**:
- **SÍ** → Asignar a feature existente
- **NO** → Crear nueva feature

---

### Paso 0.4: Asignar a Feature Existente

**Si el trabajo encaja en feature existente**:

1. **Leer documentación completa**:
   - `README.md`
   - `architecture.md`
   - `roadmap.md`
   - `use_cases.md`
   - `code_location.md`
   - `testing.md`

2. **Verificar sub-feature**:
   - ¿Encaja en sub-feature existente?
   - SÍ → Usar sub-feature
   - NO → Crear nueva sub-feature

---

### Paso 0.5: Crear Nueva Feature

**Si el trabajo NO encaja en feature existente**:

1. **Determinar número**:
   - Leer `features_master_list.md`
   - Encontrar siguiente número disponible

2. **Crear estructura**:
   ```
   docs/features/[N]-[feature-name]/
   ├── README.md
   ├── architecture.md
   ├── roadmap.md
   ├── use_cases.md
   ├── testing.md
   └── code_location.md
   ```

3. **Actualizar master list**:
   - Agregar entrada a `features_master_list.md`

---

### Paso 0.6: Crear Nueva Sub-Feature

**Si necesita nueva sub-feature**:

1. **Determinar número**:
   - Leer feature's `README.md`
   - Encontrar siguiente número disponible (decimal)

2. **Crear estructura**:
   ```
   docs/features/[N]-[feature-name]/[N.M]-[sub-feature-name]/
   └── README.md  # Mínimo requerido
   ```

3. **Actualizar feature README**:
   - Agregar entrada de sub-feature

---

## 🔍 Validación

### Validar Compliance FDD

```bash
# Bash
ai_workflow/scripts/validate-fdd-compliance.sh

# PowerShell
ai_workflow/scripts/validate-fdd-compliance.ps1
```

**Verifica**:
- Feature references en código
- Documentación completa de features
- Consistencia con master list
- Organización de tests por feature

---

## 📚 Referencias

- **Game Definition**: `decision-trees/game-definition.yaml` - Prerequisito para nuevos proyectos
- **Decision Tree**: `decision-trees/feature-discovery.yaml`
- **Schema**: `schemas/feature-schema.yaml`
- **Scripts**: `scripts/validate-fdd-compliance.*`

---

**Última Actualización**: 2025-01-XX

