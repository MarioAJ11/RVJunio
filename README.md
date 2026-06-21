# Practicas RV / RA — Mario Alguacil Juarez

Repositorio local con los proyectos portables, videos de demostracion e instrucciones de ejecucion.

**Autor:** Alguacil Juarez, Mario  
**Asignatura:** Realidad Virtual y Aumentada

---

## Estructura del repositorio

```
Repo/
├── README.md          ← este archivo
├── videos/            ← videos de demostracion
├── T2P2/              ← Practica 2.2 (Unity)
└── T4P1/              ← Practica 4.1 (Godot)
```

---

## Practica 2.2 — Proyecto de Realidad Virtual (Unity)

**Titulo:** RV-T2P2 — Sala de interacciones VR  
**Motor:** Unity 6.3.18f1 (6000.3.18f1)  
**Dispositivo:** Meta Quest 2 / 3  
**SDK:** Meta XR All-in-One SDK

### Video

| Archivo | Descripcion |
|---|---|
| [videos/T2P2.mp4](videos/T2P2.mp4) | Demostracion del proyecto 2.2 |

### Requisitos para ejecutar

| Requisito | Version / nota |
|---|---|
| Unity Hub + Editor | **6000.3.18f1** (Unity 6.3) |
| Meta XR SDK | Instalado via Package Manager (incluido en `Packages/`) |
| Android Build Support | Para exportar APK a Quest |
| Meta Quest Link o USB | Para probar en gafas (opcional) |
| Meta XR Simulator | Para probar en PC sin gafas |

### Como abrir en otro ordenador

1. Copia la carpeta **`Repo/T2P2/`** completa al nuevo PC.
2. Abre **Unity Hub** → **Add** → selecciona la carpeta `T2P2`.
3. Unity regenerara `Library/` automaticamente (no se incluye a proposito).
4. Espera a que termine la importacion de paquetes XR.
5. Abre la escena principal desde `Assets/` (cuando este montada).
6. **Build Settings:** plataforma Android, escena en build.

### Probar sin gafas (simulador)

1. Cierra Meta Quest Link si esta abierto.
2. **Meta → Meta XR Simulator → Activate**.
3. **Project Settings → XR Plug-in Management → PC:** solo **OpenXR** activo.
4. Play en el editor; usa la ventana del simulador.

### Exportar a Quest

1. **File → Build Settings → Android → Switch Platform**.
2. Conecta las gafas por USB (modo desarrollador).
3. **Build and Run** o genera APK en carpeta `build/`.

### Entrega asociada

- PDF: `AlguacilJuarezMario_T2P2.pdf`
- Codigo: esta carpeta `T2P2/`

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
- Codigo: esta carpeta `T4P1/`
