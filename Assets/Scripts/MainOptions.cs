using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.SceneManagement;

public class MainOptions : MonoBehaviour
{
    public TMP_Text spawnJitterValue;
    public TMP_Text particleRenderSizeValue;
    public TMP_Text particleRadiusValue;
    public GameObject spawnJitter;
    public GameObject particleRenderSize;
    public GameObject particleRadius;
    public Slider spawnJitterSlider;
    public Slider particleRenderSizeSlider;
    public Slider particleRadiusSlider;



    public void Awake()
    {
        spawnJitterSlider = spawnJitter.GetComponent<Slider>();
        particleRenderSizeSlider = particleRenderSize.GetComponent<Slider>();
        particleRadiusSlider = particleRadius.GetComponent<Slider>();
        spawnJitterSlider.value = SPH.spawnJitter;
        particleRenderSizeSlider.value = SPH.particleRenderSize;
        particleRadiusSlider.value = SPH.particleRadius;
    }

    public void SetSpawnJitter(float value)
    {
        SPH.spawnJitter = value;
        spawnJitterValue.text = value.ToString();
    }
    public void SetParticleRenderSize(float value)
    {
        SPH.particleRenderSize = value;
        particleRenderSizeValue.text = value.ToString();
    }
    public void SetParticleRadius(float value)
    {
        SPH.particleRadius = value;
        particleRadiusValue.text = value.ToString();
    }
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void GoToMainScene()
    {
        SceneManager.LoadScene(1);
    }
}