# Plan de Migración: Extracción del Sistema de Localización

> **Objetivo**: Extraer todo el sistema de localización a un módulo independiente `PokemonUltimate.Localization`

## 📋 Resumen Ejecutivo

Actualmente, el sistema de localización está distribuido entre:

-   **PokemonUltimate.Core**: Interfaces, servicios y helpers (`ILocalizationProvider`, `LocalizationService`, `LocalizationData`, `LocalizationKey`, `LocalizationHelper`)
-   **PokemonUltimate.Content**: Implementación y datos (`LocalizationProvider`, `LocalizationDataProvider` y sus 17 archivos parciales)

**Objetivo**: Consolidar todo en un nuevo proyecto `PokemonUltimate.Localization` que sea independiente y reutilizable.

---

## 🎯 Estructura Objetivo

```
PokemonUltimate.Localization/
├── Providers/
│   ├── ILocalizationProvider.cs          # Interfaz (desde Core)
│   ├── LocalizationProvider.cs          # Implementación (desde Content)
│   └── LocalizationDataProvider.cs      # Datos (desde Content)
│   └── LocalizationDataProvider.*.cs    # 17 archivos parciales (desde Content)
├── Services/
│   └── LocalizationService.cs           # Servicio singleton (desde Core)
├── Data/
│   └── LocalizationData.cs              # Estructura de datos (desde Core)
├── Constants/
│   └── LocalizationKey.cs               # Constantes de claves (desde Core)
└── Helpers/
    └── LocalizationHelper.cs            # Helpers (desde Core)
```

---

## 📝 Pasos Detallados

### Fase 1: Creación del Nuevo Proyecto

#### 1.1 Crear el proyecto

```bash
dotnet new classlib -n PokemonUltimate.Localization -f netstandard2.1 -o PokemonUltimate.Localization
```

#### 1.2 Configurar el .csproj

-   Target Framework: `netstandard2.1`
-   Nullable: `disable`
-   LangVersion: `7.3` (para compatibilidad con Unity)
-   **NO** agregar referencias a otros proyectos todavía

#### 1.3 Agregar al solution

```bash
dotnet sln PokemonUltimate.sln add PokemonUltimate.Localization/PokemonUltimate.Localization.csproj
```

---

### Fase 2: Mover Componentes desde Core

#### 2.1 Mover desde `PokemonUltimate.Core/Infrastructure/Localization/`:

**Archivos a mover:**

-   `ILocalizationProvider.cs` → `PokemonUltimate.Localization/Providers/ILocalizationProvider.cs`
-   `LocalizationService.cs` → `PokemonUltimate.Localization/Services/LocalizationService.cs`
-   `LocalizationData.cs` → `PokemonUltimate.Localization/Data/LocalizationData.cs`
-   `LocalizationKey.cs` → `PokemonUltimate.Localization/Constants/LocalizationKey.cs`
-   `LocalizationHelper.cs` → `PokemonUltimate.Localization/Helpers/LocalizationHelper.cs`

**Cambios necesarios en cada archivo:**

-   Actualizar `namespace` de `PokemonUltimate.Core.Infrastructure.Localization` a `PokemonUltimate.Localization`
-   Actualizar referencias a otros namespaces si es necesario
-   Mantener los comentarios XML y referencias a features

**Dependencias a verificar:**

-   `LocalizationHelper.cs` usa `PokemonUltimate.Core.Data.Enums` y extension methods → **Necesitará referencia a Core**
-   `LocalizationService.cs` no tiene dependencias externas
-   `LocalizationData.cs` solo usa `System.Collections.Generic`
-   `LocalizationKey.cs` no tiene dependencias externas
-   `ILocalizationProvider.cs` solo usa `System.Collections.Generic`

---

### Fase 3: Mover Componentes desde Content

#### 3.1 Mover desde `PokemonUltimate.Content/Providers/`:

**Archivos a mover:**

-   `LocalizationProvider.cs` → `PokemonUltimate.Localization/Providers/LocalizationProvider.cs`
-   `Localization/LocalizationDataProvider.cs` → `PokemonUltimate.Localization/Providers/LocalizationDataProvider.cs`
-   `Localization/LocalizationDataProvider.*.cs` (17 archivos parciales) → `PokemonUltimate.Localization/Providers/`

**Lista completa de archivos parciales:**

1. `LocalizationDataProvider.cs` (archivo principal)
2. `LocalizationDataProvider.BattleMessages.cs`
3. `LocalizationDataProvider.Moves.cs`
4. `LocalizationDataProvider.Pokemon.cs`
5. `LocalizationDataProvider.Abilities.cs`
6. `LocalizationDataProvider.Items.cs`
7. `LocalizationDataProvider.Weather.cs`
8. `LocalizationDataProvider.Terrain.cs`
9. `LocalizationDataProvider.SideConditions.cs`
10. `LocalizationDataProvider.FieldEffects.cs`
11. `LocalizationDataProvider.Hazards.cs`
12. `LocalizationDataProvider.StatusEffects.cs`
13. `LocalizationDataProvider.StatusEffectNames.cs`
14. `LocalizationDataProvider.Types.cs`
15. `LocalizationDataProvider.Natures.cs`
16. `LocalizationDataProvider.Stats.cs`
17. `LocalizationDataProvider.Party.cs`
18. `LocalizationDataProvider.Helpers.cs`

**Cambios necesarios:**

-   Actualizar `namespace` de `PokemonUltimate.Content.Providers` a `PokemonUltimate.Localization.Providers`
-   `LocalizationProvider.cs` usa `PokemonUltimate.Core.Data.Constants.ErrorMessages` → **Necesitará referencia a Core**
-   `LocalizationDataProvider` usa `PokemonUltimate.Core.Infrastructure.Localization.LocalizationData` → Ya estará en el mismo proyecto después de moverlo

---

### Fase 4: Configurar Dependencias

#### 4.1 Dependencias del nuevo proyecto `PokemonUltimate.Localization`:

**Necesita referenciar:**

-   `PokemonUltimate.Core` (para `ErrorMessages` y extension methods usados en `LocalizationHelper`)

**Actualizar .csproj:**

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netstandard2.1</TargetFramework>
    <Nullable>disable</Nullable>
    <LangVersion>7.3</LangVersion>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\PokemonUltimate.Core\PokemonUltimate.Core.csproj" />
  </ItemGroup>
</Project>
```

#### 4.2 Actualizar proyectos que usan localización:

**Proyectos que necesitan actualizar referencias:**

1. **PokemonUltimate.Core**

    - Eliminar referencias internas a `Infrastructure.Localization`
    - Agregar referencia a `PokemonUltimate.Localization`
    - Actualizar `using` statements

2. **PokemonUltimate.Content**

    - Eliminar `LocalizationProvider.cs` y carpeta `Providers/Localization/`
    - Agregar referencia a `PokemonUltimate.Localization`
    - Actualizar `using` statements

3. **PokemonUltimate.Combat**

    - Agregar referencia a `PokemonUltimate.Localization`
    - Actualizar `using` statements de `PokemonUltimate.Core.Infrastructure.Localization` a `PokemonUltimate.Localization`

4. **PokemonUltimate.BattleSimulator**

    - Agregar referencia a `PokemonUltimate.Localization`
    - Actualizar `using` statements

5. **PokemonUltimate.DeveloperTools**

    - Agregar referencia a `PokemonUltimate.Localization`
    - Actualizar `using` statements

6. **PokemonUltimate.DataViewer**
    - Agregar referencia a `PokemonUltimate.Localization`
    - Actualizar `using` statements

---

### Fase 5: Actualizar Referencias en el Código

#### 5.1 Buscar y reemplazar `using` statements:

**Buscar:**

```csharp
using PokemonUltimate.Core.Infrastructure.Localization;
using PokemonUltimate.Content.Providers;
```

**Reemplazar por:**

```csharp
using PokemonUltimate.Localization;
using PokemonUltimate.Localization.Providers;
using PokemonUltimate.Localization.Services;
using PokemonUltimate.Localization.Data;
using PokemonUltimate.Localization.Constants;
using PokemonUltimate.Localization.Helpers;
```

#### 5.2 Archivos que necesitan actualización:

**Usar grep para encontrar todos los archivos:**

```bash
# Buscar referencias a LocalizationProvider
grep -r "LocalizationProvider" --include="*.cs"

# Buscar referencias a LocalizationService
grep -r "LocalizationService" --include="*.cs"

# Buscar referencias a LocalizationData
grep -r "LocalizationData" --include="*.cs"

# Buscar referencias a LocalizationKey
grep -r "LocalizationKey" --include="*.cs"

# Buscar using statements antiguos
grep -r "using PokemonUltimate.Core.Infrastructure.Localization" --include="*.cs"
grep -r "using PokemonUltimate.Content.Providers" --include="*.cs"
```

**Archivos conocidos que necesitan actualización:**

-   `PokemonUltimate.Combat/Infrastructure/Messages/BattleMessageFormatter.cs`
-   `PokemonUltimate.Core/Utilities/Extensions/*.cs` (extension methods que usan ILocalizationProvider)
-   Todos los archivos en `PokemonUltimate.Content/Extensions/` que usan localización
-   Archivos en proyectos WinForms (BattleSimulator, DeveloperTools, DataViewer)

---

### Fase 6: Verificación y Compilación

#### 6.1 Verificar estructura del proyecto:

```bash
# Verificar que todos los archivos están en su lugar
ls -R PokemonUltimate.Localization/

# Verificar que no quedan archivos antiguos
find PokemonUltimate.Core -name "*Localization*" -type f
find PokemonUltimate.Content -name "*Localization*" -type f
```

#### 6.2 Compilar el proyecto:

```bash
dotnet build PokemonUltimate.Localization/PokemonUltimate.Localization.csproj
```

#### 6.3 Compilar solución completa:

```bash
dotnet build PokemonUltimate.sln
```

#### 6.4 Ejecutar tests:

```bash
dotnet test
```

---

### Fase 7: Limpieza

#### 7.1 Eliminar archivos antiguos:

-   Eliminar carpeta `PokemonUltimate.Core/Infrastructure/Localization/` (si está vacía después de mover archivos)
-   Eliminar `PokemonUltimate.Content/Providers/LocalizationProvider.cs`
-   Eliminar carpeta `PokemonUltimate.Content/Providers/Localization/`

#### 7.2 Verificar que no hay referencias rotas:

```bash
# Buscar referencias a rutas antiguas
grep -r "Core.Infrastructure.Localization" --include="*.cs"
grep -r "Content.Providers.Localization" --include="*.cs"
```

---

## 🔍 Verificaciones Post-Migración

### Checklist de Verificación:

-   [ ] Proyecto `PokemonUltimate.Localization` creado y agregado al solution
-   [ ] Todos los archivos movidos desde Core
-   [ ] Todos los archivos movidos desde Content
-   [ ] Namespaces actualizados en todos los archivos movidos
-   [ ] Referencias de proyecto actualizadas en todos los .csproj
-   [ ] `using` statements actualizados en todos los archivos
-   [ ] Proyecto compila sin errores
-   [ ] Solución completa compila sin errores
-   [ ] Tests pasan correctamente
-   [ ] Archivos antiguos eliminados
-   [ ] No quedan referencias a rutas antiguas

---

## 📊 Impacto en Proyectos

### Dependencias Actualizadas:

| Proyecto                          | Acción                 | Razón                                 |
| --------------------------------- | ---------------------- | ------------------------------------- |
| `PokemonUltimate.Localization`    | Crear nuevo            | Nuevo módulo independiente            |
| `PokemonUltimate.Core`            | Actualizar referencias | Ya no contiene código de localización |
| `PokemonUltimate.Content`         | Actualizar referencias | Ya no contiene código de localización |
| `PokemonUltimate.Combat`          | Agregar referencia     | Usa `ILocalizationProvider`           |
| `PokemonUltimate.BattleSimulator` | Agregar referencia     | Usa localización                      |
| `PokemonUltimate.DeveloperTools`  | Agregar referencia     | Usa localización                      |
| `PokemonUltimate.DataViewer`      | Agregar referencia     | Usa localización                      |

---

## ⚠️ Consideraciones Importantes

### 1. Dependencia Circular

-   `PokemonUltimate.Localization` necesita referenciar `PokemonUltimate.Core` para:
    -   `ErrorMessages` (usado en `LocalizationProvider`)
    -   Extension methods (usados en `LocalizationHelper`)
-   Esto está bien porque `Core` es una dependencia base y `Localization` es un módulo de infraestructura.

### 2. Compatibilidad con Unity

-   Mantener `netstandard2.1` y `LangVersion 7.3` para compatibilidad
-   No usar características de C# modernas que Unity no soporte

### 3. Tests

-   Verificar que los tests de localización sigan funcionando
-   Puede ser necesario actualizar rutas de namespaces en tests

### 4. Documentación

-   Actualizar `docs/features/4-unity-integration/4.9-localization-system/code_location.md`
-   Actualizar referencias en documentación de arquitectura

---

## 🚀 Orden de Ejecución Recomendado

1. **Crear proyecto** (Fase 1)
2. **Mover archivos desde Core** (Fase 2)
3. **Mover archivos desde Content** (Fase 3)
4. **Configurar dependencias** (Fase 4)
5. **Actualizar referencias** (Fase 5) - Proyecto por proyecto
6. **Compilar y verificar** (Fase 6)
7. **Limpiar archivos antiguos** (Fase 7)

---

## 📚 Referencias

-   Documentación del sistema de localización: `docs/features/4-unity-integration/4.9-localization-system/`
-   Arquitectura: `docs/features/4-unity-integration/4.9-localization-system/architecture.md`
-   Código actual:
    -   Core: `PokemonUltimate.Core/Infrastructure/Localization/`
    -   Content: `PokemonUltimate.Content/Providers/Localization/`

---

**Fecha de creación**: 2025-01-XX  
**Estado**: Planificación  
**Próximo paso**: Ejecutar Fase 1 (Crear proyecto)
