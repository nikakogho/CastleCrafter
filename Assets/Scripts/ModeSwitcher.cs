using UnityEngine;

public class ModeSwitcher : MonoBehaviour
{
    [Header("Scene Objects")]
    public GameObject rtsRig;     // CameraRig root
    public GameObject rtsUI;
    public GameObject fpsPlayer;  // FirstPersonController root

    bool inFps;

    public static ModeSwitcher Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start() => SetMode(false);   // start in RTS

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            SetMode(!inFps);
    }

    void SetMode(bool toFps)
    {
        inFps = toFps;
        fpsPlayer.SetActive(inFps);
        rtsRig.SetActive(!inFps);
        rtsUI.SetActive(!inFps);

        Cursor.lockState = inFps ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !inFps;

        /* Drop FPS safely to ground under RTS camera */
        if (inFps)
        {
            var cam = rtsRig.GetComponentInChildren<Camera>();
            Ray downward = new Ray(cam.transform.position + Vector3.up * 50, Vector3.down);
            if (Physics.Raycast(downward, out var hit, 200f))
                fpsPlayer.transform.position = hit.point + Vector3.up * 1.8f;
        }
    }

    public bool IsInFPS => inFps;
}
