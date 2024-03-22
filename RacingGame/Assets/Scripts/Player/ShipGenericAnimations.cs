using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//controls animator variables for general ship behaviours, such as leaning while turning

public class ShipGenericAnimations : MonoBehaviour
{
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(GetComponent<SmoothFollowObject>().target != null)
        {
            anim.SetFloat("lean", GetComponent<SmoothFollowObject>().target.GetComponent<ShipController>().turnValue);

        }
    }
}
