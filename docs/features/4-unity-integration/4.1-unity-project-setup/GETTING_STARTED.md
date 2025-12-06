# Getting Started: Unity Integration - Phase 4.1

> **Quick start guide for setting up Unity project and integrating PokemonUltimate DLLs.**

## Prerequisites

- ✅ Unity 2021.3+ or Unity 6 installed
- ✅ .NET SDK installed (for building DLLs)
- ✅ PokemonUltimate project built successfully

## Step-by-Step Setup

### Step 1: Create Unity Project

1. **Open Unity Hub**
2. **Create New Project**
   - Template: **2D (URP)** recommended
   - Project Name: `PokemonUltimateUnity` (or your preferred name)
   - Location: Choose a location (can be sibling to PokemonUltimate repo)
   - Click **Create**

3. **Wait for Unity to initialize** (may take a few minutes)

### Step 2: Build DLLs

1. **Open PowerShell** in the PokemonUltimate project root

2. **Run build script:**
   ```powershell
   .\ai_workflow\scripts\build-dlls-for-unity.ps1
   ```

   Or manually:
   ```powershell
   dotnet build -c Release
   ```

3. **Verify DLLs were created:**
   - `PokemonUltimate.Core/bin/Release/netstandard2.1/PokemonUltimate.Core.dll`
   - `PokemonUltimate.Combat/bin/Release/netstandard2.1/PokemonUltimate.Combat.dll`
   - `PokemonUltimate.Content/bin/Release/netstandard2.1/PokemonUltimate.Content.dll`

### Step 3: Copy DLLs to Unity

**Option A: Automatic (Recommended)**

If your Unity project is at `C:\Path\To\UnityProject`:

```powershell
.\ai_workflow\scripts\build-dlls-for-unity.ps1 -UnityProjectPath "C:\Path\To\UnityProject"
```

**Option B: Manual**

1. In Unity project, create folder: `Assets/Plugins/`
2. Copy these DLLs from PokemonUltimate project:
   - `PokemonUltimate.Core.dll`
   - `PokemonUltimate.Combat.dll`
   - `PokemonUltimate.Content.dll`
3. Paste into `Assets/Plugins/`

### Step 4: Configure DLLs in Unity

1. **Select each DLL** in Unity Project window
2. **In Inspector**, verify settings:
   - **API Compatibility Level**: `.NET Standard 2.1`
   - **Platform**: `Any Platform` ✓
   - **Auto Reference**: ✓ Enabled

3. **Check Console** - Should have no errors

### Step 5: Create Test Script

1. **Create folder**: `Assets/Scripts/`

2. **Create script**: `Assets/Scripts/PokemonTest.cs`

```csharp
using UnityEngine;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Content.Catalogs.Pokemon;

public class PokemonTest : MonoBehaviour
{
    void Start()
    {
        // Test creating a Pokemon
        var pikachu = PokemonFactory.Create(PokemonCatalog.Pikachu, 50);
        
        Debug.Log($"Created {pikachu.Name}!");
        Debug.Log($"Level: {pikachu.Level}");
        Debug.Log($"HP: {pikachu.CurrentHP}/{pikachu.MaxHP}");
        Debug.Log($"Attack: {pikachu.Attack}");
        Debug.Log($"Defense: {pikachu.Defense}");
        Debug.Log($"Speed: {pikachu.Speed}");
    }
}
```

3. **Create empty GameObject** in scene
4. **Add `PokemonTest` component** to GameObject
5. **Press Play**
6. **Check Console** - Should see Pokemon stats logged

### Step 6: Verify Integration

✅ **Success indicators:**
- No errors in Unity Console
- Can create `PokemonInstance` from script
- Can access `PokemonCatalog`
- Test script runs without errors

❌ **If errors occur:**
- Check DLL API Compatibility Level is `.NET Standard 2.1`
- Verify all 3 DLLs are in `Assets/Plugins/`
- Check Unity version compatibility (2021.3+)
- Rebuild DLLs if needed

## Next Steps

Once DLLs are integrated successfully:

1. ✅ **Phase 4.1 Complete** - DLLs integrated
2. ➡️ **Phase 4.2** - Create basic UI foundation (HP bars, Pokemon display)
3. ➡️ **Phase 4.3** - Implement `IBattleView` interface

## Troubleshooting

### DLL Import Errors

**Error**: "Assembly 'PokemonUltimate.Core' has reference to non-existent assembly"

**Solution**: Make sure all 3 DLLs are copied together (Core, Combat, Content)

### API Compatibility Errors

**Error**: "The type or namespace name 'PokemonUltimate' could not be found"

**Solution**: 
1. Select DLL in Project window
2. Check Inspector → API Compatibility Level = `.NET Standard 2.1`
3. Click "Apply"
4. Reimport DLLs

### Build Errors

**Error**: DLLs not found after build

**Solution**:
1. Check build output directory: `bin/Release/netstandard2.1/`
2. Verify `TargetFramework` in `.csproj` files is `netstandard2.1`
3. Rebuild: `dotnet clean && dotnet build -c Release`

## Related Documents

- **[Roadmap](../roadmap.md)** - Complete Unity integration phases
- **[Architecture](../architecture.md)** - Technical integration guide
- **[Code Location](../code_location.md)** - Where Unity code will live

---

**Last Updated**: 2025-01-XX

