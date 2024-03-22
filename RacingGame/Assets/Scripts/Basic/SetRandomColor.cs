using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//simple script to give some background objects such as cars a random color

public class SetRandomColor : MonoBehaviour
{
    public int indexOfMat;

    public bool alsoEmissionColor;

    public bool useColorList;
    public Color[] colorList;

    private Color color;

    // Start is called before the first frame update
    void Start()
    {
        if (!useColorList || colorList.Length == 0)
        {
            color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        }
        else
        {
            color = colorList[Random.Range(0, colorList.Length)];
        }

        GetComponent<Renderer>().materials[indexOfMat].color = color;
        //public static Color ColorHSV(float hueMin, float hueMax, float saturationMin, float saturationMax, float valueMin, float valueMax, float alphaMin, float alphaMax);
    
        if(alsoEmissionColor)
        {
            GetComponent<Renderer>().materials[indexOfMat].SetColor("_EmissionColor", color * 2f);

        }
    }
}
