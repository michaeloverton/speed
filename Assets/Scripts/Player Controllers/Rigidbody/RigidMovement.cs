using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidMovement : MonoBehaviour
{
    float playerHeight = 2f;
    Rigidbody rb;
    RigidLook rigidLook;

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
    public float fallingForce = 20f;
    public int maxJumpCount = 2;
    private int currentJumpCount = 0;

    [Header("Pound")]
    public float poundForce = 150f;
    public int maxPoundCount = 2;
    private int currentPoundCount = 0;

    [Header("Dash")]
    public float dashForce = 100f;

    [Header("Blast")]
    public float blastForce = 75f;
    public GunSystem gunSystem;
    public LayerMask playerMask; // Use this so that the gun hit detection ignores player layer.

    [Header("Trap")]
    public GameObject bouncePad;

    // Ground detection.
    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundSphereRadius = 0.4f;
    public LayerMask groundMask;
    bool isGrounded;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey = KeyCode.LeftShift;
    public KeyCode poundKey = KeyCode.LeftControl;
    public KeyCode shootKey = KeyCode.E;
    public KeyCode trapKey = KeyCode.Q;

    // Slopes.
    RaycastHit slopeHit;
    public float slopeHitMargin = 0.5f;

    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rigidLook = GetComponent<RigidLook>();
    }

    void Update() {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundSphereRadius, groundMask);
        if(isGrounded) {
            currentJumpCount = 0;
            currentPoundCount = 0;
        }

        GetInput();
        ControlDrag();
        // ControlSpeed();

        // These should likely be handled in FixedUpdate.
        if(Input.GetKeyDown(jumpKey)) {
            Jump();
        }
        if(Input.GetKeyDown(poundKey)) {
            Pound();
        }
        if(Input.GetKeyDown(dashKey)) {
            Dash();
        }
        if(Input.GetKeyDown(shootKey)) {
            Shoot();
        }
        if(Input.GetKeyDown(trapKey)) {
            Trap();
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
            rb.AddForce(slopeMovementDirection.normalized * movementSpeed * groundMovementMultiplier, ForceMode.Acceleration);
        } else if(!isGrounded) {
            rb.AddForce(movementDirection.normalized * movementSpeed * groundMovementMultiplier * airMovementMultiplier, ForceMode.Acceleration);

            // Add downwards force to make falling faster and get better gravity feel.
            rb.AddForce(Vector3.down * fallingForce, ForceMode.Acceleration);
            // rb.AddForce(Vector3.down * fallingForce, ForceMode.Force);
        }
    }

    void ControlSpeed() {
        if(Input.GetKey(dashKey) && isGrounded) {
            movementSpeed = Mathf.Lerp(movementSpeed, sprintSpeed, sprintAcceleration * Time.deltaTime);
        } else {
            movementSpeed = Mathf.Lerp(movementSpeed, walkSpeed, sprintAcceleration * Time.deltaTime);
        }
    }

    void Jump() {
        if(isGrounded) {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            currentJumpCount++;
        } else if(currentJumpCount < maxJumpCount) {
            // rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            currentJumpCount++;
        }
    }

    void Pound() {
        // Prob do a raycast check and make sure we're a certain distance above the ground before enabling pound.
        if(!isGrounded && currentPoundCount==0) {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.down * poundForce, ForceMode.Impulse);
            currentPoundCount++;
        } else if(!isGrounded && currentPoundCount < maxPoundCount) {
            rb.AddForce(Vector3.down * poundForce, ForceMode.Impulse);
            currentPoundCount++;
        }
    }

    void Dash() {
        rb.AddForce(orientation.forward * dashForce, ForceMode.Impulse);
    }

    void Shoot() {
        Transform cameraOrientation = rigidLook.getCameraHolder();
        gunSystem.ShowGunBlast();

        // Blast the player "backwards", simulating huge recoil.
        rb.AddForce(-cameraOrientation.forward * blastForce, ForceMode.Impulse);

        // Explode any objects that are hit by the blast.
        RaycastHit blastHit;
        if(Physics.Raycast(transform.position, cameraOrientation.forward, out blastHit, 100, ~playerMask)) {
            Vector3 blastPosition = blastHit.point;
            gunSystem.ShowImpactBlast(blastPosition);
            
            Collider[] blastedColliders = Physics.OverlapSphere(blastPosition, 30);
            foreach(Collider blastedCollider in blastedColliders) {
                Rigidbody blastedBody = blastedCollider.GetComponent<Rigidbody>();
                if(blastedBody != null) {
                    blastedBody.AddExplosionForce(1300, blastPosition, 6);
                }
            }
        }
    }

    void Trap() {
        GameObject pad = Instantiate(bouncePad, transform.position + 5*orientation.forward, Quaternion.identity);

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
