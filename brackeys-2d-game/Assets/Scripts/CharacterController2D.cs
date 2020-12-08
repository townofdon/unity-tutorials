using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
  [Tooltip("Amount of force added when the player jumps.")]
  [SerializeField] private float m_JumpForce = 400f;

  [Tooltip("Amount of maxSpeed applied to crouching movement. 1 = 100%")]
  [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;

  [Tooltip("How much to smooth out the movement")]
  [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;

  [Tooltip("How early the player can jump before touching the ground again")]
  [Range(0, 0.5f)] [SerializeField] private float m_JumpEarlyTime = 0.2f;

  [Tooltip("How late the player can jump after leaving a surface")]
  [Range(0, 0.2f)] [SerializeField] private float m_JumpLateTime = 0.05f;

  [Tooltip("Whether or not a player can steer while jumping")]
  [SerializeField] private bool m_AirControl = false;

  [Tooltip("A mask determining what is ground to the character")]
  [SerializeField] private LayerMask m_WhatIsGround;

  [Tooltip("A position marking where to check if the player is grounded")]
  [SerializeField] private Transform m_GroundCheck;

  [Tooltip("A position marking where to check for ceilings")]
  [SerializeField] private Transform m_CeilingCheck;

  [Tooltip("A collider that will be disabled when crouching")]
  [SerializeField] private Collider2D m_CrouchDisableCollider;

  const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
  private bool m_Grounded;            // Whether or not the player is grounded.
  private bool m_GroundedTemp;
  const float k_CeilingRadius = .2f;  // Radius of the overlap circle to determine if the player can stand up
  private Rigidbody2D m_Rigidbody2D;
  private bool m_FacingRight = true;  // For determining which way the player is currently facing.
  private Vector3 m_Velocity = Vector3.zero;
  private float m_JumpEarlyTimeElapsed = 0f;
  private float m_JumpLateTimeElapsed = 0f;

  private float temp_elapsed = 0f;

  [Header("Events")]
  [Space]

  public UnityEvent OnLandEvent;

  [System.Serializable]
  public class BoolEvent : UnityEvent<bool> { }

  public BoolEvent OnCrouchEvent;
  private bool m_wasCrouching = false;

  private void Awake()
  {
    m_Rigidbody2D = GetComponent<Rigidbody2D>();

    if (OnLandEvent == null)
      OnLandEvent = new UnityEvent();

    if (OnCrouchEvent == null)
      OnCrouchEvent = new BoolEvent();
  }

  private void FixedUpdate()
  {
    m_GroundedTemp = checkIsGrounded();
    // implement coyote (hang) time before disallowing jump
    if (m_GroundedTemp)
    {
      m_Grounded = true;
      m_JumpLateTimeElapsed = 0;
    }
    else if (m_JumpLateTimeElapsed > m_JumpLateTime)
    {
      m_Grounded = false;
    }
    else
    {
      m_JumpLateTimeElapsed += Time.fixedDeltaTime;
    }
  }

  private bool checkIsGrounded()
  {
    bool wasGrounded = m_Grounded;
    // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
    // This can be done using layers instead but Sample Assets will not overwrite your project settings.
    Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
    for (int i = 0; i < colliders.Length; i++)
    {
      if (colliders[i].gameObject != gameObject)
      {
        m_Grounded = true;
        if (!wasGrounded)
          OnLandEvent.Invoke();
        return true;
      }
    }
    return false;
  }


  public void Move(float move, bool crouch, bool jump)
  {
    // If crouching, check to see if the character can stand up
    if (!crouch)
    {
      // If the character has a ceiling preventing them from standing up, keep them crouching
      if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
      {
        crouch = true;
      }
    }

    //only control the player if grounded or airControl is turned on
    if (m_Grounded || m_AirControl)
    {

      // If crouching
      if (crouch)
      {
        if (!m_wasCrouching)
        {
          m_wasCrouching = true;
          OnCrouchEvent.Invoke(true);
        }

        // Reduce the speed by the crouchSpeed multiplier
        move *= m_CrouchSpeed;

        // Disable one of the colliders when crouching
        if (m_CrouchDisableCollider != null)
          m_CrouchDisableCollider.enabled = false;
      }
      else
      {
        // Enable the collider when not crouching
        if (m_CrouchDisableCollider != null)
          m_CrouchDisableCollider.enabled = true;

        if (m_wasCrouching)
        {
          m_wasCrouching = false;
          OnCrouchEvent.Invoke(false);
        }
      }

      // Move the character by finding the target velocity
      Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
      // And then smoothing it out and applying it to the character
      m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

      // If the input is moving the player right and the player is facing left...
      if (move > 0 && !m_FacingRight)
      {
        // ... flip the player.
        Flip();
      }
      // Otherwise if the input is moving the player left and the player is facing right...
      else if (move < 0 && m_FacingRight)
      {
        // ... flip the player.
        Flip();
      }
    }

    // handle early jump button press
    if (jump && !m_Grounded)
    {
      m_JumpEarlyTimeElapsed = 0;
    }

    // If the player should jump...
    if (m_Grounded && (jump || m_JumpEarlyTimeElapsed < m_JumpEarlyTime))
    {
      // Add a vertical force to the player.
      m_Grounded = false;
      m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
    }

    m_JumpEarlyTimeElapsed += Time.fixedDeltaTime;
  }


  private void Flip()
  {
    // Switch the way the player is labelled as facing.
    m_FacingRight = !m_FacingRight;

    // Multiply the player's x local scale by -1.
    Vector3 theScale = transform.localScale;
    theScale.x *= -1;
    transform.localScale = theScale;
  }
}