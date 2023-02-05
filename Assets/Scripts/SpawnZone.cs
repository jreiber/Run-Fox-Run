using UnityEngine;

public class SpawnZone : MonoBehaviour
{

    public Vector3 SpawnPoint
    {
        
        get
        {
            Vector3 spawnPos = Random.insideUnitSphere * 5f;
            spawnPos.y = 0f;
            spawnPos += transform.position;
            return spawnPos;
        }
    }
}
