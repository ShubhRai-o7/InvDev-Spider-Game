using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class InvenCamLook : MonoBehaviour
{
    public CinemachineFreeLook freeLookCam;

    private string originalXAxisName;
    private string originalYAxisName;
    private bool isDragging; // Track if we're currently dragging

    private void Start()
    {
        // Store the original input axis names
        originalXAxisName = freeLookCam.m_XAxis.m_InputAxisName;
        originalYAxisName = freeLookCam.m_YAxis.m_InputAxisName;

        // Disable axis input initially (no input unless LMB is pressed)
        freeLookCam.m_XAxis.m_InputAxisName = "";
        freeLookCam.m_YAxis.m_InputAxisName = "";
    }

    private void Update()
    {
        if (!CameraManager.instance.isVirtualCamActive){
            // Check if the cursor is over a UI element
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // If the cursor is over the UI, make sure it's visible and don't allow rotation
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                freeLookCam.m_XAxis.m_InputAxisName = ""; // Disable camera rotation
                freeLookCam.m_YAxis.m_InputAxisName = "";
                isDragging = false; // Reset dragging state
            }
            else
            {
                // Cursor is not over UI
                HandleCameraRotation();
            }

        }
        
    }

    private void HandleCameraRotation()
    {
        // Check if LMB is being held down
        if (Input.GetMouseButton(0)) // 0 is the Left Mouse Button
        {
            if (!isDragging)
            {
                // Start dragging
                StartCoroutine(EnableCameraControl());
            }
        }
        else
        {
            // Reset cursor visibility and lock state when mouse button is not pressed
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            freeLookCam.m_XAxis.m_InputAxisName = ""; // Disable camera rotation
            freeLookCam.m_YAxis.m_InputAxisName = "";
            isDragging = false; // Reset dragging state
        }
    }

    private IEnumerator EnableCameraControl()
    {
        isDragging = true; // Set dragging state to true
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        freeLookCam.m_XAxis.m_InputAxisName = originalXAxisName;
        freeLookCam.m_YAxis.m_InputAxisName = originalYAxisName;

        // Wait until the mouse button is released
        while (Input.GetMouseButton(0))
        {
            yield return null; // Wait for the next frame
        }

        // Reset the input axes and cursor state when dragging ends
        freeLookCam.m_XAxis.m_InputAxisName = "";
        freeLookCam.m_YAxis.m_InputAxisName = "";
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        isDragging = false; // Reset dragging state
    }
}
