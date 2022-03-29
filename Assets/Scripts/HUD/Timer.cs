using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI text;
    private float time = 0f;

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        text.SetText(time + "s");
    }
}
