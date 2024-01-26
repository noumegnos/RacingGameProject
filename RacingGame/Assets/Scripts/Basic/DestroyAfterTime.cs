using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script is attached to objects which should be removed from the game world after a time, such as debris from destroyed objects

public class DestroyAfterTime : MonoBehaviour
{
    public float time = 2f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, time);
    }
}
