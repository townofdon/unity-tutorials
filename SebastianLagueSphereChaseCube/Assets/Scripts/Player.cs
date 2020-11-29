using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 10;

    // Update is called once per frame
    void Update()
    {
        Vector3 input = new Vector3(
          // Input.GetAxis vs. Input.GetAxisRaw
          // Input.GetAxis smoothes the input over time, whereas
          // Input.GetAxisRaw applies no smoothing. The latter is
          // prefferable if you want to add your own easing.
          Input.GetAxisRaw("Horizontal"),
          0,
          Input.GetAxisRaw("Vertical")
        );
        // print(input);

        Vector3 direction = input.normalized;
        Vector3 velocity = direction * speed;
        Vector3 moveAmount = velocity * Time.deltaTime;

        transform.position += moveAmount;
    }
}
