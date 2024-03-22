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

    public bool restartOnGoal;

    //how many racers should reach goal before the track resets
    public int racersInGoalBeforeReset = 3;

    //racers who have reached the goal will be placed in the goal holder to preserve their position (must be a better way ugh)
    public Transform goalHolder;
    int goalCount;


    // Start is called before the first frame update
    void Start()
    {
        raceManager = GameObject.FindObjectOfType<RaceManager>();
        levelGenerator = GameObject.FindObjectOfType<LevelGenerator>();

        //restartOnGoal = !raceManager.careerMode;

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

            if (other.gameObject.GetComponentInChildren<CharacterSheet>() != null && goalCount == 0) // && raceManager.GoalReached.Count == 0)
            {
                other.gameObject.GetComponentInChildren<CharacterSheet>().wins += 1;
            }

            raceManager.GoalReached.Add(other.gameObject);
            raceManager.UpdateVictoryList();

            other.gameObject.transform.position = goalHolder.GetChild(goalCount).transform.position;
            goalCount++;



            //if(!levelGenerator.useResetTimer)
            //{
            //    levelGenerator.useResetTimer = true;


            //}

            if (levelGenerator.resetTimer > 1800f)
            {
                levelGenerator.resetTimer = 1800;
            }

            if (!other.gameObject.GetComponent<ShipController>().isHuman)
            {
                other.GetComponent<ShipController>().timeout = other.GetComponent<ShipController>().defTimeout;

                other.gameObject.GetComponent<MLADrive2>().AddReward(1f / raceManager.GoalReached.Count);
                //other.gameObject.GetComponent<MLADrive2>().EndEpisode();
            }

            if(restartOnGoal)
            {
                if (raceManager.GoalReached.Count >= racersInGoalBeforeReset)
                {
                    levelGenerator.ResetLevel();
                }
            }

            if (other.gameObject.GetComponent<ShipController>().isHuman && !restartOnGoal)
            {
                //raceManager.OnRaceFinished();
                raceManager.EnableVictoryScreen();
            }
        }
    }
}
