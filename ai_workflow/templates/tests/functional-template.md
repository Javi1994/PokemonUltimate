# Functional Test Template

> **Template para tests funcionales con ejemplo completo**  
> **Específico para**: C# .NET con NUnit (SDKs para Unity)

## Template Structure (C# / NUnit)

```csharp
using NUnit.Framework;
using ProjectName.[Namespace];

namespace ProjectName.Tests.[Feature].[Component]
{
    /// <remarks>
    /// **Feature**: [N]: [Feature Name]
    /// **Sub-Feature**: [N.M]: [Sub-Feature Name] (if applicable)
    /// **Documentation**: See `docs/features/[N]-[feature-name]/testing.md`
    /// **Note**: Tests are pure C# - no Unity dependencies
    /// </remarks>
    [TestFixture]
    public class [Component]Tests
    {
        private [Component] _component;
        
        [SetUp]
        public void SetUp()
        {
            // Arrange common setup
            _component = new [Component]();
        }
        
        [Test]
        public void MethodName_NormalScenario_ReturnsExpectedResult()
        {
            // Arrange
            var input = [concrete_example_value];
            var expected = [concrete_expected_value];
            
            // Act
            var result = _component.Method(input);
            
            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
        
        [Test]
        public void MethodName_AnotherScenario_ReturnsAnotherResult()
        {
            // Arrange
            // ... concrete example
            
            // Act
            var result = _component.AnotherMethod();
            
            // Assert
            Assert.That(result, Is.Not.Null);
        }
    }
}
```

---

## Complete Example (C# / NUnit) ⭐ **Primary Example**

```csharp
using NUnit.Framework;
using ProjectName.Services.Authentication;

namespace ProjectName.Tests.Services.Authentication
{
    /// <remarks>
    /// **Feature**: 1: User Authentication
    /// **Sub-Feature**: 1.2: Authentication Service
    /// **Documentation**: See `docs/features/1-user-authentication/testing.md`
    /// </remarks>
    [TestFixture]
    public class AuthenticationServiceTests
    {
        private AuthenticationService _service;
        
        [SetUp]
        public void SetUp()
        {
            _service = new AuthenticationService();
        }
        
        [Test]
        public void Authenticate_ValidCredentials_ReturnsTrue()
        {
            // Arrange
            var username = "testuser";
            var password = "password123";
            
            // Act
            var result = _service.Authenticate(username, password);
            
            // Assert
            Assert.That(result, Is.True);
        }
        
        [Test]
        public void Authenticate_InvalidCredentials_ReturnsFalse()
        {
            // Arrange
            var username = "testuser";
            var password = "wrongpassword";
            
            // Act
            var result = _service.Authenticate(username, password);
            
            // Assert
            Assert.That(result, Is.False);
        }
        
        [Test]
        public void GetUser_ExistingUserId_ReturnsUser()
        {
            // Arrange
            var userId = "user123";
            
            // Act
            var user = _service.GetUser(userId);
            
            // Assert
            Assert.That(user, Is.Not.Null);
            Assert.That(user.Id, Is.EqualTo(userId));
        }
    }
}
```

---

## Complete Example (JavaScript / Jest)

```javascript
const AuthenticationService = require('../services/AuthenticationService');

/**
 * Feature: 1: User Authentication
 * Sub-Feature: 1.2: Authentication Service
 * Documentation: See `docs/features/1-user-authentication/testing.md`
 */
describe('AuthenticationService', () => {
    let service;
    
    beforeEach(() => {
        service = new AuthenticationService();
    });
    
    test('Authenticate_ValidCredentials_ReturnsTrue', () => {
        // Arrange
        const username = 'testuser';
        const password = 'password123';
        
        // Act
        const result = service.authenticate(username, password);
        
        // Assert
        expect(result).toBe(true);
    });
    
    test('Authenticate_InvalidCredentials_ReturnsFalse', () => {
        // Arrange
        const username = 'testuser';
        const password = 'wrongpassword';
        
        // Act
        const result = service.authenticate(username, password);
        
        // Assert
        expect(result).toBe(false);
    });
});
```

---

## Complete Example (Python / pytest)

```python
import pytest
from services.authentication import AuthenticationService

"""
Feature: 1: User Authentication
Sub-Feature: 1.2: Authentication Service
Documentation: See `docs/features/1-user-authentication/testing.md`
"""

class TestAuthenticationService:
    @pytest.fixture
    def service(self):
        return AuthenticationService()
    
    def test_authenticate_valid_credentials_returns_true(self, service):
        # Arrange
        username = "testuser"
        password = "password123"
        
        # Act
        result = service.authenticate(username, password)
        
        # Assert
        assert result is True
    
    def test_authenticate_invalid_credentials_returns_false(self, service):
        # Arrange
        username = "testuser"
        password = "wrongpassword"
        
        # Act
        result = service.authenticate(username, password)
        
        # Assert
        assert result is False
```

---

## Naming Convention

**Pattern**: `MethodName_Scenario_ExpectedResult`

**Examples**:
- `Authenticate_ValidCredentials_ReturnsTrue`
- `GetUser_ExistingId_ReturnsUser`
- `CreateSession_ValidToken_ReturnsSession`
- `ValidateToken_ExpiredToken_ReturnsFalse`

---

## Location

**Pattern**: `Tests/[Feature]/[Component]Tests.[ext]`

**Examples**:
- `Tests/Authentication/AuthenticationServiceTests.cs`
- `Tests/Authentication/UserRepositoryTests.cs`
- `Tests/Payment/PaymentProcessorTests.js`

---

## Required Sections

1. **Arrange**: Setup test data and dependencies
2. **Act**: Execute the method being tested
3. **Assert**: Verify the result

---

**Última Actualización**: 2025-01-XX

