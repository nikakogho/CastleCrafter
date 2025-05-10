# Castle Crafter 🏰
A sandbox castle-building prototype for PC (Unity **Built-in** render pipeline).  
Click a part in the build bar, watch a translucent ghost snap to the grid, and place walls & towers.  
Hit **Tab** any time (coming soon) to roam your creation in first-person “Enjoy Mode”.

---

## ✨ Current Feature Set
| Milestone | Commit Tag | What You Can Do |
|-----------|------------|-----------------|
| **Init project** | `init-project` | Clean Git repo, Unity .gitignore, first commit. |
| **Scene scaffold** | `scene-scaffold` | Folder hygiene, green grass ground, baseline lighting, `CameraRig`. |
| **RTS camera** | `rts-camera` | WASD / middle-drag pan · scroll zoom (inverted) · RMB rotate (Scene-view style). |
| **Grid system** | `grid-system` | Visible green 2 m grid & `Grid.I.Snap()` helper. |
| **Part system** | `part-assets` | `PartData` ScriptableObject, ghost-preview placement, Wall & Tower samples. |
| **Pretty UI** | `build-ui` | Bottom build bar with icons, hotkeys (1–9), tooltips, Sprite-swap styling. |
| **Ghost prefab refactor** | `ghost-prefab` | Each `PartData` may now specify a dedicated low-poly ghost prefab. |

---

## ▶️ Quick Start

```bash
git clone https://github.com/YourUser/CastleCrafter.git
cd CastleCrafter
```

### open the project in Unity Hub ► Open
Open Scenes/CastlePlayground.

Press Play.

Wall or Tower button (or hotkeys 1 / 2) → ghost follows cursor.

Left-click to place, Right-click to cancel.

## 🎮 Controls

| Action                          | Key / Mouse                                      |
| ------------------------------- | ------------------------------------------------ |
| Pan                             | **W A S D**, ← ↑ ↓ →, or **Middle-mouse drag**   |
| Rotate                          | **Right-mouse drag**                             |
| Zoom                            | **Mouse wheel** (forward = zoom in) or **Q / E** |
| Select part                     | Click icon or **1–9**                            |
| Place part                      | **Left-click**                                   |
| Cancel ghost                    | **Right-click**                                  |
| *(upcoming)* Rotate ghost       | **R**                                            |
| *(upcoming)* Delete placed part | **Delete**                                       |
| *(upcoming)* Enjoy Mode         | **Tab**                                          |

## 🗂️ Project Layout

Assets/
├─ Art/            # future textures, models
├─ Materials/
├─ Prefabs/
│   ├─ Wall.prefab
│   └─ Tower.prefab
├─ Parts/          # ScriptableObjects (PartData)
├─ Scenes/
│   └─ Castle Playground.unity
├─ Scripts/
│   ├─ Camera/     # RtsCamera.cs (MIT)
│   ├─ Grid.cs
│   ├─ BuildManager.cs
│   ├─ PartData.cs
│   └─ UI/
│       ├─ BuildUIButton.cs
│       └─ BuildHotkeys.cs
└─ UI/
    ├─ Sprites/    # 9-slice panel & button sprites
    ├─ Icons/      # Part icons
    └─ Prefabs/
        └─ BuildButton.prefab

## 🛠️ Building & Contributing

One feature per branch → pull request → merge.

Keep commits small (git tag after each milestone).

No large binaries; art gets its own Git LFS track when needed.

## 📅 Roadmap

| Next Up                     | Description                                                      |
| --------------------------- | ---------------------------------------------------------------- |
| **Rotate / Delete tool**    | `R` to rotate ghost 90°, `Delete` to remove hovered piece.       |
| **Enjoy Mode**              | Import Unity Starter Assets – First Person; toggle with **Tab**. |
| **Furniture placement**     | Surface-snap tables, chairs (new `FurnitureData`).               |
| **Stairs & Multilevel**     | `level` integer grid snap, stairs prefab raises placement layer. |
| **Secret doors & basement** | Interactable bookshelf pivot, underground room.                  |
| **Save / Load**             | JSON persistence (`Ctrl+S / Ctrl+L`).                            |
| **Runtime wall painting**   | Brush tool via Polybrush or custom shader.                       |

## ⚖️ Licensing & Credits

Core code © 2025 Nika Koghuashvili – MIT License.
RtsCamera.cs adapted from Over42 RTS-Camera (MIT).

Not affiliated with or endorsed by Unity Technologies.
