using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaCoil : MonoBehaviour, IMyCommonFunction
{
    public Vector3 shootpoint;
    public float maxAimAssistDistance = 10f; // Maximum distance for the aim assist
    public LayerMask enemyLayer; // Set a layer for enemies in Unity Inspector

    public float maxUseDuration = 5f; // Max energy
    private float currentUseDuration;
    public float rechargeRate = 1f; // Energy recharge per second
    public float depletionRate = 1f; // Energy depletion per second

    public float baseDamage = 10f; // Minimum damage
    public float maxDamage = 50f; // Maximum damage
    public float growthRate = 2f;
    public float damageIncreaseRate = 5f; // Damage increase per second
    private float currentDamage;

    private bool isRecharging = false;
    private bool notFullyDepleted;
    private Electric electric; // Reference to Electric.cs

    private float timeShooting;

    public float minWidth = 0.2f; // Minimum width at base damage
    public float maxWidth = 1f;   // Maximum width at max damage

    public Renderer renDerer;

    bool canUse;

    void Start()
    {
        // Set initial values for energy and damage
        currentUseDuration = maxUseDuration;
        currentDamage = baseDamage;
        timeShooting = 0f; // Initialize timeShooting
        // Get reference to Electric component
        electric = GetComponent<Electric>();
        notFullyDepleted = true;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("notFullyDepleted? " + notFullyDepleted);
        if (CameraManager.instance.isVirtualCamActive)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Define the XZ plane
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            float distanceToPlane;
            if (plane.Raycast(ray, out distanceToPlane))
            {
                // Get point where the ray hits the XZ plane
                Vector3 hitPoint = ray.GetPoint(distanceToPlane);

                // Set shootpoint based on mouse position but zero out Y-axis
                shootpoint = hitPoint;
                shootpoint.y = 0;

                // Find the nearest enemy in the direction of the shootpoint
                Transform nearestEnemy = FindNearestEnemy(shootpoint);

                // If we found an enemy, override the shootpoint to be the enemy's position
                if (nearestEnemy != null)
                {
                    shootpoint = nearestEnemy.position;
                }

                if (Input.GetMouseButton(0) && currentUseDuration > 0f && notFullyDepleted && canUse) // On left-click, shoot the arc
                {
                    // Deplete use duration (energy)
                    currentUseDuration -= depletionRate * Time.deltaTime;
                    // Increase the damage over time
                    timeShooting += Time.deltaTime;
                    currentDamage = baseDamage * Mathf.Pow(1 + growthRate, timeShooting);
                    currentDamage = Mathf.Clamp(currentDamage, baseDamage, maxDamage); // Ensure it doesn't exceed max damage

                    float normalizedDamage = (currentDamage - baseDamage) / (maxDamage - baseDamage);
                    electric.SetLineWidth(Mathf.Lerp(minWidth, maxWidth, normalizedDamage));

                    // Call ShootArc with the updated damage
                    
                    electric.ShootArc(currentDamage);
                    if (nearestEnemy.GetComponent<SuicideBomber>())
                        nearestEnemy.GetComponent<SuicideBomber>().TakeDamage(currentDamage);
                    else if (nearestEnemy.GetComponent<HeavyShooter>())
                        nearestEnemy.GetComponent<HeavyShooter>().TakeDamage(currentDamage);


                    isRecharging = false; // Stop recharging while using

                }
                else if ((Input.GetMouseButton(0) || Input.GetMouseButtonUp(0)) && currentUseDuration <= 0f)
                {
                    timeShooting = 0f;
                    //currentUseDuration = 0f;
                    isRecharging = true;
                    electric.DisableArc();
                    electric.SetLineWidth(minWidth); // Reset line width
                    currentDamage = baseDamage;
                    notFullyDepleted = false;
                }
                else if (Input.GetMouseButtonUp(0) && currentUseDuration <= maxUseDuration && currentUseDuration > 0f && notFullyDepleted)
                {
                    timeShooting = 0f;
                    // Disable the arc and reset the damage when the player releases LMB
                    electric.DisableArc();
                    currentDamage = baseDamage;
                    electric.SetLineWidth(minWidth); // Reset line width
                    isRecharging = true;
                    notFullyDepleted = true;
                }
                else
                {
                    electric.DisableArc();
                }
                if (isRecharging)
                {
                    currentUseDuration += rechargeRate * Time.deltaTime;
                    if (currentUseDuration >= maxUseDuration && notFullyDepleted)
                    {
                        currentUseDuration = maxUseDuration;
                        isRecharging = false;
                    }
                    if (currentUseDuration >= maxUseDuration && !notFullyDepleted)
                    {
                        currentUseDuration = maxUseDuration;
                        isRecharging = false;
                        notFullyDepleted = true;
                    }
                }
            }
        }
    }

    // Function to find the nearest enemy in the direction of the shootpoint
    Transform FindNearestEnemy(Vector3 shootDirection)
    {
        // Get all colliders (enemies) within a certain range
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, maxAimAssistDistance, enemyLayer);

        Transform nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Collider enemy in enemiesInRange)
        {
            Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;
            float dotProduct = Vector3.Dot(directionToEnemy, (shootDirection - transform.position).normalized);

            // Ensure the enemy is generally in the direction of the shootpoint
            if (dotProduct > 0.8f) // Adjust threshold for how strict the direction check should be
            {
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                if (distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = enemy.transform;
                }
            }
        }

        return nearestEnemy;
    }
    public void EnableWeapon()
    {
        canUse = true;
        renDerer.enabled = true;
    }
    public void DisableWeapon()
    {
        canUse = false;
        renDerer.enabled = false;
    }
}
