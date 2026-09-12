using RoguelikeGeneratorPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    public GameObject invenCanvas;
    public GameObject GameCanvas;
    //public GameObject EnvironmentObjects;
    public float transitionDuration = 0.001f; // Duration of the transition

    private Coroutine pauseCoroutine;

    public Volume Volume;
    private ChromaticAberration aberration;
    private Vignette vignette;
    private Bloom bloom;
    private ColorAdjustments colorAdjustments;
    private FilmGrain film;

    private float currentVignetteValue;
    private float currentAberrationValue;
    private float currentBloomValue;
    private float currentColorAdjustmentValue;

    public GameObject wallTiles;
    private void Start()
    {

        Volume.profile.TryGet(out aberration);
        Volume.profile.TryGet(out vignette);
        Volume.profile.TryGet(out bloom);
        Volume.profile.TryGet(out colorAdjustments);
        Volume.profile.TryGet(out film);
        currentBloomValue = bloom.intensity.value;
        currentVignetteValue = vignette.intensity.value;
        currentAberrationValue = aberration.intensity.value;
        currentColorAdjustmentValue = colorAdjustments.contrast.value;
        
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !SlowMotion.instance.isSlowed)
        {
            CameraManager.instance.SwitchCamera();

            if (CameraManager.instance.isVirtualCamActive)
            {
                if (pauseCoroutine != null) StopCoroutine(pauseCoroutine);
                pauseCoroutine = StartCoroutine(ResumeGradually());
                
            }
            else
            {
                if (pauseCoroutine != null) StopCoroutine(pauseCoroutine);
                pauseCoroutine = StartCoroutine(PauseGradually());
            }
        }
        
        
    }
    private IEnumerator PauseGradually()
    {
        float elapsedTime = 0f;

        //MusicManager.Instance.PauseMusic(); // Transition to paused music

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            aberration.intensity.value = Mathf.Lerp(currentAberrationValue, 1f, elapsedTime / transitionDuration);

            vignette.intensity.value = Mathf.Lerp(currentVignetteValue, 0.5f, elapsedTime / transitionDuration);

            bloom.intensity.value = Mathf.Lerp(currentBloomValue, 0.5f, elapsedTime / transitionDuration);

            colorAdjustments.contrast.value = Mathf.Lerp(currentColorAdjustmentValue, 60f, elapsedTime / transitionDuration);
            yield return null;
        }
        film.intensity.value = 1f;
        invenCanvas.SetActive(true);
        GameCanvas.SetActive(false);
        //EnvironmentObjects.SetActive(false);
        //gameCanvas.SetActive(false);
        aberration.intensity.value = 1f;
        vignette.intensity.value = .5f;
    }

    private IEnumerator ResumeGradually()
    {
        invenCanvas.SetActive(false);
        GameCanvas.SetActive(true);
        //EnvironmentObjects.SetActive(true);
        float elapsedTime = 0f;
        film.intensity.value = 0f;
        //MusicManager.Instance.ResumeMusic(); // Transition to default music

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            aberration.intensity.value = Mathf.Lerp(1f, currentAberrationValue, elapsedTime / transitionDuration);

            vignette.intensity.value = Mathf.Lerp(0.5f, currentVignetteValue, elapsedTime / transitionDuration);

            bloom.intensity.value = Mathf.Lerp(0.5f, currentBloomValue, elapsedTime / transitionDuration);

            colorAdjustments.contrast.value = Mathf.Lerp(60f, currentColorAdjustmentValue, elapsedTime / transitionDuration);
            yield return null;
        }
        aberration.intensity.value = currentAberrationValue;
        vignette.intensity.value = currentVignetteValue;
        colorAdjustments.contrast.value = currentColorAdjustmentValue;
    }


}
