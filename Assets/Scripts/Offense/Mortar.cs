using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class Mortar : MonoBehaviour, IMyCommonFunction
{
    [Header("Shooting Settings")]
    public GameObject Shell; // The grenade object
    //private float LaunchForce;
    public Transform shotPoint; // Where the grenade is launched from
    public float maxLaunchSpeed; // Launch force forward
    public float upwardForceMultiplier = 1f; // Upward force for the arc
    public float velocityMultiplier = 1.5f;  // Multiplier for increasing velocity
    //public float maxRange = 10f;  // Limit the range of the shell

    [Header("Trajectory Settings")]
    public GameObject pointPrefab; // Point object for trajectory visualization
    private GameObject[] trajectoryPoints; // Array to store the points
    public int numberOfPoints = 30; // Number of trajectory points
    public float spaceBetweenPoints = 0.1f; // Distance between each trajectory point
    //private Vector3 direction; // Direction of the shot

    [Header("Reload Settings")]
    public int CurrentClip = 5;
    //public TextMeshProUGUI MagSize;
    //public Animator animator;
    private bool isReloading = false;
    public bool inHotbar;
    bool canUse;
    public Renderer rEnderer;
    //[SerializeField] private AudioSource GrenadeReload;
    //[SerializeField] private AudioSource GrenadeLaunch;

    private void Start()
    {
        // Initialize trajectory points
        trajectoryPoints = new GameObject[numberOfPoints];
        for (int i = 0; i < numberOfPoints; i++)
        {
            trajectoryPoints[i] = Instantiate(pointPrefab, shotPoint.position, Quaternion.identity);
            trajectoryPoints[i].SetActive(false); // Hide them initially
        }
    }

    void Update()
    {
        //MagSize.text = CurrentClip.ToString();

        // Get mouse position in world space (for isometric, we may need to adjust how we get the position)

        if (CameraManager.instance.isVirtualCamActive && canUse)
        {
            if (isReloading) return;

            if (Input.GetMouseButtonDown(0) && CurrentClip > 0)
            {
                Shoot();
            }
            else if (Input.GetMouseButton(0) && CurrentClip == 0)
            {
                StartCoroutine(Reload());
            }

            UpdateTrajectoryPoints();
        }
        
    }

    IEnumerator Reload()
    {
        canUse = false;
        isReloading = true;
        //animator.SetBool("Reloading", true);
        //GrenadeReload.Play();
        yield return new WaitForSeconds(2); // Reload delay
        //animator.SetBool("Reloading", false);
        CurrentClip = 5; 
        isReloading = false;
        canUse = true;
    }

    void Shoot()
    {
        if (!isReloading)
        {
            GameObject newShell = Instantiate(Shell, shotPoint.position, shotPoint.rotation);
            Rigidbody shellRigidbody = newShell.GetComponent<Rigidbody>();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 targetPoint = hit.point;
                Vector3 launchVelocity = CalculateLaunchVelocity(targetPoint);

                shellRigidbody.linearVelocity = launchVelocity;
            }

            CurrentClip--;
        }
    }
    Vector3 CalculateLaunchVelocity(Vector3 targetPoint)
    {
        Vector3 direction = targetPoint - shotPoint.position;
        float horizontalDistance = new Vector3(direction.x, 0, direction.z).magnitude;
        float heightDifference = direction.y;

        float gravity = Mathf.Abs(Physics.gravity.y);
        float angle = 45f;
        float launchSpeed = Mathf.Sqrt((gravity * horizontalDistance * horizontalDistance) /
                                       (2 * (horizontalDistance * Mathf.Tan(Mathf.Deg2Rad * angle) - heightDifference)));
        Vector3 launchDirection = (new Vector3(direction.x, 0, direction.z)).normalized;
        Vector3 launchVelocity = launchDirection * launchSpeed + Vector3.up * launchSpeed * Mathf.Tan(Mathf.Deg2Rad * angle);

        return launchVelocity;
    }

    void UpdateTrajectoryPoints()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetPoint = hit.point;
            Vector3 launchVelocity = CalculateLaunchVelocity(targetPoint);

            for (int i = 0; i < numberOfPoints; i++)
            {
                float t = i * spaceBetweenPoints;

                Vector3 pointPosition = CalculatePointPosition(t, launchVelocity);
                if (Vector3.Distance(pointPosition, shotPoint.position) <= Vector3.Distance(targetPoint, shotPoint.position))
                {
                    trajectoryPoints[i].transform.position = pointPosition;
                    trajectoryPoints[i].SetActive(true);
                }
                else
                {
                    trajectoryPoints[i].SetActive(false);
                }
            }
        }
    }
    Vector3 CalculatePointPosition(float time, Vector3 launchVelocity)
    {
        Vector3 gravityEffect = 0.5f * Physics.gravity * (time * time);
        Vector3 pointPosition = shotPoint.position + launchVelocity * time + gravityEffect;
        return pointPosition;
    }
    public void EnableWeapon()
    {
        canUse = true;
        rEnderer.enabled = true;
    }
    public void DisableWeapon()
    {
        canUse = false;
        rEnderer.enabled = false;
    }
}
