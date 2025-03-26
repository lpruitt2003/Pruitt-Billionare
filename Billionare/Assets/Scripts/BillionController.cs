using System.Collections.Generic;
using UnityEngine;

public class BillionController : MonoBehaviour
{
    public string billionColor; // "Green", "Yellow", "Red", "Blue"
    
    public float acceleration = 5f;
    public float maxSpeed = 10f;
    public float decelerationDistance = 3f;
    public float pushForce = 2f;
    
    public float maxHealth = 100f;
    private float currentHealth;

    public GameObject basePrefab;
    public GameObject innerHealthCircle; // Assign this in the Inspector

    private Rigidbody2D rb;
    private Dictionary<string, List<GameObject>> flags;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D not found on Billion!");
        }

        FlagPlacement flagPlacement = FindObjectOfType<FlagPlacement>();

        if (flagPlacement != null)
        {
            flags = flagPlacement.flags;
        }
        else
        {
            Debug.LogError("FlagPlacement not found in scene!");
        }

        UpdateHealthDisplay();
    }

    private void Update()
    {
        if (flags == null || flags.Count == 0)
            return;

        if (flags.ContainsKey(billionColor) && flags[billionColor].Count > 0)
        {
            MoveToFlag(billionColor);
        }
        else
        {
            MoveToBase();
        }

        TargetBillion();
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

        float speedFactor = Mathf.Clamp01(distance / decelerationDistance);
        float targetSpeed = maxSpeed * speedFactor; 

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

    private void MoveToBase()
    {
        if (basePrefab == null)
            return;

        Vector2 direction = (basePrefab.transform.position - transform.position);
        float distance = direction.magnitude;
        direction.Normalize();

        float speedFactor = Mathf.Clamp01(distance / decelerationDistance);
        float targetSpeed = maxSpeed * speedFactor; 

        Vector2 desiredVelocity = direction * targetSpeed;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, desiredVelocity, Time.fixedDeltaTime * acceleration);
    }

    private void OnMouseDown() // Left-click for damage
    {
        TakeDamage(20f);
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(2)) // Middle-click for damage
        {
            TakeDamage(20f);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        UpdateHealthDisplay();

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void UpdateHealthDisplay()
    {
        if (innerHealthCircle == null) return;

        float healthRatio = currentHealth / maxHealth;
        float minSize = 0.3f; // Minimum inner circle size (30% of max size)
        float sizeRatio = Mathf.Lerp(minSize, 1f, healthRatio);

        innerHealthCircle.transform.localScale = new Vector3(sizeRatio, sizeRatio, 1f);
    }

    //billions will turn to face the closest billion of a different color
    private void TargetBillion()
    {
        GameObject closestEnemyBillion = null;
        float minDistance = Mathf.Infinity;

        // Find all billions in the scene
        GameObject[] allBillions = GameObject.FindGameObjectsWithTag("Billion");

        foreach (GameObject billion in allBillions)
        {
            if (billion == gameObject) continue; // Skip self

            BillionController bc = billion.GetComponent<BillionController>();
            if (bc == null || bc.billionColor == billionColor) continue; // Skip same-color billions

            // Calculate distance
            float distance = Vector2.Distance(billion.transform.position, transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestEnemyBillion = billion;
            }
        }

        // Rotate towards the closest enemy billion
        if (closestEnemyBillion != null)
        {
            Vector2 direction = (closestEnemyBillion.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
        }
    }
}
