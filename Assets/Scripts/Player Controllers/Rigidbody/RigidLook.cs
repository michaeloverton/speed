using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidLook : MonoBehaviour
{
    [SerializeField] RigidWallRun wallRun;
    [SerializeField] CameraMoveTilt cameraMoveTilt;
    [SerializeField] private float sensitivityX;
    [SerializeField] private float sensitivityY;

    [SerializeField] Transform cam;
    [SerializeField] Transform orientation;

    float mouseX;
    float mouseY;

    float multiplier = 0.01f;

    float xRotation;
    float yRotation;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update() {
        GetInput();

        cam.transform.localRotation = Quaternion.Euler(xRotation, yRotation, wallRun.currentTilt + cameraMoveTilt.currentMoveTilt); // Takes into account the possible tilting of camera.
        orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    void GetInput() {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        yRotation += mouseX * sensitivityX * multiplier;
        xRotation -= mouseY * sensitivityY * multiplier;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    }

    public Transform getCameraHolder() {
        return cam;
    }
}
