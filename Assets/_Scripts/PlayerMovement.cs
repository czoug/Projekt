using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float rotationSpeed;

    public float groundDrag;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    private float angle = 0;

    Rigidbody rb;

    public MovementState state;
    public enum MovementState
    {
        walking,
        air
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //ground check
        grounded = Physics.Raycast(orientation.position, Vector3.down, playerHeight * 0.5f + 0.1f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();

        //handle drag
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

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    public void StateHandler()
    {
        if (grounded)
        {
            state = MovementState.walking;
        }
        else
        {
            state= MovementState.air;
        }
    }

    private void MovePlayer()
    {
        //calculate move direction
        moveDirection = orientation.forward * verticalInput + orientation.right * 0f;

        Vector3 up = new Vector3(0, 1, 0);

        if (Vector3.Dot(orientation.up, up) > 0.5)
        {
            //on slope
            if (OnSlope())
            {
                rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 10f, ForceMode.Force);
                rb.AddForce(Vector3.down * 5f, ForceMode.Force);
                // rotate
                transform.Rotate(0, horizontalInput * Time.deltaTime * rotationSpeed, 0);
            }
            //on ground
            else if (grounded)
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
                rb.AddForce(Vector3.down * 5f, ForceMode.Force);
                // rotate
                transform.Rotate(0, horizontalInput * Time.deltaTime * rotationSpeed, 0);
            }
            //in air
            else if (!grounded)
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * 1f, ForceMode.Force);
                rb.AddForce(Vector3.down * 5f, ForceMode.Force);
            }
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //limit velocity if needed
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            angle = Vector3.Angle(Vector3.down, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}


