using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncher : MonoBehaviour, IMyCommonFunction
{
    public ParticleSystem missiles;
    bool canUse;
    public Renderer reNderer;
   // public AudioClip missileLaunch;
   // private AudioSource missileSource;
   // public GameObject reloadPrompt;
    void Start()
    {

    }
    void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }

        GetComponentInChildren<particleAttractorLinear>().target = closestEnemy;
        missiles.Play();
    }
    void Update()
    {
        if (CameraManager.instance.isVirtualCamActive && canUse)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Step 2: Define the plane you want to raycast onto (in your case, the XZ plane)
            Plane plane = new Plane(Vector3.up, Vector3.zero); // Assuming you're on the XZ plane

            // Step 3: Perform raycast and get the point where it hits the plane
            float distanceToPlane;
            if (plane.Raycast(ray, out distanceToPlane))
            {
                // This is the point where the ray intersects the XZ plane
                Vector3 hitPoint = ray.GetPoint(distanceToPlane);

                // Step 4: Calculate the direction to look at (ignore Y-axis)
                Vector3 lookDir = hitPoint - transform.position;
                lookDir.y = 0; // Prevent rotation in the Y-axis

                // Step 5: Rotate the gun to face the hit point
                Quaternion targetRotation = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 50f); // Smooth rotation
                if (Input.GetKeyDown(KeyCode.Mouse0))
                    FindClosestEnemy();
            }
                       
        }
    }
    public void EnableWeapon()
    {
        canUse = true;
        reNderer.enabled = true;
    }
    public void DisableWeapon()
    {
        canUse = false;
        reNderer.enabled = false;
    }
    /*private IEnumerator Reload()
    {
        isReloading = true;
        //reloadPrompt.SetActive(true);
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(reloadTime);
        Debug.Log("WaitForSeconds complete");
        currentMissiles = maxMissiles;
        //reloadPrompt.SetActive(false);
        isReloading = false;
        Debug.Log("Reloaded!");
    }*/

}
