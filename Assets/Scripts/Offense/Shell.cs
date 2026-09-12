using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    Rigidbody rb;
    bool hasHit;
    public GameObject ExplosionEffect;

    public float impactField;
    //public float force;
    public LayerMask ToHit;
    //[SerializeField] private AudioSource Impact;
    CinemachineImpulseSource explode;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        explode = GetComponent<CinemachineImpulseSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hasHit == false)
        {
            //float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            //transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided");
        hasHit = true;
        Instantiate(ExplosionEffect, transform.position, Quaternion.identity);
        explode.GenerateImpulse();
        //CameraShaker.Instance.ShakeOnce(2f, 3f, .1f, .5f);
        //Impact.Play();
        //Destroy(gameObject);

        Collider[] objects = Physics.OverlapSphere(transform.position, impactField, ToHit);

        foreach (Collider obj in objects)
        {
            // Check if the object has the BaseEnemyAI component
            SuicideBomber enemyComponent = obj.GetComponent<SuicideBomber>();

            if (enemyComponent != null)
            {
                // Apply damage to the enemy
                enemyComponent.TakeDamage(250f);
            }

            //Check if the object has the MonsterTrigger component
            //MonsterTrigger monsterTrigger = obj.GetComponent<MonsterTrigger>();

        }
        Destroy(gameObject);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, impactField);
    }
}
