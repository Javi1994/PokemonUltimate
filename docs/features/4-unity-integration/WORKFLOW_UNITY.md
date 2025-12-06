# Feature 4: Unity Integration - Workflow Guide

> **Guía completa para trabajar con PokemonUltimate (.NET) y Unity simultáneamente**

**Feature Number**: 4  
**Feature Name**: Unity Integration  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## 🎯 Overview

Esta guía explica cómo trabajar eficientemente con ambos proyectos:
- **PokemonUltimate** (repo .NET) - Lógica del juego
- **PokemonUltimateUnity** (proyecto Unity) - Presentación visual

## 📁 Estructura Recomendada

### Opción 1: Monorepo (Recomendada) ⭐

```
PokemonUltimate/                    # Repo principal
├── PokemonUltimate.Core/          # Proyectos .NET
├── PokemonUltimate.Combat/
├── PokemonUltimate.Content/
├── PokemonUltimate.Tests/              # All tests here (no Unity tests)
├── docs/                          # Documentación (incluye Feature 4)
│   └── features/
│       └── 4-unity-integration/
└── PokemonUltimateUnity/          # Proyecto Unity (subdirectorio)
    ├── Assets/
    │   ├── Plugins/               # DLLs (gitignored, generados)
    │   └── Scripts/               # Código Unity
    ├── Scenes/
    └── (No Tests/)                    # No Unity tests
```

**Ventajas**:
- ✅ Todo en un solo lugar
- ✅ Fácil sincronización de cambios
- ✅ El asistente de IA puede ver ambos proyectos
- ✅ Documentación compartida
- ✅ Historial unificado

**Desventajas**:
- ⚠️ Repo más grande
- ⚠️ Necesita `.gitignore` para archivos Unity

### Opción 2: Repos Separados

```
PokemonUltimate/                   # Repo .NET
└── (proyectos .NET)

PokemonUltimateUnity/              # Repo Unity separado
└── (proyecto Unity)
```

**Ventajas**:
- ✅ Repos más pequeños
- ✅ Separación clara

**Desventajas**:
- ❌ Más difícil sincronizar cambios
- ❌ El asistente de IA necesita contexto de ambos repos
- ❌ Documentación duplicada o desincronizada

## 🚀 Workflow Recomendado: Monorepo

### Setup Inicial

1. **Crear proyecto Unity dentro del repo**:
   ```bash
   # Desde PokemonUltimate/
   mkdir PokemonUltimateUnity
   # Crear proyecto Unity en Unity Hub apuntando a PokemonUltimateUnity/
   ```

2. **Configurar .gitignore**:
   ```gitignore
   # Unity (ya incluido en .gitignore)
   [Ll]ibrary/
   [Tt]emp/
   [Ll]ogs/
   [Uu]ser[Ss]ettings/
   
   # DLLs generados (no versionar)
   PokemonUltimateUnity/Assets/Plugins/*.dll
   PokemonUltimateUnity/Assets/Plugins/*.pdb
   ```

3. **Crear script de sincronización**:
   ```powershell
   # ai_workflow/scripts/sync-dlls-to-unity.ps1
   # Ya existe: build-dlls-for-unity.ps1
   ```

### Workflow Diario

#### 1. Trabajando en .NET (PokemonUltimate)

```bash
# Hacer cambios en .NET
# Ejemplo: Agregar método a IBattleView

# Build y sincronizar DLLs
.\ai_workflow\scripts\build-dlls-for-unity.ps1 -UnityProjectPath ".\PokemonUltimateUnity"

# Unity detecta cambios automáticamente
```

**Con el asistente de IA**:
- El asistente puede ver todo el código .NET
- Puede leer documentación de Feature 4
- Puede hacer cambios y actualizar DLLs

#### 2. Trabajando en Unity (PokemonUltimateUnity)

```bash
# Abrir Unity Editor
# Trabajar en Assets/Scripts/

# Los cambios en Unity no afectan .NET
# Solo usa los DLLs como plugins
```

**Con el asistente de IA**:
- El asistente puede leer código Unity si está en el workspace
- Puede leer documentación de Feature 4
- Puede hacer cambios en scripts Unity

#### 3. Cambios que Afectan Ambos

**Ejemplo: Cambiar IBattleView**

1. **En .NET**:
   ```csharp
   // PokemonUltimate.Combat/View/IBattleView.cs
   Task NewMethod(BattleSlot slot);
   ```

2. **Build y sincronizar**:
   ```powershell
   .\ai_workflow\scripts\build-dlls-for-unity.ps1 -UnityProjectPath ".\PokemonUltimateUnity"
   ```

3. **En Unity**:
   ```csharp
   // Assets/Scripts/Battle/UnityBattleView.cs
   public Task NewMethod(BattleSlot slot) {
       // Implementar
   }
   ```

## 🤖 Trabajando con el Asistente de IA

### Configuración del Workspace

**Para que el asistente vea ambos proyectos**:

1. **Abrir workspace en Cursor**:
   ```
   File > Open Folder > PokemonUltimate/
   ```

2. **El asistente puede ver**:
   - ✅ Todo el código .NET
   - ✅ Código Unity (si está en el workspace)
   - ✅ Documentación completa
   - ✅ Scripts de build

### Comandos Útiles para el Asistente

**Cuando trabajas en .NET**:
```
"Implementa método X en IBattleView"
"Actualiza la documentación de Feature 4"
"Build DLLs y sincroniza con Unity"
```

**Cuando trabajas en Unity**:
```
"Implementa UnityBattleView con el nuevo método"
"Lee la documentación de Feature 4 para ver qué métodos necesito"
"Actualiza el código Unity para usar la nueva versión de IBattleView"
```

**Cuando cambias interfaces**:
```
"Cambié IBattleView, actualiza UnityBattleView y la documentación"
"Build DLLs y verifica que Unity compile"
```

## 📋 Checklist de Sincronización

### Cuando Cambias .NET

- [ ] Hacer cambios en código .NET
- [ ] Ejecutar tests: `dotnet test`
- [ ] Build Release: `dotnet build -c Release`
- [ ] Sincronizar DLLs: `.\ai_workflow\scripts\build-dlls-for-unity.ps1`
- [ ] Verificar que Unity compile sin errores
- [ ] Actualizar documentación si cambió interfaz

### Cuando Cambias Unity

- [ ] Hacer cambios en código Unity
- [ ] Verificar que usa métodos correctos de IBattleView
- [ ] Ejecutar tests Unity (si existen)
- [ ] Verificar que compila

### Cuando Cambias Ambos

- [ ] Cambiar interfaz en .NET
- [ ] Build y sincronizar DLLs
- [ ] Actualizar implementación Unity
- [ ] Actualizar documentación
- [ ] Ejecutar tests en ambos proyectos

## 🔄 Automatización

### Script de Sincronización Automática

```powershell
# ai_workflow/scripts/watch-and-sync.ps1
# Watch .NET changes and auto-sync DLLs to Unity

$watcher = New-Object System.IO.FileSystemWatcher
$watcher.Path = "PokemonUltimate.Combat"
$watcher.Filter = "*.cs"
$watcher.IncludeSubdirectories = $true

$watcher.OnChanged = {
    Write-Host "Change detected, rebuilding DLLs..."
    .\ai_workflow\scripts\build-dlls-for-unity.ps1 -UnityProjectPath ".\PokemonUltimateUnity"
}

$watcher.EnableRaisingEvents = $true
```

### Pre-commit Hook (Opcional)

```bash
# .git/hooks/pre-commit
# Verificar que DLLs están actualizados antes de commit
```

## 📚 Documentación Compartida

### Ubicación

```
PokemonUltimate/
└── docs/
    └── features/
        └── 4-unity-integration/
            ├── architecture.md      # Especificación técnica
            ├── roadmap.md           # Plan de implementación
            ├── code_location.md    # Dónde está el código
            └── WORKFLOW_UNITY.md    # Este archivo
```

### Mantenimiento

- **Documentación .NET**: En `docs/features/4-unity-integration/`
- **Documentación Unity**: Puede estar en:
  - `PokemonUltimateUnity/README.md` (proyecto específico)
  - `docs/features/4-unity-integration/` (compartida)

## 🎯 Mejores Prácticas

### 1. Separación de Responsabilidades

- **.NET**: Lógica del juego, interfaces
- **Unity**: Implementación visual, UI, audio

### 2. Versionado

- **DLLs**: No versionar (generados)
- **Código fuente**: Sí versionar
- **Documentación**: Sí versionar

### 3. Testing

- **.NET**: Tests unitarios e integración (NUnit)
- **Unity**: Tests Unity (EditMode/PlayMode)

### 4. Builds

- **.NET**: `dotnet build -c Release`
- **Unity**: Build desde Unity Editor

## 🐛 Troubleshooting

### DLLs no se actualizan

```powershell
# Forzar rebuild
dotnet clean
dotnet build -c Release
.\ai_workflow\scripts\build-dlls-for-unity.ps1 -UnityProjectPath ".\PokemonUltimateUnity"
```

### Unity no encuentra tipos

1. Verificar que DLLs están en `Assets/Plugins/`
2. Verificar API Compatibility Level = `.NET Standard 2.1`
3. Reimportar DLLs en Unity

### Cambios en interfaz no se reflejan

1. Verificar que DLLs están actualizados
2. Verificar que Unity recompiló scripts
3. Cerrar y reabrir Unity si es necesario

## 📖 Referencias

- **[Architecture](architecture.md)** - Especificación técnica de IBattleView
- **[Roadmap](roadmap.md)** - Plan de implementación
- **[Code Location](code_location.md)** - Dónde está el código
- **[Getting Started](4.1-unity-project-setup/GETTING_STARTED.md)** - Setup inicial

---

**Last Updated**: 2025-01-XX

