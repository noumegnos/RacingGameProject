using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script is attached to a track piece
//on the track I add a series of objects, which I then add to the list here
//this script runs through these objects and randomly disables some

public class RandomlyDisableListObjects : MonoBehaviour
{
    public List<GameObject> objects = new List<GameObject>();

    public float chance = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject item in objects)
        {
            if(Random.Range(0f,1f) > chance)
            {
                item.SetActive(false);
            }
        }
    }
}
