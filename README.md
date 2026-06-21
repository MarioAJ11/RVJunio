# Practicas RV / RA — Mario Alguacil Juarez

Repositorio local con los proyectos portables, videos de demostracion e instrucciones de ejecucion.

**Autor:** Alguacil Juarez, Mario  
**Asignatura:** Realidad Virtual y Aumentada


## Practica 2.2 — Proyecto de Realidad Virtual (Unity)

**Titulo:** RV-T2P2 — Sala de interacciones VR  
**Motor:** Unity 6.3.18f1 (6000.3.18f1)  
**Dispositivo:** Meta Quest 2 / 3  
**SDK:** Meta XR All-in-One SDK **203**  
**Escena:** `Assets/Scenes/SampleScene.unity` (principal) · `Assets/VRInteractions.unity` (Meta grab)

### Video

| Archivo | Descripcion |
|---|---|
| [videos/T2P2.mp4](videos/T2P2.mp4) | PC o Quest: agarre, UI (tecla H), locomotion, teleport |

### Requisitos para ejecutar

| Requisito | Version / nota |
|---|---|
| Unity Hub + Editor | **6000.3.18f1** (Unity 6.3) |
| Meta XR SDK | 203 (incluido en `Packages/`) |
| URP | Incluido en proyecto |
| Android Build Support | Para exportar APK |
| Meta XR Simulator | Opcional — probar interacciones Meta en PC |

### Como abrir en otro ordenador

1. Copia **`T2P2/`** al nuevo PC.
2. Unity Hub → **Add** → carpeta `T2P2`.
3. Espera importacion (regenera `Library/`).
4. Abre **`Assets/Scenes/SampleScene.unity`**.

La escena incluye `GameplayObjects` (mesa + objetos agarrables) y `GameplaySystems` (scripts).

### Probar en PC (grabar video, sin gafas)

1. Abre `SampleScene.unity` → **Play**.
2. **Clic en ventana Game** → WASD mover, ratón mirar.
3. **Mantener clic** en objetos `Grab_*` para agarrar.
4. **H** = mostrar/ocultar UI.

Detalle: `Entregables/GRABAR_T2P2.md`

### Probar con simulador Meta

1. Cierra Meta Quest Link.
2. **Meta → Meta XR Simulator → Activate**.
3. Abre `VRInteractions.unity` → Play.
4. Tecla **H** = UI (**U** = grip, **B** = saltar en simulador).
5. **Project Settings → XR → PC:** solo OpenXR.

### Completar entrega

Guia: `Entregables/GUIA_TERMINAR_T2P2.md` (locomotion, teleport, snap, PDF, Git, video).

### Entrega asociada

- PDF: `AlguacilJuarezMario_T2P2.pdf`
- Codigo: `T2P2/` o GitHub

---

## Practica 4.1 — Proyecto RA/RM (Godot)

**Titulo:** RV-T4P1 — Malabares en realidad mixta  
**Motor:** Godot 4.6 (modo **Compatibility**)  
**Dispositivo:** Meta Quest 2 / 3 / 3S  
**Plugins:** Godot OpenXR Vendors v5 + Godot XR Tools 4.4

### Video

| Archivo | Descripcion |
|---|---|
| [videos/T4P1.mp4](videos/T4P1.mp4) | Gameplay malabares + configuracion XR/export |

### Requisitos para ejecutar

| Requisito | Version / nota |
|---|---|
| Godot Engine | **4.6** (`Godot_v4.6-stable_win64.exe`) |
| Modo renderizado | **Compatibility** (arriba derecha en el editor) |
| JDK | 17 (solo para exportar APK) |
| Android SDK | Android Studio (solo para exportar APK) |
| Meta Quest | Opcional; en PC funciona simulador de raton |

### Como abrir en otro ordenador

1. Copia la carpeta **`Repo/T4P1/`** completa al nuevo PC.
2. Abre Godot 4.6 → **Import** → selecciona la carpeta `T4P1`.
3. Godot regenerara `.godot/` automaticamente (no se incluye a proposito).
4. Modo **Compatibility** arriba a la derecha.
5. Abre `scenes/main.tscn` → **F6** (Play Scene).

### Controles en PC (simulacion para video)

| Accion | Control |
|---|---|
| Agarrar / soltar pelota | Clic izquierdo |
| Lanzamiento suave | Soltar sin mover el raton |
| Lanzamiento alto | **Espacio** (con pelota agarrada) |
| Fondo webcam (simula passthrough) | **C** |

En Quest: agarrar con **grip**; la fuerza del lanzamiento la marca el **movimiento de la mano** al soltar.

### Exportar a Quest

1. **Editor → Editor Settings → Export → Android:** JDK 17 + ruta SDK.
2. **Project → Install Android Build Template** (crea `android/` en el PC destino).
3. **Project → Export → Meta Quest** → `builds/RV-T4P1-MR.apk`.
4. Instalar: `adb install builds/RV-T4P1-MR.apk`.

### Entrega asociada

- PDF: `AlguacilJuarezMario_T4P1.pdf`
- Codigo: esta carpeta `T4P1/` o GitHub
