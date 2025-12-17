using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private static SaveManager instance;
    public static SaveManager Instance => instance ?? CreateInstance();

    private static SaveManager CreateInstance()
    {
        GameObject go = new GameObject("SaveManager");
        instance = go.AddComponent<SaveManager>();
        DontDestroyOnLoad(go);
        return instance;
    }

    private string savePath;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            savePath = Path.Combine(Application.persistentDataPath, "savegame.json");
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame(GameData data)
    {
        try
        {
            File.WriteAllText(savePath, JsonUtility.ToJson(data, true));
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка сохранения: {e.Message}");
        }
    }

    public GameData LoadGame()
    {
        try
        {
            return File.Exists(savePath) 
                ? JsonUtility.FromJson<GameData>(File.ReadAllText(savePath)) 
                : new GameData();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка загрузки: {e.Message}");
            return new GameData();
        }
    }

    public bool SaveFileExists() => File.Exists(savePath);
}

