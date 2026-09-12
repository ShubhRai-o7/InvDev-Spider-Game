using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class inGameIcon : MonoBehaviour
{

    public GameObject primaryIcon;
    public GameObject secondaryIcon;
    // Start is called before the first frame update
    void Start()
    {
        Color imageColor = gameObject.GetComponent<Image>().color;

        imageColor.a = 0f;
        gameObject.GetComponent<Image>().color = imageColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (primaryIcon != null || secondaryIcon != null)
        {
            if (Input.GetMouseButton(1))
            {
                Color imageColor = gameObject.GetComponent<Image>().color;
                imageColor.a = 1;
                gameObject.GetComponent<Image>().sprite = secondaryIcon.GetComponentInChildren<DraggableItem>().gameObject.GetComponent<Image>().sprite;
                gameObject.GetComponent<Image>().color = imageColor;
            }
            else
            {
                Color imageColor = gameObject.GetComponent<Image>().color;
                imageColor.a = 1;
                gameObject.GetComponent<Image>().sprite = primaryIcon.GetComponentInChildren<DraggableItem>().gameObject.GetComponent<Image>().sprite;
                gameObject.GetComponent<Image>().color = imageColor;
            }
        }
        
    }
}
