using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script is attached to the goal trigger object
//when a ship crosses the finish line, the order they did so is recorded

public class Goal : MonoBehaviour
{
    //move this list to race manager for example
    //public List<GameObject> GoalReached = new List<GameObject>();

    public RaceManager raceManager;
    public LevelGenerator levelGenerator;

    //how many racers should reach goal before the track resets
    public int racersInGoalBeforeReset = 3;

    // Start is called before the first frame update
    void Start()
    {
        raceManager = GameObject.FindObjectOfType<RaceManager>();
        levelGenerator = GameObject.FindObjectOfType<LevelGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<ShipController>() != null)
        {
            other.GetComponent<ShipController>().controlsEnabled = false;

            raceManager.GoalReached.Add(other.gameObject);

            if(!other.gameObject.GetComponent<ShipController>().isHuman)
            {
                other.gameObject.GetComponent<MLADrive2>().AddReward(1f / raceManager.GoalReached.Count);
            }

            if(raceManager.GoalReached.Count >= racersInGoalBeforeReset)
            {
                levelGenerator.ResetLevel();
            }

        }
    }
}
