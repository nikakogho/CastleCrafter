/*
 * SaveLoadManager
 * ───────────────
 * World-level persistence for Castle Crafter.
 *  • Ctrl+S  → save to JSON
 *  • Ctrl+L  → load from JSON
 *
 * Files live in:  <persistentDataPath>/world.json
 *
 * Each save entry stores:
 *   id   – PartData.id
 *   pos  – world position (Vector3)
 *   rot  – world rotation  (Vector3 euler)
 *
 * On load we:
 *   1. Destroy every PlacedPart in the scene
 *   2. Build a lookup (id → PartData) by scanning Resources
 *   3. Instantiate each entry, add PlacedPart, occupy grid if needed
 */

using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class SaveLoadManager : MonoBehaviour
{
    public PartData[] allPartData;

    [System.Serializable]
    struct SaveEntry
    {
        public string name;
        public Vector3 pos;
        public Vector3 rot;
    }

    [System.Serializable]
    struct SaveFile
    {
        public List<SaveEntry> parts;
    }

    const string FileName = "world.json";

    /* ───────────────────────────────────────── hotkeys ─── */

    void Update()
    {
        bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        if (ctrl && Input.GetKeyDown(KeyCode.S)) SaveWorld();
        if (ctrl && Input.GetKeyDown(KeyCode.L)) LoadWorld();
    }

    /* ───────────────────────────────────────── SAVE ─── */

    void SaveWorld()
    {
        var list = new List<SaveEntry>();

        foreach (PlacedPart pp in FindObjectsOfType<PlacedPart>())
        {
            list.Add(new SaveEntry
            {
                name = pp.partData.name,
                pos = pp.transform.position,
                rot = pp.transform.eulerAngles
            });
        }

        SaveFile file = new() { parts = list };
        string json = JsonUtility.ToJson(file, true);

        File.WriteAllText(PathWorld(), json);
        Debug.Log($"[Save] {list.Count} parts → {PathWorld()}");
    }

    /* ───────────────────────────────────────── LOAD ─── */

    void LoadWorld()
    {
        string path = PathWorld();
        if (!File.Exists(path))
        {
            Debug.LogWarning("[Load] No save file found");
            return;
        }

        // 1. wipe current build
        foreach (PlacedPart pp in FindObjectsOfType<PlacedPart>())
            Destroy(pp.gameObject);

        // 2. build PartData lookup
        Dictionary<string, PartData> map = new();
        foreach (PartData pd in allPartData)
            map[pd.name] = pd;

        // 3. read file & spawn
        SaveFile file = JsonUtility.FromJson<SaveFile>(File.ReadAllText(path));
        int count = 0;

        foreach (SaveEntry e in file.parts)
        {
            if (!map.TryGetValue(e.name, out PartData pd))
            {
                Debug.LogWarning($"[Load] missing PartData name='{e.name}'"); continue;
            }

            Quaternion rot = Quaternion.Euler(e.rot);
            GameObject go = Instantiate(pd.prefab, e.pos, rot);
            go.tag = "PlacedPart";

            // add PlacedPart component
            var pp = go.AddComponent<PlacedPart>();
            pp.partData = pd;
            pp.rotY = Mathf.RoundToInt(e.rot.y) % 360;

            // occupy grid if this part reserves cells (floors)
            if (pd.useGrid && pd.isFloor)
                Grid.Instance.OccupyArea(e.pos, pd.footprint, pp.rotY);

            count++;
        }

        Debug.Log($"[Load] spawned {count} parts from save");
    }

    /* ───────────────────────────────────────── utils ─── */

    static string PathWorld()
        => Path.Combine(Application.persistentDataPath, FileName);
}
