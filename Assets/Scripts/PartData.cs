using UnityEngine;

[CreateAssetMenu(menuName = "CastleCrafter/Part")]
public class PartData : ScriptableObject
{
    new public string name = "wall";
    public GameObject prefab;
    public GameObject ghostPrefab;

    [Header("Placement")]
    public bool snapToSurface;
    public bool raisesLevel = false;   // ← tick for stairs / ladders
    public float raiseHeight = 3f;     // metres added if raisesLevel

    public Sprite icon;
    public Vector2Int footprint = Vector2Int.one;
}
