using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, 1)] [SerializeField] private float m_WalkSpeed = .36f;
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
	[SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;
	public bool floating;
	public float jumpDelay = 0.5f;
	public bool doubleJumpReady = false;
	public CameraShake cameraShake;

	[Header("Events")]
	[Space]
	
	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;

	public BoolEvent OnWalkEvent;
	private bool m_wasWalking = false;

	public bool jump = false;
	private int extraJump;
	public int extraJumpValue;

	public Animator animator;
	public ParticleSystem dust;

	private void Awake()
	{
		extraJump = extraJumpValue;
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();

		if (OnWalkEvent == null)
			OnWalkEvent = new BoolEvent();
	}
	private void FixedUpdate()
	{
		jump = false;
		Physics2D.gravity = new Vector2(0, -9.8f);
		bool wasGrounded = m_Grounded;
		m_Grounded = false;
		floating = false;
		animator.SetBool("isTurning", false);


		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
				//StartCoroutine(cameraShake.Shake(.15f, .4f));
				//StartCoroutine(cameraShake.Shaking());
			}
		}
		/*if (!wasGrounded && Input.GetButton("Jump")) REPÜLÉS ANIMÁCIÓ
		{
			Physics2D.gravity = new Vector2(0,-0.8f);
			floating = true;
			animator.SetBool("isJumping", false);
			animator.SetBool("isFloating", true);
		}*/
		
		if (!m_Grounded && Input.GetButton("Jump") && m_Rigidbody2D.velocity.y < 0f) // floating, grind
		{
			Physics2D.gravity = new Vector2(0, -0.8f);
			jump = false;
			floating = true;
			animator.SetBool("isJumping", false);
			animator.SetBool("isFloating", true);
		}
	}
	private void Update()
    {
		if (m_Grounded == true)
		{
			extraJump = extraJumpValue;			
        }
       
		if (Input.GetButtonDown("Jump") && extraJump > 0) {
			CreateDust();
			jump = true;
			animator.SetBool("isJumping", true);
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
			if (doubleJumpReady)
            {
				DoubleJump();	
			}
			else
            {
				PrepareJump();
            }
		}
	}

	void DoubleJump()
    {
		doubleJumpReady = false;
		m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce/4));
		animator.SetBool("isJumping", false);
		animator.SetBool("jumpedAlready", true);
		extraJump--;
		cameraShake.start = true;
	}
	void PrepareJump()
    {
		//this is where the handling happens
		CancelInvoke("NoJump");
		Invoke("NoJump", jumpDelay);
		doubleJumpReady = true;
	}

	void NoJump()
    {
		doubleJumpReady = false;
	}

    public void Move(float move, bool crouch, bool jump, bool walk)
	{
		if (!crouch)
		{
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				crouch = true;
			}
		}
		if (m_Grounded && jump)
		{
			m_Grounded = false;
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
		}
		if (walk)
        {
			if (!m_wasWalking)
            {
				m_wasWalking = true;
				OnWalkEvent.Invoke(true);
			}
			move *= m_WalkSpeed;
		}
		else
        {
			if (m_wasWalking)
			{
				m_wasWalking = false;
				OnWalkEvent.Invoke(false);
			}
		}


		if (m_Grounded || m_AirControl)
		{
			if (crouch)
			{
				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}
				move *= m_CrouchSpeed;

				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			}
			else
			{
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			if (move > 0 && !m_FacingRight)
			{
				CreateDust(); // PARTICLE WHEN TURNING
				Flip();
				//animator.SetBool("isTurning", true);
			}
			else if (move < 0 && m_FacingRight)
			{
				CreateDust(); // PARTICLE WHEN TURNING
				Flip();
				//animator.SetBool("isTurning", false);
			}
		}
	}


	private void Flip()
	{
		m_FacingRight = !m_FacingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
		animator.SetBool("isTurning", true);
	}

	void CreateDust()
    {
		dust.Play();
    }
}