using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
  public float jumpForce = 5f;

  private Rigidbody2D rb;
  private BoxCollider2D boxCollider;
  private float rbHeight = 0f;
  private float rbWidth = 0f;


  private float speed = 10;
  private float groundedDistance = 0.03f;
  private bool isGrounded = false;
  private float coyoteTime = 0.1f;
  private float currentCoyoteTime = 0f;


  // CACHED VALUES
  // The size of the array determines how many raycasts will occur
  RaycastHit2D[] _groundRaycastResults = new RaycastHit2D[1];
  private float _inputX = 0f;
  private Vector2 _bottomPos;

  // Start is called before the first frame update
  void Start()
  {
    rb = gameObject.GetComponent<Rigidbody2D>();
    boxCollider = gameObject.GetComponent<BoxCollider2D>();
    rbHeight = boxCollider.bounds.size.y;
    rbWidth = boxCollider.bounds.size.x;

    if (!rb)
    {
      Debug.LogError("Player does not have a Rigidbody2D component!!");
    }

    if (!boxCollider)
    {
      Debug.LogError("Player does not have a BoxCollider2D component!!");
    }
  }

  // Update is called once per frame
  void Update()
  {
    _inputX = Input.GetAxisRaw("Horizontal");

    if (Mathf.Abs(_inputX) > 0)
    {
      Walk(_inputX);
    }

    if (Input.GetAxisRaw("Jump") > 0 && isGrounded)
    {
      Jump();
    }
  }

  void FixedUpdate()
  {
    _bottomPos = rb.position - new Vector2(0f, rbHeight / 2);

    bool realIsGrounded = CheckIsTouchingGround();

    if (realIsGrounded)
    {
      isGrounded = true;
      currentCoyoteTime = 0f;
    }

    // simulate hang-time
    if (isGrounded && !realIsGrounded)
    {
      if (currentCoyoteTime >= coyoteTime)
      {
        isGrounded = false;
      }
      else
      {
        currentCoyoteTime += Time.deltaTime;
      }
    }
  }

  bool CheckIsTouchingGround()
  {
    int numHits = boxCollider.Raycast(
      Vector2.down,
      _groundRaycastResults,
      groundedDistance + rbHeight / 2
    );
    if (numHits <= 0) return false;

    float distance = Mathf.Abs(_groundRaycastResults[0].point.y - _bottomPos.y);
    return distance <= groundedDistance;
  }

  void Walk(float x)
  {
    Vector3 input = new Vector3(
      x,
      0,
      0
    );

    Vector3 direction = input.normalized;
    Vector3 velocity = direction * speed;
    Vector3 moveAmount = velocity * Time.deltaTime;

    transform.position += moveAmount;
  }

  void Jump()
  {
    isGrounded = false;
    rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
  }
}
