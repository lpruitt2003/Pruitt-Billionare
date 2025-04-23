using UnityEngine;

public class ExpController : MonoBehaviour
{
    public bool giveExp = false;
    public string billionColor;
    public float experienceRequired = 100f;
    public float experienceAmount = 0f;
    [SerializeField] private float radius = 0.5f;
    [SerializeField] private int segments = 100;
    public float rank = 1f;

    private LineRenderer lineRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
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

        drawExpBar();
    }

    void Update()
    {
    
    }

    public void drawExpBar()
    {
        if (lineRenderer == null) return;

        float ExpPercent = Mathf.Clamp01(experienceAmount / experienceRequired);
        int visibleSegments = Mathf.RoundToInt(segments * ExpPercent);

        lineRenderer.positionCount = visibleSegments + 1;
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
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

    // Detects if base receives Experience from a billion
    public void updateExp(string Color)
    {
        if (Color == billionColor)
        {
            Debug.Log("Experience given to base: " + billionColor);
            AddExperience(15f); // Add experience points
        }
    }

    // Adds experience points to the base
    public void AddExperience(float Amount)
    {
        experienceAmount += Amount;
        if (experienceAmount >= experienceRequired)
        {
            experienceAmount -= experienceRequired; // Reset experience amount
            rank += 1f; // Increase rank
            Debug.Log("Base leveled up! New rank: " + rank);
            AddExperience(experienceAmount); // Add remaining experience
        }

        drawExpBar();
    }   
}
