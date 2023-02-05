using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PredatorController : MonoBehaviour
{
    private GameManager gameManager;
    public float defaultSpeed;
    private float speed;
    private GameObject player;
    private Animator predatorAnim;
    private GameObject predatorTarget;


    // Start is called before the first frame update
    void Start()
    {
        predatorTarget = GameObject.Find("PredatorTarget");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        predatorAnim = GetComponentInChildren<Animator>();
        player = GameObject.FindWithTag("Player");
        predatorAnim.SetBool("Walk Forward", true);
        speed = defaultSpeed;
    }

    // Update is called once per frame
    void Update()
    {

            if (gameManager.isGameActive && !gameManager.predNoMove) { PredatorMovementController(); }
            else { predatorAnim.SetBool("Walk Forward", false); }

    }


    /*
     * Logic for predator movement
     * if player is not hidden then move toward player
     * if player is hidden then wander in random direction for random number of seconds
     */
    void PredatorMovementController()
    {
        if (!player.GetComponent<PlayerController>().isHidden)
        {
            predatorTarget.GetComponent<PredatorTarget>().SetPredatorTarget(player);
        }
        MoveTowardTarget(predatorTarget);
    }



    void MoveTowardTarget(GameObject target)
    {
        Vector3 moveDirection = (target.transform.position - transform.position).normalized;
        transform.position += moveDirection * speed * Time.deltaTime;
        transform.LookAt(target.transform);
    }

    private void OnTriggerEnter(Collider other)
    {
       // Debug.Log("predator ontriggerenter: " + other);
        if (other.CompareTag("Target") && player.GetComponent<PlayerController>().isHidden) {
            //Debug.Log("setting new predator target location (predator class)");
            predatorTarget.GetComponent<PredatorTarget>().SetPredatorTarget(); }
    }



}
