using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerBehavior : MonoBehaviour
{
    [Header("Player Movement")]
    public float horizontalForce;
    public float horizontalSpeed;
    public float verticalForce;
    public float airFactor;
    public Transform groundPoint;
    public float groundRadius;
    public LayerMask groundLayerMask;
    public bool isGrounded;


    [Header("Controller")]
    public Joystick leftJoystick;
    [Range(0.1f, 1.0f)]
    public float horizontalSensitivity;
    [Range(0.1f, 1.0f)]
    public float verticalSensitivity;

    [Header("Animations")]
    public Animator animator;
    public PlayerAnimationState playerAnimationState;

    private Rigidbody2D rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        leftJoystick = GameObject.FindObjectOfType<Joystick>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Collider2D hit = Physics2D.OverlapCircle(groundPoint.position, groundRadius, groundLayerMask);
        isGrounded = hit;
        
        Move();
        Jump();
        AirCheck();
    }

    private void Move()
    {
        float joysticHorizontal = leftJoystick != null ? leftJoystick.Horizontal : 0.0f;

        float x = Input.GetAxisRaw("Horizontal") + joysticHorizontal * horizontalSensitivity;
        if (x != 0.0f)
            x = x > 0.0f ? 1.0f : -1.0f;
        rigidbody.AddForce(Vector2.right * x * horizontalForce * (isGrounded ? 1 : airFactor));
        Vector2 newVelocity = new Vector2(Mathf.Clamp(rigidbody.velocity.x, -horizontalSpeed, horizontalSpeed), rigidbody.velocity.y);
        rigidbody.velocity = newVelocity;

        Flip(x);

        ChangeAnimation(PlayerAnimationState.RUN);

        if ( isGrounded && x == 0.0f)
        {
            ChangeAnimation(PlayerAnimationState.IDLE);
        }
    }

    private void AirCheck()
    {
        if (!isGrounded)
        {
            ChangeAnimation(PlayerAnimationState.JUMP);
        }
    }

    private void Flip(float x)
    {
        if (x != 0.0f)
        {
            bool flip = false;
            if (x < 0.0f)
            {
                flip = true;
            }
            GetComponent<SpriteRenderer>().flipX = flip;
        }
    }
   
    private void Jump()
    {
        float joysticVertical = leftJoystick != null ? leftJoystick.Vertical : 0.0f;

        float y = Input.GetAxis("Jump") + joysticVertical;
        if (isGrounded && y > verticalSensitivity)
        {
            rigidbody.AddForce(Vector2.up * verticalForce, ForceMode2D.Impulse);
        }
    }

    private void ChangeAnimation(PlayerAnimationState animationState)
    {
        playerAnimationState = animationState;
        animator.SetInteger("AnimationState", (int)playerAnimationState);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundPoint.position, groundRadius);
    }
}
