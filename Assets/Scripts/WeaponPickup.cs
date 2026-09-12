using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public GameObject weapon;
    //public GameObject weaponIcon;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player is the one colliding with the weapon
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Weapon collected");
            // Add the weapon to the player's weapon switcher
            WeaponSwitcher weaponSwitcher = other.gameObject.GetComponentInChildren<WeaponSwitcher>();
            

            if (weaponSwitcher != null)
            {
                // Add the weapon and dummy to the arrays
                weaponSwitcher.AddWeapon(weapon.name);
                weaponSwitcher.UpdateWeapons();
                // Destroy the pickup after it's been collected
                Destroy(gameObject);
            }
        }
    }
}
