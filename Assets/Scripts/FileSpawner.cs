using UnityEngine;
using TMPro;

public class FileSpawner : MonoBehaviour
{
    public GameObject[] cubesToSpawn;
    public string[] spawnLabels;
    public Vector3 spawnAreaSize = new Vector3(5f, 0f, 5f);
    public GameManager gameManager;

    GameObject currentInstance;
    int spawnIndex;

    void Start()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        spawnIndex = 0;
        TrySpawn();
    }

    void Update()
    {
        if (currentInstance == null)
            TrySpawn();
    }

    void TrySpawn()
    {
        if (spawnIndex >= cubesToSpawn.Length) return;
        var prefab = cubesToSpawn[spawnIndex];
        if (prefab == null) return;
        if (gameManager != null && spawnIndex >= gameManager.totalCubes) return;

        Vector3 half = spawnAreaSize * 0.5f;
        Vector3 localOffset = new Vector3(
            Random.Range(-half.x, half.x),
            Random.Range(0f, spawnAreaSize.y),
            Random.Range(-half.z, half.z)
        );

        Vector3 spawnPosition = transform.position + transform.TransformVector(localOffset);
        currentInstance = Instantiate(prefab, spawnPosition, Quaternion.identity);

        var fileComp = currentInstance.GetComponent<SpawnedFile>();
        if (fileComp != null)
        {
            fileComp.index = spawnIndex;
            if (spawnLabels != null && spawnLabels.Length > spawnIndex)
                fileComp.label = spawnLabels[spawnIndex];
            else
                fileComp.label = "Item " + (spawnIndex + 1);
        }

        var tm = currentInstance.GetComponentInChildren<TMP_Text>();
        if (tm != null)
            tm.text = (fileComp != null && !string.IsNullOrEmpty(fileComp.label)) ? fileComp.label : ("Item " + (spawnIndex + 1));

        spawnIndex++;
    }
}
