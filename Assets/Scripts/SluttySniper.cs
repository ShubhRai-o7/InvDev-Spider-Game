using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;

public class SluttySniper : MonoBehaviour
{
    Transform player;
    public NavMeshAgent agent; // Reference to the NavMesh agent
    //public GameObject bulletPrefab; // The bullet prefab
    //public Transform firePoint; // Point where the bullet is fired from
    public float attackRange = 10f; // Distance within which the enemy stops and attacks
    public float runAwayRange = 5f;
    //public float bulletSpeed = 20f; // Speed at which the bullet is fired
    public float accuracyMeter = 0.8f; // How accurate the enemy is (0 = not accurate, 1 = very accurate)
    private bool canShoot = true;
    float moveSpeed;
    public ParticleSystem Sniper;
    public Transform sniper;
    public float Health;
    public float maxHealth;
    public HealthSystem healthSystem;

    public LayerMask obstacleMask; // Mask to define what objects are considered obstacles
    public LayerMask playerMask; // Mask to define what is considered the player

    public float followRange = 1500f;
    private bool hasClearLineOfSight = true;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        moveSpeed = 20;
        agent.speed = moveSpeed; // Set the NavMeshAgent speed
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        RotateGunTowardsPlayer();
        hasClearLineOfSight = CheckLineOfSight();
        if (hasClearLineOfSight)
        {
            // Run away if the player is too close and visible
            if (distanceToPlayer <= runAwayRange)
            {
                RunAwayFromPlayer();
                ShootPlayer();
            }
            // If the player is within follow range but outside attack range
            else if (distanceToPlayer <= followRange && distanceToPlayer > attackRange)
            {
                FollowPlayer();
            }
            // If the player is within attack range and visible
            else if (distanceToPlayer <= attackRange)
            {
                StopAndShoot();
            }
        }
        else
        {
            // If player is hidden behind an obstacle, find a new shooting position
            MoveAroundObstacle();
        }
    }

    void FollowPlayer()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    void StopAndShoot()
    {
        agent.isStopped = true; // Stop moving
        if (canShoot)
        {

            ShootPlayer();
        }
    }

    void ShootPlayer()
    {
        if (!Sniper.isPlaying) // Ensure the particle system isn't already playing
        {
            Sniper.Play();
        };
    }
    void RunAwayFromPlayer()
    {
        if (hasClearLineOfSight)
        {
            Vector3 runDirection = (transform.position - player.position).normalized;
            Vector3 newRunPosition = transform.position + runDirection * runAwayRange;

            agent.isStopped = false;
            agent.SetDestination(newRunPosition);
        }
    }
    Vector3 GetInaccurateDirection(Vector3 directionToPlayer)
    {
        // Create random deviation based on the accuracy meter
        float inaccuracy = 1f - accuracyMeter;

        // Introduce random inaccuracy on the X and Y axes
        float deviationX = Random.Range(-inaccuracy, inaccuracy);
        float deviationY = Random.Range(-inaccuracy, inaccuracy);
        Vector3 deviatedDirection = directionToPlayer + new Vector3(deviationX, deviationY, 0f);

        // Normalize the direction to ensure it has proper magnitude
        return deviatedDirection.normalized;
    }
    bool CheckLineOfSight()
    {
        Vector3 directionToPlayer = player.position - sniper.position;
        Ray ray = new Ray(sniper.position, directionToPlayer);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, followRange, obstacleMask | playerMask))
        {
            // If the ray hits the player, the player is visible
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
            else
            {
                // Ray hit an obstacle
                return false;
            }
        }

        return false; // Default to no line of sight
    }

    void MoveAroundObstacle()
    {
        if (!hasClearLineOfSight)
        {
            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(player.position, path);

            if (path.status == NavMeshPathStatus.PathComplete)
            {
                agent.SetDestination(path.corners[path.corners.Length - 1]);
            }
            else
            {
                // Fallback to simple follow behavior if no valid path
                FollowPlayer();
            }
        }
    }
    public void TakeDamage(float damageAmount)
    {
        Health -= damageAmount;
        healthSystem.UpdateHealthBar(Health, maxHealth);
        {
            if (Health <= 0)
            {
                Destroy(gameObject.GetComponentInParent<Transform>().gameObject);
            }
        }
    }
    void RotateGunTowardsPlayer()
    {
        // Find the direction from the gun to the player
        Vector3 directionToPlayer = (player.position - sniper.position).normalized;
        Vector3 inaccurateDirection = GetInaccurateDirection(directionToPlayer);
        // Calculate the look rotation towards the player (only rotating on the Y axis to prevent tilting up/down)
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(inaccurateDirection.x, 0, inaccurateDirection.z));

        // Smoothly rotate the gun towards the player
        sniper.rotation = Quaternion.Slerp(sniper.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize follow and attack ranges in the Scene view
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, runAwayRange);
    }
}
