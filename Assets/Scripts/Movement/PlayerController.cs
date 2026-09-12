using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    // Player movement script



    public static PlayerController instance;
    public float MoveSpeed = 3.8f;
    public float RotSpeed = 80.0f;

    public float DashSpeed = 25f; // Speed when dashing
    public float DashDuration = 0.2f; // How long the dash lasts
    //public float DashCooldown = 1.0f; // Cooldown between dashes

    public bool isDashing = false;
    public bool canDash = true;

    public bool canMove = true;

    public float moveInput;
    public float turnInput;

    private Vector3 lastMoveDirection; // Store the last movement direction
    private NavMeshAgent navMeshAgent;

    public GameObject PlayerDieNuke;

    public Volume Volume;
    private Vignette vignette;
    private FilmGrain film;

    public CinemachineVirtualCamera mainCam;
    CinemachineBasicMultiChannelPerlin perlinNoise;

    public float transitionTime;

    public float shakeIntensity;
    public float shakeTime;

    public bool playerDying;

    private void Awake()
    {
        Volume.profile.TryGet(out film);
        Volume.profile.TryGet(out vignette);
        instance = this;
        navMeshAgent = GetComponent<NavMeshAgent>(); // Get the NavMeshAgent component
        navMeshAgent.speed = MoveSpeed; // Set the agent's speed to match movement speed

        perlinNoise = mainCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
    void Update()
    {
        navMeshAgent.isStopped = true;
        if (CameraManager.instance.isVirtualCamActive && canMove)
        {
            if (Input.GetKey(KeyCode.LeftShift)) MoveSpeed = 18f; else MoveSpeed = 18f;
            moveInput = Input.GetAxisRaw("Vertical");
            turnInput = Input.GetAxisRaw("Horizontal");
            Vector3 moveDirection = transform.forward * moveInput;

            if (moveInput != 0)
            {
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(transform.position + moveDirection); // Move the player
                lastMoveDirection = transform.forward; // Store the last move direction
            }
            float turn = turnInput * RotSpeed * Time.deltaTime;
            transform.Rotate(0, turn, 0);

            if (Input.GetKeyDown(KeyCode.Space) && moveInput != 0 && canDash && SlowMotion.instance.isSlowed)
            {
                StartCoroutine(Dash());
            }

        }      
    }

    public IEnumerator PlayerDie()
    {
        foreach (GameObject enemy in EnemyManager.instance.activeEnemies)
        {
            Destroy(enemy.GetComponentInParent<Transform>().gameObject);
        }
        playerDying = true;
        canDash = false;
        canMove = false;
        CameraManager.instance.SwitchToDeathCam();

        //mainCam.m_Lens.OrthographicSize = 80f;
        vignette.intensity.value = .28f;
        film.intensity.value = 1f;
        Instantiate(PlayerDieNuke, transform.position, Quaternion.identity);

        Debug.Log("code finished death");
        perlinNoise.m_AmplitudeGain = shakeIntensity;
        yield return new WaitForSeconds(shakeTime);
        perlinNoise.m_AmplitudeGain = 0f;
        // Ensure the final size is exactly the target size

    }
    public IEnumerator Dash()
    {
        Debug.Log("Code reached dash algorithm");
        isDashing = true;
        //PlayerLeg.instance.Movable = false;
        canDash = false;
        navMeshAgent.speed = DashSpeed;
        lastMoveDirection = lastMoveDirection.normalized;
        float dashTime = DashDuration;
        while (dashTime > 0)
        {
            navMeshAgent.SetDestination(transform.position + lastMoveDirection * DashSpeed * Time.unscaledDeltaTime);
            dashTime -= Time.deltaTime;
            yield return null;
        }
        Debug.Log("Code finished dash");
        navMeshAgent.speed = MoveSpeed;
        //PlayerLeg.instance.Movable = true;
        canDash = true;
        isDashing = false;
        SlowMotion.instance.StopMyCoroutine();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (isDashing)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                if (collision.gameObject.GetComponent<SuicideBomber>())
                {
                    collision.gameObject.GetComponent<SuicideBomber>().Explode();
                }
                
                else if (collision.gameObject.GetComponent<HeavyShooter>())
                {
                    collision.gameObject.GetComponent<HeavyShooter>().TakeDamage(100f);
                }

                else if (collision.gameObject.GetComponent<SluttySniper>())
                {
                    collision.gameObject.GetComponent<SluttySniper>().TakeDamage(100f);
                }
            }
        }
    }
}

