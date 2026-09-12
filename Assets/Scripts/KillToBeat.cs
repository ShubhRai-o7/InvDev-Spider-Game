using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillToBeat : MonoBehaviour
{
    private Vector3 originalPos;
    [SerializeField] float smashDistance;
    [SerializeField] float returnSpeed;
    // Start is called before the first frame update
    void Start()
    {
        originalPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, originalPos, Time.deltaTime * returnSpeed);
    }
    public void Smash()
    {
        transform.position = new Vector3(originalPos.x, originalPos.y + smashDistance, originalPos.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.gameObject.GetComponent<SuicideBomber>())
            {
                collision.gameObject.GetComponent<SuicideBomber>().TakeDamage(1000f);
            }
            else if (collision.gameObject.GetComponent<SluttySniper>())
            {
                collision.gameObject.GetComponent<SluttySniper>().TakeDamage(1000f);
            }
        }
    }
}
