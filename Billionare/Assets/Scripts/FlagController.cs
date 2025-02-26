using UnityEngine;
using System.Collections.Generic;

public class FlagPlacement : MonoBehaviour
{
    public GameObject yellowFlag; // Assign a flag prefab in the inspector
    public GameObject greenFlag; // Assign a flag prefab in the inspector
    public Camera mainCamera;
    
    private Dictionary<string, List<GameObject>> flags = new Dictionary<string, List<GameObject>>()
    {
        { "Green", new List<GameObject>() },
        { "Yellow", new List<GameObject>() }
    };

    private GameObject selectedFlag = null;
    private LineRenderer lineRenderer;
    private string currentColor = "Green"; // Default color for placing flags

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Initialize a LineRenderer for drawing movement lines
        GameObject lineObj = new GameObject("FlagMovementLine");
        lineRenderer = lineObj.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.gray;
        lineRenderer.endColor = Color.gray;
        lineRenderer.positionCount = 0;
    }

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        // Switch flag color using keyboard (optional)
        if (Input.GetKey(KeyCode.Alpha1)) currentColor = "Green";
        if (Input.GetKey(KeyCode.Alpha2)) currentColor = "Yellow";

        if (Input.GetMouseButtonDown(0)) // Left-click places/moves Green flags
        {
            HandleFlagPlacement("Green");
        }
        else if (Input.GetMouseButtonDown(1)) // Right-click places/moves Yellow flags
        {
            HandleFlagPlacement("Yellow");
        }

        // Dragging logic
        // place selected flag at the end of the line
        if (selectedFlag != null)
        {
            Vector3 mousePosition = GetMouseWorldPosition();
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, selectedFlag.transform.position);
            lineRenderer.SetPosition(1, mousePosition);
        }

        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            Vector3 mousePosition = GetMouseWorldPosition();
            selectedFlag.transform.position = mousePosition;
            selectedFlag = null;
            lineRenderer.positionCount = 0; // Hide the line
        }
    }

    private void HandleFlagPlacement(string color)
    {
        Vector3 spawnPosition = GetMouseWorldPosition();
        
        List<GameObject> playerFlags = flags[color];

        if (playerFlags.Count < 2) // Place new flag
        {
            GameObject newFlag = Instantiate(color == "Green" ? greenFlag : yellowFlag, spawnPosition, Quaternion.identity);
            playerFlags.Add(newFlag);
        }
        else // Move nearest flag
        {
            GameObject closestFlag = GetNearestFlag(playerFlags, spawnPosition);
            if (closestFlag != null)
            {
                selectedFlag = closestFlag;
                closestFlag.transform.position = spawnPosition;
            }
        }
    }

    private GameObject GetNearestFlag(List<GameObject> playerFlags, Vector3 position)
    {
        GameObject nearestFlag = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject flag in playerFlags)
        {
            float distance = Vector3.Distance(flag.transform.position, position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestFlag = flag;
            }
        }

        return nearestFlag;
    }

   private Vector3 GetMouseWorldPosition()
{
    Vector3 mousePosition = Input.mousePosition;
    mousePosition.z = 10f; // Distance in front of the camera

    Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
    worldPosition.z = -1f; // Lock z position

    return worldPosition;
}
}

