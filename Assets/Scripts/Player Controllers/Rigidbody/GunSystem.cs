using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSystem : MonoBehaviour
{
    public GameObject gunBlast;
    public float blastDisplayTime = 0.1f;

    private GameObject instantiatedImpactBlast;
    public GameObject impactBlast;
    public float impactBlastDisplayTime = 0.2f;

    void Start() {
        gunBlast.SetActive(false);
    }

    public void ShowGunBlast() {
        gunBlast.SetActive(true);
        Invoke("HideBlast", blastDisplayTime);
    }

    void HideBlast() {
        gunBlast.SetActive(false);
    }

    public void ShowImpactBlast(Vector3 position) {
        instantiatedImpactBlast = Instantiate(impactBlast, position, Quaternion.identity);
        Invoke("HideImpactBlast", impactBlastDisplayTime);
    }

    void HideImpactBlast() {
        Destroy(instantiatedImpactBlast);
    }
}
