using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePad : MonoBehaviour
{
    public float bounceForce = 200f;

    void OnCollisionEnter(Collision collision) {
        if(collision.rigidbody != null)
            collision.rigidbody.AddForce(collision.gameObject.transform.up * bounceForce, ForceMode.Impulse);
    }
}
