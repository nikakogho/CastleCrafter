using UnityEngine;

public class BuildHotkeys : MonoBehaviour
{
    public BuildUIButton[] buttons;

    void Update()
    {
        for (int i = 0; i < buttons.Length && i < 9; i++)
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                buttons[i].GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
    }
}
