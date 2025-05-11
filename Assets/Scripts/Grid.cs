using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Grid : MonoBehaviour
{
    public static Grid Instance;
    [Min(0.1f)] public float cellSize = 2f;
    
    // Each entry is an absolute Y coordinate (0 = ground)
    [HideInInspector] public List<float> levels = new List<float> { 0f };

    private const float LevelAllowedError = 0.01f;

    void Awake() => Instance = this;
    void OnValidate() => Instance = this;

    public float YForLevel(int level)
        => levels[Mathf.Clamp(level, 0, levels.Count - 1)];

    public int AddLevel(float y)
    {
        for (int i = 0; i < levels.Count; i++)
        {
            float error = Mathf.Abs(levels[i] - y);

            if (error < LevelAllowedError) return i;
        }

        int insertAt = levels.BinarySearch(y);

        if (insertAt < 0) insertAt = ~insertAt;

        levels.Insert(insertAt, y);

        return insertAt;
    }

    /// <summary>Snaps any world position to the nearest grid cell on X-Z plane.</summary>
    public Vector3 Snap(Vector3 worldPos, int level)
    {
        float x = Mathf.Round(worldPos.x / cellSize) * cellSize;
        float z = Mathf.Round(worldPos.z / cellSize) * cellSize;
        return new Vector3(x, YForLevel(level), z);
    }

#if UNITY_EDITOR
    /* ---------- Draw green grid lines in the Scene view ---------- */
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        const int half = 50;
        foreach (float y in levels)
        {
            for (int i = -half; i <= half; i++)
            {
                var a = new Vector3(i * cellSize, y, -half * cellSize);
                var b = new Vector3(i * cellSize, y, half * cellSize);
                var c = new Vector3(-half * cellSize, y, i * cellSize);
                var d = new Vector3(half * cellSize, y, i * cellSize);
                Gizmos.DrawLine(a, b);
                Gizmos.DrawLine(c, d);
            }
        }
    }
#endif
}
