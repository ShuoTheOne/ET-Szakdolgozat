using UnityEngine;

public class Character_Controller_Old : MonoBehaviour
{

    public float movementSpeed;
    public float JumpForce;

    private Rigidbody2D _rigidbody;

    private bool faceRight = true;

    private bool isGrounded;
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask whatIsGround;

    private int extraJump;
    public int extraJumpValue;

    public Animator animator;


    void Start()
    {
        extraJump = extraJumpValue;
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

        var movement = Input.GetAxis("Horizontal");
        transform.position += new Vector3(movement, 0, 0) * Time.deltaTime * movementSpeed;

        animator.SetFloat("Speed", Mathf.Abs(movementSpeed));


        if (Input.GetButtonDown("Jump") && Mathf.Abs(_rigidbody.velocity.y) < 0.001f)
        {
            _rigidbody.AddForce(new Vector2(0,JumpForce), ForceMode2D.Impulse);
            animator.SetBool("isJumping", true);
        }

        if(faceRight == false && movement > 0)
        {
            Flip();
        }
        else if (faceRight == true && movement < 0)
        {
            Flip();
        }
    }

    public void OnLanding()
    {
        animator.SetBool("isJumping", false);
    }

    private void Update()
    {
        if(isGrounded == true)
        {
            extraJump = extraJumpValue;
        }

        if (Input.GetButtonDown("Jump") && extraJump > 0)
        {
            _rigidbody.velocity = Vector2.up * JumpForce;
            extraJump--;
        } else if (Input.GetButtonDown("Jump") && extraJump == 0 && isGrounded == true) {
            _rigidbody.velocity = Vector2.up * JumpForce;
        }
    }

    void Flip()
    {
        faceRight = !faceRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }
}
