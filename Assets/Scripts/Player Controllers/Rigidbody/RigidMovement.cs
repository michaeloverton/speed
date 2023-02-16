using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidMovement : MonoBehaviour
{
    Rigidbody rb;
    RigidLook rigidLook;
    CapsuleCollider playerCollider;

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

    [Header("Crouch/Slide")]
    public float flatSlideForce = 5f;
    public float slopeSlideForce = 10f;
    private bool sliding = false;
    private float basePlayerHeight;
    public float crouchedPlayerHeight = 1f;
    private bool crouched = false;
    public Transform cameraPosition;
    private Vector3 baseCameraPosition;
    public float crouchedCameraHeight = .25f;
    private Vector3 crouchedCameraPosition;
    public float crouchTime = 10f;
    private bool uncrouching = false;
    private Vector3 slideVector = Vector3.zero;
    public float slideMovementReducer = 0.2f;

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
    public KeyCode crouchPoundKey = KeyCode.LeftControl;
    public KeyCode shootKey = KeyCode.E;
    public KeyCode trapKey = KeyCode.Q;

    // Slopes.
    RaycastHit slopeHit;
    public float slopeHitMargin = 0.5f;

    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rigidLook = GetComponent<RigidLook>();

        playerCollider = GetComponent<CapsuleCollider>();
        basePlayerHeight = playerCollider.height;
        baseCameraPosition = cameraPosition.localPosition;
        crouchedCameraPosition = new Vector3(cameraPosition.localPosition.x, crouchedCameraHeight, cameraPosition.localPosition.z);
    }

    void Update() {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundSphereRadius, groundMask);
        if(isGrounded) {
            // Reset jumps and pounds.
            currentJumpCount = 0;
            currentPoundCount = 0;
        }

        slopeMovementDirection = Vector3.ProjectOnPlane(movementDirection, slopeHit.normal);

        GetInput();
        ControlDrag();

        // These should likely be handled in FixedUpdate.
        if(Input.GetKeyDown(jumpKey)) {
            Jump();
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

        if(Input.GetKeyDown(crouchPoundKey) && !isGrounded) {
            Pound();
        } else if(Input.GetKey(crouchPoundKey) && isGrounded) {
            // If grounded and holding crouch key, slide.
            Crouch();
            sliding = true;

            if(slideVector == Vector3.zero) {
                // If slideVector was 0, the button was just pressed.
                // Store the direction we are looking when we press the slide button.
                slideVector = orientation.forward;
            }
        }
        if(Input.GetKeyUp(crouchPoundKey)) {
            Uncrouch();
            sliding = false;
            slideVector = Vector3.zero; // When we uncrouch, return the slideVector to 0 state.
        }
        if(uncrouching == true && !crouched) {
            // We must continue to call Uncrouch until we have lerped back to original camera position.
            Uncrouch();
        }
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
        if(isGrounded && !OnSlope() && !sliding) {
            rb.AddForce(movementDirection.normalized * movementSpeed * groundMovementMultiplier, ForceMode.Acceleration);
        } else if(isGrounded && OnSlope() && !sliding) {
            rb.AddForce(slopeMovementDirection.normalized * movementSpeed * groundMovementMultiplier, ForceMode.Acceleration);
        } else if(!isGrounded) {
            rb.AddForce(movementDirection.normalized * movementSpeed * groundMovementMultiplier * airMovementMultiplier, ForceMode.Acceleration);

            // Add downwards force to make falling faster and get better gravity feel.
            rb.AddForce(Vector3.down * fallingForce, ForceMode.Acceleration);
        } else if(sliding) {
            // Allow some movement adjustments, but reduce the effect of adjustment via slideMovementReducer.
            rb.AddForce(movementDirection.normalized * movementSpeed * groundMovementMultiplier * slideMovementReducer, ForceMode.Acceleration);
            Slide();
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
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
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

    void Crouch() {
        if(!crouched) {
            playerCollider.height = crouchedPlayerHeight;
            playerCollider.center = new Vector3(0,-0.5f,0);
            crouched = true;
        }

        cameraPosition.localPosition = Vector3.Lerp(cameraPosition.localPosition, crouchedCameraPosition, crouchTime * Time.deltaTime);
    }

    void Uncrouch() {
        if(crouched) {
            playerCollider.height = basePlayerHeight;
            playerCollider.center = new Vector3(0,0,0);
            crouched = false;
        }
        
        cameraPosition.localPosition = Vector3.Lerp(cameraPosition.localPosition, baseCameraPosition, 2 * crouchTime * Time.deltaTime);
        
        if(Mathf.Abs(cameraPosition.localPosition.y - baseCameraPosition.y) < 0.05) {
            // If camera is "close enough" to its original position, reset the camera back to its original position and stop uncrouching.
            uncrouching = false;
            cameraPosition.localPosition = baseCameraPosition;
        } else {
            uncrouching = true;
        }
    }

    void Slide() {
        if(!OnSlope()) {
            rb.AddForce(slideVector * flatSlideForce, ForceMode.Impulse);
        } else {
            // Get the vector pointing down the slope.
            Vector3 left = Vector3.Cross(slopeHit.normal, Vector3.up); 
            Vector3 slope = Vector3.Cross(slopeHit.normal, left);

            // Get the steepness of the slope.
            float angle = Vector3.Angle(slope, Vector3.up);

            // The angle is between 90 and 180. Remap its value to between 0 and 1.
            float slideForceScale = (angle -  90)*(1)/90;
            
            // Multiply the force by this scaling amount. Steeper slopes slide faster.
            rb.AddForce(slope.normalized * slopeSlideForce * slideForceScale, ForceMode.Impulse);
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
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, (basePlayerHeight / 2) + slopeHitMargin)) {
            if(slopeHit.normal != Vector3.up) {
                // Debug.Log("on slope");
                return true;
            }
        }

        // Debug.Log("off slope");
        return false;
    }
}
