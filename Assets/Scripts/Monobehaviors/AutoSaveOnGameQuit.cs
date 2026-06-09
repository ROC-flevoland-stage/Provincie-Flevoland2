using UnityEngine;

public class AutoSaveOnGameQuit : MonoBehaviour
{
    private static AutoSaveOnGameQuit _instance;

    public static AutoSaveOnGameQuit Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<AutoSaveOnGameQuit>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("AutoSaveOnGameQuit");
                    _instance = obj.AddComponent<AutoSaveOnGameQuit>();
                    DontDestroyOnLoad(obj);
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != this && _instance != null)
        {
            Destroy(this);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnApplicationQuit()
    {
        SaveManager.SaveDataToFile();
    }
}
