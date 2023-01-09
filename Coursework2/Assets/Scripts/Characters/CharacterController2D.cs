using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
	[SerializeField] private float m_ClimbSpeed = 50f;							// The speed that the player can climb walls.
	[SerializeField] private int m_NumberOfJumps = 1;							// If the player can double jump
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
	[SerializeField] private Transform m_climb_check;							// A position marking where to check if there is a surface to be climbed

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	const float k_ClimbableRadius = .2f; // Radius of the overlap circle to determine if able to climb
	private bool m_Grounded;            // Whether or not the player is grounded.
	private bool m_IsAllowedToClimb = true; // Whether or not the player has the ability to climb
	private bool m_CanClimb;            // Whether or not the player can climb
	private bool m_IsClimbing;
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;
	private int jumpsRemaining;

	[Header("Events")]
	[Space]

	public UnityEvent OnJumpedEvent;
	public UnityEvent OnLandEvent;
	public UnityEvent OnClimbStartEvent;
	public UnityEvent OnClimbStopEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool>
	{
	}

	private void Awake() {
		Vector3 theScale = transform.localScale;
		if (theScale.x < 0) {
			m_FacingRight = false;
		}
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();
		if (OnClimbStartEvent == null)
			OnClimbStartEvent = new UnityEvent();
		if (OnClimbStopEvent == null)
			OnClimbStopEvent = new UnityEvent();
		if (OnJumpedEvent == null)
			OnJumpedEvent = new UnityEvent();


		jumpsRemaining = m_NumberOfJumps;
	}

	private void FixedUpdate() {
		bool wasGrounded = m_Grounded;
		m_Grounded = false;
		m_CanClimb = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++) {
			if (colliders[i].gameObject != gameObject) {
				m_Grounded = true;
				if (!wasGrounded) {
					OnLandEvent.Invoke();
					jumpsRemaining = m_NumberOfJumps;
				}
			}
		}

		colliders = Physics2D.OverlapCircleAll(m_climb_check.position, k_ClimbableRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++) {
			if (colliders[i].gameObject != gameObject) {
				m_CanClimb = true;
			}
		}
		if (!m_CanClimb && m_IsClimbing) {
			m_IsClimbing = false;
			OnClimbStopEvent.Invoke();
		}
	}

	public void changeSettings(bool canDoubleJump, bool canWallClimb) {
		if (canDoubleJump) {
			m_NumberOfJumps = 2;
		} else {
			m_NumberOfJumps = 1;
		}
		jumpsRemaining = m_NumberOfJumps;
		m_IsAllowedToClimb = canWallClimb;
	}


	public void Move(float move, bool jump, bool jumpedThisUpdate) {

		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl) {

			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight) {
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight) {
				// ... flip the player.
				Flip();
			}
		}

		if (m_IsAllowedToClimb && m_CanClimb && jump) {
			if (!m_IsClimbing)
				OnClimbStartEvent.Invoke();
			m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, m_ClimbSpeed);
			m_IsClimbing = true;
		}
		else if (m_IsClimbing && !jump) {
			OnClimbStopEvent.Invoke();
			m_IsClimbing = false;
		} else if ((m_Grounded && jumpedThisUpdate) || (jumpsRemaining > 0 && jumpedThisUpdate)) {
			// Add a vertical force to the player.
			m_Grounded = true;
			m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0f);
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
			jumpsRemaining--;
			OnJumpedEvent.Invoke();
		}
	}


	public void Flip() {
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	public void FaceLeft() {
		if (m_FacingRight) {
			Flip();
		}
	}

	public void FaceRight() {
		if (!m_FacingRight) {
			Flip();
		}
	}
}