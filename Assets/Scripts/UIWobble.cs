using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIWobble : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector3 hoverRotation; // Rotation when hovered
    public float rotationDuration = 0.3f; // Duration of the rotation animation

    private Quaternion originalRotation;

    void Start()
    {
        // Store the original rotation
        originalRotation = transform.rotation;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Rotate to the hover rotation smoothly
        transform.DORotate(hoverRotation, rotationDuration).SetEase(Ease.OutQuad).SetUpdate(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Rotate back to the original rotation smoothly
        transform.DORotateQuaternion(originalRotation, rotationDuration).SetEase(Ease.OutQuad).SetUpdate(true);
    }
}
