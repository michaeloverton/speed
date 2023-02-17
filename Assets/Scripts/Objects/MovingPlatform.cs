using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    Rigidbody rb;

    void Start()
    {
        // Animator an = GetComponent<Animator>();
        // an.updateMode = AnimatorUpdateMode.AnimatePhysics;

        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 pos = transform.position;
        rb.MovePosition(new Vector3(pos.x, pos.y, pos.z + 10*Mathf.Sin(Time.time) * .01f));
    }

    // void OnTriggerEnter(Collider other)
    // {
    //     if(other.gameObject.name == "Player Container")
    //     {
    //         Debug.Log("playerLayer!");
    //         other.gameObject.transform.parent = this.transform;
    //     }
    // }

    // void OnTriggerExit(Collider other)
    // {
    //     if(other.gameObject.name == "Player Container")
    //     {
    //         Debug.Log("leaving!");
    //         other.gameObject.transform.parent = null;
    //     }
    // }
}
