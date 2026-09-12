using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jiggle : MonoBehaviour
{
    public float wobbleSpeed = 2f;  // Speed of the wobble
    public float wobbleAmount = 15f;  // Maximum rotation angle (degrees)

    private float wobbleTimer;

    // Update is called once per frame
    void Update()
    {
        // Increase the wobble timer over time, scaled by speed
        wobbleTimer += Time.unscaledDeltaTime * wobbleSpeed;

        // Calculate the rotation angle using a sine wave
        float wobbleAngle = Mathf.Sin(wobbleTimer) * wobbleAmount;

        // Apply the rotation on the Z axis (for a 2D image)
        transform.rotation = Quaternion.Euler(9f, 9f, wobbleAngle);
    }
}
