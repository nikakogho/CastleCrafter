using UnityEngine;
using UnityEngine.EventSystems;

public class BuildManager : MonoBehaviour
{
    [SerializeField] LayerMask groundMask;
    [SerializeField] Material fallbackGhostMat;
    [SerializeField] GameObject buildUI;
    [SerializeField] KeyCode buildUIToggleKey = KeyCode.B;

    public GameObject tooltip;

    PartData currentPart;
    GameObject ghost;

    public static BuildManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(buildUIToggleKey)) buildUI.SetActive(!buildUI.activeSelf);

        if (currentPart == null) return;

        /* block if pointer over UI */
        if (EventSystem.current.IsPointerOverGameObject()) return;

        /* Ray from camera -> ground */
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 500f, groundMask))
            return;

        Vector3 snapPos = Grid.Instance.Snap(hit.point);
        if (ghost) ghost.transform.position = snapPos;

        /* Place */
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(currentPart.prefab, snapPos, Quaternion.identity);
        }

        /* Cancel */
        if (Input.GetMouseButtonDown(1))
        {
            CancelBuild();
        }
    }

    /* ---------- public interface ---------- */
    public void SelectPart(PartData part)
    {
        CancelBuild();
        currentPart = part;
        if (part == null) return;

        GameObject toSpawn = part.ghostPrefab ? part.ghostPrefab : part.prefab;
        ghost = Instantiate(toSpawn);
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
