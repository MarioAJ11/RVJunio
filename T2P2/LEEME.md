# RV-T2P2 — Practica 2.2

Copia portable del proyecto Unity. Abrir con **Unity Hub 6000.3.18f1**.

## Escena principal

**`Assets/Scenes/SampleScene.unity`**

Contiene todo lo necesario para probar y entregar:

| Nodo | Contenido |
|---|---|
| `GameplaySystems` | `DesktopVRRecorder` (modo PC para grabar) + `VRUiBootstrap` (UI, tecla H) |
| `GameplayObjects` | Mesa + cubo/esfera/cilindro agarrables (`Grab_*`) |
| `[BuildingBlock] Camera Rig` | Rig Meta XR (Quest / simulador) |
| `Floor` | Suelo |

## Scripts (`Assets/Scripts/`)

- `DesktopVRRecorder.cs` — WASD + ratón + clic para agarrar (solo PC/editor)
- `DesktopGrabbable.cs` — marca objetos agarrables en escena
- `VRUiBootstrap.cs` + `ScriptUI.cs` — UI interactiva
- `RaycastPlacement.cs` — colocación por raycast

## Probar en PC (grabar video)

1. Abre `SampleScene.unity`
2. **Play** → clic en ventana **Game**
3. WASD mover, ratón mirar, **mantener clic** en objetos `Grab_*`
4. **H** = panel UI (opcional)

Guía de grabación: `Desktop/RV/Entregables/GRABAR_T2P2.md`

## Probar con simulador / Quest

1. Cierra Meta Quest Link si molesta
2. **Meta → Meta XR Simulator → Activate** (PC) o build Android (Quest)
3. Play — usa interacciones Meta del Camera Rig

## Primer arranque

1. Unity Hub → Add → esta carpeta
2. Esperar importación (crea `Library/`)
3. Abrir `SampleScene.unity`

## Reconfigurar escena (editor)

Menú **T2P2 → Configurar escena de gameplay** (solo si faltan objetos).

## Entrega

Guía completa: `Desktop/RV/Entregables/GUIA_TERMINAR_T2P2.md`  
Memoria PDF: `Desktop/RV/Entregables/T2P2_Memoria.md`

Instrucciones generales: `../README.md`
