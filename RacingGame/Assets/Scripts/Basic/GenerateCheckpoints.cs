using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script is a test

//the purpose is to automatically add checkpoint objects to the track, using a series of single-point vertices along the track

//drawback: the checkpoint object will not be able to have the correct rotations, but would work with simple tracks that dont require rotated checkpoints

//alternatively, the object which contains the collider can be created in blender, and this script can be used to add the checkpoint script to that object
//this would allow the checkoint trigger have the correct size and rotation

//how to make the simplest version? generating too much stuff constantly may become a bit processing heavy
//I think the simplest is to actually do it all manually but that sounds horrifying, since it involves manually adding several things to hundreds of objects
//though at least I can make it a little easier by having a series of verts in place as the positions of the checkpoints, but I would need to manually rotate them
//no I don't have to manually rotate them, since I can use an array of cubes which I manually turn into trigger objects
//if I can select all the cubes and add the required stuff to them all at once this will be the easiest probably, so to testing

//so its easy to do manually, no need for this script at all

public class GenerateCheckpoints : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
