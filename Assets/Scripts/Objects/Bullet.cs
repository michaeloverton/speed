using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float lifespan = 5.0f;
    [SerializeField] GameObject bulletExplosion;
    float elapsed = 0;

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;
        if(elapsed > lifespan)
        {
            Destroy(this.gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject explosion = Instantiate(bulletExplosion);
        explosion.transform.parent = null;
        explosion.transform.position = transform.position;

        Destroy(this.gameObject);
    }
}
