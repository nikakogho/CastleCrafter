/*
 * SaveLoadManager
 * ───────────────
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

    public static SaveLoadManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void SaveWorld(string filename)
    {
        var path = PathWorld(filename);
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

        File.WriteAllText(path, json);
        Debug.Log($"[Save] {list.Count} parts → {path}");
    }

    /* ───────────────────────────────────────── LOAD ─── */

    public void LoadWorld(string filename)
    {
        var path = PathWorld(filename);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"[Load] No save file found at {path}");
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

        Debug.Log($"[Load] spawned {count} parts from save at {path}");
    }

    /* ───────────────────────────────────────── utils ─── */

    static string PathWorld(string filename)
        => Path.Combine(Application.persistentDataPath, filename);
}
