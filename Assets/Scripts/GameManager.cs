using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using Unity.VisualScripting;


public class GameManager : MonoBehaviour
{
    // boundaries of the game board
    private float zRange = 13.7f;
    private float xRange = 23.0f;
    private float score = 0;

    private SpawnManager spawnManager;

    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI gameOverReasonText;
    public TextMeshProUGUI titleText;
    public bool isGameActive;
    public Button restartButton;
    public Button startButton;
    public Slider staminaSlider;
    public Slider foodSlider;
    public TextMeshProUGUI scoreText;

    //Debug mode features
    public bool infiniteHealth;
    public bool infiniteStamina;
    public bool predNoSpawn;
    public bool predNoMove;


    // Start is called before the first frame update
    void Start()
    {
        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isGameActive)
        {
            score += Time.deltaTime;
            scoreText.text = "Score: " + System.Math.Round(score,2) + " seconds";

        }
    }

    public float GetXRange()
    {
        return xRange;
    }

    public float GetZRange() { return zRange; }

    public void GameOver(string reason)
    {
        if(reason == "Predator")
        {
            gameOverReasonText.text = "You've been caught by a predator!";
        }
        else if(reason == "Starvation")
        {
            gameOverReasonText.text = "You've died of starvation!";
        }
        gameOverText.gameObject.SetActive(true);
        gameOverReasonText.gameObject.SetActive(true);
        isGameActive = false;
        restartButton.gameObject.SetActive(true);
        //Debug.Log("Score = " + score);

    }

   /*
    * used both to start and restart a game by clearing the game board and spawning a player, predator, hiding spot and food
    */
    public void ResetGameplay()
    {
        // destroy player, predator, food, and hiding spot game objects
        GameObject[] allGameObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject gO in allGameObjects)
        {
            if (gO.tag == "Player" || gO.tag == "Predator" || gO.tag == "Food" || gO.tag == "Hiding Spot") { Destroy(gO); }
        }
        gameOverText.gameObject.SetActive(false);
        gameOverReasonText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        titleText.gameObject.SetActive(false);
        startButton.gameObject.SetActive(false);

        isGameActive = true;
        score = 0;
        staminaSlider.value = 1;
        foodSlider.value = 1;
        
        spawnManager.SpawnInitial();
    }

}
