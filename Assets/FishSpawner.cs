using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject fishPrefab;  // The fish prefab to spawn
    public float spawnRate = 1f;   // Fish per second
    public float spawnRangeY = 1f; // Vertical spawn range (+/-)

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 1f / spawnRate)
        {
            SpawnFish();
            timer = 0f;
        }
    }

    void SpawnFish()
    {
        // X position is the same as the spawner
        float spawnX = transform.position.x;

        // Y position randomly above or below the spawner
        float spawnY = transform.position.y + Random.Range(-spawnRangeY, spawnRangeY);

        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0f);

        Instantiate(fishPrefab, spawnPosition, Quaternion.identity);
    }
}
