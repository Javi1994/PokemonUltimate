# Ejemplo Práctico: PokemonParty + Battle Engine

> Ejemplo completo mostrando cómo usar PokemonParty con el battle engine.

## Escenario: Batalla con 3 Pokemon vs 2 Pokemon

### Paso 1: Crear los Parties

```csharp
// Player tiene 3 Pokemon
var playerParty = new PokemonParty
{
    PokemonFactory.Create(PokemonCatalog.Pikachu, 50),      // Index 0
    PokemonFactory.Create(PokemonCatalog.Charmander, 50),   // Index 1
    PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)    // Index 2
};

// Enemy tiene 2 Pokemon
var enemyParty = new PokemonParty
{
    PokemonFactory.Create(PokemonCatalog.Squirtle, 50),     // Index 0
    PokemonFactory.Create(PokemonCatalog.Eevee, 50)         // Index 1
};
```

### Paso 2: Inicializar BattleField

```csharp
var field = new BattleField();
var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };

field.Initialize(rules, playerParty, enemyParty);
```

**Estado después de Initialize():**

```
Player Side:
├── Party: [Pikachu, Charmander, Bulbasaur]  ← Referencia completa
└── Slots:
    └── Slot[0]: Pikachu  ← Pokemon activo (referencia a party[0])

Enemy Side:
├── Party: [Squirtle, Eevee]  ← Referencia completa
└── Slots:
    └── Slot[0]: Squirtle  ← Pokemon activo (referencia a party[0])
```

### Paso 3: Verificar Switches Disponibles

```csharp
// ¿Qué Pokemon puede cambiar el player?
var availableSwitches = field.PlayerSide.GetAvailableSwitches().ToList();
// Resultado: [Charmander, Bulbasaur]
// Pikachu NO está disponible porque ya está en Slot[0]
```

### Paso 4: Cambiar de Pokemon

```csharp
var playerSlot = field.PlayerSide.GetSlot(0);
var newPokemon = availableSwitches[0]; // Charmander

var switchAction = new SwitchAction(playerSlot, newPokemon);
switchAction.ExecuteLogic(field);
```

**Estado después del Switch:**

```
Player Side:
├── Party: [Pikachu, Charmander, Bulbasaur]  ← NO cambia, todos siguen aquí
└── Slots:
    └── Slot[0]: Charmander  ← Ahora Charmander está activo

// Pikachu sigue en party[0], solo salió del slot
// Charmander sigue en party[1], ahora está en el slot
```

### Paso 5: Verificar Switches Disponibles Nuevamente

```csharp
var availableSwitchesAfter = field.PlayerSide.GetAvailableSwitches().ToList();
// Resultado: [Pikachu, Bulbasaur]
// Ahora Charmander NO está disponible porque está en Slot[0]
// Pikachu SÍ está disponible porque salió del slot
```

---

## Integración con CombatEngine

### Ejemplo Completo con CombatEngine

```csharp
// 1. Crear parties
var playerParty = new PokemonParty
{
    PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
    PokemonFactory.Create(PokemonCatalog.Charmander, 50),
    PokemonFactory.Create(PokemonCatalog.Bulbasaur, 50)
};

var enemyParty = new PokemonParty
{
    PokemonFactory.Create(PokemonCatalog.Squirtle, 50),
    PokemonFactory.Create(PokemonCatalog.Eevee, 50)
};

// 2. Crear engine y AI
var engine = CombatEngineTestHelper.CreateCombatEngine();
var playerAI = new SmartAI(switchThreshold: 0.3, switchChance: 0.8);
var enemyAI = new AlwaysAttackAI();
var view = new NullBattleView();

// 3. Inicializar batalla
var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
engine.Initialize(rules, playerParty, enemyParty, playerAI, enemyAI, view);

// 4. Ejecutar batalla
// Durante la batalla:
// - Si Pikachu tiene HP < 30%, SmartAI puede decidir cambiar
// - GetAvailableSwitches() devuelve [Charmander, Bulbasaur]
// - SmartAI selecciona uno aleatoriamente
// - Se ejecuta SwitchAction automáticamente
// - El Pokemon cambia y la batalla continúa

var result = await engine.RunBattle();
```

---

## Flujo de Datos

```
┌─────────────────────────────────────────────────────────────┐
│                    PokemonParty                             │
│  [Pikachu, Charmander, Bulbasaur, Squirtle, Eevee, ...]   │
│  (Almacén completo del equipo)                              │
└───────────────────────┬─────────────────────────────────────┘
                        │
                        │ BattleField.Initialize(party)
                        │
                        ▼
┌─────────────────────────────────────────────────────────────┐
│                    BattleSide                               │
│                                                              │
│  Party: IReadOnlyList<PokemonInstance>                      │
│  └── Referencia al PokemonParty completo                    │
│                                                              │
│  Slots: List<BattleSlot>                                    │
│  ├── Slot[0]: PokemonInstance (referencia a party[0])       │
│  ├── Slot[1]: PokemonInstance (referencia a party[1])       │
│  └── ... (para doubles/triples)                             │
│                                                              │
│  GetAvailableSwitches():                                    │
│  └── Filtra party: NO en slots, NO desmayados              │
└─────────────────────────────────────────────────────────────┘
                        │
                        │ CombatEngine usa BattleField
                        │
                        ▼
┌─────────────────────────────────────────────────────────────┐
│                    CombatEngine                              │
│                                                              │
│  - Ejecuta turnos                                           │
│  - Procesa acciones (UseMoveAction, SwitchAction)            │
│  - Verifica estado de batalla                               │
│  - Usa IActionProvider para obtener acciones                │
│                                                              │
│  IActionProvider puede:                                     │
│  - Usar moves (UseMoveAction)                               │
│  - Cambiar Pokemon (SwitchAction) usando                    │
│    side.GetAvailableSwitches()                              │
└─────────────────────────────────────────────────────────────┘
```

---

## Puntos Clave de la Integración

### 1. PokemonParty es Transparente

```csharp
// PokemonParty implementa IReadOnlyList<PokemonInstance>
// Por lo tanto, funciona directamente con BattleField.Initialize()

IReadOnlyList<PokemonInstance> party = new PokemonParty { ... };
field.Initialize(rules, party, enemyParty);  // ✅ Funciona perfectamente
```

### 2. El Party NO se Modifica Durante la Batalla

```csharp
// Antes de la batalla
var party = new PokemonParty { pikachu, charmander };
Assert.That(party.Count, Is.EqualTo(2));

// Después de cambiar de Pokemon
// ... switching ocurre ...

// El party sigue igual
Assert.That(party.Count, Is.EqualTo(2));  // ✅ Sigue siendo 2
Assert.That(party.Contains(pikachu), Is.True);  // ✅ Pikachu sigue ahí
Assert.That(party.Contains(charmander), Is.True);  // ✅ Charmander sigue ahí
```

### 3. Switching Solo Cambia Referencias en Slots

```csharp
// Antes del switch
slot.Pokemon == party[0]  // Pikachu

// Después del switch
slot.SetPokemon(party[1]);  // Charmander
slot.Pokemon == party[1]    // Ahora Charmander

// Pero el party NO cambia
party[0] == pikachu      // ✅ Sigue siendo Pikachu
party[1] == charmander   // ✅ Sigue siendo Charmander
```

### 4. GetAvailableSwitches() Es Dinámico

```csharp
// Estado inicial: Pikachu en Slot[0]
var switches1 = side.GetAvailableSwitches();
// → [Charmander, Bulbasaur]

// Después de cambiar a Charmander
var switches2 = side.GetAvailableSwitches();
// → [Pikachu, Bulbasaur]  ← Pikachu ahora está disponible de nuevo!
```

---

## Casos de Uso Comunes

### Caso 1: Auto-Switch cuando Pokemon se Desmaya

```csharp
// Si el Pokemon activo se desmaya, cambiar automáticamente
var activeSlot = field.PlayerSide.GetSlot(0);
if (activeSlot.Pokemon.IsFainted)
{
    var nextPokemon = playerParty.GetFirstActivePokemon();
    if (nextPokemon != null)
    {
        var switchAction = new SwitchAction(activeSlot, nextPokemon);
        switchAction.ExecuteLogic(field);
    }
}
```

### Caso 2: Verificar Si Hay Pokemon Disponibles

```csharp
// Antes de intentar cambiar, verificar disponibilidad
var available = field.PlayerSide.GetAvailableSwitches().ToList();
if (available.Count == 0)
{
    // No hay Pokemon disponibles para cambiar
    // Todos están desmayados o ya están en slots
    return; // No se puede cambiar
}
```

### Caso 3: Batalla con Múltiples Slots (Doubles)

```csharp
var rules = new BattleRules { PlayerSlots = 2, EnemySlots = 2 };

field.Initialize(rules, playerParty, enemyParty);

// Estado:
// - playerSide.Slots[0]: party[0] (Pikachu)
// - playerSide.Slots[1]: party[1] (Charmander)
// - party[2] (Bulbasaur) está en party pero NO en slot

// GetAvailableSwitches() devuelve solo Bulbasaur
var available = field.PlayerSide.GetAvailableSwitches();
// → [Bulbasaur]  (Pikachu y Charmander están en slots activos)
```

---

## Resumen

1. **PokemonParty** contiene todos los Pokemon del equipo (máx 6)
2. **BattleSide** mantiene una referencia al party completo
3. **BattleSlot** contiene referencias a Pokemon del party que están activos
4. **GetAvailableSwitches()** filtra el party para encontrar Pokemon disponibles
5. **SwitchAction** cambia qué Pokemon está en el slot, pero NO modifica el party
6. El party es la fuente de verdad; los slots solo referencian

**La integración es completamente transparente** - `PokemonParty` funciona exactamente igual que `List<PokemonInstance>` porque implementa `IReadOnlyList<PokemonInstance>`.
