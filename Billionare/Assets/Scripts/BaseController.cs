using System.Collections;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    // Script to spawn a character prefab next to the base
    // This script is attached to the base

    public GameObject characterPrefab;
    public float spawnDistance = 1.5f;
    private float lastFireTime = 0f;
    public float safeSpawnHeight = 1f; // Adjust if needed
    public float maxHealth = 500f;
    private float currentHealth;
    private Rigidbody2D rb;
    public string billionColor; // "Green", "Yellow", "Red", "Blue"
    public GameObject blasterShotPrefab; // Assign this in the Inspector
    
    public GameObject ExpPrefab; // Assign this in the Inspector
    [SerializeField] private float radius = 1f;
    [SerializeField] private int segments = 100;

    private LineRenderer lineRenderer;
    

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        Material mat = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.material = mat;
        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = false;
        lineRenderer.widthMultiplier = 0.1f;
        lineRenderer.positionCount = segments + 1;

        // Get sprite size and calculate radius
        SpriteRenderer spriteRenderer = GetComponentInParent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            float spriteWidth = spriteRenderer.bounds.size.x;
            float spriteHeight = spriteRenderer.bounds.size.y;
            float spriteMax = Mathf.Max(spriteWidth, spriteHeight);

            radius = spriteMax * 0.55f; // Slightly larger than sprite edge
        }
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        StartCoroutine(SpawnCharacter());
        UpdateHealthDisplay();
    }

    private void Update()
    {
        TargetBillion();
        drawHealthBar();
    }

    private IEnumerator SpawnCharacter()
    {
        while (true)
        {
            Vector3 spawnPosition = transform.position;
            spawnPosition.y += safeSpawnHeight; // Raise to avoid collisions

            // Check if the spawn position is clear
            Collider[] colliders = Physics.OverlapSphere(spawnPosition, 0.5f);
            if (colliders.Length == 0) // Only spawn if no collisions detected
            {
                Instantiate(characterPrefab, spawnPosition, Quaternion.identity);
            }

            yield return new WaitForSeconds(5f);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void UpdateHealthDisplay()
    {
        // Update the health display here
        float healthRatio = currentHealth / maxHealth;
    }

    //rotate turret at a constant rate towards the nearest billion
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

            // Check if the billion is within a certain range
            // If so, fire a blaster shot
            if (distance < 4f)
            {
                if (Time.time - lastFireTime >= 2f)
                {
                    SpawnBlasterShot();
                    lastFireTime = Time.time;
                }
            }
        }

        // Rotate towards the closest enemy billion at a fixed rate
        if (closestEnemyBillion != null)
        {
            Vector2 direction = (closestEnemyBillion.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2f);
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

    private void drawHealthBar()
    {
        if (lineRenderer == null) return;

        float healthPercent = Mathf.Clamp01(currentHealth / maxHealth);
        int visibleSegments = Mathf.RoundToInt(segments * healthPercent);

        lineRenderer.positionCount = visibleSegments + 1;
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;
        float angleStep = 360f / segments;
        float angle = 0f;

        for (int i = 0; i <= visibleSegments; i++)
        {
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            float y = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
            angle += angleStep;
        }
    }

    public void HandleExp(string Color)
    {
        if (Color == billionColor)
        {
            Debug.Log("Experience given to base: " + billionColor);
            ExpController expController = ExpPrefab.GetComponent<ExpController>();
            expController.updateExp(billionColor); // Update experience
        }
    }
}
