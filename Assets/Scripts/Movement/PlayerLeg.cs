using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLeg : MonoBehaviour
{
    public static PlayerLeg instance;
    // Self-explanatory variable names
    private PlayerLegController legController;

    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Transform rayOrigin;
    public GameObject ikTarget;
    public Transform ankle;

    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private AnimationCurve heightCurve;

    private float tipMaxHeight = 0.4f;
    private float tipAnimationTime = 0.08f;
    private float tipAnimationFrameTime = 1 / 60.0f;

    private float ikOffset = .8f;
    private float tipMoveDist = 0.85f;
    private float maxRayDist = 7.0f;
    private float tipPassOver = 0.55f / 2.0f;
    private float ankleRotationSpeed = 500f;

    public AudioSource moveSFX;
    public AudioClip MoveSfx;

    public Vector3 TipPos { get; private set; }
    public Vector3 TipUpDir { get; private set; }
    public Vector3 RaycastTipPos { get; private set; }
    public Vector3 RaycastTipNormal { get; private set; }

    public bool Animating { get; private set; } = false;
    public bool Movable { get; set; } = false;
    public float TipDistance { get; private set; }

    private void Awake()
    {
        legController = GetComponentInParent<PlayerLegController>();
        instance = this;

        transform.parent = bodyTransform;
        rayOrigin.parent = bodyTransform;
        TipPos = ikTarget.transform.position;
    }

    private void Start()
    {
        UpdateIKTargetTransform();
    }

    private void Update()
    {
        //if (PlayerController.instance.MoveSpeed == 10f) tipAnimationTime = .1f; else tipAnimationTime = 0.08f;
        RaycastHit hit;
        float playerSpeed = PlayerController.instance.MoveSpeed; // Replace with actual player speed access

        // Scale tipMoveDist based on player speed
        tipMoveDist = Mathf.Lerp(0.25f, 1.0f, playerSpeed / 10.0f); // Adjust range as needed
                                                                    // Calculate the tip target position
        if (Physics.Raycast(rayOrigin.position, bodyTransform.up.normalized * -1, out hit, maxRayDist))
        {
            RaycastTipPos = hit.point;
            RaycastTipNormal = hit.normal;

        }

        TipDistance = (RaycastTipPos - TipPos).magnitude;

        // If the distance gets too far, animate and move the tip to new position
        if (!Animating && TipDistance > tipMoveDist && Movable)
        {
            StartCoroutine(AnimateLeg());
        }

        RotateAnkle();

    }

    private IEnumerator AnimateLeg()
    {
        
        Animating = true;

        float timer = 0.0f;
        float animTime;

        Vector3 startingTipPos = TipPos;
        Vector3 tipDirVec = RaycastTipPos - TipPos;
        tipDirVec += tipDirVec.normalized * tipPassOver;

        Vector3 right = Vector3.Cross(bodyTransform.up, tipDirVec.normalized).normalized;
        TipUpDir = Vector3.Cross(tipDirVec.normalized, right);

        while (timer < tipAnimationTime + tipAnimationFrameTime)
        {
            animTime = speedCurve.Evaluate(timer / tipAnimationTime);

            // If the target is keep moving, apply acceleration to correct the end point
            float tipAcceleration = Mathf.Max((RaycastTipPos - startingTipPos).magnitude / tipDirVec.magnitude, 1.0f);

            TipPos = startingTipPos + tipDirVec * tipAcceleration * animTime; // Forward direction of tip vector
            TipPos += TipUpDir * heightCurve.Evaluate(animTime) * tipMaxHeight; // Upward direction of tip vector

            UpdateIKTargetTransform();

            timer += tipAnimationFrameTime;

            yield return new WaitForFixedUpdate();
        }

        Animating = false;
    }

    private void UpdateIKTargetTransform()
    {
        // Update leg ik target transform depend on tip information
        ikTarget.transform.position = TipPos + bodyTransform.up.normalized * ikOffset;
        ikTarget.transform.rotation = Quaternion.LookRotation(TipPos - ikTarget.transform.position) * Quaternion.Euler(90, 0, 0);
    }
    private void RotateAnkle()
    {
        Vector3 directionToTarget = ikTarget.transform.position - ankle.position;

        // If the direction is valid (not too small), rotate the ankle to follow
        if (directionToTarget.sqrMagnitude > 0.0001f)
        {
            Quaternion ankleRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);

            // Interpolate rotation for smoothness if needed
            ankle.rotation = Quaternion.Slerp(ankle.rotation, ankleRotation, Time.deltaTime * ankleRotationSpeed);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(RaycastTipPos, 0.1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(TipPos, 0.1f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(TipPos, RaycastTipPos);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(ikTarget.transform.position, 0.1f);
    }
}
