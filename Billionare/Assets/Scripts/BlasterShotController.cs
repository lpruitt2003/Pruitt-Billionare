using UnityEngine;

public class BlasterShotController : MonoBehaviour
{
    //public BillionController billion; // Reference to the BillionController script
    public float speed = 2f;
    public float damage = 20f;
    public float lifetime = 5f;

    public string billionColor; // "Green", "Yellow", "Red", "Blue"

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D not found on BlasterShot!");
            return;
        }

        rb.linearVelocity = transform.up * speed;
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Billion"))
        {
            BillionController otherBillion = other.GetComponent<BillionController>();
            if (otherBillion != null && otherBillion.billionColor == billionColor)
            {
                // Ignore collision with the same color billion
                return;
            }

            // Deal damage to the billion
            otherBillion.TakeDamage(damage);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
