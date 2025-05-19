using UnityEngine;

[CreateAssetMenu(menuName = "CastleCrafter/Part")]
public class PartData : ScriptableObject
{
    new public string name;

    public GameObject prefab;
    public GameObject ghostPrefab;

    [Header("Grid")]
    public bool useGrid = true;
    public bool isFloor;
    public Vector2Int footprint = Vector2Int.one;   // cells in X,Z

    [Header("Free-surface parts")]
    public bool alignToSurface = false;
    public float surfaceOffset = 0.1f;

    [Header("Size (metres, world space)")]
    public float height = 2f;                 // Y size

    [Header("UI")]
    public Sprite icon;
}
