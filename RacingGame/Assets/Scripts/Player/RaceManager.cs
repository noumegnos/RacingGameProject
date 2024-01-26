using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//this keeps track of the order of the racers, so we know who is in the lead
//so, it needs access to all the racers, and their current checkpoint

public class RaceManager : MonoBehaviour
{

    public List<GameObject> RacersList = new List<GameObject>();

    public GameObject[] Racers;

    public List<GameObject> Orders = new List<GameObject>();

    //list of ships that have reached the goal
    public List<GameObject> GoalReached = new List<GameObject>();



    //what I actuall want out of this list is to have it be ordered according to who is in the lead
    //ok good enough for now
    //next the level generator needs to access this list and use the leader as the entity that's checked when to generate new levelParts

    // Start is called before the first frame update
    void Start()
    {
        RacersList.AddRange(GameObject.FindGameObjectsWithTag("Ship"));

        //temporarily disabled until I update the system below to use ShipController instead of PlayerController
        //reenabled, but the sysrtem still needs to be fixed properly to use distance to next check
        StartCoroutine(UpdateOrderedList());

    }

    public IEnumerator UpdateOrderedList()
    {

        Orders = OrderRacers();

        Orders.Reverse();

        yield return new WaitForSeconds(1);


        StartCoroutine(UpdateOrderedList());





    }

    public List<GameObject> OrderRacers()
    {
        IOrderedEnumerable<GameObject> RacersListOrdered = RacersList.OrderBy(x => x.GetComponent<ShipController>().lastCheckpointNumber);

        return RacersListOrdered.ToList<GameObject>();
    }

    //static int SortByCheckpoint(PlayerController p1, PlayerController p2)
    //{
    //    return p1.lastCheckpointTouched.CompareTo(p2.lastCheckpointTouched);
    //}


    //this idea is based on ships being destroyed on failure, leading to only one winner remaining
    //its a good idea but needs to be worked on
    //also, I do not think I want the AI to train like that
    //public void CheckRacerList(){

    //    int racersLeft = Racers.Length;

    //    GameObject lastRacer = null;

    //    for (int i = 0; i < Racers.Length; i++)
    //    {
    //        if(Racers[i].gameObject == null){
    //            racersLeft -= 1;

    //            if(racersLeft == 1)
    //                lastRacer = Racers[i];
    //        }
    //    }

    //    if(racersLeft > 1)
    //        print("There are " + racersLeft + " racers left!");
    //    else
    //        print(lastRacer + " is the Winner!");
    //    if (!lastRacer.GetComponent<PlayerController>().isHuman)
    //    {
    //        lastRacer.GetComponent<MLADrive>().AddReward(10f);
    //        lastRacer.GetComponent<MLADrive>().EndEpisode();

    //    }


    //}

    // Update is called once per frame
    void Update()
    {
        
    }
}
