using UnityEngine;

[CreateAssetMenu(menuName = "CastleCrafter/Part")]
public class PartData : ScriptableObject
{
    new public string name = "wall";
    public GameObject prefab;
    public GameObject ghostPrefab;
    public Sprite icon;
    public Vector2Int footprint = Vector2Int.one;
}
