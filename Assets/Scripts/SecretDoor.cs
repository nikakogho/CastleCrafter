using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SecretDoor : MonoBehaviour
{
    [Header("Door Settings")]
    public Transform pivot;          // assign the child pivot object
    public float openAngle = -90f;   // degrees to swing open
    public float speed = 120f;       // degrees per second

    bool isOpen = false;
    float targetAngle = 0f;

    void Update()
    {
        // Smoothly rotate toward target angle
        Quaternion goal = Quaternion.Euler(0, targetAngle, 0);
        pivot.localRotation = Quaternion.RotateTowards(
            pivot.localRotation, goal, speed * Time.deltaTime
        );
    }

    void OnTriggerStay(Collider other)
    {
        // Only respond to the player
        if (!other.CompareTag("Player")) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            isOpen = !isOpen;
            targetAngle = isOpen ? openAngle : 0f;
        }
    }
}
