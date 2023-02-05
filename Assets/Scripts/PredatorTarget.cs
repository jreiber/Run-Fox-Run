using UnityEngine;

/*
 * creates a random target for predator when player is hiding
 * initial random location is set ontriggerenter for player
 * new random location is triggered by predator when it reaches target and player is still hiding
 */
public class PredatorTarget : MonoBehaviour
{
    private GameManager gameManager;
    private float xRange;
    private float zRange;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        // get map boundaries
        zRange = gameManager.GetZRange();
        xRange = gameManager.GetXRange();
    }

    // sets random position within predator spawn zone
    public void SetPredatorTarget()
    {
        
        Vector3 randomSpawnLocation = new Vector3(Random.Range(-xRange, xRange), transform.position.y, Random.Range(-zRange, zRange));
        transform.position = randomSpawnLocation;
        //Debug.Log("SetPredatorTarget() to " + randomSpawnLocation);
    }

    // sets position to match argument game object position, such as the player object
    public void SetPredatorTarget(GameObject target)
    {
        Vector3 newPos = target.transform.position;
        newPos.y = transform.position.y;
        transform.position = newPos;
    }
}
