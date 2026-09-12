using System.Collections;
using UnityEngine;
[RequireComponent(typeof(ParticleSystem))]
public class particleAttractorLinear : MonoBehaviour {
	ParticleSystem ps;
	ParticleSystem.Particle[] m_Particles;
	public Transform target;
	public float speed = 5f;
	int numParticlesAlive;
	void Start () {
		ps = GetComponent<ParticleSystem>();
		if (!GetComponent<Transform>()){
			GetComponent<Transform>();
		}
	}
	void Update () {
		m_Particles = new ParticleSystem.Particle[ps.main.maxParticles];
		numParticlesAlive = ps.GetParticles(m_Particles);
		float step = speed * Time.deltaTime;
		for (int i = 0; i < numParticlesAlive; i++) {
			m_Particles[i].position = Vector3.LerpUnclamped(m_Particles[i].position, target.position, step);
		}
		ps.SetParticles(m_Particles, numParticlesAlive);
	}
	private void OnParticleCollision(GameObject other)
	{
        if (gameObject.CompareTag("Punisher") && other.gameObject.CompareTag("Enemy"))
        {
            //Instantiate(ExplosionEffect, other.gameObject.transform.position, Quaternion.identity);
            if (other.gameObject.GetComponent<SuicideBomber>())
                other.gameObject.GetComponent<SuicideBomber>().TakeDamage(30f);
            else if (other.gameObject.GetComponent<HeavyShooter>())
                other.gameObject.GetComponent<HeavyShooter>().TakeDamage(30f);
            else if (other.gameObject.GetComponent<SluttySniper>())
                other.gameObject.GetComponent<SluttySniper>().TakeDamage(30f);
            //int scoreValue = other.gameObject.name.Contains("EnemyPlaneB") ? Random.Range(300, 361) : Random.Range(100, 141);
            //ScoreManager.instance.AddScore(scoreValue);
            //GameManager.instance.EnemyKilled();

        }
        else if (gameObject.CompareTag("Enemy") && other.gameObject.CompareTag("Player"))
        {
            Destroy(other.gameObject);
        }
    }
}
