using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CameraMoveTilt : MonoBehaviour
{
    [SerializeField] Transform orientation;
    [SerializeField] float startVelocity;
    [SerializeField] float tiltTime;
    [SerializeField] float maxTilt;
    public float currentMoveTilt { get; private set; }
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 localVelocity = orientation.InverseTransformDirection(rb.velocity);
        // Tilt based on velocity. Causes camera jitter why?
        // currentMoveTilt = -Utility.Remap(localVelocity.x, 0, 30, 0, maxTilt);

        if(localVelocity.x > startVelocity)
        {
            currentMoveTilt = Mathf.Lerp(currentMoveTilt, -maxTilt, tiltTime * Time.deltaTime);
        }
        else if(localVelocity.x < -startVelocity)
        {
            currentMoveTilt = Mathf.Lerp(currentMoveTilt, maxTilt, tiltTime * Time.deltaTime);
        }
        else
        {
            currentMoveTilt = Mathf.Lerp(currentMoveTilt, 0, tiltTime * Time.deltaTime);
        }

        
    }
}
