/*
 * RtsCamera.cs  –  MIT-licensed RTS / city-builder camera controller
 * Source:  github.com/Over42/RTS-Camera  (commit 77e15…)
 * Copyright (c) Over42, MIT License
 */

using UnityEngine;

namespace RtsCam
{
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("RTS Camera")]
    public class RtsCamera : MonoBehaviour
    {
        /* ---------- MOVEMENT ---------- */
        public bool is2d = false;
        public float keyboardMovementSpeed = 20f;
        public float screenEdgeMovementSpeed = 15f;
        public float panningSpeed = 35f;
        public float followingSpeed = 25f;

        /* ---------- ZOOM ---------- */
        public float minHeight = 8f;
        public float maxHeight = 60f;
        public float heightDampening = 10f;
        public float keyboardZoomSens = 4f;
        public float wheelZoomSens = 40f;

        /* ---------- ROTATION ---------- */
        public float keyboardRotSpeed = 90f;
        public float mouseRotSpeed = 150f;

        /* ---------- LIMITS ---------- */
        public bool limitMap = true;
        public float limitX = 50f;
        public float limitY = 50f;

        /* ---------- INPUT SETTINGS ---------- */
        public bool useScreenEdge = true;
        public float screenEdgeBorder = 25f;
        public bool usePanning = true;
        public KeyCode panningKey = KeyCode.Mouse2;
        public bool useMouseRot = true;
        public KeyCode mouseRotKey = KeyCode.Mouse1;
        public bool useKeyboardRot = true;
        public KeyCode rotLeftKey = KeyCode.Z;
        public KeyCode rotRightKey = KeyCode.X;
        public bool useKeyboardZoom = true;
        public KeyCode zoomInKey = KeyCode.E;
        public KeyCode zoomOutKey = KeyCode.Q;
        public bool useScrollZoom = true;
        public string zoomAxis = "Mouse ScrollWheel";
        public string horizontalAxis = "Horizontal";
        public string verticalAxis = "Vertical";

        /* ---------- PRIVATE ---------- */
        Transform tr;
        Camera cam;
        float zoom01;          // 0-1 interpolation value

        void Start()
        {
            tr = transform;
            cam = GetComponent<Camera>();
            zoom01 = Mathf.InverseLerp(maxHeight, minHeight, tr.position.y);
        }

        void Update() => CameraUpdate();

        /* ========================= MAIN UPDATE ========================= */
        void CameraUpdate()
        {
            Move();
            Zoom();
            Rotate();
            LimitPosition();
        }

        /* ------------------------- MOVE ------------------------- */
        void Move()
        {
            Vector3 move = Vector3.zero;

            /* Keyboard */
            move += new Vector3(
                Input.GetAxisRaw(horizontalAxis),
                0,
                Input.GetAxisRaw(verticalAxis));

            /* Screen edge */
            if (useScreenEdge)
            {
                Vector2 m = Input.mousePosition;
                if (m.x < screenEdgeBorder) move.x -= 1;
                else if (m.x > Screen.width - screenEdgeBorder) move.x += 1;
                if (m.y < screenEdgeBorder) move.z -= 1;
                else if (m.y > Screen.height - screenEdgeBorder) move.z += 1;
            }

            /* Middle-mouse panning */
            if (usePanning && Input.GetKey(panningKey))
            {
                Vector2 axis = new Vector2(
                    Input.GetAxis("Mouse X"),
                    Input.GetAxis("Mouse Y"));
                move += new Vector3(-axis.x, 0, -axis.y) * panningSpeed / keyboardMovementSpeed;
            }

            if (move.sqrMagnitude < 0.01f) return;

            /* Apply speeds & orientation */
            move = Quaternion.Euler(0, tr.eulerAngles.y, 0) * move.normalized;
            move *= (move.magnitude > 0.5f ? screenEdgeMovementSpeed : keyboardMovementSpeed);
            tr.Translate(move * Time.deltaTime, Space.World);
        }

        /* ------------------------- ZOOM ------------------------- */
        void Zoom()
        {
            float zoomDir = 0;

            if (useScrollZoom) zoomDir += -Input.GetAxis(zoomAxis) * wheelZoomSens;
            if (useKeyboardZoom) zoomDir += (Input.GetKey(zoomInKey) ? -1 : Input.GetKey(zoomOutKey) ? 1 : 0)
                                           * keyboardZoomSens * Time.deltaTime;

            if (Mathf.Approximately(zoomDir, 0)) return;

            zoom01 = Mathf.Clamp01(zoom01 + zoomDir * 0.01f);
            float targetY = Mathf.Lerp(maxHeight, minHeight, zoom01);

            Vector3 pos = tr.position;
            pos.y = Mathf.Lerp(pos.y, targetY, Time.deltaTime * heightDampening);
            tr.position = pos;
        }

        /* ------------------------- ROTATE ------------------------- */
        void Rotate()
        {
            float rot = 0;

            if (useKeyboardRot)
                rot += (Input.GetKey(rotLeftKey) ? -1 : Input.GetKey(rotRightKey) ? 1 : 0)
                       * keyboardRotSpeed * Time.deltaTime;

            if (useMouseRot && Input.GetKey(mouseRotKey))
                rot += -Input.GetAxis("Mouse X") * mouseRotSpeed * Time.deltaTime;

            if (Mathf.Approximately(rot, 0)) return;

            tr.Rotate(Vector3.up, rot, Space.World);
        }

        /* ------------------------- LIMITS ------------------------- */
        void LimitPosition()
        {
            if (!limitMap) return;

            Vector3 pos = tr.position;
            pos.x = Mathf.Clamp(pos.x, -limitX, limitX);
            pos.z = Mathf.Clamp(pos.z, -limitY, limitY);
            tr.position = pos;
        }
    }
}
