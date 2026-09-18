# HU Cave Cameras

Unity package for a CAVE (Cave Automatic Virtual Environment) multi-projector camera setup. A CAVE is a room-scale display system where images are projected onto multiple surrounding screens (walls, floor, ceiling) to create an immersive environment. This package provides four off-axis perspective cameras, manual calibration tools, and editor utilities to drive such a setup from a single Unity project.

**Package ID:** `com.hu.cavecameras`  
**Version:** `0.3.9`  
**Minimum Unity:** `2019.0`

---

## Features

- **Off-axis frustum projection** — each camera computes a correct perspective frustum from three physical screen corner positions and a tracked eye position, eliminating the distortion that standard on-axis cameras would produce on angled screens. The projection matrix correctly accounts for reversed-Z depth buffers, so results are consistent across graphics APIs.
- **Five-display management** — `CaveCameraManager` activates Unity Display 1 through Display 5 at runtime and drives all cameras from a single `CaveData` ScriptableObject. By default the four cave cameras render to Display 2–5, leaving Display 1 free for the operator/main window.
- **Configurable, swappable display output routing** — `CaveData.CameraDisplayOutputs` controls which physical display each camera renders to, and the calibration UI's compass-rose controls let you swap two cameras' outputs interactively without touching code.
- **Manual calibration UI** — a UI Toolkit panel (`ManualCalibrationUI`) with live two-way data binding lets you adjust `CaveCenter` (shown as "Cave Offset"), `CaveSize`, and `EyeHeight` in Play mode and persist the result.
- **Editor Inspector calibration** — the `CaveCameraManager` Inspector also exposes Center/Rotation/Size/Eye Height fields with "Resize" and "Resize as Root" buttons, for adjusting the camera frustums directly in the Editor outside of Play mode.
- **JSON persistence** — calibration is saved to and loaded from `%LOCALAPPDATA%\CaveCameras\caveData.json` so settings survive between sessions.
- **One-click scene setup** — an editor menu item builds the full camera hierarchy with Undo support, auto-assigns the shared `CaveData` asset, and attaches the calibration UI in one step.
- **Game View layout tool** — opens and positions four named Game Views, one per connected display, from a single menu item.

---

## Requirements

- Unity 2019.0 or newer
- Four connected displays (or display outputs) for a full CAVE setup
- UI Toolkit (included in Unity since 2019.3; for earlier 2019.x, verify availability)

---

## Installation

### Via Unity Package Manager (local)

1. Clone or download this repository.
2. In Unity, open **Window > Package Manager**.
3. Click **+** and choose **Add package from disk...**.
4. Navigate to the `package.json` file inside `HU-Cave-cameras-main` and select it.

### Manual

Copy the `HU-Cave-cameras-main` folder into your project's `Packages/` directory. Unity will detect and import it automatically.

---

## Getting Started

### 1. Insert the Camera Setup

Open your scene and go to **GameObject > Cave > Insert Camera Setup**.

This creates the full camera hierarchy:
- A `CaveCameraManager` root object
- Four child `CaveCamera` objects (one per screen/display)
- The calibration UI canvas

The wizard will also prompt you to configure the Main Camera and attach the calibration UI.

### 2. Configure Cave Data

A `CaveData` ScriptableObject drives all cameras. **Insert Camera Setup** automatically assigns the shared `CaveData` asset to the new `CaveCameraManager`, so no manual assignment is required. Key fields:

| Field | Description | Default |
|---|---|---|
| `CaveCenter` | World-space center of the CAVE volume. Shown as **"Cave Offset"** in the calibration UI. | — |
| `CaveSize` | Physical dimensions (width × height × depth) | — |
| `EyeHeight` | Viewer eye height in metres | 1.8 m |
| `CameraDisplayOutputs` | Physical display index each of the 4 cameras renders to | `[1, 2, 3, 4]` |

### 3. Run Calibration

Enter Play mode. The **Manual Calibration UI** panel appears with live controls for **Cave Offset**, **Cave Size**, and **Eye Height**. Adjust until the projection aligns with your physical screens, then press **Save**. Settings are written to `%LOCALAPPDATA%\CaveCameras\caveData.json` and reloaded automatically on next launch.

Press **Recalculate Cameras** to recompute the camera frustums immediately after any change.

The **Display Output Order** compass rose lets you re-route cameras to different physical displays: click one direction button, then another, to swap the display outputs assigned to those two camera positions.

Outside of Play mode, the same Center/Size/Eye Height adjustments are available directly in the `CaveCameraManager` Inspector via the **Resize** (custom center/rotation) and **Resize as Root** (uses the manager's own transform) buttons.

### 4. Arrange Game Views

Go to **Window > Cave > Cave Game Displays**. This opens four named Game View windows and positions each one on a separate connected display, ready for fullscreen projection.

---

## How Off-Axis Projection Works

Each `CaveCamera` takes three physical corner positions of its screen (bottom-left, bottom-right, top-left) and the current eye position. It computes a projection matrix each `FixedUpdate` such that screen edges align exactly with the physical screen boundaries regardless of the viewer's position, correctly compensating for reversed-Z depth buffers so the result is consistent across graphics APIs. Scene-view gizmos visualise the frustum in the Editor.

---

## Folder Structure

```
HU-Cave-cameras-main/
├── package.json                       # UPM manifest (com.hu.cavecameras, 0.3.9)
├── CHANGELOG.md                       # Version history
├── Runtime/
│   ├── CaveCamera.cs                  # Per-camera off-axis frustum projection, display swapping
│   ├── CaveCameraManager.cs           # Orchestrates four cameras, activates displays, output routing
│   ├── Data/
│   │   ├── CaveData.cs                # ScriptableObject: CaveCenter, CaveSize, EyeHeight, CameraDisplayOutputs
│   │   └── CaveDataLoader.cs          # JSON save/load to %LOCALAPPDATA%
│   ├── ScriptableObjects/
│   │   └── CaveData.asset             # Shared CaveData instance, auto-assigned on setup
│   └── UI/
│       ├── ManualCalibrationUI.uxml   # UI Toolkit calibration panel + display swap controls
│       ├── ManualCalibrationUI.uss    # Calibration panel styling
│       ├── CavePanelSettings.asset    # UI Toolkit panel settings
│       └── UnityDefaultRuntimeTheme.tss
└── Editor/
    ├── CaveCameraEditor.cs            # Custom Inspector + "Insert Camera Setup" menu item
    └── GameWindows.cs                 # "Cave Game Displays" Game View layout tool
```

---

## License

See the repository root for license information. If no license file is present, all rights are reserved by the author.
