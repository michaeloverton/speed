using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletExplosion : MonoBehaviour
{
    [SerializeField] float lifespan = 0.1f;
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
}
