using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

//this is a super simple script to make cars in traffic move forwards

//I will now add a raycast that checks if anything is in the way

//also, I need to add a system to delete cars when they are no longer needed
//I could use a timer
//I could check for ground? Deleting excess cars is most important when they go off the track, so sounds best

public class CarMove : MonoBehaviour
{
    public float speed = 5f;

    public Transform rayOriginLeft;
    public Transform rayOriginRight;
    public LayerMask obstacles;

    public bool checkForObstacles = true;
    public float obstacleDistance;
    public bool randomizedDistance;
    public float randomness;

    public bool drive = true;

    public bool destroyWhenOffTrack;

    private void Start()
    {
        if(randomizedDistance)
        {
            obstacleDistance = obstacleDistance + Random.Range(-randomness, randomness);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (checkForObstacles)
        {
            CheckForObstacles();
        }

        if (drive)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }
    }

    //send a ray out to see if anything is in front
    //I think this function could be simplified a little
    public void CheckForObstacles()
    {
        RaycastHit hit;

        Ray ray1 = new Ray(rayOriginLeft.position, transform.forward);
        Ray ray2 = new Ray(rayOriginRight.position, transform.forward);


        if (Physics.Raycast(ray1, out hit, obstacleDistance, obstacles) || Physics.Raycast(ray2, out hit, obstacleDistance, obstacles))
        {
            drive = false;


            //if (hit.distance < 5f)
            //{
            //    drive = false;
            //}
            //else
            //{
            //    drive = true;
            //}
        }
        else
        {
            drive = true;

        }

        if(destroyWhenOffTrack)
        {
            Ray ray3 = new Ray(rayOriginRight.position, -transform.up);

            if(!Physics.Raycast(ray3, out hit,10f, obstacles))
            {
                Destroy(this.gameObject, 3f);
            }
        }
    }
}
