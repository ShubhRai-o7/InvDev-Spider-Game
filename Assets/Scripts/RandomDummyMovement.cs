using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDummyMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed at which the object moves
    public float minX = -10f; // Minimum X position
    public float maxX = 10f;  // Maximum X position
    public float minZ = -10f; // Minimum Z position
    public float maxZ = 10f;  // Maximum Z position

    private Vector3 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        // Set an initial random target position when the game starts
        SetRandomTargetPosition();
    }

    // Update is called once per frame
    void Update()
    {
        // Move towards the target position
        MoveToTarget();

        // If we are close to the target position, choose a new random position
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            SetRandomTargetPosition();
        }
    }

    // Function to set a random target position within the specified ranges
    private void SetRandomTargetPosition()
    {
        float randomX = Random.Range(minX, maxX);
        float randomZ = Random.Range(minZ, maxZ);

        targetPosition = new Vector3(randomX, transform.position.y, randomZ); // Keep y position unchanged
    }

    // Function to move the object towards the target position
    private void MoveToTarget()
    {
        // Move towards the target position at the specified speed
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }
}
