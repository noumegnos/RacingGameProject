using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    //the checkpoint number needs to be incremented when the levelPart this checkpoint belongs to is generated
    //the levelPart has a list of the checkpoints inside it, in order
    //so when the levelPart is created, this number is updated to reflect the checkpoints position in the full list
    public int checkpointNumber;

    //the respawn point is given to the player when they cross the checkpoint trigger
    //isRespawnCheckpoint is used on only some Checkpoints, because it seems I have to manually add an object to each checkpoint if I want them to give a good rotation
    //in the newer version, I can easily set checkpoint respawn object to the prefab, so they can always have it - it was only a concern previously because I had a large list of checkpoints premade as an array of objects via blender 
    public bool isRespawnCheckpoint;
    public Transform respawnPoint;
    public Quaternion respawnRotation;
    public bool useRespawnRotation = false; //rotation should not be relevant in the simplified version
    
    // Start is called before the first frame update
    void Start()
    {
        respawnPoint = transform;

        //if the checkpoint has a child component (transform), use its rotation as the respawnRotation
        if(isRespawnCheckpoint)
        {
            if (GetComponentInChildren<SetRespawnRotation>() != null)
            {
                respawnRotation = GetComponentInChildren<SetRespawnRotation>().gameObject.transform.rotation;
                respawnPoint = GetComponentInChildren<SetRespawnRotation>().gameObject.transform;
                useRespawnRotation = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<ShipController>() != null)
        {
            if (!other.GetComponent<ShipController>().isHuman)
            {
                if(other.GetComponent<ShipController>().lastCheckpointNumber < checkpointNumber || other.GetComponent<ShipController>().lastCheckpointNumber == 0)
                {
                    other.GetComponent<ShipController>().timeout = other.GetComponent<ShipController>().defTimeout;

                    other.GetComponent<MLADrive2>().AddReward(0.8f);

                    if (isRespawnCheckpoint)
                    {
                        other.GetComponent<MLADrive2>().EndEpisode();

                    }
                }
                
                if(other.GetComponent<ShipController>().lastCheckpointNumber > checkpointNumber)
                {
                    other.GetComponent<MLADrive2>().AddReward(-0.5f);
                }
            }

            if (other.GetComponent<ShipController>().lastCheckpointNumber < checkpointNumber || other.GetComponent<ShipController>().lastCheckpointNumber == 0)
            {
                if(isRespawnCheckpoint)
                {
                    other.GetComponent<ShipController>().lastCheckpoint = this.gameObject;

                }

                other.GetComponent<ShipController>().lastCheckpointNumber = checkpointNumber;

            }


            if (other.GetComponent<ShipController>().checkpointManager.dictOfChecks.Count > checkpointNumber)
            {
                other.GetComponent<ShipController>().nextCheckpointTransform = other.GetComponent<ShipController>().checkpointManager.dictOfChecks[checkpointNumber + 1];
            }
            else
            {
                //else the track is ending, next checkpoint is goal
                //just make the goal a checkpoint, make everything easier
                //need anything in here?
            }
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    //if(TryGetComponent(out PlayerController playerController)){
    //    if(other.gameObject.layer == LayerMask.NameToLayer("Ship")){
    //        //print("layered");



    //        if(!other.gameObject.GetComponent<PlayerController>().isHuman)
    //        {
    //            if(checkpointNumber > other.gameObject.GetComponent<PlayerController>().lastCheckpointKnown)
    //            {
    //                other.gameObject.GetComponent<MLADrive>().AddReward(0.01f);
    //                //other.gameObject.GetComponent<MLADrive>().EndEpisode();
    //                //print("test");

    //                //the timer here is used for agents, but players should also have one?
    //                other.gameObject.GetComponent<PlayerController>().timeLeft = other.gameObject.GetComponent<PlayerController>().timeToReachNextCheckpoint;

    //            }
    //            else if(checkpointNumber < other.gameObject.GetComponent<PlayerController>().lastCheckpointKnown)
    //            {
    //                other.gameObject.GetComponent<MLADrive>().AddReward(-0.01f);
    //            }
    //        }

    //        other.gameObject.GetComponent<PlayerController>().lastCheckpointTouched = checkpointNumber;
    //        other.gameObject.GetComponent<PlayerController>().lastCheckpointKnown = checkpointNumber;

    //        if (other.gameObject.GetComponent<PlayerController>().currentCheckpoint == checkpointNumber){

    //            other.gameObject.GetComponent<PlayerController>().currentCheckpoint++;

    //            if (isRespawnCheckpoint)
    //            {
    //                other.gameObject.GetComponent<PlayerController>().lastRespawnPoint = respawnPoint;

    //                if (useRespawnRotation)
    //                {
    //                    other.gameObject.GetComponent<PlayerController>().lastRespawnRotation = respawnRotation;
    //                }
    //                else
    //                {
    //                    other.gameObject.GetComponent<PlayerController>().lastRespawnRotation = other.gameObject.GetComponent<PlayerController>().transform.rotation;
    //                }
    //            }




    //            if (!other.gameObject.GetComponent<PlayerController>().isHuman)
    //            {
    //                other.gameObject.GetComponent<MLADrive>().AddReward(other.gameObject.GetComponent<PlayerController>().currentSpeed / 100f); //currentSpeed goes up to 80
    //                //other.gameObject.GetComponent<MLADrive>().EndEpisode();

    //                other.gameObject.GetComponent<MLADrive>().AddReward(other.gameObject.GetComponent<PlayerController>().timeLeft / 10f);
    //            }
    //            //other.gameObject.GetComponent<MLADrive>().AddReward(0.1f + (other.gameObject.GetComponent<PlayerController>().currentSpeed / 10f));
    //            //other.gameObject.GetComponent<MLADrive>().EndEpisode();




    //        }
    //        else if(other.gameObject.GetComponent<PlayerController>().currentCheckpoint < checkpointNumber)
    //        {
    //            //this happens if the ship ends up skipping a checkpoint, like if it jumps to another part of track
    //            //ideally this case couldn't happen, I should design the track to prevent it
    //            //OR I should embrace it, and allow skipping ahead
    //            //BUT then I must fix the physics so you can drop down onto another track
    //            print("checkpoint missed");
    //            //this should be put on the UI if the ship is the players

    //            if(!other.gameObject.GetComponent<PlayerController>().isHuman)
    //                other.gameObject.GetComponent<MLADrive>().AddReward(-0.1f);
    //                //other.gameObject.GetComponent<MLADrive>().EndEpisode();

    //        }
    //        else if (other.gameObject.GetComponent<PlayerController>().currentCheckpoint > checkpointNumber)
    //        {
    //            //this happens if ship is going backwards, but also if it has been respawned a few steps back
    //            //need another check to see if ship is going backward, since this should incur a large penalty for the agent
    //            //but it shouldn't be punished after having been respawned, if it's trying to move forwards
    //            //I will remove the punishment for now; there are other negative rewards for idling and not driving, they should cover it
    //            print("wrong checkpoint");

    //            //if (!other.gameObject.GetComponent<PlayerController>().isHuman)
    //            //    other.gameObject.GetComponent<MLADrive>().AddReward(-0.1f);
    //        }
    //    }
    //}
}
