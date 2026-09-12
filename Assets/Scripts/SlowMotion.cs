using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;

public class SlowMotion : MonoBehaviour
{
    public float detectionRadius = 5f;   // Radius to detect incoming projectiles
    public float particleDetectionRadius = 20f;
    public LayerMask projectileLayer;    // Assign the layer for the projectiles
    public float slowMotionFactor = 0.2f; // Time scale for slow-motion
    public float slowMotionDuration = 2f; // Duration of the slow-motion effect
    //public float cooldownDuration = 5f;   // Cooldown duration after slow motion ends

    public bool isSlowed = false;
    //private bool isOnCooldown = false; // Track cooldown status
    public bool canSlowMo = true; // To manage cooldown

    public Volume PostVolume;
    private ChromaticAberration aberration;
    private Vignette vignette;
    private Bloom bloom;

    private float currentVignetteValue;
    private float currentAberrationValue;
    private float currentBloomValue;

    public ParticleSystem[] particlesToDetect;

    public static SlowMotion instance;

    private Coroutine activeCoroutine; // To keep track of the running coroutine
    private bool isCoroutineRunning = false; // Flag to track the coroutine state

    public Slider energyBar;
    public float maxEnergy = 100f;
    [HideInInspector] public float currentEnergy;
    public float depletionRate = 10f;

    public TextMeshProUGUI info1;
    public TextMeshProUGUI info2;
    //public Image info3;
    //public Image info4;

    public Animator energyDecrease;
    private void Start()
    {
        PostVolume.profile.TryGet(out aberration);
        PostVolume.profile.TryGet(out vignette);
        PostVolume.profile.TryGet(out bloom);
        currentBloomValue = bloom.intensity.value;
        currentVignetteValue = vignette.intensity.value;
        currentAberrationValue = aberration.intensity.value;

        instance = this;

        currentEnergy = maxEnergy; // Set the energy to full at the start
        energyBar.maxValue = maxEnergy;
        energyBar.value = currentEnergy;

        Color textT = info1.color;
        textT.a = 0.33f;
        info1.color = textT;

        Color textS = info2.color;
        textS.a = 0.33f;
        info2.color = textS;
    }

    void Update()
    {
        DetectProjectiles();
        DetectEnemyParticles();
        UpdateEnergyBar();
        if (Input.GetKeyDown(KeyCode.E) && !PlayerController.instance.isDashing)
        {
            StopMyCoroutine(); // Stop or cancel the coroutine
        }
        if (PlayerController.instance.playerDying)
            StopMyCoroutine();
    }

    void DetectProjectiles()
    {
        // Check for any projectiles within the detection radius
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, projectileLayer);

        // If a projectile is detected and slow motion isn't already active or on cooldown
        if (hits.Length > 0 && !isSlowed && currentEnergy > 0 && Shield.instance._renderer.material.GetFloat("_Disolve") > 0.4f)
        {
            // Enter slow motion
            activeCoroutine = StartCoroutine(DoSlowMotion());
            Debug.Log("current coroutine" + activeCoroutine);
        }

        
    }
    void DetectEnemyParticles()
    {
        foreach (var particleSystem in particlesToDetect)
        {
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
            int particleCount = particleSystem.GetParticles(particles);

            for (int i = 0; i < particleCount; i++)
            {
                if (Vector3.Distance(transform.position, particles[i].position) <= particleDetectionRadius && !isSlowed  && currentEnergy > 0 && Shield.instance._renderer.material.GetFloat("_Disolve") > 0.4f)
                {
                    //activeCoroutine = StartCoroutine(DoSlowMotion());
                    SlowDownParticles(particleSystem);
                }
            }
        }
    }
    void SlowDownParticles(ParticleSystem particleSystem)
    {
        // Slow down the particle system by adjusting the simulation speed
        var main = particleSystem.main;
        main.simulationSpeed = 0.1f; // Slow down the particle system
    }
    public IEnumerator DoSlowMotion()
    {
        while(currentEnergy > 0)
        {
            

            Color textT = info1.color;
            textT.a = 1f;
            info1.color = textT;

            Color textS = info2.color;
            textS.a = 1f;
            info2.color = textS;

            isCoroutineRunning = true;
            isSlowed = true;
            canSlowMo = false; // Disable slow motion until cooldown
            energyDecrease.SetBool("EnergyDeplete", true);
            Time.timeScale = slowMotionFactor;
            Time.fixedDeltaTime = Time.timeScale * 0.02f; // Adjust physics calculations

            vignette.intensity.value = .5f;
            if (!isCoroutineRunning) // Check if the coroutine was cancelled
            {
                Debug.Log("Slow Mo canceled");
                StopMyCoroutine();
                yield break; // Exit the coroutine early
            }
            currentEnergy -= depletionRate * Time.unscaledDeltaTime;
            currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
            UpdateEnergyBar();

            if (currentEnergy <= 0)
            {
                Debug.Log("Energy depleted, stopping coroutine automatically");
                StopMyCoroutine(); // Stop the coroutine when energy runs out
                yield break;
            }

            yield return null; // Wait for next frame
        }
        
        // Wait for the cooldown duration before allowing slow motion to be used again
        //yield return new WaitForSecondsRealtime(cooldownDuration);

    }
    public void UpdateEnergyBar()
    {
        energyBar.value = currentEnergy;
    }
    public void StopMyCoroutine()
    {
        isCoroutineRunning = false; // Set the flag to false to exit the coroutine
        Color textT = info1.color;
        textT.a = 0.33f;
        info1.color = textT;

        Color textS = info2.color;
        textS.a = 0.33f;
        info2.color = textS;

        energyDecrease.SetBool("EnergyDeplete", false);
        vignette.intensity.value = currentVignetteValue;
        bloom.intensity.value = currentBloomValue;
        aberration.intensity.value = currentAberrationValue;

        foreach (var particleSystem in particlesToDetect)
        {
            ResetParticleSpeed(particleSystem);
        }

        isSlowed = false;
        canSlowMo = true;
        StopCoroutine(activeCoroutine); // Optional: Stop it immediately if needed
        activeCoroutine = null;
        Debug.Log("Coroutine stopped successfully");
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f; // Reset physics calculations
    }
    void ResetParticleSpeed(ParticleSystem particleSystem)
    {
        // Reset the particle system's simulation speed to normal (1)
        var main = particleSystem.main;
        main.simulationSpeed = 1f;
    }
    // To visualize the detection radius in the Unity editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, particleDetectionRadius);
    }
}
