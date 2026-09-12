using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Shotty : MonoBehaviour, IMyCommonFunction
{
    public ParticleSystem[] Shotguns;
    public CinemachineImpulseSource ShootImpulse;
    bool canUse;

    public Renderer rendeRer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
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
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                //nextFireTime = Time.time + 1f / fireRate;
                //Shoot();
                //CameraShaker.Instance.ShakeOnce(.1f, 1f, .1f, .2f);
                //gunShoot.Play();
                //ShootEffect.Play();
                foreach (ParticleSystem shotgun in Shotguns)
                {
                    if (!shotgun.isPlaying) // Ensure the particle system isn't already playing
                    {
                        shotgun.Play();
                        ShootImpulse.GenerateImpulse();
                    }
                }

            }
            if (Input.GetMouseButtonUp(0))
            {
            }
            /*else if (Input.GetMouseButton(0) && (Time.time >= nextFireTime || Time.time <= nextFireTime))
            {
                //StartCoroutine(Reload());
                //gunDryShoot.Play();

            }*/
        }
    }
    public void EnableWeapon()
    {
        canUse = true;
        rendeRer.enabled = true;
    }
    public void DisableWeapon()
    {
        canUse = false;
        rendeRer.enabled = false;
    }
}
