using UnityEngine;

public class bulletScript : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody2D rb;
    public float _bulletSpeed = 30f;
    private float _DespawnTimer = 0f;
    private float _DespawnCooldown = 3f;

    // Update is called once per frame
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(-this.transform.right * _bulletSpeed, ForceMode2D.Impulse);
    }
    void FixedUpdate()
    {
        _DespawnTimer += Time.deltaTime;

        if (_DespawnTimer >= _DespawnCooldown)
        {
            Die();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Destroy(collision.collider);
        }
            Die();
    }

    void Die()
    {
        Destroy(this.gameObject);
    }
}
