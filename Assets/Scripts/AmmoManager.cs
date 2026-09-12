using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoManager : MonoBehaviour
{
    public int maxAmmo = 10; // Maximum bullets the player can have
    public GameObject bulletPrefab; // Prefab of bullet image
    public Transform ammoGrid; // Parent Grid Layout (UI) where bullets are displayed
    public float maxBulletHeight = 50f; // Set a fixed bullet height
    public float padding = 10f; // Optional padding between bullets

    private GridLayoutGroup gridLayoutGroup; // Reference to Grid Layout Group component
    private List<GameObject> bullets = new List<GameObject>(); // List to store active bullets

    public static AmmoManager instance;

    void Start()
    {
        gridLayoutGroup = ammoGrid.GetComponent<GridLayoutGroup>(); // Get the Grid Layout Group component
        AdjustCellSize(); // Adjust the width of the bullets dynamically to fit the grid
        InitializeAmmo(maxAmmo); // Instantiate the ammo images
        instance = this;
    }

    // Adjust the width of the cells to fit all bullets inside the grid
    void AdjustCellSize()
    {
        RectTransform gridRectTransform = ammoGrid.GetComponent<RectTransform>();
        float gridWidth = gridRectTransform.rect.width;

        // Calculate cell width to fit all bullets horizontally, minus padding between bullets
        float cellWidth = (gridWidth - padding * (maxAmmo - 1)) / maxAmmo;

        // Apply dynamic width but keep the bullet height fixed
        gridLayoutGroup.cellSize = new Vector2(cellWidth, maxBulletHeight);
        gridLayoutGroup.spacing = new Vector2(padding, 0); // Set horizontal spacing
    }
    private void Update()
    {
    }
    // Initialize the ammo display by instantiating bullets
    void InitializeAmmo(int magSize)
    {
        for (int i = 0; i < magSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, ammoGrid);
            bullets.Add(bullet);
        }
    }

    // Call this function when the player shoots
    public void Shoot()
    {
        if (bullets.Count > 0)
        {
            // Get the last bullet on the right
            GameObject lastBullet = bullets[bullets.Count - 1];

            // Disable the bullet's image
            lastBullet.GetComponent<Image>().enabled = false;

            // Remove it from the list
            bullets.RemoveAt(bullets.Count - 1);
        }
        else
        {
            Debug.Log("Out of ammo!");
        }
    }

    // Call this to reload ammo
    public void Reload(int magSize)
    {
        foreach (GameObject bullet in bullets)
        {
            bullet.GetComponent<Image>().enabled = true;
        }
        bullets.Clear();
        InitializeAmmo(magSize); // Reset the bullets
    }
}
