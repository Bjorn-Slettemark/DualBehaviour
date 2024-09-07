using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{


    public int score = 0;

    public float moveSpeed = 5.0f;

    public float jumpForce = 10.0f;

    public bool isGrounded = false;

    [SerializeField] public Rigidbody rb;


    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");

        Vector3 movement = new Vector3(moveHorizontal * moveSpeed, rb.velocity.y, 0f);

        rb.velocity = movement;

        if (Input.GetButtonDown("Jump") && isGrounded == true)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }




    void OnTriggerEnter(Collider collisionObject)
    {
        if (collisionObject.CompareTag("Coin"))
        {
            score = score + 1;
            Destroy(collisionObject.gameObject);
        }
    }


    void OnCollisionEnter(Collision collisionObject)
    {
        if (collisionObject.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}