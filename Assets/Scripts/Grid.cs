using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
public class Grid : MonoBehaviour
{
    public static Grid Instance;
    public float cellSize = 2f;   // X-Z grid
    public float levelStep = 2f;   // vertical spacing between floors

    readonly HashSet<Vector3Int> occupied = new();  // X,Ylevel,Z

    void Awake() => Instance = this;

    /* ---------- helpers ---------- */
    public Vector3Int WorldToCell3D(Vector3 w)
        => new(Mathf.RoundToInt(w.x / cellSize),
               Mathf.RoundToInt(w.y / levelStep),
               Mathf.RoundToInt(w.z / cellSize));

    public Vector3 Snap(Vector3 w)
        => new(Mathf.Round(w.x / cellSize) * cellSize,
               Mathf.Round(w.y / levelStep) * levelStep,
               Mathf.Round(w.z / cellSize) * cellSize);

    static Vector2Int RotFoot(Vector2Int fp, int rotY)
        => rotY % 180 == 0 ? fp : new Vector2Int(fp.y, fp.x);

    /* ---------- queries ---------- */
    public bool IsAreaFree(Vector3 worldPos, Vector2Int fp, int rotY)
    {
        Vector3Int baseCell = WorldToCell3D(worldPos);
        Vector2Int size = RotFoot(fp, rotY);

        for (int dx = 0; dx < size.x; dx++)
            for (int dz = 0; dz < size.y; dz++)
                if (occupied.Contains(new(baseCell.x + dx,
                                          baseCell.y,
                                          baseCell.z + dz)))
                    return false;
        return true;
    }

    public void OccupyArea(Vector3 worldPos, Vector2Int fp, int rotY)
    {
        if (!IsAreaFree(worldPos, fp, rotY)) return;

        Vector3Int baseCell = WorldToCell3D(worldPos);
        Vector2Int size = RotFoot(fp, rotY);

        for (int dx = 0; dx < size.x; dx++)
            for (int dz = 0; dz < size.y; dz++)
                occupied.Add(new(baseCell.x + dx,
                                 baseCell.y,
                                 baseCell.z + dz));
    }

    public void FreeArea(Vector3 worldPos, Vector2Int fp, int rotY)
    {
        Vector3Int baseCell = WorldToCell3D(worldPos);
        Vector2Int size = RotFoot(fp, rotY);

        for (int dx = 0; dx < size.x; dx++)
            for (int dz = 0; dz < size.y; dz++)
                occupied.Remove(new(baseCell.x + dx,
                                    baseCell.y,
                                    baseCell.z + dz));
    }
}
