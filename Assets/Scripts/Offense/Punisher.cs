using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Punisher : MonoBehaviour, IMyCommonFunction
{
    //public Transform shotPoint;
    //public GameObject bulletPrefab;
    //public float bulletForce = 20f;
    public float fireRate = 10f;
    private float nextFireTime = 0f;
    //public ParticleSystem ShootEffect;
    //public TextMeshProUGUI ClipSize;
    bool isReloading;
    public ParticleSystem machineGun;
    bool canUse;
    public Renderer Renderer;
    //[SerializeField] private AudioSource gunShoot;
    //[SerializeField] private AudioSource gunReload;
    //[SerializeField] private AudioSource gunDryShoot;

    //[SerializeField] private AudioSource gunShoot;
    //[SerializeField] private AudioSource gunReload;
    //[SerializeField] private AudioSource gunDryShoot;
    private void Start()
    {
    }
    private void Update()
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

            if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + 1f / fireRate;
                //Shoot();
                //CameraShaker.Instance.ShakeOnce(.1f, 1f, .1f, .2f);
                //gunShoot.Play();
                //ShootEffect.Play();
                machineGun.Play();

            }
            else
            {
                machineGun.Stop();
            }
            /*else if (Input.GetMouseButton(0) && (Time.time >= nextFireTime || Time.time <= nextFireTime))
            {
                //StartCoroutine(Reload());
                //gunDryShoot.Play();

            }*/
        }
    }
    void Shoot()
    {
        //GameObject bullet = Instantiate(bulletPrefab, shotPoint.position, transform.rotation);

        //Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        //currentClip--;
    }

    public void EnableWeapon()
    {
        canUse = true;
        Renderer.enabled = true;
    }
    public void DisableWeapon()
    {
        canUse = false;
        Renderer.enabled = false;
    }
}
