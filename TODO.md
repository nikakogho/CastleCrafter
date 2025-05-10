# Castle Crafter — Development Milestones

| Status | #  | Commit Tag            | Goal                                            | Testable Result                                                                          |
| :----: |:-: | --------------------- | ----------------------------------------------- | ---------------------------------------------------------------------------------------- |
| ☑️ | **0** | `init-project`        | **Git + housekeeping**                          | Empty Unity project is under version control, clean `.gitignore`, first commit.          |
| ☑️ | **1** | `scene-scaffold`      | **Project folders & baseline scene**            | Flat ground plane, organised `Assets/` folders, baseline lighting, scene saved.          |
| ☑️ | **2** | `rts-camera`          | **Top-down movable camera**                     | WASD / middle-drag pan, scroll zoom, RMB rotate (Scene-view style, no edge scroll).      |
| ☑️ | **3** | `grid-system`         | **Grid snap & gizmo**                           | Green grid visible; any object can snap to nearest 2 m cell via `Grid.I.Snap()`.         |
| ☑️ | **4** | `part-assets`         | **PartData SOs & sample prefabs**               | Build bar shows Wall & Tower icons; ghost preview follows cursor, snaps to grid.         |
| ⬜️ | **5** | `build-manager`       | **Placement & delete tool**                     | `R` rotates ghost 90°; **Delete** removes hovered part; Right-click cancels build mode.  |
| ⬜️ | **6** | `fps-enjoy-mode`      | **First-Person walk-through toggle**            | Press **Tab** to switch between RTS build mode and FPS mode with full collision.         |
| ⬜️ | **7** | `furniture-placement` | **Surface-snap furniture**                      | Place a table on a castle floor; rotate with **Q/E** before placing.                     |
| ⬜️ | **8** | `stairs-multilevel`   | **Multi-storey support**                        | Build stairs; place furniture on an upper floor that snaps correctly.                    |
| ⬜️ | **9** | `secret-door`         | **Interactable bookshelf door**                 | In FPS, press **F** to swing bookshelf and reveal a hidden passage or basement.          |
| ⬜️ | **10**| `save-load`           | **JSON persistence**                            | **Ctrl+S** saves the build; **Ctrl+L** reloads it in a fresh session.                    |
| ⬜️ | **11**| `texture-paint`       | **Runtime wall painting** *(optional)*          | Select wall → brush blends textures / colours live in play mode.                         |
