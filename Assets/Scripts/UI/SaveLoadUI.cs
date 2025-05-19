using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadUI : MonoBehaviour
{
    [Header("Refs")]
    public GameObject savePanel;
    public TMP_InputField saveNameInput;
    public Button saveButton;
    public Button cancelSaveButton;

    public GameObject loadPanel;
    public RectTransform loadContent;     // inside ScrollView
    public Button loadButtonPrefab;       // simple UI Button prefab
    public Button cancelLoadButton;

    const string DirWorlds = "Worlds";
    string DirPath => Path.Combine(Application.persistentDataPath, DirWorlds);
    const string FileSuffix = ".json";

    void Awake()
    {
        savePanel.SetActive(false);
        loadPanel.SetActive(false);
        saveButton.onClick.AddListener(OnSaveClicked);
        cancelSaveButton.onClick.AddListener(OnCancelSaveClicked);
        cancelLoadButton.onClick.AddListener(OnCancelLoadClicked);
    }

    /* ─── Hotkeys ───────────────────────────────────────── */
    void Update()
    {
        bool ctrl = Input.GetKey(KeyCode.LeftControl) ||
                     Input.GetKey(KeyCode.RightControl);

        if (ctrl && Input.GetKeyDown(KeyCode.S))
            ToggleSaveUI();
        if (ctrl && Input.GetKeyDown(KeyCode.L))
            ToggleLoadUI();
    }

    /* ─── SAVE ──────────────────────────────────────────── */
    void ToggleSaveUI()
    {
        savePanel.SetActive(!savePanel.activeSelf);
        loadPanel.SetActive(false);
        saveNameInput.text = "";
        saveNameInput.ActivateInputField();
    }

    void OnSaveClicked()
    {
        string name = saveNameInput.text.Trim();
        if (string.IsNullOrEmpty(name)) return;

        Directory.CreateDirectory(DirPath);
        string path = Path.Combine(DirPath, name + FileSuffix);
        SaveLoadManager.Instance.SaveWorld(path);   // call into existing logic
        savePanel.SetActive(false);
    }

    /* ─── LOAD ──────────────────────────────────────────── */
    void ToggleLoadUI()
    {
        loadPanel.SetActive(!loadPanel.activeSelf);
        savePanel.SetActive(false);

        if (loadPanel.activeSelf) PopulateLoadList();
    }

    void PopulateLoadList()
    {
        // clear existing buttons
        foreach (Transform child in loadContent) Destroy(child.gameObject);

        if (!Directory.Exists(DirPath)) return;

        foreach (string file in Directory.GetFiles(DirPath, "*.json"))
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            Button b = Instantiate(loadButtonPrefab, loadContent);
            b.GetComponentInChildren<TMP_Text>().text = fileName;
            b.onClick.AddListener(() => OnLoadClicked(file));
        }
    }

    void OnLoadClicked(string path)
    {
        SaveLoadManager.Instance.LoadWorld(path);
        loadPanel.SetActive(false);
    }

    void OnCancelSaveClicked()
    {
        savePanel.SetActive(false);
        saveNameInput.text = "";
    }

    void OnCancelLoadClicked()
    {
        loadPanel.SetActive(false);
    }
}
