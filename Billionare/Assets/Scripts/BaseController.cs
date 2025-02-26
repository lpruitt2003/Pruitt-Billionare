using System.Collections;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    // Script to spawn a character prefab next to the base
    // This script is attached to the base

    public GameObject characterPrefab;
    public float spawnDistance = 1.5f;
    public float safeSpawnHeight = 1f; // Adjust if needed

    private void Start()
    {
        StartCoroutine(SpawnCharacter());
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
}
