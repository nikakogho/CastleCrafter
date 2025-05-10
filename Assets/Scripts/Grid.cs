using UnityEngine;

[ExecuteAlways]
public class Grid : MonoBehaviour
{
    public static Grid Instance;
    [Min(0.1f)] public float cellSize = 2f;

    void Awake() => Instance = this;
    void OnValidate() => Instance = this;

    /// <summary>Snaps any world position to the nearest grid cell on X-Z plane.</summary>
    public Vector3 Snap(Vector3 worldPos)
    {
        float x = Mathf.Round(worldPos.x / cellSize) * cellSize;
        float z = Mathf.Round(worldPos.z / cellSize) * cellSize;
        return new Vector3(x, 0, z);
    }

#if UNITY_EDITOR
    /* ---------- Draw green grid lines in the Scene view ---------- */
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        const int half = 50;                   // 50 cells each way → 100×100 grid
        for (int i = -half; i <= half; i++)
        {
            Vector3 fromX = new Vector3(i * cellSize, 0, -half * cellSize);
            Vector3 toX = new Vector3(i * cellSize, 0, half * cellSize);
            Vector3 fromZ = new Vector3(-half * cellSize, 0, i * cellSize);
            Vector3 toZ = new Vector3(half * cellSize, 0, i * cellSize);

            Gizmos.DrawLine(fromX, toX);
            Gizmos.DrawLine(fromZ, toZ);
        }
    }
#endif
}
