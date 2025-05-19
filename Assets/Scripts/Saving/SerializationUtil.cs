using System.IO;
using UnityEngine;

public static class SerializationUtil
{
    public static void WriteJson<T>(string path, T obj)
        => File.WriteAllText(path, JsonUtility.ToJson(obj, true));

    public static T ReadJson<T>(string path)
        => JsonUtility.FromJson<T>(File.ReadAllText(path));
}
