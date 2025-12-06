# TDD Guide - Complete Reference

> **Guía completa de Test-Driven Development optimizada para IA**

## 🎯 Overview

Esta guía explica cómo usar el sistema TDD optimizado para desarrollo con IA.

---

## 📋 Workflow TDD

### Paso 1: Determinar Tipo de Test

Usa el decision tree: `decision-trees/tdd-workflow.yaml`

**Opciones**:
- **Nueva funcionalidad** → Functional test
- **Casos límite** → Edge cases test
- **Interacciones de sistemas** → Integration test

---

### Paso 2: Escribir Test (Red Phase)

1. **Usar template apropiado**:
   - Functional → `templates/tests/functional-template.md`
   - Edge cases → `templates/tests/edgecases-template.md`
   - Integration → `templates/tests/integration-template.md`

2. **Seguir naming convention**:
   - Pattern: `MethodName_Scenario_ExpectedResult`
   - Example: `Authenticate_ValidCredentials_ReturnsTrue`

3. **Escribir test que FALLE**:
   - El test debe fallar porque la implementación no existe
   - Si el test pasa → ERROR: Revisar test o implementación

---

### Paso 3: Verificar Test Falla (Red Phase)

**Validación**:
```bash
# Ejecutar tests
[test_command] Tests/[Feature]/[Component]Tests.[ext]

# Resultado esperado: FAIL
# Si pasa → ERROR: Debe fallar en red phase
```

---

### Paso 4: Implementar Feature (Green Phase)

1. **Leer código existente**:
   - Leer `code_location.md` del feature
   - Leer archivos clave mencionados
   - Entender patrones existentes

2. **Escribir implementación mínima**:
   - Solo lo necesario para pasar el test
   - Seguir patrones existentes
   - Agregar feature reference

3. **Agregar feature reference**:
   ```csharp
   /// <remarks>
   /// **Feature**: [N]: [Feature Name]
   /// **Sub-Feature**: [N.M]: [Sub-Feature Name]
   /// **Documentation**: See `docs/features/[N]-[feature-name]/README.md`
   /// </remarks>
   ```

---

### Paso 5: Verificar Test Pasa (Green Phase)

**Validación**:
```bash
# Ejecutar tests
[test_command] Tests/[Feature]/[Component]Tests.[ext]

# Resultado esperado: PASS
# Si falla → Arreglar implementación
```

---

### Paso 6: Escribir Edge Cases

1. **Leer use cases**:
   - Leer `use_cases.md` del feature
   - Identificar casos límite

2. **Usar template**:
   - `templates/tests/edgecases-template.md`

3. **Categorías**:
   - Null inputs
   - Boundary values
   - Invalid inputs
   - Special cases

---

### Paso 7: Escribir Integration Tests

**Condición**: Solo si el feature interactúa con múltiples sistemas

1. **Usar template**:
   - `templates/tests/integration-template.md`

2. **Ubicación**:
   - `Tests/[Feature]/Integration/[Category]/`

---

### Paso 8: Refactor

**Principio**: Mejorar código sin cambiar comportamiento

**Validación**:
- Todos los tests deben seguir pasando
- Seguir reglas de código en `.cursorrules` y `docs/ai/guidelines/`
- Evitar `docs/ai/anti-patterns.md`

---

### Paso 8.5: Validate Implementation ⭐ **OBLIGATORIO - EJECUTAR TODAS LAS VALIDACIONES**

**OBLIGATORIO: Ejecutar todos los scripts de validación antes de continuar**

**Proceso automático** (seguir `ai_workflow/decision-trees/tdd-workflow.yaml` Step 8):

1. **Validar Estructura de Tests** (OBLIGATORIO):
   ```powershell
   ai_workflow/scripts/validate-test-structure.ps1 -TestDir Tests
   ```
   - **Debe salir con código 0**
   - Si código != 0 → Corregir problemas de estructura, reintentar
   - NO continuar hasta que pase

2. **Validar Compliance FDD** (OBLIGATORIO):
   ```powershell
   ai_workflow/scripts/validate-fdd-compliance.ps1 -CodeDir EcosystemGameSdk -FeaturesDir docs/features -MasterList docs/features_master_list.md
   ```
   - **Debe salir con código 0**
   - Si código != 0 → Corregir problemas de compliance FDD, reintentar
   - NO continuar hasta que pase

3. **Validación de Build**:
   - `dotnet build` - Debe tener 0 warnings, 0 errores
   - Si hay warnings/errores → Corregir, reintentar

4. **Validación de Tests**:
   - `dotnet test` - Todos los tests deben pasar
   - Si fallan → Corregir, reintentar

**CRÍTICO**: Solo continuar al Paso 9 cuando TODAS las validaciones pasen (código 0 para scripts, 0 warnings/errores para build, todos los tests pasan)

---

### Paso 9: Check for Smoke Tests Creation ⭐ **AUTOMÁTICO**

**Cuándo se ejecuta**: Después de completar cada feature

**Criterios**:
- **5+ features principales completadas** (no sub-features)
- **O** una **fase completa** de desarrollo terminada (Phase 1, Phase 2, etc.)

**Proceso automático**:
1. Contar features completadas
2. Si criterios cumplidos:
   - Verificar si proyecto smoke tests existe
   - Si no existe → Crear `Tests/EcosystemGameSdk.SmokeTests/`
   - Crear smoke tests para features completadas
   - Verificar happy path funciona
   - Ejecutar smoke tests

**Template**: `ai_workflow/templates/tests/smoke-template.md`

**Ver**: `ai_workflow/docs/SMOKE_TESTS_GUIDE.md` para guía completa

---

### Paso 10: Update Documentation ⭐ **OBLIGATORIO**

**Documentación de Feature**:
- `roadmap.md` - Marcar fases/sub-features completadas como ✅
- `architecture.md` - Actualizar si implementación difiere de spec
- `use_cases.md` - Marcar casos de uso completados
- `code_location.md` - Agregar nuevos archivos/clases
- `testing.md` - Documentar nuevos tests

**Documentos Maestros**:
- `docs/features_master_list.md` - Actualizar estado de feature
- `.ai/context.md` - Actualizar estado del proyecto

**CRÍTICO**: La documentación debe reflejar siempre el estado real de implementación

---

## 🔍 Validación

### Validar Estructura de Tests

```bash
# Bash
ai_workflow/scripts/validate-test-structure.sh Tests/

# PowerShell
ai_workflow/scripts/validate-test-structure.ps1 -TestDir Tests
```

### Validar Compliance FDD

```bash
# Bash
ai_workflow/scripts/validate-fdd-compliance.sh

# PowerShell
ai_workflow/scripts/validate-fdd-compliance.ps1
```

---

## 📚 Referencias

- **Decision Tree**: `decision-trees/tdd-workflow.yaml`
- **Templates**: `templates/tests/`
- **Schemas**: `schemas/test-structure-schema.yaml`
- **Scripts**: `scripts/validate-test-structure.*`

---

**Última Actualización**: 2025-01-XX

