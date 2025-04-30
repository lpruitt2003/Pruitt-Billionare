using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class BaseSpawner : MonoBehaviour
{
    public GameObject[] basePrefabs; // Array of base prefabs
    public float safeSpawnHeight = 1f; // Adjust if needed
    public float maxSpawnDistance = 10f; // Maximum distance from the center of the tilemap to spawn bases
    public Tilemap tileMap;

    //iterate through the base prefabs and spawn them
    private void Start()
    {
        foreach (GameObject basePrefab in basePrefabs)
        {
            SpawnBase(basePrefab);
        }
    }

    //place a base prefab randomly on top of the Tilemap
    private void SpawnBase(GameObject basePrefab)
    {
        Vector3Int randomTilePosition = GetRandomTilePosition();
        Vector3 spawnPosition = tileMap.GetCellCenterWorld(randomTilePosition);
        spawnPosition.y += safeSpawnHeight; // Adjust height to avoid clipping

        GameObject newBase = Instantiate(basePrefab, spawnPosition, Quaternion.identity);
        newBase.transform.position = new Vector3(newBase.transform.position.x, newBase.transform.position.y, -1f); // Set z to -1
        newBase.transform.parent = transform; // Set the base as a child of this object
    }

    // Get a random tile position within the bounds of the tilemap
    private Vector3Int GetRandomTilePosition()
    {
        BoundsInt bounds = tileMap.cellBounds;
        int randomX = Random.Range(bounds.x, bounds.xMax);
        int randomY = Random.Range(bounds.y, bounds.yMax);
        Vector3Int randomTilePosition = new Vector3Int(randomX, randomY, 0);

        // Check if the tile is walkable (not occupied by a wall)
        while (tileMap.GetTile(randomTilePosition) == null)
        {
            randomX = Random.Range(bounds.x, bounds.xMax);
            randomY = Random.Range(bounds.y, bounds.yMax);
            randomTilePosition = new Vector3Int(randomX, randomY, 0);
        }

        return randomTilePosition;
    }
}
