using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//simple script to give some background objects such as cars a random color

public class SetRandomColor : MonoBehaviour
{
    public int indexOfMat;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().materials[indexOfMat].color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        //public static Color ColorHSV(float hueMin, float hueMax, float saturationMin, float saturationMax, float valueMin, float valueMax, float alphaMin, float alphaMax);
    }
}
