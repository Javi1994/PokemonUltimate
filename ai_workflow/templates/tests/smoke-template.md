# Smoke Test Template

> **Template para smoke tests con ejemplo completo**  
> **Específico para**: C# .NET con NUnit (SDKs para Unity)

## Template Structure (C# / NUnit)

```csharp
using NUnit.Framework;
using EcosystemGameSdk.Core.[System];

namespace EcosystemGameSdk.SmokeTests.Core.[System]
{
    /// <remarks>
    /// **Smoke Tests**: Verificar que features principales funcionan juntas
    /// **Scope**: Features [N1], [N2], [N3] ([Feature Names])
    /// **Purpose**: Verificar happy path básico funciona
    /// **Note**: Tests are pure C# - no Unity dependencies
    /// </remarks>
    [TestFixture]
    public class [System]SystemSmokeTests
    {
        [Test]
        public void [System]_CreateBasicInstance_Works()
        {
            // Arrange & Act
            // Crear instancia básica de la feature principal
            // var instance = new [MainClass]();
            
            // Assert
            // Verificar que se crea correctamente
            // Assert.That(instance, Is.Not.Null);
        }
        
        [Test]
        public void [System]_BasicOperation_Works()
        {
            // Arrange
            // Setup básico
            
            // Act
            // Operación básica (happy path)
            
            // Assert
            // Verificar que funciona sin errores
        }
        
        [Test]
        public void [System]_RelatedFeatures_WorkTogether()
        {
            // Arrange
            // Setup con features relacionadas
            
            // Act
            // Interacción básica entre features
            
            // Assert
            // Verificar que funcionan juntas
        }
    }
}
```

---

## Complete Example (C# / NUnit)

```csharp
using NUnit.Framework;
using EcosystemGameSdk.Core.Entities;
using EcosystemGameSdk.Core.Entities.Animals;

namespace EcosystemGameSdk.SmokeTests.Core.Entities
{
    /// <remarks>
    /// **Smoke Tests**: Verificar que Entity System funciona correctamente
    /// **Scope**: Features 1 (Entity System), 2 (Animal System)
    /// **Purpose**: Verificar happy path básico funciona
    /// **Note**: Tests are pure C# - no Unity dependencies
    /// </remarks>
    [TestFixture]
    public class EntitySystemSmokeTests
    {
        [Test]
        public void EntitySystem_CreateEntity_Works()
        {
            // Arrange & Act
            // Cuando Animal esté implementado:
            // var animal = new Animal();
            
            // Assert
            // Assert.That(animal, Is.Not.Null);
            // Assert.That(animal, Is.InstanceOf<Entity>());
            // Assert.That(animal, Is.InstanceOf<IEntity>());
        }
        
        [Test]
        public void EntitySystem_EntityCanBeUsedPolymorphically_Works()
        {
            // Arrange
            // var entities = new IEntity[] { new Animal(), new Plant() };
            
            // Act & Assert
            // Assert.That(entities, Is.Not.Null);
            // Assert.That(entities.Length, Is.EqualTo(2));
        }
        
        [Test]
        public void EntitySystem_EntityAndEnvironment_WorkTogether()
        {
            // Arrange
            // var environment = new Environment();
            // var animal = new Animal();
            
            // Act
            // environment.AddEntity(animal);
            
            // Assert
            // Assert.That(environment.Contains(animal), Is.True);
        }
    }
}
```

---

## Naming Convention

**Pattern**: `[System]_[BasicOperation]_Works`

**Examples**:
- `EntitySystem_CreateEntity_Works`
- `AnimalSystem_CreateAnimal_Works`
- `EnvironmentSystem_AddEntity_Works`
- `EntitySystem_EntityAndEnvironment_WorkTogether`

---

## Location

**Pattern**: `SmokeTests/Core/[System]/[System]SystemSmokeTests.cs`

**Examples**:
- `SmokeTests/Core/Entities/EntitySystemSmokeTests.cs`
- `SmokeTests/Core/Environment/EnvironmentSystemSmokeTests.cs`
- `SmokeTests/Core/Behavior/BehaviorSystemSmokeTests.cs`

---

## What to Test

### ✅ DO Test
- Happy path básico de cada feature
- Creación de instancias básicas
- Operaciones básicas funcionan
- Features relacionadas pueden interactuar

### ❌ DON'T Test
- Edge cases (eso va en edge cases tests)
- Errores complejos (eso va en integration tests)
- Casos exhaustivos (solo happy path)

---

**Última Actualización**: 2025-01-XX

