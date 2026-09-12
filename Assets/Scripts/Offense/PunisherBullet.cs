using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunisherBullet : MonoBehaviour
{
    public float bulletSpeed = 10f;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * bulletSpeed;
        Destroy(gameObject, 1f);
    }
    // Start is called before the first frame update


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Enemy hit");
            //collision.gameObject.GetComponent<Rigidbody>().AddExplosionForce(10f, collision.gameObject.transform.position, 1f, 0f, ForceMode.Impulse);
            collision.gameObject.GetComponent<SuicideBomber>().TakeDamage(1f);
            Destroy(gameObject);
        }
        Destroy(gameObject);
    }
}
