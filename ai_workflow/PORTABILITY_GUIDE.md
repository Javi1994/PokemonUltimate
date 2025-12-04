# Portability Guide - AI Workflow Optimization

> **Guía completa para llevar el workflow a otro proyecto desde cero**  
> **Última actualización**: 2025-01-27

## ✅ ¿Es Portable?

**SÍ, el workflow es completamente portable**. Todo el sistema `ai_workflow/` está diseñado para funcionar en cualquier proyecto nuevo sin modificaciones.

---

## 📦 Qué Copiar

### Opción A: Copiar Todo `ai_workflow/` (Recomendado)

```bash
# Desde el proyecto actual
cp -r ai_workflow/ /ruta/nuevo-proyecto/

# O con Git
git clone <repo> nuevo-proyecto
# El ai_workflow/ ya está incluido
```

**Estructura completa a copiar:**
```
ai_workflow/
├── README.md
├── START_HERE.md ⭐ **LEER PRIMERO**
├── INDEX.md
├── PORTABILITY_GUIDE.md (este archivo)
├── PROMPTS_GUIDE.md
├── AI_QUICK_REFERENCE.md
├── schemas/
├── decision-trees/
├── templates/
├── scripts/
└── docs/
```

**✅ Todo esto es genérico y funciona sin cambios**

---

## 🔧 Setup en Nuevo Proyecto

### Paso 1: Copiar `ai_workflow/`

```bash
# Copiar carpeta completa
cp -r /proyecto-origen/ai_workflow/ /nuevo-proyecto/
```

### Paso 2: Ejecutar Script de Setup

```powershell
# PowerShell
cd nuevo-proyecto
ai_workflow/scripts/setup-project.ps1
```

```bash
# Bash
cd nuevo-proyecto
chmod +x ai_workflow/scripts/setup-project.sh
./ai_workflow/scripts/setup-project.sh
```

**Esto crea automáticamente:**
- `docs/` y `docs/features/`
- `Tests/`
- `.ai/`
- `.cursorrules` (desde template)
- `.gitignore` (desde template)

### Paso 3: Configurar Proyecto de Tests (C# .NET)

```powershell
# Ajustar nombres según tu proyecto
dotnet new nunit -n Tests.TuProyecto -o Tests
dotnet add Tests/TuProyecto.Tests.csproj reference TuProyecto/TuProyecto.csproj
dotnet sln add Tests/TuProyecto.Tests.csproj
```

**Nota**: Los scripts de validación están optimizados para C#. Si usas otro lenguaje, necesitarás adaptar los scripts (ver abajo).

### Paso 4: Definir el Juego

```markdown
Di a la IA: "Define game" o "What is this game?"
```

El sistema generará automáticamente:
- `docs/GAME_DEFINITION.yaml`
- `docs/features_master_list.md`
- `docs/features_master_list_detailed.md`
- `docs/features_master_list_index.md`

---

## ⚙️ Configuración Necesaria

### 1. Scripts de Validación (Parámetros)

Los scripts de validación aceptan parámetros para adaptarse a cualquier proyecto:

**`validate-fdd-compliance.ps1`:**
```powershell
# Parámetros por defecto (funcionan si estructura es estándar)
ai_workflow/scripts/validate-fdd-compliance.ps1

# O especificar manualmente
ai_workflow/scripts/validate-fdd-compliance.ps1 `
  -CodeDir "TuProyecto" `
  -FeaturesDir "docs/features" `
  -MasterList "docs/features_master_list.md"
```

**`validate-test-structure.ps1`:**
```powershell
# Parámetro por defecto: Tests/
ai_workflow/scripts/validate-test-structure.ps1

# O especificar manualmente
ai_workflow/scripts/validate-test-structure.ps1 -TestDir "Tests"
```

**✅ Los scripts son genéricos y funcionan con cualquier estructura estándar**

### 2. Actualizar `.cursorrules` (Opcional)

El `.cursorrules` se genera automáticamente del template. Solo necesitas personalizarlo si:

- Cambias el lenguaje (no C#)
- Cambias el framework de tests (no NUnit)
- Cambias la estructura de directorios

**Por defecto, funciona sin cambios para C# .NET + NUnit**

### 3. Ejemplos en Templates

Los templates contienen ejemplos con nombres como "EcosystemGame" o "EcosystemGameSdk", pero son **solo ejemplos**. La IA los reemplazará automáticamente con los nombres de tu proyecto.

**✅ No necesitas cambiar nada en los templates**

---

## 🔍 Qué Buscar y Reemplazar (Opcional)

Si quieres personalizar completamente, busca estos términos (pero **no es necesario**):

### En Documentación (Solo Ejemplos)
- `EcosystemGame` → Tu nombre de juego
- `EcosystemGameSdk` → Tu nombre de proyecto SDK

### En Scripts (Ya Parametrizados)
- Los scripts ya usan parámetros, no necesitas cambiar nada

### En Templates (Solo Ejemplos)
- Los ejemplos son solo guías, la IA los adapta automáticamente

---

## ✅ Checklist de Portabilidad

- [ ] Copiar carpeta `ai_workflow/` completa
- [ ] Ejecutar `setup-project.ps1` o `.sh`
- [ ] Configurar proyecto de tests (C# .NET + NUnit)
- [ ] Verificar que scripts funcionan:
  ```powershell
  ai_workflow/scripts/validate-test-structure.ps1 -TestDir Tests
  ai_workflow/scripts/validate-fdd-compliance.ps1
  ```
- [ ] Definir juego: "Define game"
- [ ] Implementar primera feature: "Implement Feature X"

---

## 🎯 Casos Especiales

### Si NO es C# .NET

Los scripts de validación están optimizados para C#. Necesitarás:

1. **Adaptar scripts de validación**:
   - `validate-test-structure.ps1` - Ajustar regex para tu lenguaje
   - `validate-fdd-compliance.ps1` - Ajustar búsqueda de archivos

2. **Adaptar templates de tests**:
   - `templates/tests/functional-template.md`
   - `templates/tests/edgecases-template.md`
   - `templates/tests/integration-template.md`

3. **Actualizar `.cursorrules`**:
   - Cambiar referencias a NUnit por tu framework
   - Cambiar comandos de build/test

### Si NO es SDK para Unity

El workflow está optimizado para SDKs que se integran con Unity, pero funciona igual para:

- SDKs para otros motores (Unreal, Godot, etc.)
- Librerías standalone
- Aplicaciones completas

**Solo cambia**:
- En Game Definition, especifica tu plataforma objetivo
- El workflow se adapta automáticamente

---

## 📚 Documentación de Referencia

- **`START_HERE.md`** - Guía paso a paso para proyectos nuevos
- **`PROMPTS_GUIDE.md`** - Cómo interactuar con la IA
- **`AI_QUICK_REFERENCE.md`** - Referencia rápida para la IA
- **`INDEX.md`** - Índice completo de todos los archivos

---

## 🚀 Resumen

**El workflow es 100% portable**. Solo necesitas:

1. Copiar `ai_workflow/`
2. Ejecutar script de setup
3. Configurar proyecto de tests
4. Definir juego
5. ¡Empezar a desarrollar!

**Todo lo demás funciona automáticamente sin cambios.**

---

**¿Preguntas?** Revisa `START_HERE.md` o `INDEX.md` para más detalles.

