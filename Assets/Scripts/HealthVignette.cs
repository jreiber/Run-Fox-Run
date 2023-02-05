using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
public class HealthVignette : MonoBehaviour
{
    private GameManager gameManager;
    PostProcessVolume m_Volume;
    Vignette m_Vignette;
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        // Create an instance of a vignette
        m_Vignette = ScriptableObject.CreateInstance<Vignette>();
        m_Vignette.enabled.Override(true);
        m_Vignette.intensity.Override(1f);
        m_Vignette.roundness.Override(1f);
        m_Vignette.smoothness.Override(1f);
        m_Vignette.color.Override(Color.red);
        // Use the QuickVolume method to create a volume with a priority of 100, and assign the vignette to this volume
        m_Volume = PostProcessManager.instance.QuickVolume(gameObject.layer, 100f, m_Vignette);
    }
    void Update()
        {
        // Change vignette intensity using a sinus curve
        if (gameManager.foodSlider.value < .5f) { m_Vignette.intensity.value = .5f - gameManager.foodSlider.value; }
        else { m_Vignette.intensity.value = 0f; }
        }
    void OnDestroy()
        {
            RuntimeUtilities.DestroyVolume(m_Volume, true, true);
        }
   }