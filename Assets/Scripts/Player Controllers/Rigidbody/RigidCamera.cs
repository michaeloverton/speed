using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CAN WE JUST PULL THIS CLASS INTO RIGIDMOVEMENT?
public class RigidCamera : MonoBehaviour
{
    [SerializeField] Transform cameraPosition;

    // Update is called once per frame
    void Update()
    {
        transform.position = cameraPosition.position;
    }
}
