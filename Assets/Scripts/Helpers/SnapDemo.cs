using UnityEngine;

public class SnapDemo : MonoBehaviour
{
    void Update()
    {
        transform.position = Grid.Instance.Snap(transform.position);
    }
}
