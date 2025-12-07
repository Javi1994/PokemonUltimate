# Feature 7: Game Implementation - Use Cases

> All game scenarios and player interactions.

**Feature**: 7: Game Implementation  
**See**: [`README.md`](README.md) for feature overview.

## Overview

This document describes all use cases and scenarios for the text-based demo game. Use cases are organized by game system.

## Use Case Categories

1. **Game Loop** - Core game flow and state management
2. **Battle** - Battle scenarios and interactions
3. **Pokemon Management** - Party, PC, catching
4. **World** - World navigation and exploration
5. **Encounters** - Wild Pokemon, trainers, bosses
6. **Progression** - EXP, level ups, rewards
7. **UI** - Menu navigation and displays

---

## Game Loop Use Cases

### UC-GL-001: Start New Game

**Actor**: Player  
**Precondition**: Game is at main menu  
**Flow**:
1. Player selects "New Game"
2. System prompts for player name
3. Player enters name
4. System creates new player
5. System initializes starting Pokemon
6. System transitions to world map

**Postcondition**: Player is in world map with starting Pokemon

---

### UC-GL-002: Load Saved Game

**Actor**: Player  
**Precondition**: Game is at main menu, save file exists  
**Flow**:
1. Player selects "Load Game"
2. System displays save files
3. Player selects save file
4. System loads game state
5. System transitions to saved location

**Postcondition**: Game state restored, player at saved location

---

### UC-GL-003: Save Game

**Actor**: Player  
**Precondition**: Player is in game (not in battle)  
**Flow**:
1. Player opens menu
2. Player selects "Save Game"
3. System saves current game state
4. System confirms save

**Postcondition**: Game state saved to file

---

### UC-GL-004: Exit Game

**Actor**: Player  
**Precondition**: Player is in game  
**Flow**:
1. Player opens menu
2. Player selects "Exit Game"
3. System prompts for confirmation
4. Player confirms
5. System saves (if auto-save enabled)
6. System exits

**Postcondition**: Game closed

---

## Battle Use Cases

### UC-BT-001: Start Wild Encounter

**Actor**: Player, System  
**Precondition**: Player is in world map  
**Flow**:
1. Player moves in world
2. System checks encounter chance
3. System triggers wild encounter
4. System displays encounter message
5. System transitions to battle state
6. System initializes battle with wild Pokemon

**Postcondition**: Battle started with wild Pokemon

---

### UC-BT-002: Start Trainer Battle

**Actor**: Player, System  
**Precondition**: Player is in world map, trainer present  
**Flow**:
1. Player approaches trainer
2. System triggers trainer encounter
3. System displays trainer dialogue
4. System transitions to battle state
5. System initializes battle with trainer's Pokemon

**Postcondition**: Battle started with trainer

---

### UC-BL-003: Player Selects Move

**Actor**: Player  
**Precondition**: Player is in battle, player's turn  
**Flow**:
1. System displays available moves
2. Player selects move
3. System validates move (PP, status)
4. System queues move action
5. System processes turn

**Postcondition**: Move queued and processed

---

### UC-BT-004: Player Switches Pokemon

**Actor**: Player  
**Precondition**: Player is in battle, can switch  
**Flow**:
1. Player selects "Switch Pokemon"
2. System displays party
3. Player selects Pokemon
4. System validates selection
5. System switches Pokemon
6. System processes switch

**Postcondition**: Pokemon switched in battle

---

### UC-BT-005: Battle Victory

**Actor**: System  
**Precondition**: Battle in progress, opponent faints  
**Flow**:
1. System detects opponent fainted
2. System checks for remaining opponents
3. If no remaining opponents:
   - System displays victory message
   - System calculates rewards
   - System distributes EXP
   - System processes level ups
   - System transitions to world map
4. If remaining opponents:
   - System sends out next Pokemon

**Postcondition**: Battle won, rewards distributed, return to world

---

### UC-BT-006: Battle Defeat

**Actor**: System  
**Precondition**: Battle in progress, player's Pokemon all faint  
**Flow**:
1. System detects all player Pokemon fainted
2. System displays defeat message
3. System handles defeat (game over or return to last checkpoint)
4. System transitions to appropriate state

**Postcondition**: Battle lost, appropriate state transition

---

## Pokemon Management Use Cases

### UC-PM-001: View Party

**Actor**: Player  
**Precondition**: Player is in game  
**Flow**:
1. Player opens menu
2. Player selects "Party"
3. System displays party Pokemon
4. System shows Pokemon details (name, level, HP, status)

**Postcondition**: Party displayed

---

### UC-PM-002: Switch Party Order

**Actor**: Player  
**Precondition**: Player is viewing party  
**Flow**:
1. Player selects "Reorder Party"
2. System displays party with selection
3. Player selects Pokemon to move
4. Player selects new position
5. System reorders party

**Postcondition**: Party order changed

---

### UC-PM-003: Deposit Pokemon to PC

**Actor**: Player  
**Precondition**: Player is viewing party, party has Pokemon  
**Flow**:
1. Player selects "PC"
2. System displays PC boxes
3. Player selects Pokemon from party
4. Player selects PC box
5. System deposits Pokemon to PC

**Postcondition**: Pokemon moved to PC

---

### UC-PM-004: Withdraw Pokemon from PC

**Actor**: Player  
**Precondition**: Player is viewing PC, party has space  
**Flow**:
1. Player selects "PC"
2. System displays PC boxes
3. Player selects PC box
4. Player selects Pokemon
5. System withdraws Pokemon to party

**Postcondition**: Pokemon moved to party

---

### UC-PM-005: Catch Wild Pokemon

**Actor**: Player  
**Precondition**: Player is in battle with wild Pokemon  
**Flow**:
1. Player selects "Catch Pokemon"
2. System checks if player has Poke Ball
3. If has Poke Ball:
   - System uses Poke Ball
   - System calculates catch rate
   - System determines catch success
   - If successful: Pokemon added to party or PC
   - If failed: Pokemon breaks free
4. If no Poke Ball: System displays error

**Postcondition**: Pokemon caught (if successful) or battle continues

---

## World Use Cases

### UC-WL-001: Navigate World Map

**Actor**: Player  
**Precondition**: Player is in world map  
**Flow**:
1. System displays current location
2. System displays available directions
3. Player selects direction
4. System validates movement
5. System moves player to new location
6. System updates display

**Postcondition**: Player moved to new location

---

### UC-WL-002: View Location Information

**Actor**: Player  
**Precondition**: Player is in world map  
**Flow**:
1. Player selects "Location Info"
2. System displays location name
3. System displays location description
4. System displays available encounters

**Postcondition**: Location information displayed

---

## Encounter Use Cases

### UC-EN-001: Trigger Wild Encounter

**Actor**: System  
**Precondition**: Player is moving in world  
**Flow**:
1. System checks encounter rate for location
2. System rolls encounter chance
3. If encounter triggered:
   - System selects Pokemon from encounter table
   - System determines Pokemon level
   - System creates wild encounter
   - System transitions to battle

**Postcondition**: Wild encounter triggered

---

### UC-EN-002: Trigger Trainer Encounter

**Actor**: System  
**Precondition**: Player is in location with trainer  
**Flow**:
1. System detects trainer in location
2. System checks if trainer already defeated
3. If not defeated:
   - System displays trainer dialogue
   - System initializes trainer battle
   - System transitions to battle

**Postcondition**: Trainer encounter triggered

---

### UC-EN-003: Trigger Boss Encounter

**Actor**: System  
**Precondition**: Player meets boss conditions  
**Flow**:
1. System detects boss conditions met
2. System displays boss introduction
3. System initializes boss battle
4. System transitions to battle

**Postcondition**: Boss encounter triggered

---

## Progression Use Cases

### UC-PR-001: Gain EXP After Battle

**Actor**: System  
**Precondition**: Battle won  
**Flow**:
1. System calculates EXP for each Pokemon
2. System distributes EXP to participating Pokemon
3. System checks for level ups
4. System processes level ups if any
5. System displays EXP gained

**Postcondition**: EXP distributed, level ups processed

---

### UC-PR-002: Level Up Pokemon

**Actor**: System  
**Precondition**: Pokemon gained enough EXP  
**Flow**:
1. System detects level up
2. System increases Pokemon level
3. System recalculates stats
4. System checks for move learning
5. System checks for evolution
6. System displays level up message

**Postcondition**: Pokemon leveled up, stats updated

---

### UC-PR-003: Learn New Move on Level Up

**Actor**: System  
**Precondition**: Pokemon leveled up, learns move at new level  
**Flow**:
1. System detects move learned at level
2. System displays move learned message
3. System prompts to learn move
4. If party has space: System adds move
5. If party full: System prompts to forget move
6. System processes move learning

**Postcondition**: Move learned or forgotten

---

### UC-PR-004: Evolve Pokemon on Level Up

**Actor**: System  
**Precondition**: Pokemon leveled up, evolution conditions met  
**Flow**:
1. System detects evolution conditions met
2. System displays evolution message
3. System prompts for evolution
4. If player confirms: System evolves Pokemon
5. System updates Pokemon species
6. System recalculates stats

**Postcondition**: Pokemon evolved

---

### UC-PR-005: Receive Post-Battle Rewards

**Actor**: System  
**Precondition**: Battle won  
**Flow**:
1. System calculates money reward
2. System determines item rewards (if any)
3. System adds money to player
4. System adds items to inventory
5. System displays rewards

**Postcondition**: Rewards distributed

---

## UI Use Cases

### UC-UI-001: Navigate Menu

**Actor**: Player  
**Precondition**: Menu is displayed  
**Flow**:
1. System displays menu options
2. Player navigates with arrow keys
3. System highlights selected option
4. Player confirms selection
5. System executes action

**Postcondition**: Menu action executed

---

### UC-UI-002: Display Battle UI

**Actor**: System  
**Precondition**: Battle in progress  
**Flow**:
1. System displays player Pokemon info
2. System displays opponent Pokemon info
3. System displays available moves
4. System displays battle options
5. System updates display on each turn

**Postcondition**: Battle UI displayed and updated

---

### UC-UI-003: Display Party UI

**Actor**: System  
**Precondition**: Player viewing party  
**Flow**:
1. System displays party Pokemon list
2. System displays selected Pokemon details
3. System displays party options
4. System updates display on selection

**Postcondition**: Party UI displayed

---

### UC-UI-004: Display Inventory UI

**Actor**: System  
**Precondition**: Player viewing inventory  
**Flow**:
1. System displays inventory items
2. System displays item categories
3. System displays selected item details
4. System displays item options (use, toss)

**Postcondition**: Inventory UI displayed

---

## Edge Cases

### EC-001: Party Full When Catching Pokemon

**Precondition**: Player catches Pokemon, party has 6 Pokemon  
**Flow**: System automatically deposits to PC

---

### EC-002: No Poke Balls When Trying to Catch

**Precondition**: Player tries to catch, no Poke Balls  
**Flow**: System displays error, battle continues

---

### EC-003: All Pokemon Fainted

**Precondition**: All player Pokemon fainted  
**Flow**: System handles game over or checkpoint return

---

### EC-004: PC Full When Depositing

**Precondition**: Player tries to deposit, PC full  
**Flow**: System displays error, deposit fails

---

**Last Updated**: January 2025

