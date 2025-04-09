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
    public string billionColor; // "Green", "Yellow", "Red", "Blue"
    public GameObject blasterShotPrefab; // Assign this in the Inspector

    private void Start()
    {
        StartCoroutine(SpawnCharacter());
    }

    private void Update()
    {
        TargetBillion();
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
}
