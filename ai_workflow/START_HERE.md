# 🚀 Start Here - Primeros Pasos para Nuevo Proyecto

> **Guía paso a paso para empezar un proyecto desde cero con AI Workflow**  
> **Específico para**: C# .NET SDKs integrados con Unity

## ⚡ Setup Inicial (5 minutos)

### Paso 1: Crear Estructura de Directorios

**Opción A: Script Automático (Recomendado)**

```powershell
# PowerShell
ai_workflow/scripts/setup-project.ps1
```

```bash
# Bash
chmod +x ai_workflow/scripts/setup-project.sh
./ai_workflow/scripts/setup-project.sh
```

**Opción B: Manual**

```powershell
# PowerShell
New-Item -ItemType Directory -Force -Path "docs", "docs/features", "Tests", ".ai"
```

```bash
# Bash
mkdir -p docs/features Tests .ai
```

**Estructura esperada:**
```
tu-proyecto/
├── ai_workflow/          # Sistema AI Workflow (ya existe)
├── docs/                 # Documentación del proyecto
│   └── features/         # Features del proyecto
├── Tests/                # Tests organizados por feature
├── .ai/                  # Contexto para IA
└── .cursorrules          # Configuración de Cursor (crear en Paso 2)
```

---

### Paso 2: Crear `.cursorrules` y `.gitignore`

El script de setup crea ambos automáticamente. Si lo haces manualmente:

```powershell
# PowerShell
Copy-Item ai_workflow/cursorrules-template.md .cursorrules
Copy-Item ai_workflow/templates/gitignore-template .gitignore
```

```bash
# Bash
cp ai_workflow/cursorrules-template.md .cursorrules
cp ai_workflow/templates/gitignore-template .gitignore
```

**Nota**: El `.gitignore` incluye exclusiones para:
- Archivos de build de .NET (bin/, obj/)
- Archivos de Unity (si aplica)
- Archivos temporales del workflow AI (.ai/context.md)
- Archivos de IDE (Rider, Visual Studio)
- Archivos del sistema operativo

---

### Paso 3: Configurar Proyecto de Tests (C# .NET + NUnit)

**Este workflow está optimizado para C# .NET con NUnit**. Si aún no tienes proyecto de tests:

```powershell
# Crear proyecto de tests NUnit
dotnet new nunit -n Tests.ProjectName -o Tests

# Agregar referencia al proyecto principal (SDK)
dotnet add Tests/ProjectName.Tests.csproj reference ProjectName/ProjectName.csproj

# Agregar a solución
dotnet sln add Tests/ProjectName.Tests.csproj

# Verificar que compila
dotnet build Tests/
```

**Nota**: Los templates de tests están optimizados para C# y NUnit. Los scripts de validación están adaptados para C#.

**Ajusta los nombres** según tu proyecto.

---

### Paso 4: Verificar Scripts de Validación

Verifica que los scripts sean ejecutables:

```powershell
# PowerShell (Windows)
# Los scripts .ps1 deberían funcionar directamente
# Probar:
ai_workflow/scripts/validate-test-structure.ps1 -TestDir Tests
```

```bash
# Bash (Linux/Mac)
chmod +x ai_workflow/scripts/*.sh
./ai_workflow/scripts/validate-test-structure.sh Tests/
```

---

## 🎮 Paso 5: Definir el Juego ⭐ **CRÍTICO - HACER PRIMERO**

**Este es el paso más importante**. Sin esto, no puedes empezar a desarrollar features.

**Nota**: Al definir el juego, especifica que es un **SDK para Unity**. El sistema generará features considerando:
- Lógica del juego en el SDK (C# .NET Standard 2.1)
- Integración con Unity (interfaces, eventos, etc.)
- Separación entre lógica del juego y presentación (Unity)

### Opción A: Usar IA (Recomendado)

Simplemente di:
```
"Define game" o "What is this game?"
```

El sistema ejecutará automáticamente:
- `ai_workflow/decision-trees/game-definition.yaml`
- Creará `docs/GAME_DEFINITION.yaml` (con Unity como plataforma)
- Generará `docs/features_master_list.md`
- Generará `docs/features_master_list_detailed.md`

### Opción B: Manual

1. Lee `ai_workflow/decision-trees/game-definition.yaml`
2. Usa `ai_workflow/templates/game-definition-template.yaml`
3. Crea `docs/GAME_DEFINITION.yaml` manualmente
4. Genera las master lists

### Opción C: Mejorar Definición Existente

Si ya tienes una definición y quieres mejorarla:

```
"Redefine game definition" o "Improve game definition"
```

El sistema:
1. Lee tu definición actual
2. Analiza completitud y mejores prácticas
3. Identifica mejoras y conceptos faltantes
4. Sugiere mejoras específicas
5. Actualiza la definición con mejoras confirmadas

**Ver**: [`docs/GAME_DEFINITION_GUIDE.md`](docs/GAME_DEFINITION_GUIDE.md) para detalles completos

---

### Paso 6: Estrategia de Organización de Código (Automático)

Si tu proyecto tiene **5+ features principales** o **20+ sub-features**, el workflow creará automáticamente `docs/CODE_ORGANIZATION.md` después de la Game Definition.

Este documento define:
- **Cómo organizar el código** por sistema técnico (no por feature)
- **Cómo organizar los tests** reflejando la estructura del código
- **Mapeo Feature → Código** para facilitar navegación

**Si no se crea automáticamente**, puedes crearlo manualmente siguiendo el ejemplo en `docs/CODE_ORGANIZATION.md` de este proyecto.

**Ver**: `docs/CODE_ORGANIZATION.md` para la estrategia completa

---

## ✅ Checklist de Setup Completo

Antes de empezar a desarrollar, verifica:

- [ ] Estructura de directorios creada (`docs/`, `docs/features/`, `Tests/`, `.ai/`)
- [ ] `.cursorrules` creado en la raíz del proyecto
- [ ] Proyecto de tests configurado (si aplica)
- [ ] Scripts de validación funcionando
- [ ] **Juego definido** (`docs/GAME_DEFINITION.yaml` existe)
- [ ] Master lists generadas (`docs/features_master_list.md` existe)
- [ ] **Estrategia de organización definida** (`docs/CODE_ORGANIZATION.md` existe) ⭐ **Recomendado para proyectos grandes**

---

## 🎯 Siguiente Paso: Desarrollar Primera Feature

Una vez completado el setup:

1. **Di**: "Implement [nombre de feature]" o "Add [funcionalidad]"
2. El sistema ejecutará automáticamente:
   - Feature Discovery (`ai_workflow/decision-trees/feature-discovery.yaml`)
   - TDD Workflow (`ai_workflow/decision-trees/tdd-workflow.yaml`)
   - Creará documentación de feature
   - Escribirá tests primero
   - Implementará código

**Ver**: [`docs/TDD_GUIDE.md`](docs/TDD_GUIDE.md) y [`docs/FDD_GUIDE.md`](docs/FDD_GUIDE.md) para más detalles

---

## 🆘 Troubleshooting

### Problema: `.cursorrules` no se reconoce
- Verifica que esté en la raíz del proyecto
- Verifica que tenga extensión `.cursorrules` (sin extensión)
- Reinicia Cursor si es necesario

### Problema: Scripts no funcionan
- PowerShell: Verifica política de ejecución (`Set-ExecutionPolicy RemoteSigned`)
- Bash: Verifica permisos (`chmod +x`)

### Problema: Game Definition no funciona
- Verifica que `docs/` existe
- Verifica que tienes permisos de escritura
- Lee `ai_workflow/docs/GAME_DEFINITION_GUIDE.md` para troubleshooting

---

## 📚 Documentación de Referencia

- [`README.md`](README.md) - Overview del sistema
- [`PROMPTS_GUIDE.md`](PROMPTS_GUIDE.md) ⭐ **Cómo pedir cosas - Guía de prompts**
- [`INDEX.md`](INDEX.md) - Índice completo
- [`docs/GAME_DEFINITION_GUIDE.md`](docs/GAME_DEFINITION_GUIDE.md) - Guía Game Definition
- [`docs/TDD_GUIDE.md`](docs/TDD_GUIDE.md) - Guía TDD
- [`docs/FDD_GUIDE.md`](docs/FDD_GUIDE.md) - Guía FDD

---

**Última Actualización**: 2025-01-XX

