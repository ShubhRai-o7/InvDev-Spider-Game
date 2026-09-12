using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class HeavyShooter : MonoBehaviour
{
    Transform player;
    public NavMeshAgent agent; // Reference to the NavMesh agent
    //public GameObject bulletPrefab; // The bullet prefab
    //public Transform firePoint; // Point where the bullet is fired from
    public float attackRange = 10f; // Distance within which the enemy stops and attacks
    public float shootingCooldown = 1f; // Time between enemy attacks
    //public float bulletSpeed = 20f; // Speed at which the bullet is fired
    public float accuracyMeter = 0.8f; // How accurate the enemy is (0 = not accurate, 1 = very accurate)
    public float secondaryWeaponCooldown = 5f; // Cooldown for the secondary weapon

    private bool canShoot = true;
    private bool canUseSecondaryWeapon = true;
    float moveSpeed;

    public ParticleSystem MachineGun;
    
    public Transform gun;

    public float Health;
    public float maxHealth;
    public HealthSystem healthSystem;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        moveSpeed = Random.Range(7, 10);
        agent.speed = moveSpeed; // Set the NavMeshAgent speed
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        RotateGunTowardsPlayer();
        
        // If the player is within follow range but outside attack range
        if (distanceToPlayer > attackRange)
        {
            FollowPlayer();
        }
        // If the player is within attack range
        else if (distanceToPlayer <= attackRange)
        {
            StopAndShoot();

        }

        // Secondary weapon usage
        if (canUseSecondaryWeapon)
        {
            UseSecondaryWeapon();
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
            
            //StartCoroutine(ShootPlayer());
        }
    }

    IEnumerator ShootPlayer()
    {
        canShoot = false;
        // Calculate accuracy deviation
        //Vector3 directionToPlayer = player.position - firePoint.position;
        //Vector3 inaccurateDirection = GetInaccurateDirection(directionToPlayer);

        // Fire a bullet
        //Shoot(inaccurateDirection);

        yield return new WaitForSeconds(shootingCooldown);
        canShoot = true;
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
        Vector3 directionToPlayer = (player.position - gun.position).normalized;
        Vector3 inaccurateDirection = GetInaccurateDirection(directionToPlayer);
        // Calculate the look rotation towards the player (only rotating on the Y axis to prevent tilting up/down)
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(inaccurateDirection.x, 0, inaccurateDirection.z));

        // Smoothly rotate the gun towards the player
        gun.rotation = Quaternion.Slerp(gun.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void UseSecondaryWeapon()
    {
        //canUseSecondaryWeapon = false;
        // Add your secondary weapon logic here (e.g., launching a missile, using an AoE attack)
        if (!MachineGun.isPlaying) // Ensure the particle system isn't already playing
        {
            MachineGun.Play();
            Debug.Log("Using secondary weapon!");
        };
        
        //MachineGun.Stop();
        //yield return new WaitForSeconds(secondaryWeaponCooldown);
        //canUseSecondaryWeapon = true;
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize follow and attack ranges in the Scene view
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
