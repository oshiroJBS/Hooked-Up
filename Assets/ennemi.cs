using UnityEngine;

public class ennemi : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody2D rb;
    Vector2 move;
    float _JumpAngle;
    float _PlayerAngle;
    player_movement _Player;

    public float startingSpeed = 2f;
    public float speed = 2f;

    private float _VisionAngle = 27f;
    private float _VisionDistance = 11f;

    [SerializeField] private Transform _BulletSpwanner1;
    [SerializeField] private Transform _BulletSpwanner2;
    [SerializeField] private GameObject _Bullet;
    private float _ShootTimer = 0f;
    private float _ShootCooldown = 2f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        if (_Player == null) _Player = GameObject.FindObjectOfType<player_movement>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float Vangle = _VisionAngle;
        _ShootTimer += Time.deltaTime;

        // this one is not good, problem with angle 
        // to change
        if (speed > 0)
        {
            _PlayerAngle = Vector2.Angle(_Player.transform.position - this.transform.position, this.transform.forward);

            //
            Debug.DrawRay(this.transform.position, Quaternion.AngleAxis(-Vangle, new Vector3(0, 0, 1)) * this.transform.forward * _VisionDistance);
            Debug.DrawRay(this.transform.position, Quaternion.AngleAxis(Vangle, new Vector3(0, 0, 1)) * this.transform.forward * _VisionDistance);
        }
        else
        {
            _PlayerAngle = Vector2.Angle(_Player.transform.position - this.transform.position, -this.transform.forward);

            //
            Debug.DrawRay(this.transform.position, Quaternion.AngleAxis(-Vangle, new Vector3(0, 0, 1)) * -this.transform.forward * _VisionDistance);
            Debug.DrawRay(this.transform.position, Quaternion.AngleAxis(Vangle, new Vector3(0, 0, 1)) * -this.transform.forward * _VisionDistance);
        }

        // is player in distance and angle
        if (Vector2.Distance(this.transform.position, _Player.transform.position) <= _VisionDistance && Mathf.Abs(_PlayerAngle) <= Vangle)
        {
            Debug.DrawRay(this.transform.position, _Player.transform.position - this.transform.position, Color.red);
        }
        else
        {
            Debug.DrawRay(this.transform.position, _Player.transform.position - this.transform.position, Color.blue);
        }

        RaycastHit2D _hit = Physics2D.Raycast(this.transform.position, _Player.transform.position - this.transform.position, _VisionDistance);

        if (/*_hit.GetType() == typeof(player_movement) &&*/ _ShootTimer >= _ShootCooldown)
        {
            Shoot();
            _ShootTimer = 0;
        }
        // Mouvement
        move.x = speed;
        move.y = rb.velocity.y + Physics2D.gravity.y * rb.gravityScale * Time.deltaTime;

        rb.velocity = move;
        //
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 normal = collision.GetContact(0).normal;

        if (collision.collider.CompareTag("Player"))
        {
            _JumpAngle = Vector2.Angle(_Player.transform.position - this.transform.position, this.transform.up);

            if (Mathf.Abs(_JumpAngle) <= 30f)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Destroy(collision.collider);
            }
        }
        else if (normal.y == 0)
        {
            speed *= -1;
        }
    }

    void Shoot()
    {
        if (speed > 0)
        {
            //Instantiate(_Bullet, _BulletSpwanner1.position, Quaternion.AngleAxis(_PlayerAngle, new Vector3(1, 0, 0)));
            Instantiate(_Bullet, _BulletSpwanner2.position, new Quaternion(0, 0, 1 * _PlayerAngle, 0));
        }
        else
        {
            Instantiate(_Bullet, _BulletSpwanner2.position, new Quaternion(0, 0, 1 * _PlayerAngle, 0));
        }
    }

}
