using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using UnityEngine.Animations.Rigging;

public class WeaponSwitcher : MonoBehaviour
{
    public GameObject primarySlot;               // Slot for the primary weapon
    public GameObject secondarySlot;             // Slot for the secondary weapon
    public GameObject[] inventorySlots;          // Array to hold all inventory slots

    public GameObject[] weapons;                 // Array to hold all weapon objects (e.g., Machine Gun, Mortar, Flamethrower)
    public GameObject[] weaponDummies;

    public GameObject[] allowedPrimaryWeapons;   // Array to hold allowed primary weapons
    public GameObject[] allowedSecondaryWeapons; // Array to hold allowed secondary weapons

    private IMyCommonFunction primaryWeapon;               // Current primary weapon implementing IWeapon
    private IMyCommonFunction secondaryWeapon;             // Current secondary weapon implementing IWeapon

    public GameObject newWeaponAddedPrompt;
    bool collected;
    void Start()
    {
        DeactivateAllWeaponsAndDummies();
        //UpdateWeapons();// Disable all weapons and dummies at the start
    }

    private void Update()
    {
        
        GameObject primaryIcon = GetChildIcon(primarySlot);
        GameObject secondaryIcon = GetChildIcon(secondarySlot);

        

        GameObject primaryWeaponObject = GetWeaponFromIcon(primaryIcon);
        GameObject secondaryWeaponObject = GetWeaponFromIcon(secondaryIcon);

        primaryWeapon = primaryWeaponObject?.GetComponent<IMyCommonFunction>();
        secondaryWeapon = secondaryWeaponObject?.GetComponent<IMyCommonFunction>();

        

        if (Input.GetMouseButton(1))  // If right mouse button is held, switch to secondary weapon
        {
            ActivateSecondaryWeapon();
            DeactivatePrimaryWeapon();
        }
        else  // Otherwise, use primary weapon
        {
            ActivatePrimaryWeapon();
            DeactivateSecondaryWeapon();
        }
    }
    private void ActivatePrimaryWeapon()
    {
        primaryWeapon?.EnableWeapon();
    }

    private void DeactivatePrimaryWeapon()
    {
        primaryWeapon?.DisableWeapon();
    }

    private void ActivateSecondaryWeapon()
    {
        secondaryWeapon?.EnableWeapon();
    }

    private void DeactivateSecondaryWeapon()
    {
        secondaryWeapon?.DisableWeapon();
    }
    // Call this method to update the weapon states
    public void UpdateWeapons()
    {
        // Deactivate all weapons and dummies initially
        DeactivateAllWeaponsAndDummies();

        // Get the primary and secondary weapon icons
        GameObject primaryIcon = GetChildIcon(primarySlot);
        GameObject secondaryIcon = GetChildIcon(secondarySlot);

        // Activate weapons based on their slots
        GameObject primaryWeaponObject = GetWeaponFromIcon(primaryIcon);
        GameObject secondaryWeaponObject = GetWeaponFromIcon(secondaryIcon);

        primaryWeapon = primaryWeaponObject?.GetComponent<IMyCommonFunction>();
        secondaryWeapon = secondaryWeaponObject?.GetComponent<IMyCommonFunction>();

        if (primaryWeapon != null && IsAllowedPrimaryWeapon(primaryWeaponObject))
        {
            ActivatePrimaryWeapon();
            //DeactivateDummy(primaryWeaponObject);
        }

        if (secondaryWeapon != null && IsAllowedSecondaryWeapon(secondaryWeaponObject))
        {
            ActivateSecondaryWeapon();
            //DeactivateDummy(secondaryWeaponObject);
        }

        // Check all inventory slots and activate corresponding dummies
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            GameObject inventoryIcon = GetChildIcon(inventorySlots[i]);
            GameObject inventoryWeapon = GetWeaponFromIcon(inventoryIcon);

            if (inventoryWeapon != null)
            {
                //ActivateDummy(inventoryWeapon);
            }
        }
    }
    private bool IsAllowedPrimaryWeapon(GameObject weapon)
    {
        foreach (var allowedWeapon in allowedPrimaryWeapons)
        {
            if (allowedWeapon == weapon)
                return true;
        }
        return false;
    }

    // Check if the weapon can be a secondary weapon
    private bool IsAllowedSecondaryWeapon(GameObject weapon)
    {
        foreach (var allowedWeapon in allowedSecondaryWeapons)
        {
            if (allowedWeapon == weapon)
                return true;
        }
        return false;
    }
    // Method to get the child icon from a slot
    GameObject GetChildIcon(GameObject slot)
    {
        if (slot.transform.childCount > 0)
        {
            // Assuming the first child is the icon; adjust index if necessary
            return slot.transform.GetChild(0).gameObject;  
        }
        return null; // No icon present
    }

    // This method will find the actual weapon from the dragged icon
    GameObject GetWeaponFromIcon(GameObject icon)
    {
        if (icon != null)
        {
            for (int i = 0; i < weapons.Length; i++)
            {
                // Check if the icon's name matches the weapon's name + "_Icon"
                if (icon.name == weapons[i].name + "_Icon")  
                {
                    return weapons[i];
                }
            }
        }
        return null;
    }

    // Activate, deactivate, and dummy methods remain unchanged

    void ActivateDummy(GameObject weapon)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (collected)
            {
                if (weapon == weapons[i])
                {
                    weaponDummies[i].SetActive(true);
                }
            }
            
        }
    }

    void DeactivateDummy(GameObject weapon)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapon == weapons[i])
            {
                weaponDummies[i].SetActive(false);
                
            }
        }
        
    }

    void DeactivateAllWeaponsAndDummies()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            IMyCommonFunction weaponScript = weapons[i].GetComponent<IMyCommonFunction>();

            if (weaponScript != null)
            {
                weaponScript.DisableWeapon();  // Disable the weapon using the interface method
            }
            else
            {
                Debug.LogWarning("Weapon script not found on " + weapons[i].name);
            }

            // Deactivate dummy, assuming dummy deactivation is still done via SetActive
            weaponDummies[i].SetActive(false);
        }
    }

    public void AddWeapon(string weaponName)
    {

        for(int i = 0; i < weapons.Length; i++)
        {
            if(weaponName == weapons[i].name)
            {
                weapons[i].SetActive(true);
                collected = true;
            }
            
        }

        for(int i = 0; i < inventorySlots.Length; i++)
        {
            if (GetChildIcon(inventorySlots[i]).name == weaponName + "_Icon")
            {
                GetChildIcon(inventorySlots[i]).SetActive(true);
            }
        }


        Instantiate(newWeaponAddedPrompt);
        
    }
}
