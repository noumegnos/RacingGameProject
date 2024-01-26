using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.Assertions.Must;
using Unity.VisualScripting;

public class MLADrive : Agent
{

    public float accelerate;
    public float steering;

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

        //the actions.ContinuousActions[] appear here, and must be grabbed by the controller script
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //the sensor needs to collect some information, such as speed and steering
        //sensor takes floats
        //should I make it sensor a velocity? That would be a Vector3(float,float,float)... at least it's not complaining, but is it good?
        //damn there is no speed stat, only velocity
        //velocity is added to in a bunch of different ways, I think it makes sense to just use it directly as observation

        //maybe what sensors need is simply access to the rigidbody's actual values, instead of the variables that inform the values

        //_body.velocity is a Vector3 containing the direction the ship is going in
        sensor.AddObservation(gameObject.GetComponent<PlayerController>()._body.velocity);

        //_body.rotation is a Quaternion containing the rotational information of the ship
        sensor.AddObservation(gameObject.GetComponent<PlayerController>()._body.rotation);

        //added 5 new observations
        //some information about checkpoints
        sensor.AddObservation(gameObject.GetComponent<PlayerController>().currentCheckpoint);
        sensor.AddObservation(gameObject.GetComponent<PlayerController>().lastCheckpointTouched);

        //sensor.AddObservation(raceManager.Orders[0]);
        //sensor.AddObservation(raceManager.Orders[1]);
        //sensor.AddObservation(raceManager.Orders[2]);

        //if (raceManager.Orders[0].gameObject == this.gameObject)
        //{
        //    AddReward(0.01f);
        //    print("I am first! " + raceManager.Orders[0].gameObject + this.gameObject);
        //}
        //else if (raceManager.Orders[1].gameObject == this.gameObject)
        //{
        //    AddReward(0.005f);
        //}
        //else if (raceManager.Orders[2].gameObject == this.gameObject)
        //{
        //    AddReward(0.001f);
        //}
        //else
        //{
        //    AddReward(-0.001f);
        //}

        //add some more information about the ship
        sensor.AddObservation(gameObject.GetComponent<PlayerController>().currentSpeed);
        sensor.AddObservation(gameObject.GetComponent<PlayerController>()._isGrounded);

        //add some more information about checkpoint timer (also removed information about orders)
        sensor.AddObservation(gameObject.GetComponent<PlayerController>().timeLeft);



        //QUESTION
        //does Vector Observation Space Size now take 2 things, or one for each float in the _body.velocity and rotation, which is I think 7? We'll get an error later so we will see

    }

    // ON REWARDS

    //I've looked through a few different examples of how to set up unity's ML agents, and how these apply rewards
    //it appears that you can add rewards pretty much whenever you want, there is no need for special functions
    //so I'll try to compile a list of where I ended up putting them, so I can keep track
    //I just need to call MLADrive.AddReward(#f); and EndEpisode();
    //-Reward added in PlayerController ProcessVelocityCollisions; there are two parts, one for hitting ships and one for walls
    //Add positive reward when crossing correct checkpoint, and negative if wrong one

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var action = actionsOut.ContinuousActions;

        action[1] = Input.GetAxis("Horizontal"); // Steering
        action[0] = Input.GetKey(KeyCode.W) ? 1f : 0f; // Acceleration

    }


}
