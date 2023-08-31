using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;//postavljamo vrijednost pomocu ove skripte
    public float walkingSpeed;
    public float sprintingSpeed;

    Rigidbody rb;
    public Transform orientation;//smjer u kojem je objekt
    public Transform groundCheckPosition;//(NE KORISTI SE)

    public float groundDrag;//rigidbody drag kada smo na tlu

    public bool isGrounded;
    public bool isJumpReady;

    public KeyCode jumpKey=KeyCode.Space;//za izmjenu tipki u inspektoru
    public KeyCode sprintingKey = KeyCode.LeftShift;

    public float horizontalInput;
    public float verticalInput;
    public float jumpForce;
    public float airMultiplier;

    public Vector3 moveDirection;
    public LayerMask groundCheck;


    public float jumpCooldown;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        isJumpReady = true;
    }

    private void Update()
    {
        InputKey();
        GroundCheck();

    }
    void FixedUpdate()
    {
        MovePlayer();
    }

    void InputKey()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(jumpKey) && isGrounded && isJumpReady)
        {
            isJumpReady = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKey(sprintingKey) && isGrounded)
        {
            moveSpeed = sprintingSpeed;
        }
        else
        {
            moveSpeed = walkingSpeed;
        }
    }

    void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (isGrounded)
        {
            rb.AddForce(moveDirection * moveSpeed * 10f, ForceMode.Force);

        }
        else if (!isGrounded)
        {
            rb.AddForce(moveDirection * moveSpeed * 10f * airMultiplier, ForceMode.Force);//kretanje kada sam u zraku
        }
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void ResetJump()
    {
        isJumpReady = true;
    }

    private void GroundCheck()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 2f, groundCheck);
    }
}
