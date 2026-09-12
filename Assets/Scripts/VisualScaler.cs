using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualScaler : MonoBehaviour
{
    float randomOffset;

    private void Start()
    {
        randomOffset = Random.Range(0f, 5f);
    }

    private void Update()
    {
        if (gameObject.GetComponent<Renderer>().isVisible){
            float perlin = Mathf.PerlinNoise(transform.position.x / 5f + Time.time * 1f, transform.position.z / 5f + Time.time * 1f);
            transform.localScale = new Vector3(4f, perlin * 5f + 4f, 4f);
        }
        
    }
}
