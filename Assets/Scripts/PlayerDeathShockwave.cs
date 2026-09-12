using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathShockwave : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Found Wall");
            foreach (Transform child in other.transform)
            {
                Rigidbody rb = child.gameObject.AddComponent<Rigidbody>(); // Add Rigidbody to each body part
                rb.AddExplosionForce(1000f, transform.position, 5f, 0, ForceMode.Impulse);
                child.parent = null;
            }
        }
    }
}
