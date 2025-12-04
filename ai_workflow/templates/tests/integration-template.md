# Integration Test Template

> **Template para tests de integración con ejemplo completo**  
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
public class [System1]_[System2]_IntegrationTests
{
    [Test]
    public void [System1]_[System2]_[ExpectedBehavior]()
    {
        // Arrange
        var system1 = new [System1]();
        var system2 = new [System2]();
        
        // Act
        var result = system1.Process(system2);
        
        // Assert
        Assert.That(result, Is.True);
    }
}
```

---

## Complete Example (C# / NUnit)

```csharp
using NUnit.Framework;
using ProjectName.Services.Authentication;
using ProjectName.Repositories.UserRepository;
using ProjectName.Services.Session;

namespace ProjectName.Tests.Authentication.Integration
{
    /// <remarks>
    /// **Feature**: 1: User Authentication
    /// **Sub-Feature**: 1.3: Authentication Integration
    /// **Documentation**: See `docs/features/1-user-authentication/testing.md`
    /// </remarks>
    [TestFixture]
    public class AuthenticationService_UserRepository_IntegrationTests
    {
        private AuthenticationService _authService;
        private UserRepository _userRepository;
        private SessionService _sessionService;
        
        [SetUp]
        public void SetUp()
        {
            _userRepository = new UserRepository();
            _sessionService = new SessionService();
            _authService = new AuthenticationService(_userRepository, _sessionService);
        }
        
        [Test]
        public void AuthenticationService_UserRepository_AuthenticatesAndCreatesSession()
        {
            // Arrange
            var username = "testuser";
            var password = "password123";
            
            // Act
            var authenticated = _authService.Authenticate(username, password);
            var session = _sessionService.CreateSession(username);
            
            // Assert
            Assert.That(authenticated, Is.True);
            Assert.That(session, Is.Not.Null);
            Assert.That(session.Username, Is.EqualTo(username));
            Assert.That(session.IsValid, Is.True);
        }
        
        [Test]
        public void AuthenticationService_SessionService_InvalidatesSessionOnLogout()
        {
            // Arrange
            var username = "testuser";
            var password = "password123";
            _authService.Authenticate(username, password);
            var session = _sessionService.CreateSession(username);
            
            // Act
            _authService.Logout(session.Token);
            
            // Assert
            Assert.That(_sessionService.IsValid(session.Token), Is.False);
        }
        
        [Test]
        public void UserRepository_SessionService_UserDataPersistsAcrossSessions()
        {
            // Arrange
            var user = _userRepository.CreateUser("testuser", "password123");
            var session1 = _sessionService.CreateSession(user.Username);
            var session2 = _sessionService.CreateSession(user.Username);
            
            // Act
            var userFromSession1 = _userRepository.GetUser(session1.Username);
            var userFromSession2 = _userRepository.GetUser(session2.Username);
            
            // Assert
            Assert.That(userFromSession1.Id, Is.EqualTo(userFromSession2.Id));
            Assert.That(userFromSession1.Username, Is.EqualTo(userFromSession2.Username));
        }
    }
}
```

---

## Integration Test Categories

### 1. Component Interactions
- Test how multiple components work together
- Verify data flows correctly between components

### 2. System Flows
- Test complete workflows end-to-end
- Verify all steps execute correctly

### 3. Cascading Effects
- Test side effects of operations
- Verify state changes propagate correctly

### 4. External Dependencies
- Test integration with external services
- Verify error handling for external failures

---

## Location

**Pattern**: `Tests/[Feature]/Integration/[Category]/[TestFile].[ext]`

**Examples**:
- `Tests/Authentication/Integration/AuthenticationService_UserRepository_IntegrationTests.cs`
- `Tests/Payment/Integration/PaymentProcessor_NotificationService_IntegrationTests.cs`

---

## Naming Convention

**Pattern**: `[System1]_[System2]_[ExpectedBehavior]`

**Examples**:
- `AuthenticationService_UserRepository_AuthenticatesAndCreatesSession`
- `PaymentProcessor_NotificationService_SendsConfirmationEmail`
- `OrderService_InventoryService_ReservesItems`

---

## Setup Considerations

1. **Real Dependencies**: Use real implementations or test doubles
2. **Test Data**: Set up test data before each test
3. **Cleanup**: Clean up test data after each test
4. **Isolation**: Each test should be independent

---

**Última Actualización**: 2025-01-XX

