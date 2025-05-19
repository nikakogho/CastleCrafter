using UnityEngine;

[System.Serializable]
public struct SupportAnchor
{
    [Tooltip("Local position (in prefab space) of the anchor point.")]
    public Vector3 localPosition;

    [Tooltip("Kind of support this anchor expects.")]
    public AnchorType type;

    [Tooltip("Only for Side anchors – local outward direction.")]
    public Vector3 localDirection;

    [Tooltip("Max distance to check for support (0.2-0.5 m is typical).")]
    public float maxDistance;
}

public enum AnchorType { Bottom, Side }
