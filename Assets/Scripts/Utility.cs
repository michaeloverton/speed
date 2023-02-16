using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static float Remap(float value, float inputMin, float inputMax, float outputMin, float outputMax)
    {
        return outputMin + (value-inputMin)*(outputMax-outputMin)/(inputMax-inputMin);
    }
}
