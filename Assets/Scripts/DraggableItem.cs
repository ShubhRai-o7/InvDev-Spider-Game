using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public Transform parentAfterDrag;
    public Image image;
    public WeaponSwitcher weaponSwitcher;
    [HideInInspector] public Vector3 originalScale;
    public GameObject weaponInfo;

    void Start()
    {
        originalScale = transform.localScale;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
        weaponInfo.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
        weaponInfo.SetActive(false);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
        weaponSwitcher.UpdateWeapons();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        weaponInfo.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        weaponInfo.SetActive(false);
    }

    
}
