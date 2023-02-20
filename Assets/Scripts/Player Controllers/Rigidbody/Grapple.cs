using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    GameObject playerContainer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] RigidLook rl;
    [SerializeField] Transform cam;
    [SerializeField] float grappleLength = 100f;
    bool cooldown;

    LineRenderer lineRenderer;
    Vector3 connectionPoint;
    Vector3 localConnectionPoint;
    Transform connectedObjectTransform;
    bool connected = false;
    SpringJoint springJoint;
    [SerializeField] float retractionRate = 60f;

    [SerializeField] GameObject grappleCrosshair;
    [SerializeField] float crosshairScaleFactor;
    Vector3 baseCrosshairScale;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;

        playerContainer = rl.gameObject;
        grappleCrosshair.SetActive(false);
        baseCrosshairScale = grappleCrosshair.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        Transform orientation = rl.getCameraHolder();
        Debug.DrawRay(cam.position, orientation.forward, Color.red);

        RaycastHit grappleHit;
        bool rayIsHitting = Physics.Raycast(cam.position, orientation.forward, out grappleHit, grappleLength, ~playerLayer);

        // Crosshair.
        if(rayIsHitting && !connected)
        {
            grappleCrosshair.transform.position = grappleHit.point;
            float grappleDistance = Vector3.Magnitude(playerContainer.transform.position - grappleHit.point);
            grappleCrosshair.transform.localScale = crosshairScaleFactor * grappleDistance * baseCrosshairScale;
            grappleCrosshair.transform.position += grappleHit.normal * 0.1f;
            grappleCrosshair.transform.rotation = Quaternion.FromToRotation(Vector3.forward, grappleHit.normal);
            grappleCrosshair.SetActive(true);
        }
        else
        {
            grappleCrosshair.SetActive(false);
        }
        
        // Pressed.
        if(Input.GetMouseButtonDown(0) && !cooldown) 
        {
            if(rayIsHitting)
            {
                lineRenderer.enabled = true;
                connected = true;
                connectionPoint = grappleHit.point;

                connectedObjectTransform = grappleHit.collider.gameObject.transform;
                localConnectionPoint = connectedObjectTransform.InverseTransformPoint(connectionPoint);

                springJoint = playerContainer.AddComponent<SpringJoint>();
                springJoint.autoConfigureConnectedAnchor = false;
                springJoint.connectedAnchor = connectionPoint;
                springJoint.spring = 4f;
                springJoint.minDistance = Vector3.Magnitude(playerContainer.transform.position - connectionPoint);
            }

            cooldown = true;
        }

        // Holding.
        if(Input.GetMouseButton(0) && connected)
        {
            connectionPoint = connectedObjectTransform.TransformPoint(localConnectionPoint);
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, connectionPoint);
            springJoint.connectedAnchor = connectionPoint;
        }

        // Retract.
        if(Input.GetMouseButton(1) && connected)
        {
            springJoint.minDistance -= retractionRate * Time.deltaTime;
        }

        // Releasing.
        if(Input.GetMouseButtonUp(0))
        {
            cooldown = false;
            lineRenderer.enabled = false;
            connected = false;
            Destroy(springJoint);
        }
        
    }
}
