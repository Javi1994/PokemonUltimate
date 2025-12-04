# Smoke Tests Guide

> **Guía completa para smoke tests en proyectos grandes**  
> **Específico para**: C# .NET SDKs integrados con Unity

## 🎯 Overview

Los **Smoke Tests** son tests básicos que verifican que las features principales funcionan correctamente juntas. Son diferentes de los tests de integración porque:

- **Smoke Tests**: Verifican que las features básicas funcionan juntas (happy path)
- **Integration Tests**: Verifican interacciones complejas entre sistemas

## 📋 Cuándo Crear Smoke Tests

### Criterios Automáticos

Los smoke tests se crean automáticamente cuando:
- **5+ features completadas** (features principales, no sub-features)
- **O** cuando se completa una **fase completa** de desarrollo (Phase 1, Phase 2, etc.)

### Trigger Manual

También puedes crear smoke tests manualmente cuando:
- Quieres verificar que varias features funcionan juntas
- Antes de un release importante
- Después de refactorizaciones grandes

## 🏗️ Estructura Propuesta

### Opción Recomendada: Proyecto Separado

```
Tests/
├── EcosystemGameSdk.Tests/          # Tests unitarios y de integración
│   └── Core/
│       └── ...
│
└── EcosystemGameSdk.SmokeTests/     # Smoke tests (proyecto separado)
    └── EcosystemGameSdk.SmokeTests.csproj
    └── Core/
        ├── Entities/
        │   └── EntitySystemSmokeTests.cs
        ├── Environment/
        │   └── EnvironmentSystemSmokeTests.cs
        └── ...
```

**Ventajas**:
- Separación clara de responsabilidades
- Puede ejecutarse independientemente
- No afecta tests unitarios
- Fácil de ejecutar en CI/CD

### Alternativa: Carpeta Separada (si prefieres un solo proyecto)

```
Tests/EcosystemGameSdk.Tests/
├── Core/
│   └── ... (tests unitarios)
│
└── SmokeTests/                       # Smoke tests en carpeta separada
    ├── Entities/
    │   └── EntitySystemSmokeTests.cs
    └── ...
```

## 📝 Contenido de Smoke Tests

### Qué Probar

Los smoke tests deben verificar:
1. **Features básicas funcionan**: Cada feature principal puede crear/instanciar sus objetos básicos
2. **Features trabajan juntas**: Features relacionadas pueden interactuar correctamente
3. **Happy path funciona**: Flujos básicos completos funcionan sin errores

### Qué NO Probar

Los smoke tests NO deben:
- Probar casos edge (eso va en edge cases tests)
- Probar errores complejos (eso va en integration tests)
- Ser exhaustivos (solo happy path básico)

## 🎯 Ejemplo de Smoke Test

```csharp
using NUnit.Framework;
using EcosystemGameSdk.Core.Entities;
using EcosystemGameSdk.Core.Environment;

namespace EcosystemGameSdk.SmokeTests.Core.Entities
{
    /// <remarks>
    /// **Smoke Tests**: Verificar que features principales funcionan juntas
    /// **Scope**: Features 1, 2, 6 (Entity System, Animal System, Environment System)
    /// **Purpose**: Verificar happy path básico funciona
    /// </remarks>
    [TestFixture]
    public class EntitySystemSmokeTests
    {
        [Test]
        public void EntitySystem_CreateEntity_Works()
        {
            // Arrange & Act
            // Crear una entidad básica (cuando esté implementado)
            // var entity = new TestEntity();
            
            // Assert
            // Verificar que se crea correctamente
            // Assert.That(entity, Is.Not.Null);
        }
        
        [Test]
        public void EntitySystem_EntityCanBeUsed_Works()
        {
            // Arrange & Act
            // Usar entidad en contexto básico
            
            // Assert
            // Verificar que funciona
        }
    }
}
```

## 🔄 Integración con Workflow

### Paso Automático en Workflow

Después de completar una feature, el sistema verifica:
1. ¿Hay 5+ features completadas?
2. ¿Se completó una fase completa?
3. Si SÍ → Crear/actualizar smoke tests automáticamente

### Proceso Manual

Si quieres crear smoke tests manualmente:
1. Di: "Create smoke tests" o "Add smoke tests"
2. El sistema verificará features completadas
3. Creará/actualizará smoke tests según features disponibles

## 📊 Estructura de Smoke Tests por Feature

```
SmokeTests/
├── Core/
│   ├── Entities/
│   │   └── EntitySystemSmokeTests.cs      # Features 1, 2, 3, 4, 5
│   ├── Environment/
│   │   └── EnvironmentSystemSmokeTests.cs  # Feature 6
│   ├── Behavior/
│   │   └── BehaviorSystemSmokeTests.cs    # Feature 7
│   └── ...
```

## ✅ Checklist de Smoke Tests

Antes de considerar smoke tests completos:
- [ ] Proyecto de smoke tests creado (si es proyecto separado)
- [ ] Smoke tests para cada feature principal completada
- [ ] Happy path básico verificado
- [ ] Features relacionadas pueden interactuar
- [ ] Todos los smoke tests pasan
- [ ] Documentación actualizada

---

**Última Actualización**: 2025-01-XX

