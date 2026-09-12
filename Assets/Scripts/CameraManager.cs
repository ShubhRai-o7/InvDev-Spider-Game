using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public CinemachineFreeLook freeLookCam;
    public CinemachineVirtualCamera virtualCam;
    public CinemachineVirtualCamera deathCam;

    public bool isVirtualCamActive = true; 

    public static CameraManager instance;

    private void Start()
    {
        instance = this;
        virtualCam.Priority = 20;
        freeLookCam.Priority = 10;
        deathCam.Priority = 8;
    }
    public void SwitchCamera()
    {
        if (isVirtualCamActive)
        {
            freeLookCam.Priority = 20;
            virtualCam.Priority = 10;
        }
        else if(!isVirtualCamActive)
        {
            freeLookCam.Priority = 10;
            virtualCam.Priority = 20;
        }
        isVirtualCamActive = !isVirtualCamActive;
    }

    public void SwitchToDeathCam()
    {
        deathCam.Priority = 80;
    }
}
