using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static GameManager instance;

    public Material blurMat;
    //public int enemiesKilled = 0;
    //public bool StartButtonPressed;
    //public bool IntroTutorialEnded;

    //public Vector3 startPos;

    //Game Start Stuff1
    //public GameObject gameCanvas;
    //public GameObject UICanvas;
    //public GameObject Spawners;
    //public GameObject Player;
    //public GameObject BGFollow;
    //public CinemachineVirtualCamera GameCam;
    //public GameObject[] Clouds;
    //public GameObject deathCanvas;
    //public GameObject leaderboardCanvas;
    //public bool PlayerDied;

    public GameObject pauseCanvas;
    public GameObject gameCanvas;
    public float transitionDuration = 0.005f; // Duration of the transition

    private bool isPaused = false;
    private Coroutine pauseCoroutine;

    public Volume Volume;
    private ColorAdjustments colorAdjustments;
    private FilmGrain film;
    private float currentColorAdjustmentValue;
    Transform player;  // Reference to the player
    private Vector3 offset;    // Offset from the player

    // Start is called before the first frame update
    void Start()
    {
        Volume.profile.TryGet(out colorAdjustments);

        colorAdjustments.contrast.value = currentColorAdjustmentValue;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        offset = transform.position - player.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (CameraManager.instance.isVirtualCamActive)
        {
            transform.position = player.position + offset;
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused)
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
        
    }
    private IEnumerator PauseGradually()
    {
        float startValue = Time.timeScale;
        float endValue = 0f;
        float elapsedTime = 0f;
        gameCanvas.SetActive(false);
        pauseCanvas.SetActive(true);
        //film.intensity.value = 1f;
        //MusicManager.Instance.PauseMusic(); // Transition to paused music

        while (elapsedTime < transitionDuration)
        {
            Time.timeScale = Mathf.Lerp(startValue, endValue, elapsedTime / transitionDuration);
            elapsedTime += Time.unscaledDeltaTime;

            // Adjust vignette intensity gradually

            colorAdjustments.contrast.value = Mathf.Lerp(currentColorAdjustmentValue, 39f, elapsedTime / transitionDuration);
            yield return null;
        }

        blurMat.SetFloat("Blur Size", 10f);
        
        //gameCanvas.SetActive(false);
        isPaused = true;
        Time.timeScale = 0;
    }

    public void ResumeByBtn()
    {
        StartCoroutine(ResumeGradually());
    }
    private IEnumerator ResumeGradually()
    {
        pauseCanvas.SetActive(false);
        gameCanvas.SetActive(true);
        //gameCanvas.SetActive(true);
        //film.intensity.value = 0f;
        float startValue = Time.timeScale;
        float endValue = 1f;
        float elapsedTime = 0f;

        //MusicManager.Instance.ResumeMusic(); // Transition to default music

        while (elapsedTime < transitionDuration)
        {
            //Time.timeScale = Mathf.Lerp(startValue, endValue, elapsedTime / transitionDuration);
            elapsedTime += Time.unscaledDeltaTime;

            // Adjust vignette intensity gradually
            yield return null;

            colorAdjustments.contrast.value = Mathf.Lerp(39f, currentColorAdjustmentValue, elapsedTime / transitionDuration);
        }
        blurMat.SetFloat("Blur Size", 0f);
        isPaused = false;
        Time.timeScale = 1f;
    }
}
