using UnityEngine;
using TMPro;
using System;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class OptionsMenu : MonoBehaviour
{
    public TMP_InputField boxSizeXInput;
    // public TMP_InputField boxSizeYInput;
    public TMP_InputField boxSizeZInput;
    // public TMP_InputField particleRenderSizeInput;
    // public TMP_InputField spawnJitterInput;
    public TMP_InputField particleMassInput;
    public TMP_InputField viscosityInput;
    public TMP_InputField restDensityInput;
    public TMP_InputField gasConstantInput;
    public TMP_InputField boundDampingInput;
    public TMP_InputField timeStepInput;
    public GameObject pauseButton;
    public GameObject playButton;
    public TMP_Text pauseButtonText;
    public Vector3 boxSize = new Vector3(5, 7, 5);
    // public float particleRadius = 0.2f;
    // public float particleRenderSize = 8f;
    // public float spawnJitter = 0.1f;
    public float particleMass = 1.0f;
    public float viscosity = -0.003f;
    public float restDensity = 1f;
    public float gasConstant = 2f;
    public float boundDamping = -0.5f;
    public float timeStep = 0.0008f;

    private bool GameIsPaused = false;

    // Reference to the SPHSimulation script
    public SPH sph;

    void Awake()
    {
        // Initialize UI with default values
        boxSizeXInput.text = boxSize.x.ToString();
        // boxSizeYInput.text = boxSize.y.ToString();
        boxSizeZInput.text = boxSize.z.ToString();
        // // particleRenderSizeInput.text = particleRenderSize.ToString();
        // spawnJitterInput.text = spawnJitter.ToString();
        particleMassInput.text = particleMass.ToString();
        viscosityInput.text = viscosity.ToString();
        restDensityInput.text = restDensity.ToString();
        gasConstantInput.text = gasConstant.ToString();
        boundDampingInput.text = boundDamping.ToString();
        timeStepInput.text = timeStep.ToString();
    }

    void Update()
    {

    }

    public void PausePlay()
    {
        if (GameIsPaused)
        {
            pauseButton.SetActive(true);
            playButton.SetActive(false);
            Time.timeScale = 1f;
            GameIsPaused = false;
            pauseButtonText.text = "Pause";
        }
        else
        {
            playButton.SetActive(true);
            pauseButton.SetActive(false);
            Time.timeScale = 0f;
            GameIsPaused = true;
            pauseButtonText.text = "Play";
        }
    }

    public void ApplySettings()
    {
        // Parse and apply values from UI
        boxSize.x = float.Parse(boxSizeXInput.text);
        // boxSize.y = float.Parse(boxSizeYInput.text);
        boxSize.z = float.Parse(boxSizeZInput.text);
        // // particleRenderSize = float.Parse(particleRenderSizeInput.text);
        // spawnJitter = float.Parse(spawnJitterInput.text);
        particleMass = float.Parse(particleMassInput.text);
        viscosity = float.Parse(viscosityInput.text);
        restDensity = float.Parse(restDensityInput.text);
        gasConstant = float.Parse(gasConstantInput.text);
        // boundDamping = float.Parse(boundDampingInput.text);
        timeStep = float.Parse(timeStepInput.text);

        // Assign values to the SPHSimulation script
        sph.boxSize = boxSize;
        // sph.particleRadius = particleRadius;
        // // sph.particleRenderSize = particleRenderSize;
        // sph.spawnJitter = spawnJitter;
        sph.particleMass = particleMass;
        sph.viscosity = viscosity;
        sph.restDensity = restDensity;
        sph.gasConstant = gasConstant;
        sph.boundDamping = boundDamping;
        sph.timeStep = timeStep;
    }
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}