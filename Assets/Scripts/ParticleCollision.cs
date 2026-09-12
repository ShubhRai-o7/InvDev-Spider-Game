using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ParticleCollision : MonoBehaviour
{
    private ParticleSystem part;
    public List<ParticleCollisionEvent> collisionEvents;
    //public CinemachineVirtualCamera cam;
    //public GameObject explosionPrefab;
    //public GameObject ExplosionEffect;

    void Start()
    {
        part = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleCollision(GameObject other)
    {

        if (gameObject.CompareTag("Punisher") && other.gameObject.CompareTag("Enemy"))
        {
            //Instantiate(ExplosionEffect, other.gameObject.transform.position, Quaternion.identity);
            if (other.gameObject.GetComponent<SuicideBomber>())
                other.gameObject.GetComponent<SuicideBomber>().TakeDamage(20f);
            else if (other.gameObject.GetComponent<HeavyShooter>())
                other.gameObject.GetComponent<HeavyShooter>().TakeDamage(20f);
            else if (other.gameObject.GetComponent<SluttySniper>())
                other.gameObject.GetComponent<SluttySniper>().TakeDamage(20f);
            Debug.Log("Enemy Hit");
            //int scoreValue = other.gameObject.name.Contains("EnemyPlaneB") ? Random.Range(300, 361) : Random.Range(100, 141);
            //ScoreManager.instance.AddScore(scoreValue);
            //GameManager.instance.EnemyKilled();

        }
        else if (gameObject.CompareTag("Shotty") && other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Enemy Hit");
            //Instantiate(ExplosionEffect, other.gameObject.transform.position, Quaternion.identity);
            if(other.gameObject.GetComponent<SuicideBomber>())
                other.gameObject.GetComponent<SuicideBomber>().TakeDamage(60f);
            else if (other.gameObject.GetComponent<HeavyShooter>())
                other.gameObject.GetComponent<HeavyShooter>().TakeDamage(60f);
            else if (other.gameObject.GetComponent<SluttySniper>())
                other.gameObject.GetComponent<SluttySniper>().TakeDamage(60f);
            //int scoreValue = other.gameObject.name.Contains("EnemyPlaneB") ? Random.Range(300, 361) : Random.Range(100, 141);
            //ScoreManager.instance.AddScore(scoreValue);
            //GameManager.instance.EnemyKilled();

        }
        else if (gameObject.CompareTag("Railgun") && other.gameObject.CompareTag("Enemy"))
        {
            //Instantiate(ExplosionEffect, other.gameObject.transform.position, Quaternion.identity);
            if (other.gameObject.GetComponent<SuicideBomber>())
                other.gameObject.GetComponent<SuicideBomber>().TakeDamage(150f);
            else if (other.gameObject.GetComponent<HeavyShooter>())
                other.gameObject.GetComponent<HeavyShooter>().TakeDamage(150f);
            else if (other.gameObject.GetComponent<SluttySniper>())
                other.gameObject.GetComponent<SluttySniper>().TakeDamage(150f);
            //int scoreValue = other.gameObject.name.Contains("EnemyPlaneB") ? Random.Range(300, 361) : Random.Range(100, 141);
            //ScoreManager.instance.AddScore(scoreValue);
            //GameManager.instance.EnemyKilled();

        }
        else if(gameObject.CompareTag("Enemy") && other.gameObject.CompareTag("Player"))
        {
            Destroy(other.gameObject);

        }
        else if(gameObject.CompareTag("Enemy") && other.gameObject.CompareTag("Shield"))
        {
            if(other.gameObject.GetComponent<Shield>()._renderer.material.GetFloat("_Disolve") <= 0.5)
            {              
                other.gameObject.GetComponent<Shield>().HitShield();
            }

            
        }
    }
}
