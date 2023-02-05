
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject predatorPrefab;
    public GameObject hidingSpotPrefab;
    public GameObject[] foodPrefabs;
    public GameObject playerPrefab;
    public SpawnZone predatorSpawnZone;
    public SpawnZone playerSpawnZone;

    private float zRange;
    private float xRange;
    private GameManager gameManager;


    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        zRange = gameManager.GetZRange();
        xRange = gameManager.GetXRange();
        
    }


    private Vector3 GenerateSpawnPosition()
    {
        float spawnPosX = Random.Range(-xRange, xRange);
        float spawnPosZ = Random.Range(-zRange, zRange);
        return new Vector3(spawnPosX, predatorPrefab.transform.position.y, spawnPosZ);
    }

    public void SpawnFood()
    {
        int rng = Random.Range(0, foodPrefabs.Length);
        Instantiate(foodPrefabs[rng], GenerateSpawnPosition(), foodPrefabs[rng].transform.rotation);
    }

    public void SpawnInitial()
    {
        Instantiate(playerPrefab, playerSpawnZone.SpawnPoint, playerPrefab.transform.rotation);
        if (!gameManager.predNoSpawn) { Instantiate(predatorPrefab, predatorSpawnZone.SpawnPoint, predatorPrefab.transform.rotation); }
        Instantiate(hidingSpotPrefab, GenerateSpawnPosition(), hidingSpotPrefab.transform.rotation);
        SpawnFood();
    }

    
}
