using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{

    public float lifetime;
    public GameObject text;
    public bool canSelfDestroy;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(flash());
    }

    // Update is called once per frame
    void Update()
    {
        if(canSelfDestroy)
            Destroy(gameObject, lifetime);

    }
    IEnumerator flash()
    {
        text.SetActive(false);
        yield return new WaitForSeconds(0.05f);
        text.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        text.SetActive(false);
        yield return new WaitForSeconds(0.05f);
        text.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        text.SetActive(false);
        yield return new WaitForSeconds(0.05f);
        text.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        text.SetActive(false);
        yield return new WaitForSeconds(0.05f);
        text.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        text.SetActive(false);
        yield return new WaitForSeconds(0.05f);
        text.SetActive(true);
        yield return new WaitForSeconds(0.05f);
    }
}
