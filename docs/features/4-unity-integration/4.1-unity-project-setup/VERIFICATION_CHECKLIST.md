# Verification Checklist - Unity Integration

> **Quick checklist to verify Unity integration is working correctly**

**Feature**: 4: Unity Integration  
**Sub-Feature**: 4.1: Unity Project Setup

## ✅ Pre-Verification

- [ ] Unity Editor is open
- [ ] Project loads without errors
- [ ] No compilation errors in Console

## 1. DLL Configuration ✅

**Location**: `Assets/Plugins/`

For each DLL (`PokemonUltimate.Core.dll`, `PokemonUltimate.Combat.dll`, `PokemonUltimate.Content.dll`):

- [ ] Select DLL in Project window
- [ ] Check Inspector → **API Compatibility Level** = `.NET Standard 2.1`
- [ ] Check **Platform** = `Any Platform` ✓
- [ ] Click **Apply**
- [ ] Verify no errors in Console

**Expected Result**: No import errors, DLLs appear with correct settings.

## 2. Compilation Check ✅

- [ ] Open **Console** window (Window → General → Console)
- [ ] Check for **red errors** (should be none)
- [ ] Check for **yellow warnings** (acceptable, but review if many)
- [ ] Verify scripts compile successfully

**Expected Result**: Clean compilation, 0 errors.

## 3. Unity Tests (Optional but Recommended) ✅

**Note**: No Unity tests - verify DLL integration manually or via C# tests

- [ ] Open **Test Runner** (Window → General → Test Runner)
- [ ] Switch to **EditMode** tab
- [ ] Find tests under `DLLIntegration/`
- [ ] Click **Run All** or run individually:
  - `DLLs_LoadWithoutErrors`
  - `Namespaces_AreAccessible`
  - `CoreTypes_CanBeInstantiated`
- [ ] Verify all tests pass ✓

**Expected Result**: All tests pass (green checkmarks).

## 4. PokemonTest Script ✅

**Location**: `Assets/Scripts/Test/PokemonTest.cs`

- [ ] Create empty GameObject in scene
- [ ] Add component `PokemonTest` to GameObject
- [ ] Verify component appears in Inspector
- [ ] Press **Play** button
- [ ] Check **Console** output

**Expected Console Output**:
```
=== PokemonUltimate Unity Integration Tests ===

[Test 1] Creating Pokemon...
✓ Successfully created Pikachu (Level 50)

[Test 2] Checking Pokemon stats...
  Name: Pikachu
  Level: 50
  HP: XXX/XXX
  Attack: XXX
  Defense: XXX
  ...
✓ Stats accessible - HP: XXX/XXX

[Test 3] Checking Pokemon moves...
✓ Pokemon has X moves:
  - MoveName (PP: XX/XX)
  ...

[Test 4] Checking Pokemon catalog...
✓ Successfully created X/4 Pokemon from catalog

=== All Tests Complete ===
```

**Expected Result**: All tests log successfully, no errors.

## 5. Manual Test (Optional) ✅

- [ ] Right-click `PokemonTest` component in Inspector
- [ ] Select **Test Multiple Pokemon** from context menu
- [ ] Check Console for output

**Expected Result**: Teams created successfully.

## ❌ Troubleshooting

### DLL Import Errors

**Error**: "Assembly 'PokemonUltimate.Core' has reference to non-existent assembly"

**Solution**:
1. Verify all 3 DLLs are in `Assets/Plugins/`
2. Check API Compatibility Level is `.NET Standard 2.1`
3. Rebuild DLLs: `.\ai_workflow\scripts\build-dlls-for-unity.ps1 -UnityProjectPath ".\PokemonUltimateUnity"`

### Compilation Errors

**Error**: "The type or namespace name 'PokemonUltimate' could not be found"

**Solution**:
1. Select DLL in Project window
2. Inspector → API Compatibility Level = `.NET Standard 2.1`
3. Click "Apply"
4. Wait for Unity to reimport
5. Check Console again

### Test Failures

**Error**: Tests fail with "Type not found"

**Solution**:
1. Verify DLLs are imported correctly (see step 1)
2. Check Console for compilation errors
3. Rebuild DLLs if needed
4. Re-run tests

### PokemonTest Script Errors

**Error**: Script doesn't run or shows errors

**Solution**:
1. Check Console for compilation errors
2. Verify DLLs are configured correctly
3. Ensure GameObject has `PokemonTest` component
4. Check that `runOnStart` is enabled in Inspector

## ✅ Success Criteria

All checks pass:
- [x] DLLs configured correctly
- [x] No compilation errors
- [x] Unity tests pass (if run)
- [x] PokemonTest script runs successfully
- [x] Console shows expected output

## Next Steps

Once verification is complete:
- ✅ **Phase 4.1 Complete** - DLLs integrated
- ➡️ **Phase 4.2** - Create basic UI foundation (HP bars, Pokemon display)

---

**Last Updated**: 2025-01-XX

