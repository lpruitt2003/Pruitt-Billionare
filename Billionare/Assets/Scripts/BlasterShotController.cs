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
            BillionController billion = other.GetComponent<BillionController>();
            if (billion != null && billion.billionColor != billionColor)
            {
                billion.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
        else if (other.CompareTag("Base"))
        {
            BaseController baseController = other.GetComponent<BaseController>();
            if (baseController != null && baseController.billionColor != billionColor)
            {
                baseController.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
        else
        {
            // Destroy the blaster shot if it hits anything else
            Destroy(gameObject);
        }
    }
}
