using UnityEngine;

public class BillionController : MonoBehaviour
{
    // Billion character will be pushed away when it collides with any object
    // This script is attached to the billion character

    public float pushForce = 2f;

    private void OnCollisionEnter(Collision collision)
    {
        // Apply force when colliding with the Base or another Billion
        if (collision.gameObject.CompareTag("Base") || collision.gameObject.CompareTag("Billion"))
        {
            Vector3 pushDirection = transform.position - collision.transform.position;
            pushDirection.y = 0; // Keep movement horizontal

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(pushDirection.normalized * pushForce, ForceMode.Impulse);
            }
        }
    }
}
