using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    private Rigidbody2D rb;
    public float jumpForce;
    public float moveDirection;
    public Transform groundChecker;
    public float checkRadius;
    public LayerMask ground;
    public LayerMask climbLayer;
    public float rayDist;
    public float climbSpeed;
    public Transform groundLevel;
    public bool inWater;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = Input.GetAxisRaw("Horizontal");
        rb.linearVelocityX = moveDirection * speed;
        ClimbMechanics();
        if (inWater)
        {
            WaterMovement();
        } else
        {
            JumpMechanics();
        }
    }

    void JumpMechanics()
    {
        Collider2D groundCheck = Physics2D.OverlapCircle(groundChecker.position, checkRadius, ground);

        if (groundCheck != null)
        {
            if (Input.GetKeyDown("space"))
            {
                rb.linearVelocity = Vector2.up * jumpForce;
            }
        }
    }

    void ClimbMechanics()
    {
        RaycastHit2D forwardRay = Physics2D.Raycast(groundLevel.position, transform.right, rayDist, climbLayer);
        RaycastHit2D backwardRay = Physics2D.Raycast(groundLevel.position, transform.right * - 1, rayDist, climbLayer);
        Debug.DrawRay(groundLevel.position, transform.right * rayDist, forwardRay.collider ? Color.red : Color.green);
        Debug.DrawRay(groundLevel.position, transform.right * - 1 * rayDist, backwardRay.collider ? Color.red : Color.green);

        if (forwardRay && moveDirection == 1)
        {
            rb.linearVelocity = new Vector2(0, climbSpeed);
        }
        if (backwardRay && moveDirection == -1)
        {
            rb.linearVelocity = new Vector2(0, climbSpeed);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundChecker.position, checkRadius);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            inWater = true;
            rb.gravityScale /= 2;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            inWater = false;
            rb.gravityScale *= 2;
        }
    }

    private void WaterMovement()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.linearVelocity = Vector2.up * (jumpForce / 2);
        }
    }

}
