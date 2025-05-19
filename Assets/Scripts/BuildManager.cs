using UnityEngine;
using UnityEngine.EventSystems;

public class BuildManager : MonoBehaviour
{
    [Header("Layers")]
    [SerializeField] LayerMask supportMask;
    [SerializeField] LayerMask blockMask; // solid objects that block placement

    [Header("Ghost & UI")]
    [SerializeField] Material fallbackGhostMat;
    [SerializeField] GameObject buildUI;
    [SerializeField] KeyCode buildUIToggleKey = KeyCode.B;

    [Header("Rotate")]
    [SerializeField] KeyCode rotateLeftKey = KeyCode.Q;
    [SerializeField] KeyCode rotateRightKey = KeyCode.E;

    [Header("Build UI")]
    public GameObject tooltip;

    PartData currentPart;
    GameObject ghost;
    int ghostRotationY = 0;

    static readonly int _Color = Shader.PropertyToID("_BaseColor"); // URP / HDRP-safe

    public static BuildManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        // Cancel building if we enter FPS mode
        if (ModeSwitcher.Instance.IsInFPS && (ghost != null || buildUI.activeSelf))
        {
            CancelBuild();
            buildUI.SetActive(false);
            return;
        }

        // Toggle build UI
        if (Input.GetKeyDown(buildUIToggleKey))
            buildUI.SetActive(!buildUI.activeSelf);

        // Delete tool (anytime)
        if (Input.GetKeyDown(KeyCode.Delete))
            TryDeleteAtCursor();

        // If no part selected, exit
        if (currentPart == null) return;

        // Block building through UI
        if (EventSystem.current.IsPointerOverGameObject()) return;

        UpdateGhost();

        // Place on left-click
        if (Input.GetMouseButtonDown(0))
            TryPlace();

        // Cancel on right-click
        if (Input.GetMouseButtonDown(1))
            CancelBuild();
    }

    void UpdateGhost()
    {
        if (ghost == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 500f, supportMask))
        {
            TintGhost(Color.red);
            return;
        }

        var hitPart = hit.transform.GetComponent<PlacedPart>();
        var hitPartHeightOffset = hitPart == null ? 0 : hitPart.partData.height;

        Vector3 snapPos = currentPart.useGrid ? (Grid.Instance.Snap(hit.point) + Vector3.up * hitPartHeightOffset) : (hit.point + hit.normal * currentPart.surfaceOffset);

        // Move ghost
        Quaternion rot = Quaternion.Euler(0, ghostRotationY, 0);
        if (currentPart.alignToSurface)
            rot = Quaternion.LookRotation(-hit.normal);

        ghost.transform.SetPositionAndRotation(snapPos, rot);

        var ghostColor = IsPlacementValid(snapPos) ? Color.green : Color.red;
        TintGhost(ghostColor);

        // Rotate ghost
        if (Input.GetKeyDown(rotateLeftKey)) RotateGhost(-90);
        if (Input.GetKeyDown(rotateRightKey)) RotateGhost(90);
    }

    bool IsPlacementValid(Vector3 pos)
    {
        if (currentPart.useGrid && currentPart.isFloor && !Grid.Instance.IsAreaFree(pos, currentPart.footprint, ghostRotationY))
        {
            Debug.Log("Invalid placement cause area ain't free");
            return false;
        }

        if (!currentPart.useGrid) // physics overlap only for freeform parts
        {
            Bounds b = CalculateBounds(pos, ghost.transform.rotation);
            var hits = Physics.OverlapBox(b.center, b.extents * 0.95f,
                                          ghost.transform.rotation, blockMask);
            foreach (var h in hits) if (!h.isTrigger) return false;
        }

        return true; // hit exists by definition
    }

    void TryDeleteAtCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, 100f))
        {
            if (hit.transform.root.CompareTag("PlacedPart")) Destroy(hit.transform.root.gameObject);
        }
    }

    void TryPlace()
    {
        Vector3 pos = ghost.transform.position;
        Quaternion rot = ghost.transform.rotation;
        ghostRotationY = ghostRotationY % 360;

        if (!IsPlacementValid(pos)) return;

        GameObject placed = Instantiate(currentPart.prefab, pos, rot);
        placed.tag = "PlacedPart";

        if (currentPart.useGrid && currentPart.isFloor)
        {
            Grid.Instance.OccupyArea(pos, currentPart.footprint, ghostRotationY);

            var pp = placed.AddComponent<PlacedPart>();
            pp.rotY = ghostRotationY;
            pp.partData = currentPart;
        }
    }

    void RotateGhost(int delta)
    {
        ghostRotationY = (ghostRotationY + delta + 360) % 360;
    }

    void TintGhost(Color c)
    {
        foreach (var r in ghost.GetComponentsInChildren<Renderer>())
        {
            if (r.material.HasProperty(_Color))
                r.material.SetColor(_Color, c);
            else
                r.material.color = c;
        }
    }

    Bounds CalculateBounds(Vector3 pos, Quaternion rot)
    {
        float cs = Grid.Instance.cellSize;
        Vector3 size = new(
            currentPart.footprint.x * cs,
            currentPart.height,
            currentPart.footprint.y * cs);

        // if rotated 90/270 swap X/Z
        if (ghostRotationY % 180 != 0)
            (size.x, size.z) = (size.z, size.x);

        var bounds = new Bounds(Vector3.zero, size)
        {
            center = pos + Vector3.up * (size.y * 0.5f)
        };
        return bounds;
    }

    /// <summary>Selects a part and spawns its ghost.</summary>
    public void SelectPart(PartData part)
    {
        CancelBuild();
        currentPart = part;
        if (part == null) return;

        ghost = Instantiate(part.ghostPrefab != null ? part.ghostPrefab : part.prefab);
        ghostRotationY = 0;

        if (part.ghostPrefab == null)
            ApplyGhostMaterial(ghost, fallbackGhostMat);
    }

    void CancelBuild()
    {
        if (ghost != null) Destroy(ghost);
        ghost = null;
        currentPart = null;
    }

    void ApplyGhostMaterial(GameObject obj, Material mat)
    {
        foreach (var r in obj.GetComponentsInChildren<Renderer>())
            r.material = mat;
    }
}
