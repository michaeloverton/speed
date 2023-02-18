using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform player;
    [SerializeField] float fireRate = 0.5f;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform bulletSpawnPoint;
    [SerializeField] float bulletSpeed = 5.0f;
    float elapsed = 0;

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(player, Vector3.up);

        elapsed += Time.deltaTime;
        if(elapsed > fireRate)
        {
            GameObject b = Instantiate(bullet);
            b.transform.parent = null;
            b.transform.position = bulletSpawnPoint.position;
            b.GetComponent<Rigidbody>().velocity = bulletSpeed * Vector3.Normalize(player.transform.position - bulletSpawnPoint.position);

            elapsed = 0;
        }
    }
}
