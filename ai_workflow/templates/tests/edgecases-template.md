# Edge Cases Test Template

> **Template para tests de edge cases con ejemplo completo**  
> **Específico para**: C# .NET con NUnit (SDKs para Unity)

## Template Structure

```[Language]
[TestFramework imports]

/// <remarks>
/// **Feature**: [N]: [Feature Name]
/// **Sub-Feature**: [N.M]: [Sub-Feature Name] (if applicable)
/// **Documentation**: See `docs/features/[N]-[feature-name]/testing.md`
/// </remarks>
[TestClass]
public class [Component]EdgeCasesTests
{
    [Test]
    public void MethodName_NullInput_ThrowsException()
    {
        // Arrange
        var component = new [Component]();
        
        // Act & Assert
        Assert.Throws<[ExceptionType]>(() => component.Method(null));
    }
    
    [Test]
    public void MethodName_EmptyInput_ReturnsDefault()
    {
        // Arrange
        var component = new [Component]();
        
        // Act
        var result = component.Method("");
        
        // Assert
        Assert.That(result, Is.EqualTo([default_value]));
    }
    
    [Test]
    public void MethodName_MaxValue_HandlesCorrectly()
    {
        // Arrange
        var component = new [Component]();
        var maxValue = int.MaxValue;
        
        // Act
        var result = component.Method(maxValue);
        
        // Assert
        Assert.That(result, Is.Not.Null);
    }
}
```

---

## Complete Example (C# / NUnit)

```csharp
using NUnit.Framework;
using System;
using ProjectName.Services.Authentication;

namespace ProjectName.Tests.Services.Authentication
{
    /// <remarks>
    /// **Feature**: 1: User Authentication
    /// **Sub-Feature**: 1.2: Authentication Service
    /// **Documentation**: See `docs/features/1-user-authentication/testing.md`
    /// </remarks>
    [TestFixture]
    public class AuthenticationServiceEdgeCasesTests
    {
        private AuthenticationService _service;
        
        [SetUp]
        public void SetUp()
        {
            _service = new AuthenticationService();
        }
        
        [Test]
        public void Authenticate_NullUsername_ThrowsArgumentNullException()
        {
            // Arrange
            string username = null;
            var password = "password123";
            
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                _service.Authenticate(username, password));
        }
        
        [Test]
        public void Authenticate_NullPassword_ThrowsArgumentNullException()
        {
            // Arrange
            var username = "testuser";
            string password = null;
            
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                _service.Authenticate(username, password));
        }
        
        [Test]
        public void Authenticate_EmptyUsername_ReturnsFalse()
        {
            // Arrange
            var username = "";
            var password = "password123";
            
            // Act
            var result = _service.Authenticate(username, password);
            
            // Assert
            Assert.That(result, Is.False);
        }
        
        [Test]
        public void Authenticate_EmptyPassword_ReturnsFalse()
        {
            // Arrange
            var username = "testuser";
            var password = "";
            
            // Act
            var result = _service.Authenticate(username, password);
            
            // Assert
            Assert.That(result, Is.False);
        }
        
        [Test]
        public void Authenticate_WhitespaceUsername_ReturnsFalse()
        {
            // Arrange
            var username = "   ";
            var password = "password123";
            
            // Act
            var result = _service.Authenticate(username, password);
            
            // Assert
            Assert.That(result, Is.False);
        }
        
        [Test]
        public void GetUser_NonExistentId_ReturnsNull()
        {
            // Arrange
            var nonExistentId = "nonexistent123";
            
            // Act
            var user = _service.GetUser(nonExistentId);
            
            // Assert
            Assert.That(user, Is.Null);
        }
        
        [Test]
        public void GetUser_EmptyId_ThrowsArgumentException()
        {
            // Arrange
            var emptyId = "";
            
            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                _service.GetUser(emptyId));
        }
    }
}
```

---

## Edge Case Categories

### 1. Null Inputs
- Test null parameters
- Verify exception thrown or handled

### 2. Empty Inputs
- Test empty strings, arrays, collections
- Verify default behavior

### 3. Boundary Values
- Test min/max values
- Test edge of valid range

### 4. Invalid Inputs
- Test invalid data types
- Test out-of-range values
- Test malformed data

### 5. Special Cases
- Test whitespace-only strings
- Test very long strings
- Test special characters

---

## Location

**Pattern**: `Tests/[Feature]/[Component]EdgeCasesTests.[ext]`

**Examples**:
- `Tests/Authentication/AuthenticationServiceEdgeCasesTests.cs`
- `Tests/Payment/PaymentProcessorEdgeCasesTests.js`

---

## Naming Convention

**Pattern**: `MethodName_EdgeCase_ExpectedResult`

**Examples**:
- `Authenticate_NullUsername_ThrowsArgumentNullException`
- `GetUser_EmptyId_ThrowsArgumentException`
- `ProcessPayment_NegativeAmount_ThrowsArgumentOutOfRangeException`

---

**Última Actualización**: 2025-01-XX

