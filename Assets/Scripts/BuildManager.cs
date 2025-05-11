using UnityEngine;
using UnityEngine.EventSystems;

public class BuildManager : MonoBehaviour
{
    [SerializeField] LayerMask groundMask;
    [SerializeField] LayerMask surfaceMask;
    [SerializeField] Material fallbackGhostMat;
    [SerializeField] GameObject buildUI;
    [SerializeField] KeyCode buildUIToggleKey = KeyCode.B;

    [SerializeField] KeyCode rotateLeftKey = KeyCode.Q;
    [SerializeField] KeyCode rotateRightKey = KeyCode.E;

    [SerializeField] KeyCode RaiseLevel = KeyCode.PageUp;
    [SerializeField] KeyCode LowerLevel = KeyCode.PageDown;

    public GameObject tooltip;

    PartData currentPart;

    int currentLevel = 0;
    
    GameObject ghost;
    int ghostRotation = 0;

    public static BuildManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (ModeSwitcher.Instance.IsInFPS && (ghost || buildUI.activeSelf))
        {
            CancelBuild();
            buildUI.SetActive(false);
            return;
        }

        if (Input.GetKeyDown(buildUIToggleKey)) buildUI.SetActive(!buildUI.activeSelf);

        /* ============ Levels ============ */
        if (Input.GetKeyDown(RaiseLevel) && currentLevel < Grid.Instance.levels.Count - 1) currentLevel++;
        if (Input.GetKeyDown(LowerLevel) && currentLevel > 0) currentLevel--;

        /* ============ DELETE TOOL (works even when not building) ============ */
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            Ray deleteRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(deleteRay, out var deleteHit, 500f))
            {
                var root = deleteHit.transform.root;

                if (root.CompareTag("PlacedPart"))
                    Destroy(root.gameObject);
            }
        }
        
        /* ---------- BUILD MODE ---------- */
        if (currentPart == null) return;

        /* block if pointer over UI */
        if (EventSystem.current.IsPointerOverGameObject()) return;

        /* Ray from camera -> ground */
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var mask = currentPart.snapToSurface ? surfaceMask : groundMask;

        if (!Physics.Raycast(ray, out RaycastHit hit, 500f, mask))
            return;

        Vector3 snapPos = currentPart.snapToSurface ? hit.point + hit.normal * 0.01f : Grid.Instance.Snap(hit.point, currentLevel);
        if (ghost) ghost.transform.position = snapPos;

        /* Rotate ghost */
        if (Input.GetKeyDown(rotateLeftKey)) RotateGhost(-90);
        if (Input.GetKeyDown(rotateRightKey)) RotateGhost(90);

        /* Place */
        if (Input.GetMouseButtonDown(0)) Place(snapPos);

        /* Cancel */
        if (Input.GetMouseButtonDown(1))
        {
            CancelBuild();
        }
    }

    private void Place(Vector3 snapPosition)
    {
        Instantiate(currentPart.prefab, snapPosition, Quaternion.Euler(0, ghostRotation, 0));

        if (currentPart.raisesLevel)
        {
            float currentY = Grid.Instance.YForLevel(currentLevel);
            float newY = currentY + currentPart.raiseHeight;

            currentLevel = Grid.Instance.AddLevel(newY);
        }
    }

    private void RotateGhost(int delta)
    {
        ghostRotation = (ghostRotation + delta + 360) % 360;
        ghost.transform.rotation = Quaternion.Euler(0, ghostRotation, 0);
    }

    /* ---------- public interface ---------- */
    public void SelectPart(PartData part)
    {
        CancelBuild();
        currentPart = part;
        if (part == null) return;

        GameObject toSpawn = part.ghostPrefab ? part.ghostPrefab : part.prefab;
        ghost = Instantiate(toSpawn);
        ghostRotation = 0;
        if (!part.ghostPrefab) ApplyGhostMaterial(ghost, fallbackGhostMat);
    }

    void CancelBuild()
    {
        if (ghost) Destroy(ghost);
        ghost = null;
        currentPart = null;
    }

    void ApplyGhostMaterial(GameObject obj, Material mat)
    {
        foreach (Renderer r in obj.GetComponentsInChildren<Renderer>())
            r.material = mat;
    }
}
