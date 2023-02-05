using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private GameManager gameManager;
    private float runSpeed = 10f;
    private float sprintSpeed = 20f;
    private float walkSpeed = 5f;
    private float turnSpeed = 200f;
    public bool isHidden;

    private float horizontalInput;
    private float verticalInput;
    private float zRange;
    private float xRange;
    private float sprintFatigueRate = 5.0f; // higher = slower fatigue
    private float runFatigueRate = 15.0f; //higher = slower fatigue
    private float rechargeRate = 5.0f; //higher = slower recharge
    private float hungerRate = 15.0f; //higher = slower hunger
    private bool isFatigued;
    private float fatigueCooldownTime = 3.0f;

    private PredatorTarget predatorTarget;
    private Animator playerAnim;
    private SpawnManager spawnManager;
    private ParticleSystem runningParticles;
    private AudioSource playerAudioSource;
    public AudioClip foodAudio;
    public AudioClip hidingAudio;
    public AudioClip fatigueAudio;
    public AudioClip fallingAudio;

    // Start is called before the first frame update
    void Start()
    {
        predatorTarget = GameObject.Find("PredatorTarget").GetComponent<PredatorTarget>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        zRange = gameManager.GetZRange();
        xRange= gameManager.GetXRange();
        isFatigued = false;
        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        isHidden= false;
        playerAnim= GetComponentInChildren<Animator>();
        runningParticles =  GetComponentInChildren<ParticleSystem>();
        playerAudioSource = GetComponentInChildren<AudioSource>();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if(gameManager.isGameActive) {
            PlayerMovementController();
            if (!gameManager.infiniteHealth) { PlayerHunger(); }
        }
        else { 
            playerAnim.SetBool("Run Forward", false);
            playerAnim.SetBool("Walk Forward", false);
               }
        

        
    }

    /*
     * Logic for player movement
     * Arrow keys moves player
     * LShift makes player sprint, expending Stamina
     * Player cannot go beyond map boundaries
     */
    private void PlayerMovementController()
    {
        // get arrow key inputs
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        float speed = walkSpeed;
        playerAnim.SetFloat("Turn", horizontalInput);

        // if player is trying to move
        if(horizontalInput != 0 || verticalInput != 0)
        {
            // player is sprinting
            if (Input.GetKey(KeyCode.LeftShift) && !isFatigued)
            {
                if (!runningParticles.isPlaying) { runningParticles.Play(); }
                
                speed = sprintSpeed;
                playerAnim.SetBool("Walk Forward", false);
                playerAnim.SetBool("Run Forward", true);
                if (!gameManager.infiniteStamina) { gameManager.staminaSlider.value -= Time.deltaTime / sprintFatigueRate; }
            }
            // player is running
            else if (!isFatigued)
            {
                speed = runSpeed;
                playerAnim.SetBool("Walk Forward", false);
                playerAnim.SetBool("Run Forward", true);
                if (!gameManager.infiniteStamina) { gameManager.staminaSlider.value -= Time.deltaTime / runFatigueRate; }
            }
            // player is fatigued and must walk
            else
            {
                playerAnim.SetBool("Run Forward", false);
                playerAnim.SetBool("Walk Forward", true);
                speed = walkSpeed;
                gameManager.staminaSlider.value += Time.deltaTime / rechargeRate;
            }
        }
        // if player is trying to stand still, recharge stamina
        else
        {
            playerAnim.SetBool("Walk Forward", false);
            playerAnim.SetBool("Run Forward", false);
            gameManager.staminaSlider.value += Time.deltaTime / rechargeRate;

        }

        // if stamina runs all the way to 0 then there is a waiting period
        if (gameManager.staminaSlider.value == 0)
        {
            isFatigued = true;
            playerAudioSource.clip = fatigueAudio;
            playerAudioSource.Play();
            StartCoroutine(FatigueRecharge());

        }

        MovePlayer(speed, horizontalInput, verticalInput);
        PlayerBoundaryCheck();
    }

    private void MovePlayer(float speed, float horizontalInput, float verticalInput)
    {
        //Move the player forward
        transform.Translate(Vector3.forward * Time.deltaTime * speed * verticalInput);
        // turn the player
        transform.Rotate(Vector3.up, Time.deltaTime * turnSpeed * horizontalInput);
    }

    private void PlayerBoundaryCheck()
    {
        // player is prevented from leaving boundaries of game board
        if (transform.position.z > zRange) { transform.position = new Vector3(transform.position.x, transform.position.y, zRange); }
        else if (transform.position.z < -zRange) { transform.position = new Vector3(transform.position.x, transform.position.y, -zRange); }
        if (transform.position.x > xRange) { transform.position = new Vector3(xRange, transform.position.y, transform.position.z); }
        else if (transform.position.x < -xRange) { transform.position = new Vector3(-xRange, transform.position.y, transform.position.z); }
    }

    private void OnTriggerEnter(Collider other)
    {
        /*
         * Refill health based on food type (small vs large)
         * destroy food object
         * spawn new food object
         */
        if (other.CompareTag("Food"))
        {
            FoodType currentFoodType = other.GetComponent<Food>().foodType;
            if(currentFoodType == FoodType.Large)
            {
                gameManager.foodSlider.value = 1;
            }
            else if(currentFoodType == FoodType.Small)
            {
                gameManager.foodSlider.value += .5f;
            }
            playerAudioSource.clip = foodAudio;
            playerAudioSource.Play();
            Destroy(other.gameObject);
            
            spawnManager.SpawnFood();

        }
        else if (other.CompareTag("Predator"))
        {
            playerAnim.SetBool("Death", true);
            playerAudioSource.clip = fallingAudio;
            playerAudioSource.Play();
            gameManager.GameOver("Predator");
        }
        else if(other.CompareTag("Hiding Spot")){
            isHidden= true;
            predatorTarget.SetPredatorTarget();
            playerAudioSource.clip = hidingAudio;
            playerAudioSource.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("OnTriggerExit: " + other);
        if (other.CompareTag("Hiding Spot"))
        {
            isHidden = false;
        }
    }

    IEnumerator FatigueRecharge()
    {
        yield return new WaitForSeconds(fatigueCooldownTime);
        isFatigued= false;
    }

    private void PlayerHunger()
    {
        gameManager.foodSlider.value -= Time.deltaTime / hungerRate;
        if(gameManager.foodSlider.value == 0)
        {
            playerAnim.SetBool("Death", true);
            gameManager.GameOver("Starvation");
        }
    }

}
