using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script manages hit points and damage for all destructible objects and ships

public class DamageManager : MonoBehaviour
{
    public int maxHitPoints;
    public int hitPoints;

    //if the object is a racer, death causes them to be respawned
    public bool isRacer;
    ShipController shipController; //if isRacer, object has Ship Controller, which contains the repositioning function called when killed

    //this will cause a new object to replace the destroyed object
    //brokenParts can also include effects like explosions and such
    public bool spawnBrokenPartsOnDeath;
    public GameObject brokenParts;
    public List<GameObject> partsToDestroy = new List<GameObject>();

    //in some cases the transform of the broken parts needs to get its transform from another object, which needs a reference
    public bool useSpecificObjectTransform;
    public Transform specificObjectTransform;

    //carMove is set manually, and if it has been set, this object will tell Car Move script to stop driving
    //this pertains to trucks, which have multiple pieces; if the driver is destroyed the carriages should stop
    public CarMove carMove;

    // Start is called before the first frame update
    void Start()
    {
        hitPoints = maxHitPoints;

        if(isRacer)
        {
            shipController = GetComponent<ShipController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        if(GetComponent<MLADrive2>() != null)
        {
            GetComponent<MLADrive2>().AddReward(-.005f);
        }


        if (hitPoints > 0)
        {
            hitPoints = hitPoints - damage;

            if(hitPoints <= 0)
            {
                Death();
            }
        }
        else
        {
            Death();
        }
    }

    public void Death()
    {
        if(spawnBrokenPartsOnDeath)
        {
            if(!isRacer)
            {
                if(useSpecificObjectTransform)
                {
                    GameObject parts = Instantiate(brokenParts, specificObjectTransform.transform.position, specificObjectTransform.transform.rotation);

                }
                else
                {
                    GameObject parts = Instantiate(brokenParts, transform.position, transform.rotation);

                }

                if(carMove != null)
                {
                    carMove.checkForObstacles = false;
                    carMove.drive = false;
                }

                foreach (GameObject item in partsToDestroy)
                {
                    //will this work ok? It could attempt to destroy itself before all parts are destroyed. Test on barriers.
                    ////Maybe as easy as setting the object holding this script to destroy last? At least no errors that way
                    Destroy(item);
                }
            }
            else
            {
                //could add some effect here, or maybe a slight delay
                shipController.RepositionAfterDelay();
                hitPoints = maxHitPoints;

            }
        }
        else
        {
            if (carMove != null)
            {
                carMove.checkForObstacles = false;
                carMove.drive = false;
            }
            
            foreach (GameObject item in partsToDestroy)
            {
                //will this work ok? It could attempt to destroy itself before all parts are destroyed. Test on barriers
                Destroy(item);
            }

            if (isRacer)
            {
                //could add some effect here, or maybe a slight delay
                shipController.RepositionAfterDelay();
                hitPoints = maxHitPoints;
            }
        }
    }
}
