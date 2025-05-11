# Castle Crafter 🏰
A sandbox castle-building prototype for PC (Unity **Built-in** render pipeline).  
Click a part in the build bar, watch a translucent ghost snap to the grid, and place walls & towers.  
Hit **Tab** any time (coming soon) to roam your creation in first-person “Enjoy Mode”.

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
| Rotate ghost                    | **R**                                            |
| Delete placed part              | **Delete**                                       |
| Enjoy Mode                      | **Tab**                                          |

## 🗂️ Project Layout

Assets/
├─ Art/            # future textures, models
├─ Materials/
├─ Prefabs/
│   ├─ Ghosts
│   └─ Parts
│   └─ UI
├─ Parts/          # ScriptableObjects (PartData)
├─ Scenes/
│   └─ Castle Playground.unity
├─ Scripts/

## 🛠️ Building & Contributing

One feature per branch → pull request → merge.

Keep commits small (git tag after each milestone).

No large binaries; art gets its own Git LFS track when needed.

## 📅 Roadmap

Can view at [TODO.md](TODO.md)

## ⚖️ Licensing & Credits

Core code © 2025 Nika Koghuashvili – MIT License.
RtsCamera.cs adapted from Over42 RTS-Camera (MIT).

Not affiliated with or endorsed by Unity Technologies.
