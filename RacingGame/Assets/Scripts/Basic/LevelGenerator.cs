using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

//this script handles generating the track
//it contains a series of objects (levelParts) and randomly selects them
//then instantiates them in the world
//it also keeps track of the distance to levelParts, destroying old ones

public class LevelGenerator : MonoBehaviour
{
    private const float PLAYER_DISTANCE_SPAWN_LEVEL_PART = 3000f;
    private const float PLAYER_DISTANCE_DESPAWN_LEVEL_PART = 3000f;
    //[SerializeField] private List<Transform> levelPartList_blocks;
    //[SerializeField] private List<Transform> levelPartList_pipes;
    //[SerializeField] private List<Transform> levelPartList_cylinders;
    [SerializeField] private List<Transform> levelPartList01;
    [SerializeField] private List<Transform> levelPartList02;
    [SerializeField] private List<Transform> levelPartList03;
    [SerializeField] private List<Transform> levelPartListTransitions;

    [SerializeField] private List<Transform> goalPart;
    public int countUntilGoal;
    int defCountUntilGoal;

    //[SerializeField] private Transform levelPart_Start;
    public Transform levelPart_Start;
    [SerializeField] private GameObject player;
    Transform chosenLevelPart;

    //[SerializeField]
    public int levelChoice;
    public bool started = false;
    public bool randomLevels = false;
    public bool infiniteLevel = false;
    public bool randomLevelLength;

    public bool useOrdersList = false;

    //public LevelSelect levelSelect;

    Transform lastLevelPartTransform;

    private Vector3 lastEndPosition;
    private Quaternion lastEndRotation;

    public List<Transform> currentLevelParts = new List<Transform>();
    public int maxiumumCurrentLevelParts; //been using 100 during testing

    //by adding to a count every time a level part is spawned,
    //we can make every X parts be a certain kind of part
    //for example, since ships use fuel, it would be good to make refuelling stations appear regularly
    public int countSinceRefuel;
    public bool usePartsCount;
    public int refuel;

    //lets add a system that changes the main list every X number of parts
    public bool changeLists;
    public int partsUntilChangeList;
    int partsCount;
    public bool addVariationToListCount;
    public int listCountVariation;

    //may need to add a special track part between changes
    public bool transitionBetweenLists;
    bool transitioning;

    //list of lists of track parts
    List<List<Transform>> listOfLists = new List<List<Transform>>();
    int currentList; //this is the index of listOfLists

    //checkpoints
    //when a levelPart is created, any checkpoints in that levelPart must be added to the CheckpointManagers full list of checkpoints
    public CheckpointManager checkpointManager;
    public RaceManager raceManager;

    public int checkpointCounter;

    public List<Transform> listOfRespawnPoints = new List<Transform>();

    public Transform oldestCheckpoint;

    public bool useResetTimer;
    public float resetTimer;
    public float defResetTimer;

    public GameObject camObject; 

    //in the Awake, we look for the EndPosition of the initial levelPart
    private void Awake()
    {

        GameObject g = GameObject.Find("CheckpointManager");
        CheckpointManager checkpointManager = g.gameObject.GetComponent<CheckpointManager>();

        raceManager = GameObject.Find("RaceManager").GetComponent<RaceManager>();

    	checkpointCounter = 0;

        defCountUntilGoal = countUntilGoal;

        //make sure to add the lists of main level parts to the list of lists
        listOfLists.Add(levelPartList01);
        listOfLists.Add(levelPartList02);
        listOfLists.Add(levelPartList03);

        partsCount = partsUntilChangeList;

        randomLevelLength = raceManager.randomLevelLength;
        infiniteLevel = raceManager.infiniteLevel;

        if (randomLevelLength)
        {
            countUntilGoal = Random.Range(5, raceManager.maxLevelLength);
        }

        if (randomLevels)
        {
            lastEndPosition = levelPart_Start.Find("EndPosition").position;

            SpawnLevelParts();
            started = true;
        }

        defResetTimer = resetTimer;

        if(raceManager.careerMode)
        {
            useResetTimer = false;
        }


    }

    //the update keeps track of the player's distance to the EndPosition
    private void Update()
    {
        if (started)
        {
            if(useOrdersList)
            {
                if(!infiniteLevel)
                {
                    if (countUntilGoal > -1)
                    {
                        if(raceManager != null && raceManager.Orders != null)
                        {
                            if(raceManager.Orders.Count > 0)
                            {
                                if (Vector3.Distance(raceManager.Orders[0].transform.position, lastEndPosition) < PLAYER_DISTANCE_SPAWN_LEVEL_PART)
                                {
                                    SpawnLevelParts();
                                }
                            }
                            else
                            {
                                SpawnLevelParts();
                            }
                        }
                        else
                        {
                            SpawnLevelParts();
                        }

                    }
                }
                else
                {
                    if (Vector3.Distance(raceManager.Orders[0].transform.position, lastEndPosition) < PLAYER_DISTANCE_SPAWN_LEVEL_PART)
                    {
                        SpawnLevelParts();
                    }
                }
            }
            //else
            //{
            //    if(countUntilGoal > -1)
            //    {
            //        if (Vector3.Distance(player.transform.position, lastEndPosition) < PLAYER_DISTANCE_SPAWN_LEVEL_PART)
            //        {
            //            SpawnLevelParts();
            //        }
            //    }
            //}
        }


        //reset level
        if (Input.GetKeyDown(KeyCode.R))
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

            ResetLevel();
        }

        if (useResetTimer)
        {
            resetTimer -= Time.deltaTime;

            if(resetTimer < 0)
            {
                ResetLevel();
            }
        }
    }

    private void SpawnLevelParts()
    {
        //for now there are only random levels
        if (randomLevels)
        {
            if (countUntilGoal > 0 || infiniteLevel)
            {
                chosenLevelPart = listOfLists[currentList][Random.Range(0, listOfLists[currentList].Count)];

            }
            else
            {
                chosenLevelPart = goalPart[0]; //later add more goal parts? for now it's a list of 1, but could use currentList variable; each list has its own goal part
            }
        }

        lastLevelPartTransform = SpawnLevelPart(chosenLevelPart, lastEndPosition, lastEndRotation);

        if (lastLevelPartTransform != null)
        {
            foreach (Transform check in lastLevelPartTransform.GetComponent<CheckpointList>().checksList)
            {
                checkpointCounter++;

                check.gameObject.GetComponentInChildren<Checkpoint>().checkpointNumber = checkpointCounter;

                checkpointManager.dictOfChecks.Add(check.gameObject.GetComponentInChildren<Checkpoint>().checkpointNumber, check.transform.position);



            }
        }





        lastEndPosition = lastLevelPartTransform.Find("EndPosition").position;
        lastEndRotation = lastLevelPartTransform.Find("EndPosition").rotation;
    }

    private void SpawnLevelPartsOld()
    {
        //in this version Im making a single "level", so the choice is irrelevant but maybe comes back later
        //if (levelChoice == 1)
        //{
        //    chosenLevelPart = levelPartList_blocks[Random.Range(0, levelPartList_blocks.Count)];
        //}
        //if (levelChoice == 2)
        //{
        //    chosenLevelPart = levelPartList_pipes[Random.Range(0, levelPartList_pipes.Count)];
        //}
        //if (levelChoice == 3)
        //{
        //    chosenLevelPart = levelPartList_cylinders[Random.Range(0, levelPartList_cylinders.Count)];
        //}



        if (changeLists && usePartsCount)
        {
            partsUntilChangeList--;
            countSinceRefuel++;

            if (countSinceRefuel >= refuel)
            {
                chosenLevelPart = listOfLists[currentList][0];

                countSinceRefuel = 0;
            }
            else if(partsUntilChangeList <= 0 && countSinceRefuel < refuel)
            {
                partsUntilChangeList = partsCount;

                int previousList = currentList;

                //change list
                //this will cycle through them sequentially, but maybe random cycling would be preferable
                if(currentList < listOfLists.Count - 1)
                {
                    currentList++;
                }
                else
                {
                    currentList = 0;
                }

                if (transitionBetweenLists)
                {
                    //if a transitionary level part is required, figure out which one
                    //there are now 2 lists, 0 and 1
                    //if previous = 0 and current = 1
                    //use transition from list "0-1"
                    //else use transition from list "1-0"
                    //should I build a string that matches the transition objects name?
                    string selection = previousList.ToString() + "-" + currentList.ToString();

                    bool found = false;

                    for (int i = 0; i < levelPartListTransitions.Count; i++)
                    {
                        if (!found)
                        {
                            if (levelPartListTransitions[i].gameObject.name == "RoadPartTransition" + selection)
                            {
                                chosenLevelPart = levelPartListTransitions[i];
                                found = true;
                            }
                        }

                    }

                    //if a level part transition matching "selection" was not found, simply carry on as normal
                    if(!found)
                        chosenLevelPart = listOfLists[currentList][Random.Range(0, listOfLists[currentList].Count)];
                }
                else
                {
                    //no need for transition object?
                    chosenLevelPart = listOfLists[currentList][Random.Range(0, listOfLists[currentList].Count)];
                }
            }
            else
            {
                //have fuel, not changing lists
                //keep spawning level parts normally
                chosenLevelPart = listOfLists[currentList][Random.Range(0, listOfLists[currentList].Count)];

            }
        }
        else
        {
            //default behaviour
            chosenLevelPart = listOfLists[currentList][Random.Range(0, listOfLists[currentList].Count)];
            
            //this adds the checkpoints in the levelPart prefab to the overall list of all checkpoints
            if(chosenLevelPart.GetComponent<CheckpointList>() != null)
            {
                //checkpointManager.listOfChecks.AddRange(chosenLevelPart.GetComponent<CheckpointList>().checksList);
                ////now, we also need to update the variable on each checkpoint to reflect the correct order

                //foreach (Transform check in chosenLevelPart.GetComponent<CheckpointList>().checksList)
                //{

                //    check.gameObject.GetComponentInChildren<Checkpoint>().checkpointNumber = checkpointCounter;

                //    checkpointCounter++;

                //}



                //dictionary version
                foreach (Transform check in chosenLevelPart.GetComponent<CheckpointList>().checksList)
                {
                    check.gameObject.GetComponentInChildren<Checkpoint>().checkpointNumber = checkpointCounter;

                    //checkpointManager.dictOfChecks.Add(check.gameObject.GetComponentInChildren<Checkpoint>().checkpointNumber, check.transform);


                    checkpointCounter++;
                }
            }



        }

        //if (usePartsCount)
        //{
        //    countSinceRefuel++;

        //    print("Count: " + countSinceRefuel);

        //    if(countSinceRefuel >= refuel)
        //    {
        //        chosenLevelPart = listOfLists[currentList][0];

        //        countSinceRefuel = 0;
        //    }
        //    else
        //    {
        //        chosenLevelPart = listOfLists[currentList][Random.Range(0, listOfLists[currentList].Count)];
        //    }
        //}
        //else
        //{
        //    chosenLevelPart = listOfLists[currentList][Random.Range(0, listOfLists[currentList].Count)];
        //}

        //choose a level part, spawn it, and set new EndPosition and rotation

        lastLevelPartTransform = SpawnLevelPart(chosenLevelPart, lastEndPosition, lastEndRotation);

        lastEndPosition = lastLevelPartTransform.Find("EndPosition").position;
        lastEndRotation = lastLevelPartTransform.Find("EndPosition").rotation;
    }

    public Transform FindOldestExistingCheckpoint()
    {
        for (int i = listOfRespawnPoints.Count; i-- > 0;)
        {
            if (listOfRespawnPoints[i] == null)
            {
                listOfRespawnPoints.RemoveAt(i);
            }
            else
            {
                return listOfRespawnPoints[i];
            }
        }

        return null;
    }

    //instantiate a levelPart at designated Vector and rotation
    //then, it adds the levelPart to a list of currently spawned level parts
    //if this list exceeds a certain number, Destroy the oldest one
    private Transform SpawnLevelPart(Transform levelPart, Vector3 spawnPos, Quaternion spawnRot)
    {
        Transform levelPartTransform = Instantiate(levelPart, spawnPos, spawnRot);

        if(!infiniteLevel)
        {
            countUntilGoal--;
        }

        if (levelPartTransform.GetComponentInChildren<SetRespawnRotation>() != null)
        {
            //maybe i should set the respawn checkpoint so that the parent component knows where it is?
            listOfRespawnPoints.Add(levelPartTransform.GetComponentInChildren<SetRespawnRotation>().transform);
        }

        currentLevelParts.Add(levelPartTransform);
        if (currentLevelParts.Count > maxiumumCurrentLevelParts)
        {
            Destroy(currentLevelParts[0].gameObject);
            currentLevelParts.Remove(currentLevelParts[0]);
        }

        return levelPartTransform;
    }

    public void ResetLevel()
    {
        started = false;
        raceManager.updateOrders = false;

        Destroy(camObject);

        foreach (var levelPart in currentLevelParts)
        {
            Destroy(levelPart.gameObject);
        }

        checkpointCounter = 0;

        countUntilGoal = defCountUntilGoal;

        if (randomLevelLength)
        {
            countUntilGoal = Random.Range(5, 30);
        }

        StopCoroutine(raceManager.UpdateOrderedList());
        currentLevelParts.Clear();
        listOfRespawnPoints.Clear();
        //checkpointManager.listOfChecks.Clear();
        checkpointManager.dictOfChecks.Clear();


        lastEndPosition = levelPart_Start.Find("EndPosition").position;

        foreach (var racer in raceManager.RacersInRace)
        {
            if(racer.GetComponent<MLADrive2>() != null)
            {
                racer.GetComponent<MLADrive2>().EndEpisode();
            }

            Destroy(racer.gameObject);
        }

        raceManager.RacersInRace.Clear();
        raceManager.GoalReached.Clear();
        raceManager.Orders.Clear();

        raceManager.SpawnRacers();


        //foreach (var racer in raceManager.RacersList)
        //{
        //    if (racer.GetComponent<MLADrive2>() != null)
        //    {
        //        racer.GetComponent<MLADrive2>().EndEpisode();
        //    }

        //    racer.GetComponent<ShipController>().transform.position = racer.GetComponent<ShipController>().originPoint;
        //    racer.GetComponent<ShipController>().ammo = racer.GetComponent<ShipController>().maxAmmo;
        //    racer.GetComponent<ShipController>().fuel = racer.GetComponent<ShipController>().maxFuel;
        //    racer.GetComponent<DamageManager>().hitPoints = racer.GetComponent<DamageManager>().maxHitPoints;

        //    racer.GetComponent<ShipController>().lastCheckpoint = null;
        //    racer.GetComponent<ShipController>().nextCheckpointTransform = Vector3.zero;
        //    racer.GetComponent<ShipController>().lastCheckpointNumber = 0;
        //    racer.GetComponent<ShipController>().respawnIsOrigin = true;

        //    racer.transform.rotation = Quaternion.identity;

        //    //later, should add a system that enables controls shortly after race is ready to begin, with a little countdown to start
        //    racer.GetComponent<ShipController>().controlsEnabled = true;
        //}

        resetTimer = defResetTimer;

        SpawnLevelParts();
        started = true;

        raceManager.GetComponentInChildren<Animator>().SetBool("startCount", true);
        raceManager.GetComponentInChildren<Animator>().Play("startcount", -1, 0f);
        //raceManager.GetComponentInChildren<Animation>().Play("startcount");


    }
}
