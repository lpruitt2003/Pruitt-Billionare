using System.Collections.Generic;
using UnityEngine;

public class BillionController : MonoBehaviour
{
    public string billionColor; // "Green" or "Yellow"
    
    public float acceleration = 5f;
    public float maxSpeed = 10f;
    public float decelerationDistance = 3f;
    public float pushForce = 2f;

    private Rigidbody2D rb;
    private Dictionary<string, List<GameObject>> flags;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D not found on Billion!");
        }

        // Get the FlagPlacement instance in the scene
        FlagPlacement flagPlacement = FindObjectOfType<FlagPlacement>();

        if (flagPlacement != null)
        {
            flags = flagPlacement.flags;
        }
        else
        {
            Debug.LogError("FlagPlacement not found in scene!");
        }
    }

    private void FixedUpdate()
    {
        if (!string.IsNullOrEmpty(billionColor))
        {
            MoveToFlag(billionColor); // Only move towards the correct flag color
        }
    }

    private void MoveToFlag(string color)
    {
        if (flags == null || !flags.ContainsKey(color) || flags[color].Count == 0)
            return;

        GameObject nearestFlag = GetNearestFlag(color);
        if (nearestFlag == null)
            return;

        Vector2 direction = (nearestFlag.transform.position - transform.position);
        float distance = direction.magnitude;
        direction.Normalize();

        // Apply acceleration smoothly
        float speedFactor = Mathf.Clamp01(distance / decelerationDistance);
        float targetSpeed = maxSpeed * speedFactor; // Decelerates as it nears the flag

        // Instead of adding force every frame, directly adjust velocity
        Vector2 desiredVelocity = direction * targetSpeed;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, desiredVelocity, Time.fixedDeltaTime * acceleration);
    }

    private GameObject GetNearestFlag(string color)
    {
        GameObject nearestFlag = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject flag in flags[color])
        {
            float distance = Vector2.Distance(flag.transform.position, transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestFlag = flag;
            }
        }

        return nearestFlag;
    }
}
