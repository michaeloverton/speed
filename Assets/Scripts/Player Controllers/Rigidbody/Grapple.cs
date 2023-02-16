using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    [SerializeField] RigidLook rl;
    [SerializeField] Transform cam;
    [SerializeField] float grappleLength = 100f;
    [SerializeField] float cooldownTime = 1f;
    bool cooldown;

    LineRenderer lineRenderer;
    Vector3 connectionPoint;
    bool connected = false;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        Transform orientation = rl.getCameraHolder();
        // Debug.DrawRay(cam.position, orientation.TransformDirection(Vector3.forward), Color.red);
        Debug.DrawRay(cam.position, orientation.forward, Color.red);
        
        // Pressed.
        if(Input.GetMouseButtonDown(0) && !cooldown) 
        {
            RaycastHit grappleHit;
            if(Physics.Raycast(cam.position, orientation.forward, out grappleHit, grappleLength))
            {
                lineRenderer.enabled = true;
                connected = true;
                connectionPoint = grappleHit.point;
            }

            cooldown = true;
            // Invoke("resetCooldown", cooldownTime);
        }

        // Holding.
        if(Input.GetMouseButton(0) && connected)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, connectionPoint);
        }

        // Releasing.
        if(Input.GetMouseButtonUp(0))
        {
            cooldown = false;
            lineRenderer.enabled = false;
        }
        
    }
}
