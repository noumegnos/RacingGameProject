using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.Assertions.Must;
using Unity.VisualScripting;

public class MLADrive2 : Agent
{

    public float accelerate;
    public float steering;
    public float shoot;
    public float jump;
    public float boost;

    public RaceManager raceManager;

    public void Start()
    {
        raceManager = GameObject.Find("RaceManager").GetComponent<RaceManager>();

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        //actions are the things the agent is able to do
        //for starters, it needs to be able to accelerate and steer

        accelerate = actions.ContinuousActions[0];
        steering = actions.ContinuousActions[1];
        shoot = actions.ContinuousActions[2];
        jump = actions.ContinuousActions[3];
        boost = actions.ContinuousActions[4];

        if(accelerate > 0f)
        {
            accelerate = 1f;
        }
        else if(accelerate < 0f)
        {
            accelerate = -1f;
        }
        else
        {
            accelerate = 0f;
        }

        if(shoot > 0f)
        {
            shoot = 1f;
        }
        else
        {
            shoot = 0f;
        }

        if(jump > 0f)
        {
            jump = 1f;
        }
        else
        {
            jump = 0f;
        }

        if(boost > 0f)
        {
            boost = 1f;
        }
        else
        {
            boost= 0f;
        }

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //collect observations is a function into which we can add a number of things we want the agent to be able to perceive, in float form
        //mainly, we can add things like speed or position, but we need to carefully consider what is actually useful, and what can end up bloating the model
        //also we can add fuel, ammo, health etc

        sensor.AddObservation(GetComponent<ShipController>().fuel);
        sensor.AddObservation(GetComponent<ShipController>().ammo);
        sensor.AddObservation(GetComponent<DamageManager>().hitPoints);

        //one example I found used an observation like this, but I'm not sure if I want it... remove it for now, might be more relevant later once I fix racer order list
        //testing adding it back in
        sensor.AddObservation(GetComponent<ShipController>().distanceToNextCheckpoint);

        sensor.AddObservation(GetComponent<ShipController>().hitDistance);

        if (GetComponent<ShipController>().controlsEnabled)
        {
            if(GetComponent<ShipController>().hitDistance > 20f)
            {
                AddReward(-.01f);
            }
        }

        //I'll try giving sensors info about timeout
        if (GetComponent<ShipController>().useTimeout)
        {
            sensor.AddObservation(GetComponent<ShipController>().timeout);
        }

        //float positionReward = 0;

        //foreach (GameObject racer in raceManager.RacersList)
        //{
        //    sensor.AddObservation(racer.transform.position.z);

        //    if(racer.gameObject != this.gameObject)
        //    {
        //        if (transform.position.z > racer.transform.position.z)
        //        {
        //            positionReward = positionReward + 0.01f;
        //        }
        //        else
        //        {
        //            positionReward = positionReward - 0.01f;
        //        }
        //    }

        //}



        //for (int i = 0; i < raceManager.Orders.Count; i++)
        //{

        //}

        //several examples have also shown that we can add some rewards in here, such as a small negative reward over time, or a small positive reward for going at speed

        //this is a small negative reward which will encourage the agent to seek out positive rewards
        //controls must be enabled or else it might sit in the goal waiting and get a poor reward
        if(GetComponent<ShipController>().controlsEnabled)
        {
            AddReward(-.08f);
            //AddReward(positionReward);

        }
    }

    //rewards and punishments
    //any "refill" hit
    //any "take damage"
    //when a weapon hits a thing and deals damage
    //checkpoints
    //goal - on hitting goal, that racer gets a good reward, and level resets

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var action = actionsOut.ContinuousActions;

        action[1] = Input.GetAxis("Horizontal"); // Steering
        action[0] = Input.GetKey(KeyCode.W) ? 1f : 0f; // Acceleration

    }


}
