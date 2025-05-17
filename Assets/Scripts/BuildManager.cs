using UnityEngine;
using UnityEngine.EventSystems;

public class BuildManager : MonoBehaviour
{
    [Header("Placement Masks")]
    [SerializeField] LayerMask surfaceMask;   // for surface-snap parts

    [Header("Ghost & UI")]
    [SerializeField] Material fallbackGhostMat;
    [SerializeField] GameObject buildUI;
    [SerializeField] KeyCode buildUIToggleKey = KeyCode.B;

    [Header("Controls")]
    [SerializeField] KeyCode rotateLeftKey = KeyCode.Q;
    [SerializeField] KeyCode rotateRightKey = KeyCode.E;
    [SerializeField] KeyCode raiseLevelKey = KeyCode.UpArrow;
    [SerializeField] KeyCode lowerLevelKey = KeyCode.DownArrow;

    public GameObject tooltip;

    PartData currentPart;
    GameObject ghost;
    int ghostRotation = 0;

    int currentLevel = 0;  // index into Grid.Instance.levels

    public static BuildManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        // If in FPS mode, cancel any build & hide UI
        if (ModeSwitcher.Instance.IsInFPS && (ghost != null || buildUI.activeSelf))
        {
            CancelBuild();
            buildUI.SetActive(false);
            return;
        }

        // Toggle build UI
        if (Input.GetKeyDown(buildUIToggleKey))
            buildUI.SetActive(!buildUI.activeSelf);

        // Change current level
        if (Input.GetKeyDown(raiseLevelKey) && currentLevel < Grid.Instance.levels.Count - 1)
            currentLevel++;
        if (Input.GetKeyDown(lowerLevelKey) && currentLevel > 0)
            currentLevel--;

        // Delete tool (works anytime)
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            Ray deleteRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(deleteRay, out RaycastHit deleteHit, 500f))
            {
                var root = deleteHit.transform.root;
                if (root.CompareTag("PlacedPart"))
                    Destroy(root.gameObject);
            }
        }

        // If no part selected, bail out
        if (currentPart == null) return;

        // Don’t build through UI
        if (EventSystem.current.IsPointerOverGameObject()) return;

        // Compute ghost position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 snapPos;

        if (currentPart.snapToSurface)
        {
            // Surface-snap: physics hit against real colliders
            if (!Physics.Raycast(ray, out RaycastHit hit, 500f, surfaceMask))
                return;

            snapPos = hit.point + hit.normal * 0.01f;
        }
        else
        {
            // Grid-plane: infinite horizontal plane at currentLevel Y
            Plane gridPlane = new Plane(
                Vector3.up,
                new Vector3(0f, Grid.Instance.YForLevel(currentLevel), 0f)
            );

            if (!gridPlane.Raycast(ray, out float enter))
                return;

            Vector3 planePoint = ray.GetPoint(enter);
            snapPos = Grid.Instance.Snap(planePoint, currentLevel);
        }

        // Move ghost
        if (ghost != null)
            ghost.transform.position = snapPos;

        // Rotate ghost
        if (Input.GetKeyDown(rotateLeftKey)) RotateGhost(-90);
        if (Input.GetKeyDown(rotateRightKey)) RotateGhost(90);

        // Place the part
        if (Input.GetMouseButtonDown(0))
            Place(snapPos);

        // Cancel building
        if (Input.GetMouseButtonDown(1))
            CancelBuild();
    }

    private void Place(Vector3 snapPosition)
    {
        var placed = Instantiate(
            currentPart.prefab,
            snapPosition,
            Quaternion.Euler(0, ghostRotation, 0)
        );
        placed.tag = "PlacedPart";

        // If this part raises a new floor…
        if (currentPart.raisesLevel)
        {
            float baseY = Grid.Instance.YForLevel(currentLevel);
            float newY = baseY + currentPart.raiseHeight;
            // AddLevel will dedupe & sort, and return the correct index
            int newAddedLevel = Grid.Instance.AddLevel(newY);

            if (newAddedLevel <= currentLevel) currentLevel++;
        }
    }

    private void RotateGhost(int delta)
    {
        ghostRotation = (ghostRotation + delta + 360) % 360;
        if (ghost != null)
            ghost.transform.rotation = Quaternion.Euler(0, ghostRotation, 0);
    }

    /// <summary>Selects a new part to build, spawning its ghost.</summary>
    public void SelectPart(PartData part)
    {
        CancelBuild();
        currentPart = part;
        if (part == null) return;

        GameObject toSpawn = part.ghostPrefab != null
            ? part.ghostPrefab
            : part.prefab;

        ghost = Instantiate(toSpawn);
        ghostRotation = 0;

        // If using fallback material, tint it
        if (part.ghostPrefab == null)
            ApplyGhostMaterial(ghost, fallbackGhostMat);
    }

    private void CancelBuild()
    {
        if (ghost != null) Destroy(ghost);
        ghost = null;
        currentPart = null;
    }

    private void ApplyGhostMaterial(GameObject obj, Material mat)
    {
        foreach (var r in obj.GetComponentsInChildren<Renderer>())
            r.material = mat;
    }
}
