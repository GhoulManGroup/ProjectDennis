using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //https://www.youtube.com/watch?v=F20Sr5FlUlE
    //https://www.youtube.com/watch?v=xCxSjgYTw9c

    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float RunSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplyer;
    [SerializeField] bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Keybinds")]
    public KeyCode jumpkey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Grounded")]
    [SerializeField] float playerHeight;
    public LayerMask whatIsGround;
    [SerializeField] bool grounded;

    [Header("Slope)]

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState moveState;

    public enum MovementState
    {
        walking, sprinting, air, crouching
    }

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        startYScale = transform.localScale.y;
    }

    private void Update()
    {
       grounded = Physics.Raycast(transform.position, new Vector3(0f,-0.5f,0f), playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        MoveStateHandler();

        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    public void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpkey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }


    }

    private void MoveStateHandler()
    {
        if ( grounded && Input.GetKeyDown(crouchKey))
        {
            moveState = MovementState.crouching;
            moveSpeed = crouchSpeed;
            Debug.Log(moveState + "Crouching");
        }
        //Walk
        else if (grounded && Input.GetKey(sprintKey))
        {
            moveState = MovementState.sprinting;
            moveSpeed = RunSpeed;
        }
        //Run
        else if (grounded && !Input.GetKey(crouchKey))
        {
            moveState = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        //Air 
        else
        {
            moveState = MovementState.air;
        }

        Debug.Log(moveState + "After");
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplyer, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);


        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}
