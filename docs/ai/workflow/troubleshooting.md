# 🔧 Workflow Troubleshooting Guide

> Common issues during implementation and how to resolve them.

## Issue: Feature Not Assigned

**Symptoms:**
- Starting implementation without checking existing features
- Creating code without feature documentation
- Not knowing which feature/sub-feature the work belongs to

**Solution:**
1. **Stop implementation immediately**
2. **Read `docs/features_master_list.md`** - Review all existing features
3. **Determine feature assignment:**
   - If fits existing feature → Read that feature's documentation
   - If needs new feature → Create feature folder and complete documentation structure
4. **Follow `docs/feature_documentation_standard.md`** for new features
5. **Update `docs/features_master_list.md`** if creating new feature
6. **Resume implementation** only after feature is assigned and documented

## Issue: Spec is Incomplete

**Symptoms:**
- Missing method signatures
- Unclear behavior descriptions
- No examples provided

**Solution:**
1. **Stop implementation immediately**
2. **Ensure feature is assigned** (see above)
3. Read similar specs in `docs/features/[N]-[feature-name]/` or sub-feature `architecture.md` for patterns (always use numbered format)
4. Complete the spec with:
   - Method signatures (parameters, return types)
   - Behavior descriptions
   - Examples
   - Edge cases
5. Document what was missing
6. Resume implementation

**Example:**
```markdown
### What Was Missing
- Method signature for `CalculateDamage()`
- Behavior for zero-damage scenarios
- Examples of type effectiveness calculations
```

## Issue: Spec is Incorrect

**Symptoms:**
- Implementation reveals spec doesn't match game mechanics
- Spec contradicts other documentation
- Spec suggests approach that doesn't work

**Solution:**
1. **Document the discrepancy** clearly
2. **Verify** against official sources (game data, use cases)
3. **Decide:**
   - Fix spec → Update `docs/features/[N]-[feature-name]/architecture.md` or sub-feature `architecture.md` (always use numbered format)
   - Change implementation → Update spec to match reality
4. **Note decision** in `.ai/context.md` under "Key Architectural Decisions"
5. Continue with corrected understanding

**Example:**
```markdown
### Spec Discrepancy Found
- **Issue:** Spec says Burn reduces Attack by 50%, but should only affect Physical moves
- **Decision:** Update spec to match game mechanics
- **Impact:** Affects DamagePipeline BurnStep implementation
```

## Issue: Test Reveals Missing Functionality

**Symptoms:**
- Test fails because feature doesn't exist
- Edge case test reveals unhandled scenario
- Integration test shows missing interaction

**Solution:**
1. **DO NOT** skip the test or mark as "future work"
2. **Implement the missing functionality immediately** (Test-Driven Discovery)
3. Update spec/documentation if needed
4. Continue until feature is complete

**Example:**
```csharp
// Test reveals missing functionality
[Test]
public void Pokemon_WithMaxIVs_CalculatesCorrectSpeed()
{
    // This test revealed that IV calculation was missing
    // → Implement IV calculation
    // → Test now passes
}
```

## Issue: Architectural Change Required

**Symptoms:**
- Current architecture doesn't support new feature
- Discovery requires pattern change
- Multiple systems need modification

**Solution:**
1. **Pause current work**
2. **Document the discovery:**
   - What was discovered?
   - Why does it require architectural change?
   - What systems are affected?
3. **Evaluate impact:**
   - List all affected systems
   - Estimate scope of changes
   - Check if change breaks existing features
4. **Update architecture docs:**
   - Document new pattern
   - Update affected system docs
   - Add to `.ai/context.md` decisions table
5. **Resume with updated understanding**

**Example:**
```markdown
### Architectural Discovery
- **Issue:** Status effects need to trigger at different times (start of turn, end of turn, on switch)
- **Solution:** Create `IStatusTrigger` interface with timing enum
- **Impact:** Affects StatusEffect, EndOfTurnProcessor, SwitchAction
- **Status:** Pattern documented in `status_and_stat_system.md`
```

## Issue: Integration Test Reveals Bug

**Symptoms:**
- Integration test fails
- Systems don't work together correctly
- State inconsistency between systems

**Solution:**
1. **Isolate the issue:**
   - Which systems are involved?
   - What's the expected vs actual behavior?
   - Can you reproduce with unit tests?
2. **Fix the root cause:**
   - Don't patch symptoms
   - Fix the actual bug
   - Ensure fix doesn't break other tests
3. **Verify fix:**
   - All tests pass
   - Integration test passes
   - No regressions

## Issue: Use Case Reveals Missing Feature

**Symptoms:**
- Use case validation shows incomplete implementation
- Combat use case document lists unhandled scenario
- Feature doesn't match Pokemon game behavior

**Solution:**
1. **Read the use case** carefully
2. **Implement missing functionality** immediately
3. **Mark use case as complete** in `docs/combat_use_cases.md`
4. **Update tests** to cover the use case
5. **Continue** until all use cases covered

## Issue: Performance Concerns

**Symptoms:**
- Implementation is slow
- Tests take too long
- Memory usage is high

**Solution:**
1. **Measure first:**
   - Profile the code
   - Identify bottlenecks
   - Verify it's actually a problem
2. **Optimize carefully:**
   - Don't optimize prematurely
   - Maintain code clarity
   - Keep tests passing
3. **Document trade-offs:**
   - Note performance decisions in code comments
   - Update architecture docs if pattern changed

## Issue: Circular Dependency

**Symptoms:**
- Compilation errors about circular references
- System A needs System B, System B needs System A
- Architecture violation

**Solution:**
1. **Identify the cycle:**
   - Map dependencies
   - Find the circular path
2. **Break the cycle:**
   - Extract shared interface
   - Use dependency injection
   - Create abstraction layer
3. **Verify:**
   - Code compiles
   - Tests pass
   - Architecture is cleaner

## Issue: Too Many Parameters

**Symptoms:**
- Method has 5+ parameters
- Constructor is unwieldy
- Code is hard to read

**Solution:**
1. **Use parameter object pattern:**
   ```csharp
   // Before
   public void CreatePokemon(string name, int level, int hp, int attack, ...)
   
   // After
   public void CreatePokemon(PokemonCreationParams params)
   ```
2. **Or use builder pattern:**
   ```csharp
   var pokemon = PokemonBuilder.Create(species)
       .WithLevel(50)
       .WithIVs(ivs)
       .Build();
   ```

## Issue: Code Duplication

**Symptoms:**
- Same logic in multiple places
- Copy-paste code
- Maintenance nightmare

**Solution:**
1. **Extract common logic:**
   - Create shared method/class
   - Use composition
   - Don't over-abstract
2. **Follow DRY principle:**
   - But prefer duplication over wrong abstraction
   - Keep code readable

## Quick Decision Tree

```
Issue Found?
├─ Spec incomplete? → Complete spec first
├─ Spec incorrect? → Fix spec or update implementation
├─ Test reveals missing feature? → Implement immediately
├─ Architecture change needed? → Document and update architecture
├─ Integration test fails? → Fix root cause
├─ Use case missing? → Implement and mark complete
├─ Phase unclear? → Check relevant roadmap
└─ Other? → Document issue and resolve
```

## Roadmaps Reference

When troubleshooting, check the relevant roadmap to understand:
- Current phase status
- Dependencies that might affect the issue
- Expected implementation approach
- Test requirements

| Feature Area | Roadmap Document |
|--------------|------------------|
| **Combat System** | `docs/features/2-combat-system/roadmap.md` |
| **Content Expansion** | `docs/features/3-content-expansion/roadmap.md` |
| **Unity Integration** | `docs/features/4-unity-integration/roadmap.md` |
| **Game Features** | `docs/features/5-game-features/roadmap.md` |
| **Test Structure** | `docs/ai/testing_structure_definition.md` |

See `docs/features/README.md` for overview of all features.

