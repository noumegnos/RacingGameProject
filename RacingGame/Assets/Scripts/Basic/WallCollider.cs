using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollider : MonoBehaviour
{

    float bounciness = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //I made a modification which looks awful
    public Vector3 ModifyVelocity(GameObject collidingShip, RaycastHit collisionHit)//, IRacer racer)
    {

        Vector3 modifiedVelocity = Vector3.zero;

        if(collidingShip.GetComponent<PlayerController>() != null){

            if (Mathf.Abs(Vector3.Dot(collisionHit.normal, Vector3.up)) > .2f) return collidingShip.GetComponent<PlayerController>()._velocity;

            //If the ship is an AI agent, give it a negtive reward for colliding
            // if(!collidingShip.GetComponent<PlayerController>().isHuman)
            //     collidingShip.GetComponent<MLADrive>().AddReward(-1f);
            //ACTUALLY this function is called FROM the ship, so obviously just need to put the AddReward in there

            modifiedVelocity = collidingShip.GetComponent<PlayerController>()._velocity;

            if (collidingShip.GetComponent<PlayerController>()._isGrounded)
            {
                modifiedVelocity = Vector3.ProjectOnPlane(modifiedVelocity, collidingShip.GetComponent<PlayerController>().m_CurrentGroundInfo.normal);
            }


            modifiedVelocity -= Vector3.Project(modifiedVelocity, collisionHit.normal) * (1f + bounciness);

            //return modifiedVelocity;

        }
        if(collidingShip.GetComponent<TrafficController>() != null){

            if (Mathf.Abs(Vector3.Dot(collisionHit.normal, Vector3.up)) > .2f) return collidingShip.GetComponent<TrafficController>()._velocity;

            modifiedVelocity = collidingShip.GetComponent<TrafficController>()._velocity;

            if (collidingShip.GetComponent<TrafficController>()._isGrounded)
            {
                modifiedVelocity = Vector3.ProjectOnPlane(modifiedVelocity, collidingShip.GetComponent<TrafficController>().m_CurrentGroundInfo.normal);
            }

            modifiedVelocity -= Vector3.Project(modifiedVelocity, collisionHit.normal) * (1f + bounciness);

            //return modifiedVelocity;
        }

        return modifiedVelocity;

    }
}
