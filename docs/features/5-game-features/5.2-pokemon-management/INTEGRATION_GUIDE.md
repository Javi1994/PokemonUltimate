# PokemonParty Integration Guide

> Cómo se integra `PokemonParty` con los slots y el battle engine.

**Feature**: 5: Game Features  
**Sub-Feature**: 5.2: Pokemon Management  
**See**: [`architecture.md`](architecture.md) para la especificación completa

---

## 📋 Tabla de Contenidos

1. [Arquitectura General](#arquitectura-general)
2. [Flujo de Inicialización](#flujo-de-inicialización)
3. [Relación Party ↔ Slots](#relación-party--slots)
4. [Switching de Pokemon](#switching-de-pokemon)
5. [Ejemplos de Uso](#ejemplos-de-uso)

---

## 🏗️ Arquitectura General

```
┌─────────────────────────────────────────────────────────────┐
│                    PokemonParty                              │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐      │
│  │ Pikachu  │ │Charmander│ │ Bulbasaur│ │ Squirtle │ ...  │
│  └──────────┘ └──────────┘ └──────────┘ └──────────┘      │
│       ↑                                                      │
│       │ Referencia (IReadOnlyList<PokemonInstance>)         │
└───────┼─────────────────────────────────────────────────────┘
        │
        │ BattleField.Initialize(party)
        │
        ▼
┌─────────────────────────────────────────────────────────────┐
│                    BattleSide                               │
│                                                              │
│  Party: IReadOnlyList<PokemonInstance> (referencia)        │
│                                                              │
│  Slots: List<BattleSlot>                                    │
│  ┌──────────┐ ┌──────────┐                                │
│  │ Slot[0]  │ │ Slot[1]  │  (para doubles/triples)        │
│  │ Pikachu  │ │ (vacío)   │                                │
│  └──────────┘ └──────────┘                                │
│                                                              │
│  GetAvailableSwitches() → Pokemon en party NO en slots     │
└─────────────────────────────────────────────────────────────┘
```

### Componentes Clave

1. **PokemonParty**: Contiene todos los Pokemon del equipo (máx 6)
2. **BattleSide**: Mantiene referencia al party + slots activos
3. **BattleSlot**: Representa un Pokemon activo en batalla
4. **BattleField**: Coordina ambos lados (player/enemy)

---

## 🔄 Flujo de Inicialización

### Paso 1: Crear Parties

```csharp
// Crear parties con PokemonParty
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
```

### Paso 2: Inicializar BattleField

```csharp
var field = new BattleField();
var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };

// PokemonParty implementa IReadOnlyList<PokemonInstance>
field.Initialize(rules, playerParty, enemyParty);
```

**Lo que hace `Initialize()`:**

1. **Crea BattleSides** según las reglas:

    ```csharp
    _playerSide = new BattleSide(rules.PlayerSlots, isPlayer: true);
    _enemySide = new BattleSide(rules.EnemySlots, isPlayer: false);
    ```

2. **Asigna el party a cada side**:

    ```csharp
    _playerSide.SetParty(playerParty);  // Guarda referencia al party
    _enemySide.SetParty(enemyParty);
    ```

3. **Coloca Pokemon iniciales en slots**:
    ```csharp
    PlaceInitialPokemon(_playerSide, playerParty);
    PlaceInitialPokemon(_enemySide, enemyParty);
    ```

### Paso 3: PlaceInitialPokemon

```csharp
private void PlaceInitialPokemon(BattleSide side, IReadOnlyList<PokemonInstance> party)
{
    // Solo Pokemon no desmayados
    var healthyPokemon = party.Where(p => !p.IsFainted).ToList();

    // Coloca hasta llenar los slots disponibles
    for (int i = 0; i < side.Slots.Count && i < healthyPokemon.Count; i++)
    {
        var pokemon = healthyPokemon[i];
        side.Slots[i].SetPokemon(pokemon);  // Pokemon entra al slot
    }
}
```

**Resultado:**

-   `playerParty[0]` (Pikachu) → `playerSide.Slots[0]`
-   `enemyParty[0]` (Squirtle) → `enemySide.Slots[0]`
-   Los demás Pokemon quedan en el party pero NO en slots

---

## 🔗 Relación Party ↔ Slots

### Conceptos Clave

1. **Party = Almacén completo**: Todos los Pokemon del equipo (hasta 6)
2. **Slots = Pokemon activos**: Solo los Pokemon que están en batalla (1-3 según formato)
3. **Referencia, no copia**: Los slots contienen referencias a los Pokemon del party

### Ejemplo Visual

```
Party (6 Pokemon):
┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐
│ Pikachu  │ │Charmander│ │ Bulbasaur│ │ Squirtle │ │  Eevee   │ │ Snorlax  │
└────┬─────┘ └──────────┘ └──────────┘ └──────────┘ └──────────┘ └──────────┘
     │
     │ Slot[0] (activo en batalla)
     ▼
┌──────────┐
│ Pikachu  │ ← Mismo objeto PokemonInstance
└──────────┘

GetAvailableSwitches() devuelve:
- Charmander ✅ (en party, no en slot, no desmayado)
- Bulbasaur ✅ (en party, no en slot, no desmayado)
- Squirtle ✅ (en party, no en slot, no desmayado)
- Eevee ✅ (en party, no en slot, no desmayado)
- Snorlax ✅ (en party, no en slot, no desmayado)
- Pikachu ❌ (ya está en slot activo)
```

### Método GetAvailableSwitches()

```csharp
public IEnumerable<PokemonInstance> GetAvailableSwitches()
{
    if (_party == null)
        return Enumerable.Empty<PokemonInstance>();

    // 1. Obtener Pokemon actualmente en slots activos
    var activePokemon = _slots
        .Where(s => !s.IsEmpty)
        .Select(s => s.Pokemon)
        .ToHashSet();

    // 2. Filtrar party: NO desmayados, NO en slots activos
    return _party.Where(p =>
        !p.IsFainted &&           // No desmayado
        !activePokemon.Contains(p) // No está en un slot activo
    );
}
```

---

## 🔄 Switching de Pokemon

### Flujo Completo

```
1. Usuario/AI decide cambiar
   ↓
2. GetAvailableSwitches() → Lista de Pokemon disponibles
   ↓
3. Seleccionar Pokemon objetivo (ej: Charmander)
   ↓
4. Crear SwitchAction(slot, charmander)
   ↓
5. ExecuteLogic() ejecuta el switch:
   - Slot actual: Pikachu (vuelve al party)
   - Slot nuevo: Charmander (sale del party, entra al slot)
   ↓
6. El party sigue conteniendo TODOS los Pokemon
   - Pikachu ✅ (en party, disponible para cambiar de nuevo)
   - Charmander ✅ (en party, ahora activo en slot)
   - Bulbasaur ✅ (en party, disponible)
```

### Código de SwitchAction

```csharp
public override IEnumerable<BattleAction> ExecuteLogic(BattleField field)
{
    // 1. Marcar Pokemon actual como "switching out"
    Slot.AddVolatileStatus(VolatileStatus.SwitchingOut);

    // 2. Obtener Pokemon actual (antes del switch)
    var oldPokemon = Slot.Pokemon;  // Pikachu

    // 3. Colocar nuevo Pokemon en el slot
    Slot.SetPokemon(NewPokemon);  // Charmander

    // 4. SetPokemon automáticamente:
    //    - Resetea battle state (stat stages, volatile status)
    //    - El oldPokemon sigue en el party (no se elimina)

    // 5. Procesar entry hazards (si hay)
    // 6. Trigger OnSwitchIn para abilities/items

    return allActions;
}
```

**Importante:**

-   `oldPokemon` (Pikachu) **NO se elimina del party**
-   `NewPokemon` (Charmander) **sigue en el party**
-   El party es la fuente de verdad, los slots solo referencian

---

## 💡 Ejemplos de Uso

### Ejemplo 1: Batalla Simple con Party

```csharp
// Crear parties
var playerParty = new PokemonParty
{
    PokemonFactory.Create(PokemonCatalog.Pikachu, 50),
    PokemonFactory.Create(PokemonCatalog.Charmander, 50)
};

var enemyParty = new PokemonParty
{
    PokemonFactory.Create(PokemonCatalog.Squirtle, 50)
};

// Inicializar batalla
var engine = CombatEngineTestHelper.CreateCombatEngine();
var rules = new BattleRules { PlayerSlots = 1, EnemySlots = 1 };
var playerAI = new SmartAI();
var enemyAI = new AlwaysAttackAI();
var view = new NullBattleView();

engine.Initialize(rules, playerParty, enemyParty, playerAI, enemyAI, view);

// Estado inicial:
// - playerParty[0] (Pikachu) está en playerSide.Slots[0]
// - playerParty[1] (Charmander) está en party pero NO en slot
// - enemyParty[0] (Squirtle) está en enemySide.Slots[0]
```

### Ejemplo 2: Switching Manual

```csharp
var slot = engine.Field.PlayerSide.GetSlot(0);
var availableSwitches = engine.Field.PlayerSide.GetAvailableSwitches().ToList();
// availableSwitches = [Charmander]

// Cambiar a Charmander
var switchAction = new SwitchAction(slot, availableSwitches[0]);
switchAction.ExecuteLogic(engine.Field);

// Ahora:
// - slot.Pokemon = Charmander
// - playerParty[0] = Pikachu (sigue en party)
// - playerParty[1] = Charmander (ahora activo en slot)
```

### Ejemplo 3: AI con Switching Inteligente

```csharp
// SmartAI puede decidir cambiar cuando HP es bajo
var playerAI = new SmartAI(
    switchThreshold: 0.3,  // Cambia cuando HP < 30%
    switchChance: 0.7       // 70% probabilidad de cambiar
);

// Durante la batalla:
// 1. Pikachu recibe daño → HP = 25% (< 30%)
// 2. SmartAI decide cambiar (70% chance)
// 3. GetAvailableSwitches() → [Charmander]
// 4. Crea SwitchAction automáticamente
// 5. Charmander entra al campo
```

### Ejemplo 4: Verificar Estado del Party

```csharp
// Verificar cuántos Pokemon activos hay
var activeCount = playerParty.GetActivePokemon().Count();

// Verificar si el party es válido para batalla
if (!playerParty.IsValidForBattle())
{
    // Todos los Pokemon están desmayados
    // La batalla debería terminar
}

// Obtener primer Pokemon activo (para auto-switch)
var nextPokemon = playerParty.GetFirstActivePokemon();
if (nextPokemon != null)
{
    // Cambiar automáticamente cuando el actual se desmaya
    slot.SetPokemon(nextPokemon);
}
```

---

## 🔍 Puntos Importantes

### 1. Party es la Fuente de Verdad

-   El party contiene **todos** los Pokemon del equipo
-   Los slots solo contienen **referencias** a Pokemon del party
-   Un Pokemon puede estar en el party pero no en un slot (bench)

### 2. Switching NO Elimina del Party

-   Cuando cambias de Pokemon, el anterior **sigue en el party**
-   Puedes cambiar de vuelta al Pokemon original más tarde
-   El party mantiene el orden original

### 3. GetAvailableSwitches() es Inteligente

-   Excluye Pokemon desmayados automáticamente
-   Excluye Pokemon ya activos en slots
-   Devuelve solo Pokemon válidos para cambiar

### 4. Compatibilidad Total

-   `PokemonParty` implementa `IReadOnlyList<PokemonInstance>`
-   Funciona directamente con `BattleField.Initialize()`
-   No requiere cambios en el código existente

---

## 📚 Referencias

-   **[Architecture](architecture.md)** - Especificación técnica completa
-   **[BattleField.Initialize()](../../2-combat-system/2.1-battle-foundation/architecture.md)** - Inicialización del campo de batalla
-   **[SwitchAction](../../2-combat-system/2.5-combat-actions/architecture.md)** - Sistema de switching
-   **[BattleSide.GetAvailableSwitches()](../../2-combat-system/2.1-battle-foundation/architecture.md)** - Lógica de switches disponibles

---

**Última Actualización**: 2025-01-XX
