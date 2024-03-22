using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;

//this script is attached to ship's engines, causing them to change based on values, giving the sense of being animated

//first, change emission value based on speed

//also added trail renderer color set

//multiple color values can be set here

//the plan is that players can select their own colors, or use a pilot's favoured colors

public class EngineBehaviour : MonoBehaviour
{
    public Renderer rende;

    public GameObject parentShip;

    public Color[] colors;

    //public Color color1;
    //public Color color2;
    //public Color color3;
    public float intensity;

    //public float turn;
    //float Yrot;
    //float defaultYRot;

    public TrailRenderer trailRenderer;
    public int trailColorIndex1;
    public int trailColorIndex2;

    //public Animator[] anims;

    //Material[] engineGlow;

    public GameObject[] enigineExhausts;

    private void Start()
    {
        rende = GetComponent<Renderer>();

        trailRenderer = GetComponentInChildren<TrailRenderer>();

        //anims = GetComponentsInChildren<Animator>();

        if(trailRenderer != null)
        {
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(colors[trailColorIndex1], 0.0f), new GradientColorKey(colors[trailColorIndex2], 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
            );
            trailRenderer.colorGradient = gradient;
        }

        //defaultYRot = transform.eulerAngles.y;
        //Yrot = defaultYRot;

        //engineGlow = rende.materials[];
    }



    // Update is called once per frame
    void LateUpdate()
    {
        if(parentShip != null)
        {
            intensity = parentShip.GetComponent<ShipController>().currentSpeed;

            //turn = parentShip.GetComponent<ShipController>().input;

            

        }

        for (int i = 0; i < rende.materials.Length; i++)
        {
            rende.materials[i].SetColor("_EmissionColor", colors[i] * (intensity / (i + 1f * 10f)));
        }

        //rende.materials[1].SetColor("_EmissionColor", color1 * (intensity / 10f));
        //rende.materials[2].SetColor("_EmissionColor", color2 * (intensity / 20f));
        //rende.materials[3].SetColor("_EmissionColor", color3 * (intensity / 30f));


        //if(turn < 0.05f && turn > -0.05f)
        //{
        //    Yrot = Mathf.MoveTowards(Yrot, defaultYRot, 120f * Time.deltaTime);
        //}
        //else
        //{
        //    Yrot = Mathf.MoveTowards(Yrot, Yrot + turn, 120f * Time.deltaTime);
        //}

        //float clampY = Mathf.Clamp(Yrot, defaultYRot - 30f, defaultYRot + 30f);

        //transform.eulerAngles = new Vector3(transform.eulerAngles.x, clampY, transform.eulerAngles.z);

        foreach(GameObject t in enigineExhausts)
        {
            float yscale = t.transform.localScale.y * (intensity / 10f);

            float ysc = Mathf.Clamp(yscale, 0.1f, 1.2f);

            t.transform.localScale = new Vector3(t.transform.localScale.x, ysc, t.transform.localScale.z);
        }
    }


}
