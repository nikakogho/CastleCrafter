// Runtime camera controller that mimics Unity's Scene View:
//   • Hold RMB + move mouse  → look around (free-look)
//   • While RMB held:  WASD = move,  Q/E = down/up
//   • Scroll-wheel         → dolly forward/back
//   • Hold MMB + drag      → pan (truck)
//   • Shift                → 4× speed boost
//   • Alt + LMB drag       → orbit around pivot (optional)
// Drop this on the Camera or an empty parent.

using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SceneViewCameraController : MonoBehaviour
{
    [Header("Speeds")]
    public float moveSpeed = 5f;   // metres per second
    public float shiftMultiplier = 4f;   // when Shift held
    public float lookSensitivity = 3f;   // degrees per pixel
    public float scrollSpeed = 10f;  // dolly per scroll-notch
    public float panSpeed = 0.5f; // metres per pixel with MMB

    private Camera cam;

    private void Awake() => cam = GetComponent<Camera>();

    private void Update()
    {
        float dt = Time.unscaledDeltaTime;
        bool rmb = Input.GetMouseButton(1);   // right mouse button
        bool mmb = Input.GetMouseButton(2);   // middle mouse button
        bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        /* ---------- Look around (RMB) ---------- */
        if (rmb)
        {
            float yaw = Input.GetAxis("Mouse X") * lookSensitivity;
            float pitch = -Input.GetAxis("Mouse Y") * lookSensitivity;
            transform.eulerAngles += new Vector3(pitch, yaw, 0f);
        }

        /* ---------- Move (WASDQE) ---------- */
        Vector3 move = Vector3.zero;
        if (rmb)
        {
            move += transform.forward * Input.GetAxisRaw("Vertical");   // W/S
            move += transform.right * Input.GetAxisRaw("Horizontal"); // A/D
            if (Input.GetKey(KeyCode.E)) move += Vector3.up;
            if (Input.GetKey(KeyCode.Q)) move += Vector3.down;

            float speed = moveSpeed * (shift ? shiftMultiplier : 1f);
            transform.position += move.normalized * speed * dt;
        }

        /* ---------- Scroll wheel dolly ---------- */
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
            transform.position += transform.forward * scroll * scrollSpeed;

        /* ---------- Pan (MMB) ---------- */
        if (mmb && !rmb)
        {
            float panX = -Input.GetAxis("Mouse X") * panSpeed;
            float panY = -Input.GetAxis("Mouse Y") * panSpeed;
            transform.position += transform.right * panX + transform.up * panY;
        }
    }
}
