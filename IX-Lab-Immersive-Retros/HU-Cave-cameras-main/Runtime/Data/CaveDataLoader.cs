using System;
using System.IO;
using UnityEngine;

public static class CaveDataLoader
{
    public static CaveData LoadCaveData(CaveData defaultData)
    {
        string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string dataPath = Path.Combine(path, "CaveCameras", "caveData.json");

        if (File.Exists(dataPath))
            JsonUtility.FromJsonOverwrite(File.ReadAllText(dataPath), defaultData);

        Debug.Log(dataPath);

        return defaultData;
    }

    public static void SaveCaveData(CaveData data)
    {
        string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string dataPath = Path.Combine(path, "CaveCameras", "caveData.json");

        Debug.Log(dataPath);

        if (!Directory.Exists(Path.Combine(path, "CaveCameras")))
            Directory.CreateDirectory(Path.Combine(path, "CaveCameras"));
            
        File.WriteAllText(dataPath, JsonUtility.ToJson(data));
    }
}
