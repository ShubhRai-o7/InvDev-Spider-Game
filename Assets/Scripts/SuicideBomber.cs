using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.AI;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

public class SuicideBomber : MonoBehaviour
{
    public Transform player; // Reference to the player
    private float moveSpeed = 5f; // Speed at which the enemy follows the player
    public float explosionDistance = 4f; // Distance at which the enemy explodes
    public float explosionForce = 500f; // Force of the explosion
    public float explosionRadius = 5f; // Radius of the explosion
    public float upwardModifier = 1f; // How much upwards the explosion force is applied
    public LayerMask enemyLayer; // Layer to identify other enemies for chain reaction
    public LayerMask playerLayer; // Layer to identify the player for explosion force application

    public Transform[] ikTargets;
    public Transform[] ikScripts;
    public GameObject explosionEffect;

    private bool exploded = false;
    private NavMeshAgent navMeshAgent; // NavMeshAgent for pathfinding
    CinemachineImpulseSource explode;

    public float _Health;
    public float _maxHealth;
    public HealthSystem _healthSystem;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        moveSpeed = Random.Range(8, 10);
        navMeshAgent.speed = moveSpeed; // Set the NavMeshAgent speed

        explode = GetComponent<CinemachineImpulseSource>();

        //player = GameObject.FindGameObjectWithTag("Player").transform;
        EnemyManager.instance.RegisterEnemy(gameObject);
    }
    void Update()
    {
        if (exploded) return; // Don't follow or explode again after it has exploded
        navMeshAgent.destination = player.position;
        Vector3 direction = player.position - transform.position;
        direction.y = 0; // Ignore vertical movement if your game is on a flat plane

        if ((Vector3.Distance(transform.position, player.position) <= explosionDistance))
        {
            Explode();

            if (Shield.instance._shieldOn)
                Debug.Log("BLOCKED");
            else
                StartCoroutine(PlayerController.instance.PlayerDie());

        }
    }

    public void Explode()
    {
        exploded = true;

        Rigidbody rbPARENT = gameObject.GetComponentInChildren<Rigidbody>();
        rbPARENT.useGravity = true;

        for (int i = 0; i < ikTargets.Length; i++)
        {
            ikTargets[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < ikScripts.Length; i++)
        {
            ikScripts[i].gameObject.GetComponent<BomberLeg>().enabled = false;
        }
        Instantiate(explosionEffect, transform.position, Quaternion.identity);
        explode.GenerateImpulse();

        // Unregister the enemy from the manager
        EnemyManager.instance.UnregisterEnemy(gameObject);

        // Destroy the enemy GameObject after death
        Destroy(gameObject);  // or any other death logic

        foreach (Transform child in transform)
        {
            Rigidbody rb = child.gameObject.AddComponent<Rigidbody>(); // Add Rigidbody to each body part
            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardModifier, ForceMode.Impulse);
            child.parent = null;
        }
        
        Destroy(gameObject.GetComponentInParent<Transform>().gameObject, .2f);
        Destroy(_healthSystem.gameObject);
    }
    public void TakeDamage(float damageAmount)
    {
        _Health -= damageAmount;
        DynamicTextManager.CreateText(gameObject.transform.position + new Vector3(0f, 3f, 0f), "HIT", DynamicTextManager.defaultData);
        _healthSystem.UpdateHealthBar(_Health, _maxHealth);
        {
            if (_Health <= 0)
            {
                Explode();
            }
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
