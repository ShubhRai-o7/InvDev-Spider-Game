using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetFollow : MonoBehaviour
{
    public GameObject Target;

    public Vector3 offset;

    private void Start()
    {
        
    }

    private void Update()
    {
        if(gameObject.activeSelf)
            transform.position = Target.transform.position + offset;

    }

    private void MoveCameraOffset()
    {
        Vector3 RotSpeed = transform.rotation.eulerAngles;
        RotSpeed.Normalize();
        transform.position = transform.position + RotSpeed;

        CameraController.Destroy(transform);
    }
}
  