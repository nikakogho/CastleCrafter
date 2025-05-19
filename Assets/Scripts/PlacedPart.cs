using UnityEngine;

/// <summary>Attached to every placed prefab so the grid can free its cells on deletion.</summary>
public class PlacedPart : MonoBehaviour
{
    public PartData partData;
    public int rotY; // 0 / 90 / 180 / 270

    void OnDestroy()
    {
        if (Grid.Instance && partData.footprint == Vector2Int.zero) return;
        if (!partData.isFloor) return;

        var footprint = rotY % 180 == 0
                           ? partData.footprint
                           : new Vector2Int(partData.footprint.y, partData.footprint.x);

        Grid.Instance.FreeArea(transform.position, footprint, rotY);
    }
}
