using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class player_movement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D col;

    private ennemi stunned = null;
    private bool deStunIsWaiting = false;

    private float speed = 7f;
    public float jumpHeight = 1.5f;

    private bool grounded = true;
    private bool lastGrounded;
    private float fallGravityMod = 1.7f;

    float jumpSpeed;

    Vector3 velocity;
    Vector2 move;
    Vector3 mousePosition = new Vector3();

    private const float _minAcceleration = 10;

    private const float _hookMaxSpeed = 23;
    private float _hookAccelerator = 20;
    private const float _hookReset = 20;
    private Vector2 _lastHookVelocity;
    bool _isUsingHook;
    bool _lastHookUse;
    private const int _hookLenght = 10;

    [SerializeField] private LineRenderer _lineRend;
    RaycastHit2D _hit;
    RaycastHit2D[] _rayHits;
    ContactFilter2D _contactFilter;


    //residual speed after hook
    private float arbitraryX = 5;
    private float arbitraryY = 8;

    void Start()
    {
        _rayHits = new RaycastHit2D[3];
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        _lineRend.enabled = false;
    }

    void Update()
    {
        if (col == null)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);

            if (rb.position.y <= -9)
            {
                Die(2);
            }
            return;
        }

        //Debug.Log(move + " /normalized: " + move.normalized);

        velocity = rb.velocity;

        // gravity 
        if (grounded == false)
        {
            //Descente : 
            if (move.y < 0f)
            {
                move.y += Physics2D.gravity.y * rb.gravityScale * fallGravityMod * Time.deltaTime;
            }
        }
        //hook decelerattion 
        else
        {
            _hookAccelerator -= 15 * Time.deltaTime;

            //clamp
            if (_hookAccelerator <= _minAcceleration)
            {
                _hookAccelerator = _minAcceleration;
            }
        }

        _isUsingHook = HookManagement();

        // don't use hook
        if (!_isUsingHook)
        {
            if (Input.GetButtonDown("Jump") && grounded == true)
            {
                jump();
            }

            if (grounded == true)
            {
                float xMove = Input.GetAxis("Horizontal") * speed;
                move = Vector2.right * xMove;
            }
            else
            {
                move.x = Mathf.Clamp(move.x, -speed, speed);

                if (Input.GetAxis("Horizontal") != 0)
                {
                    move.x += Input.GetAxis("Horizontal") * speed * 0.1f;
                    if (!_isUsingHook)
                    {
                        move.x = Mathf.Clamp(move.x, -speed, speed);
                    }
                }
                else
                {
                    move.x *= 0.99f;
                }
            }
            move.y = velocity.y;
        }
        //hook is Used
        else
        {
            move = Vector2.zero;
            move = (new Vector3(_hit.point.x, _hit.point.y, 0) - this.transform.position).normalized * _hookAccelerator;

            _hookAccelerator += 10 * Time.deltaTime;

            //clamp
            if (_hookAccelerator >= _hookMaxSpeed)
            {
                _hookAccelerator = _hookMaxSpeed;
            }
            _lastHookVelocity = move;
        }

        if (_lastHookUse != _isUsingHook && _lastHookUse == true)
        {
            if (move.y > 0)
            {
                int divider = 25;
                move = new Vector2(_lastHookVelocity.x / divider * arbitraryX, _lastHookVelocity.y / divider * arbitraryY);
            }
        }

        if (velocity.y > 0f)
        {
            jumpSpeed = Mathf.Max(jumpSpeed - velocity.y, 0f);
        }

        rb.velocity = move;
        grounded = LookForContact();
        if (grounded != lastGrounded && grounded == false)
        {
            _hookAccelerator = _hookReset;
            move.x *= 0.7f;
        }

        lastGrounded = grounded;
        _lastHookUse = _isUsingHook;

        //look for death

    }

    private bool HookManagement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;

            Physics2D.Raycast(this.transform.position, (mousePosition - this.transform.position).normalized, _contactFilter, _rayHits, _hookLenght);

            for (int i = 0; i < _rayHits.Length; i++)
            {
                if (_rayHits[i] != false)
                {
                    if (_rayHits[i].transform.tag != this.transform.tag)
                    {
                        if (_rayHits[i].transform.name != "cameraBounds")
                        {
                            _hit = _rayHits[i];
                            i = _rayHits.Length;
                            _rayHits = new RaycastHit2D[3];
                        }
                    }
                }
            }
        }
        if (Input.GetMouseButton(0))
        {
            //
            Debug.DrawRay(this.transform.position, (mousePosition - this.transform.position).normalized * _hookLenght, Color.red);
            Debug.DrawLine(this.transform.position, mousePosition, Color.black);

            if (_hit != false)
            {
                _lineRend.enabled = true;
                _lineRend.SetPosition(0, this.transform.position);
                _lineRend.SetPosition(1, _hit.point);


                if (_hit.transform.GetComponent<ennemi>() != null)
                {
                    stunned = _hit.transform.GetComponent<ennemi>();
                    stunned.speed = 0;
                }
                return (true);
            }

            return (false);
        }
        else
        {
            _hit = new RaycastHit2D();
            _lineRend.enabled = false;

            if (stunned != null && !deStunIsWaiting)
            {
                Invoke("DeStun", 0.7f);
                deStunIsWaiting = true;
            }
            return (false);
        }
    }

    private bool LookForContact()
    {
        RaycastHit2D Rayhits;
        Vector3 TransformBottom = this.transform.position - new Vector3(0, this.transform.lossyScale.y / 2 + 0.01f);
        for (float i = -(this.transform.lossyScale.x / 2); i <= this.transform.lossyScale.x / 2; i += this.transform.lossyScale.x / 10)
        {
            // so it doesn't triggrer on Walls
            if (i == -(this.transform.lossyScale.x / 2))
            {
                Rayhits = Physics2D.Raycast(TransformBottom + new Vector3(i + 0.01f, 0), TransformBottom + new Vector3(i + 0.01f, 0.01f), 0.01f, 1);
            }
            else if (i >= this.transform.lossyScale.x / 2)
            {
                Rayhits = Physics2D.Raycast(TransformBottom + new Vector3(i - 0.01f, 0), TransformBottom + new Vector3(i - 0.01f, 0.01f), 0.01f, 1);
            }
            else
            {
                Rayhits = Physics2D.Raycast(TransformBottom + new Vector3(i, 0), TransformBottom + new Vector3(i, 0.01f), 0.01f, 1);
            }

            if (Rayhits.transform != this.transform && Rayhits.transform != null)
            {
                return true;
            }
        }
        return false;
    }

    void jump()
    {
        velocity = rb.velocity;

        jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
        velocity.y = jumpSpeed;
        grounded = false;
    }

    void DeStun()
    {
        stunned.speed = stunned.startingSpeed;
        stunned = null;
        deStunIsWaiting = false;
    }

    void Die(float delayTime)
    {
        StartCoroutine(DelayAction(delayTime));
    }

    IEnumerator DelayAction(float delayTime)
    {
        //Wait for the specified delay time before continuing.
        yield return new WaitForSeconds(delayTime);

        SceneManager.LoadScene("SampleScene");

        //Do the action after the delay time has finished.
    }
}