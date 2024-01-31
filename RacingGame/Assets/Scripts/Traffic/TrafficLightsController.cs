using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

//this script is in charge of ensuring that traffic lights and traffic barriers block traffic

//firstly, this will control the barriers which block normal traffic from colliding with trains
//the traffic lights object contains a series of transforms for raycasts, which are used to detect any object (where only trains can go)
//if ANY of the rays do detect a train, lights are RED, else GREEN

//these raycasts don't need to be too intense; no need to run from update function
//instead use a coroutine that checks them every few seconds or so

//also I don't know that I need to use a bool at all, it would be perfectly fine to directly affect the barrier object from this script
//first, I'll simply set the barrier active or not
//later, I'll make the barrier animate a little so it comes up from the ground or something

//TODO
//furthermore, this script could also generate the oncoming traffic like trains

//also, make script decide how far along the X-axis the train starts off,so they don't always spawn exactly the same

public class TrafficLightsController : MonoBehaviour
{
    //use only one of these bools
    public bool useDetection; //enables detection of oncoming traffic, which enables barriers
    public bool useRandom; //randomly decides once if it should enable barriers or not
    public bool enableBarriers; //decides if barriers should be enabled or not
    //could also add a timed barriers system, but might not be necessary as it won't be noticable in the game

    public Transform sensor1;
    public Transform sensor2;
    public Transform sensor3;
    public Transform sensor4;

    public LayerMask obstacles;
    public bool trainDetected;
    public float chance = 0.5f;

    public List<GameObject> barriers = new List<GameObject>();

    //train system has been moved to Traffic Spawner
    //odds of spawning a train

    //public List<GameObject> trainsToEnable = new List<GameObject>(); //list of trains to enable; it is actually only ever 2, so maybe something simpler than List could be used?
    //public float trainOffset = 50f; //train position on X-axis randomly offset by -trainOffset to trainOffset, so that it has a somewhat randomized starting position

    // Start is called before the first frame update
    void Start()
    {

        //if(trainsToEnable.Count > 0)
        //{
        //    foreach (GameObject go in trainsToEnable)
        //    {
        //        if (Random.Range(0f, 1f) > chance)
        //        {
        //            go.SetActive(true);

        //            go.transform.position = new Vector3(go.transform.position.x + Random.Range(-trainOffset, trainOffset), go.transform.position.y, go.transform.position.z);
        //        }
        //        else
        //        {
        //            go.SetActive(false);
        //        }
        //    }
        //}

        if (useDetection)
        {
            StartCoroutine(TrainSensor());

        }
        else if (useRandom)
        {
            if(Random.Range(0f, 1f) < chance)
            {
                EnableBarriers(true);
            }
            else
            {
                EnableBarriers(false);
            }
        }
        else if(!useRandom && !useDetection)
        {
            EnableBarriers(enableBarriers);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableBarriers(bool enable)
    {
        foreach (GameObject item in barriers)
        {
            foreach (Animator item2 in item.GetComponentsInChildren<Animator>())
            {
                item2.SetBool("stop", enable);
            }
        }
    }

    public IEnumerator TrainSensor()
    {
        RaycastHit hit;

        Ray ray1 = new Ray(sensor1.position, transform.forward);
        Ray ray2 = new Ray(sensor2.position, transform.forward);
        Ray ray3 = new Ray(sensor3.position, transform.forward);
        Ray ray4 = new Ray(sensor4.position, transform.forward);

        if (Physics.Raycast(ray1, out hit, 100f, obstacles) || Physics.Raycast(ray2, out hit, 100f, obstacles) || Physics.Raycast(ray3, out hit, 100f, obstacles) || Physics.Raycast(ray4, out hit, 100f, obstacles))
        {
            trainDetected = true;

            EnableBarriers(trainDetected);

        }
        else
        {
            trainDetected = false;

            EnableBarriers(trainDetected);

        }

        yield return new WaitForSeconds(1f);
        StartCoroutine(TrainSensor());
    }
}
