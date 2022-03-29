using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidMovement : MonoBehaviour
{
    float playerHeight = 2f;
    Rigidbody rb;

    [Header("Movement")]
    public Transform orientation;
    public float movementSpeed = 6f;
    public float groundMovementMultiplier = 10f;
    public float airMovementMultiplier = 0.4f;
    public float groundDrag = 6f;
    public float airDrag = 2f;
    
    float horizontalMovement;
    float verticalMovement;
    Vector3 movementDirection;
    Vector3 slopeMovementDirection;

    [Header("Sprinting")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 6f;
    public float sprintAcceleration = 10f;


    // Jumping.
    [Header("Jump")]
    public float jumpForce = 5f;

    // Ground detection.
    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundSphereRadius = 0.4f;
    public LayerMask groundMask;
    bool isGrounded;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    // Slopes.
    RaycastHit slopeHit;
    public float slopeHitMargin = 0.5f;

    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update() {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundSphereRadius, groundMask);

        GetInput();
        ControlDrag();
        ControlSpeed();

        if(Input.GetKeyDown(jumpKey)) {
            Jump();
        }

        slopeMovementDirection = Vector3.ProjectOnPlane(movementDirection, slopeHit.normal);
    }

    void FixedUpdate() {
        MovePlayer();
    }

    void GetInput() {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        movementDirection = (orientation.forward * verticalMovement) + (orientation.right * horizontalMovement);
    }

    void MovePlayer() {
        if(isGrounded && !OnSlope()) {
            rb.AddForce(movementDirection.normalized * movementSpeed * groundMovementMultiplier, ForceMode.Acceleration);
        } else if(isGrounded && OnSlope()) {
            rb.AddForce(slopeMovementDirection.normalized * movementSpeed * groundMovementMultiplier * airMovementMultiplier, ForceMode.Acceleration);
        } else if(!isGrounded) {
            rb.AddForce(movementDirection.normalized * movementSpeed * groundMovementMultiplier * airMovementMultiplier, ForceMode.Acceleration);
        }
    }

    void ControlSpeed() {
        if(Input.GetKey(sprintKey) && isGrounded) {
            movementSpeed = Mathf.Lerp(movementSpeed, sprintSpeed, sprintAcceleration * Time.deltaTime);
        } else {
            movementSpeed = Mathf.Lerp(movementSpeed, walkSpeed, sprintAcceleration * Time.deltaTime);
        }
    }

    void Jump() {
        if(isGrounded) {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    void ControlDrag() {
        if(isGrounded) {
            rb.drag = groundDrag;
        } else {
            rb.drag = airDrag;
        }
    }

    bool OnSlope() {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, (playerHeight / 2) + slopeHitMargin)) {
            if(slopeHit.normal != Vector3.up) {
                return true;
            }
        }

        return false;
    }
}
