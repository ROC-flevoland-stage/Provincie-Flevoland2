using UnityEngine;

public class FileSpawner : MonoBehaviour
{
    public GameObject GreenCubeFile;
    public GameObject RedCubeFile;
    public float spawnInterval = 2f;
    public Vector3 spawnAreaSize = new Vector3(5f, 0f, 5f);

    float timer;

    void Start()
    {
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer < spawnInterval) return;

        SpawnRandomCube();
        timer = 0f;
    }

    void SpawnRandomCube()
    {
        if (GreenCubeFile == null || RedCubeFile == null) return;

        GameObject prefabToSpawn = Random.value < 0.5f ? GreenCubeFile : RedCubeFile;

        Vector3 half = spawnAreaSize * 0.5f;
        Vector3 localOffset = new Vector3(
            Random.Range(-half.x, half.x),
            Random.Range(0f, spawnAreaSize.y),
            Random.Range(-half.z, half.z)
        );

        Vector3 spawnPosition = transform.position + transform.TransformVector(localOffset);
        Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
    }
}
