# Sub-Feature 2.7: Integration

> AI providers, Player input, Full battles - Complete battle integration.

**Sub-Feature Number**: 2.7  
**Parent Feature**: Feature 2: Combat System  
**See**: [`../../features_master_list.md`](../../features_master_list.md) for feature numbering standards.

## Overview

Integration provides the interfaces and implementations for connecting the battle engine with external systems:

-   **IActionProvider**: Interface for action selection (AI or player)
-   **PlayerInputProvider**: Human player input implementation
-   **AI Providers** (6 implementations): RandomAI, AlwaysAttackAI, FixedMoveAI, NoActionAI, SmartAI, TeamBattleAI
-   **IBattleView**: Interface for visual presentation
-   **Full Battle Scenarios**: End-to-end battle tests

## Current Status

-   ✅ **Implemented**: IActionProvider interface and implementations
-   ✅ **Tested**: Full battle integration tests

## Documentation

| Document                                              | Purpose                 |
| ----------------------------------------------------- | ----------------------- |
| **[Architecture](../../architecture.md#integration)** | Technical specification |
| **[Use Cases](../../use_cases.md)**                   | Integration scenarios   |
| **[Roadmap](../../roadmap.md#phase-27-integration)**  | Implementation details  |
| **[Testing](../../testing.md)**                       | Testing strategy        |
| **[Code Location](../../code_location.md)**           | Where the code lives    |

## Related Sub-Features

-   **[2.6: Combat Engine](../2.6-combat-engine/)** - Engine uses action providers
-   **[Feature 4: Unity Integration](../../../4-unity-integration/)** - Unity will implement IActionProvider

**⚠️ Always use numbered feature paths**: `../../../[N]-[feature-name]/` instead of `../../../feature-name/`

## Quick Links

-   **Key Classes**: `IActionProvider`, `PlayerInputProvider`, `IBattleView`
-   **AI Implementations**: RandomAI, AlwaysAttackAI, FixedMoveAI, NoActionAI, SmartAI, TeamBattleAI (6 total)
-   **Status**: ✅ Complete (Phase 2.7)

---

**Last Updated**: 2025-01-XX
