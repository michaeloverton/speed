using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidWallRun : MonoBehaviour
{
    private Rigidbody rb;
    public Transform orientation;
    private RigidMovement playerMovement;

    [Header("Camera")]
    public Camera cam;
    private float baseFov;
    public float wallRunFov;
    public float wallRunFovTime;
    public float camTilt;
    public float camTiltTime;
    public float currentTilt { get; private set; }

    [Header("Detection")]
    public float wallDistance = 0.5f;
    public float minimumJumpHeight = 1.5f;
    public LayerMask playerMask; // This is so we don't detect ourself.

    [Header("Parameters")]
    public float wallGravity;
    public float wallJumpForce;
    public float wallJumpMultiplier = 100f;

    bool wallLeft = false;
    bool wallRight = false;
    
    RaycastHit leftWallHit;
    RaycastHit rightWallHit;

    void Start() {
        rb = GetComponent<Rigidbody>();
        playerMovement = GetComponent<RigidMovement>();
        baseFov = cam.fieldOfView;
    }

    void Update() {
        CheckWall();

        if(CanWallRun()) {
            if(wallLeft) {
                StartWallRun();
            } else if(wallRight) {
                StartWallRun();
            } else {
                StopWallRun();
            }
        } else {
            StopWallRun();
        }
    }

    void CheckWall() {
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallDistance, ~playerMask);
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallDistance, ~playerMask);
    }

    bool CanWallRun() {
        return !Physics.Raycast(transform.position, Vector3.down, minimumJumpHeight);
    }

    void StartWallRun() {
        rb.useGravity = false;
        rb.AddForce(Vector3.down * wallGravity, ForceMode.Force);

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, wallRunFov, wallRunFovTime * Time.deltaTime);

        if(wallLeft) {
            currentTilt = Mathf.Lerp(currentTilt, -camTilt, camTiltTime * Time.deltaTime);
        } else if(wallRight) {
            currentTilt = Mathf.Lerp(currentTilt, camTilt, camTiltTime * Time.deltaTime);
        }

        if(Input.GetKeyDown(playerMovement.jumpKey)) {
            if(wallLeft) {
                Vector3 wallRunJumpDirection = transform.up + leftWallHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallRunJumpDirection * wallJumpForce * wallJumpMultiplier, ForceMode.Force);
            } else if(wallRight) {
                Vector3 wallRunJumpDirection = transform.up + rightWallHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallRunJumpDirection * wallJumpForce * wallJumpMultiplier, ForceMode.Force);
            }
        }
    }

    void StopWallRun() {
        rb.useGravity = true;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, baseFov, wallRunFovTime * Time.deltaTime);
        currentTilt = Mathf.Lerp(currentTilt, 0, camTiltTime * Time.deltaTime);
    }
}
