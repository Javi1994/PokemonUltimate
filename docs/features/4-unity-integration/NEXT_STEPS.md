# Próximos Pasos - Feature 4: Unity Integration

> Guía de qué probar ahora y qué sigue después de completar Fase 4.3.

## 🎯 Estado Actual

**Fases Completadas:**
- ✅ **4.1: Unity Project Setup** - Proyecto Unity creado, DLLs integradas
- ✅ **4.2: UI Foundation** - Componentes UI básicos (HPBar, PokemonDisplay, BattleDialog)
- ✅ **4.3: IBattleView Implementation** - UnityBattleView y BattleManager implementados

## 🧪 Qué Probar Ahora

### 1. Regenerar la Escena de Batalla

En Unity Editor:
1. Abre el menú: **PokemonUltimate → Generate Battle Scene**
2. Esto creará/actualizará la escena `Assets/Scenes/BattleScene.unity`
3. La escena incluirá:
   - Canvas con UI completa
   - UnityBattleView con todas las referencias
   - BattleManager configurado

### 2. Ejecutar una Batalla de Prueba

**Opción A: Desde Inspector**
1. Abre la escena `BattleScene`
2. Selecciona el GameObject `BattleManager` en Hierarchy
3. En Inspector, marca **"Start Battle On Start"**
4. Presiona Play (▶️)
5. La batalla debería iniciarse automáticamente

**Opción B: Desde Context Menu**
1. Con la escena abierta y en Play Mode
2. Click derecho en `BattleManager` → **"Start Test Battle"**
3. La batalla iniciará con Pokemon de prueba

### 3. Verificar Funcionalidad

**Lo que deberías ver:**
- ✅ Mensaje inicial: "A wild [Pokemon] appeared!"
- ✅ Pokemon del jugador y enemigo mostrados (nombre y nivel)
- ✅ Barras de HP actualizándose durante la batalla
- ✅ Mensajes de batalla en el diálogo
- ✅ La batalla ejecutándose automáticamente (enemigo usa RandomAI)
- ✅ Mensaje final cuando termina la batalla

**Lo que NO funciona aún:**
- ⏳ Input del jugador (selección automática por ahora)
- ⏳ Animaciones visuales (placeholders)
- ⏳ Sonidos/audio

### 4. Verificar en Console

Revisa la Console de Unity para:
- ✅ No hay errores de compilación
- ✅ Los mensajes de Debug.Log aparecen correctamente
- ✅ No hay warnings críticos

## ⏭️ Próxima Fase: 4.4 Player Input System

Una vez que hayas verificado que la batalla funciona correctamente, el siguiente paso es implementar el sistema de input del jugador.

### Qué Implementar en Fase 4.4

1. **Menú de Acción Principal**
   - Botones: Fight, Switch, Item, Run
   - Navegación con teclado/ratón
   - Integración con `SelectActionType()`

2. **Menú de Movimientos**
   - Lista de movimientos disponibles
   - Mostrar PP, tipo, categoría
   - Integración con `SelectMove()`

3. **Selección de Objetivos**
   - Highlight de objetivos válidos
   - Click/teclado para seleccionar
   - Integración con `SelectTarget()`

4. **Menú de Cambio de Pokemon**
   - Lista de Pokemon disponibles
   - Mostrar HP, estado, nivel
   - Integración con `SelectSwitch()`

### Archivos a Crear

```
Assets/Scripts/UI/
├── ActionMenu.cs              # Menú principal (Fight/Switch/Item/Run)
├── MoveMenu.cs                # Selección de movimientos
├── TargetSelector.cs          # Selección de objetivos
└── PokemonSwitchMenu.cs       # Selección de Pokemon para cambiar
```

## 🔄 Flujo de Trabajo Recomendado

1. **Ahora**: Probar batalla actual en Unity
2. **Si funciona bien**: Continuar con Fase 4.4
3. **Si hay problemas**: Reportarlos y corregirlos antes de continuar

## 📝 Notas Importantes

- **Input Actual**: Los métodos de input en `UnityBattleView` usan defaults (selección automática)
- **Animaciones**: Son placeholders, se implementarán en Fase 4.5
- **Testing**: Usa testing manual en Unity Editor (no hay tests unitarios en Unity)

## 🐛 Troubleshooting

**Si la batalla no inicia:**
- Verifica que `BattleManager` tiene `battleView` asignado
- Verifica que `UnityBattleView` tiene todos los componentes UI asignados
- Revisa la Console de Unity para errores

**Si la UI no se actualiza:**
- Verifica que los componentes UI están correctamente referenciados
- Verifica que `BindSlotsToUI()` se ejecuta correctamente
- Revisa que los Pokemon se crean correctamente

**Si hay errores de compilación:**
- Verifica que los DLLs están actualizados (`build-dlls-for-unity.ps1`)
- Verifica que no hay referencias a archivos eliminados
- Regenera los archivos `.csproj` si es necesario

---

**Última Actualización**: 2025-01-XX  
**Estado**: Fase 4.3 completada, listo para testing y Fase 4.4

