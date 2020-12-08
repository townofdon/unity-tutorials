using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
  public CharacterController2D controller;
  public float runSpeed = 40f;

  private float moveX = 0;
  private bool isJumping = false;
  private bool isCrouching = false;

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    moveX = Input.GetAxisRaw("Horizontal") * 40f;

    // This will only be `true` for a single frame on each keydown press.
    if (Input.GetButtonDown("Jump"))
    {
      isJumping = true;
    }

    // The following method achieves a "crouch while holding" behavior
    if (Input.GetButtonDown("Crouch"))
    {
      isCrouching = true;
    }
    else if (Input.GetButtonUp("Crouch"))
    {
      isCrouching = false;
    }
  }

  void FixedUpdate()
  {
    controller.Move(moveX * Time.fixedDeltaTime, isCrouching, isJumping);
    isJumping = false;
  }
}
