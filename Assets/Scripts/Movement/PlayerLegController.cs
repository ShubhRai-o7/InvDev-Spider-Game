using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLegController : MonoBehaviour
{
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private PlayerLeg[] legs;

    private float maxTipWait = 1f;
    private bool readySwitchOrder = false;
    private bool stepOrder = true; // Controls which diagonal pair steps
    private float bodyHeightBase = 1.3f;

    private Vector3 bodyPos;
    private Vector3 bodyUp;
    private Vector3 bodyForward;
    private Vector3 bodyRight;
    private Quaternion bodyRotation;

    private float PosAdjustRatio = 0.1f;
    private float RotAdjustRatio = 0.2f;

    private void Start()
    {
        // Start coroutine to adjust body transform
        StartCoroutine(AdjustBodyTransform());
    }

    private void Update()
    {
        if (legs.Length < 4) return;  // Ensure there are four legs to work with

        // Group legs into diagonal pairs
        PlayerLeg frontLeft = legs[0];
        PlayerLeg backRight = legs[1];
        PlayerLeg frontRight = legs[2];
        PlayerLeg backLeft = legs[3];

        // Determine if either leg in the first pair needs to step
        if ((frontLeft.TipDistance > maxTipWait || backRight.TipDistance > maxTipWait) && !frontRight.Animating && !backLeft.Animating)
        {
            stepOrder = true; // Diagonal pair: Front-Left and Back-Right
        }
        else if ((frontRight.TipDistance > maxTipWait || backLeft.TipDistance > maxTipWait) && !frontLeft.Animating && !backRight.Animating)
        {
            stepOrder = false; // Diagonal pair: Front-Right and Back-Left
        }

        // Ensure legs in the opposite diagonal pair stay put while current diagonal pair moves
        if (stepOrder)
        {
            frontLeft.Movable = true;
            backRight.Movable = true;
            frontRight.Movable = false;
            backLeft.Movable = false;
        }
        else
        {
            frontLeft.Movable = false;
            backRight.Movable = false;
            frontRight.Movable = true;
            backLeft.Movable = true;
        }

        // Check if all legs in the current step order have finished their steps
        if (readySwitchOrder && !frontLeft.Animating && !backRight.Animating && !frontRight.Animating && !backLeft.Animating)
        {
            stepOrder = !stepOrder;
            readySwitchOrder = false;
        }

        if (!readySwitchOrder && (frontLeft.Animating || backRight.Animating || frontRight.Animating || backLeft.Animating))
        {
            readySwitchOrder = true;
        }
    }
    private IEnumerator AdjustBodyTransform()
    {
        while (true)
        {
            Vector3 tipCenter = Vector3.zero;
            bodyUp = Vector3.zero;

            // Collect leg information to calculate body transform
            foreach (PlayerLeg leg in legs)
            {
                tipCenter += leg.TipPos;
                bodyUp += leg.TipUpDir + leg.RaycastTipNormal;
            }

            // Ensure bodyUp is correctly aligned with the terrain normal
            RaycastHit hit;
            if (Physics.Raycast(bodyTransform.position, -bodyTransform.up, out hit, 10.0f))
            {
                bodyUp += hit.normal;
            }

            tipCenter /= legs.Length;
            bodyUp.Normalize();

            // Calculate the new position and rotation of the body
            bodyPos = tipCenter + bodyUp * bodyHeightBase;

            bool isMoving = PlayerController.instance.moveInput != 0 || PlayerController.instance.turnInput != 0;  // Assuming MoveSpeed > 0 means the player is moving

            if (isMoving)
            {
                // Smooth movement and rotation when the player is moving
                bodyTransform.position = Vector3.Lerp(bodyTransform.position, bodyPos, PosAdjustRatio);

                bodyRight = Vector3.Cross(bodyUp, bodyTransform.forward);
                bodyForward = Vector3.Cross(bodyRight, bodyUp);

                bodyRotation = Quaternion.LookRotation(bodyForward, bodyUp);
                bodyTransform.rotation = Quaternion.Slerp(bodyTransform.rotation, bodyRotation, RotAdjustRatio);
            }
            else
            {
                // When idle, snap to the final position and rotation more quickly
                bodyTransform.position = Vector3.Lerp(bodyTransform.position, bodyPos, 0.5f);  // Higher value for faster snapping

                // Stop rotation when idle (or clamp it very tightly)
                if (Quaternion.Angle(bodyTransform.rotation, bodyRotation) > 1f)  // Threshold for clamping rotation
                {
                    bodyTransform.rotation = Quaternion.Slerp(bodyTransform.rotation, bodyRotation, 0.1f);  // Slower rotation when idle
                }
                else
                {
                    // Snap to the target rotation when very close
                    bodyTransform.rotation = bodyRotation;
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(bodyPos, bodyPos + bodyRight);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(bodyPos, bodyPos + bodyUp);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(bodyPos, bodyPos + bodyForward);
    }
}
