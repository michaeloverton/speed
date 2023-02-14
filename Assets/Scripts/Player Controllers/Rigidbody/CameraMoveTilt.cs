using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CameraMoveTilt : MonoBehaviour
{
    [SerializeField] Transform orientation;
    [SerializeField] float tiltTime;
    [SerializeField] float maxTilt;
    public float currentMoveTilt { get; private set; }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.A))
        {
            currentMoveTilt = Mathf.Lerp(currentMoveTilt, maxTilt, tiltTime * Time.deltaTime);
        }
        else if(Input.GetKey(KeyCode.D))
        {
            currentMoveTilt = Mathf.Lerp(currentMoveTilt, -maxTilt, tiltTime * Time.deltaTime);
        }
        else
        {
            currentMoveTilt = Mathf.Lerp(currentMoveTilt, 0, tiltTime * Time.deltaTime);
        }
    }
}
