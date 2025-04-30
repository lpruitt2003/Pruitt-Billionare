using System.Collections.Generic;
using UnityEngine;

public class BillionController : MonoBehaviour
{
    public string billionColor; // "Green", "Yellow", "Red", "Blue"
    
    public float acceleration = 5f;
    public float maxSpeed = 10f;
    public float decelerationDistance = 3f;
    public float pushForce = 2f;
    private float lastFireTime = 0f;
    
    public float maxHealth = 100f;
    private float currentHealth;

    public GameObject basePrefab;
    public GameObject innerHealthCircle; // Assign this in the Inspector
    public GameObject blasterShotPrefab; // Assign this in the Inspector

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

        TargetEnemy();
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
        if (distance < decelerationDistance)
        {
            rb.linearVelocity = Vector2.zero; // Stop moving when close to the base
        }
    }

    public void TakeDamage(float damage, string Color)
    {
        currentHealth -= damage;
        UpdateHealthDisplay();
        if (currentHealth <= 0)
        {
            //find all bases
            GameObject[] allBases = GameObject.FindGameObjectsWithTag("Base");
            foreach (GameObject baseObj in allBases)
            {
                BaseController baseController = baseObj.GetComponent<BaseController>();
                if (baseController != null && baseController.billionColor == Color)
                {
                    baseController.HandleExp(billionColor);
                }
            }
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

    //billions will turn to face the closest billion or base of a different color
    private void TargetEnemy()
    {
        GameObject closestEnemy = null;
        float minDistance = Mathf.Infinity;

        // Find all billions in the scene
        GameObject[] allBillions = GameObject.FindGameObjectsWithTag("Billion");
        // Find all bases in the scene
        GameObject[] allBases = GameObject.FindGameObjectsWithTag("Base");
        // Combine all bases and billions into one array
        GameObject[] allEnemies = new GameObject[allBillions.Length + allBases.Length];
        allBillions.CopyTo(allEnemies, 0);
        allBases.CopyTo(allEnemies, allBillions.Length);

        foreach (GameObject enemy in allEnemies)
        {
            if (enemy == gameObject) continue; // Skip self

            BillionController bc = enemy.GetComponent<BillionController>();
            BaseController baseController = enemy.GetComponent<BaseController>();

            if (bc != null && bc.billionColor == billionColor) continue; // Skip same-color billions
            if (baseController != null && baseController.billionColor == billionColor) continue; // Skip same-color bases

            // Calculate distance
            float distance = Vector2.Distance(enemy.transform.position, transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestEnemy = enemy;
            }
        }

        // Rotate towards the closest enemy immediately
        if (closestEnemy != null)
        {
            Vector2 direction = (closestEnemy.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));

            // Fire blaster shot if within range
            if (minDistance < 4f && Time.time - lastFireTime >= 2f)
            {
                SpawnBlasterShot();
                lastFireTime = Time.time;
            }
        }   
    }

    private void SpawnBlasterShot()
    {
        GameObject blasterShot = Instantiate(blasterShotPrefab, transform.position, transform.rotation);
        BlasterShotController blasterShotController = blasterShot.GetComponent<BlasterShotController>();
        if (blasterShotController != null)
        {
            blasterShotController.billionColor = billionColor;
        }
    }      
}
