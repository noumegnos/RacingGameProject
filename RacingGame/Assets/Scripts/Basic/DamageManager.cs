using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script manages hit points and damage for all destructible objects and ships

public class DamageManager : MonoBehaviour
{
    public float maxHitPoints;
    public float hitPoints;

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

    public bool dropPickupOnDeath; //some objects will drop a single use powerup when destroyed
    public float chanceToDrop;
    public GameObject[] drops;

    public bool doExplode;
    public GameObject explosion;

    public bool doSparks;
    //public GameObject sparks;

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

    public bool TakeDamage(float damage)
    {
        if(GetComponent<MLADrive2>() != null)
        {
            GetComponent<MLADrive2>().AddReward(-.005f);
        }

        if(doSparks)
        {
            GetComponentInChildren<ParticleSystem>().Play();
        }

        if (GetComponent<ShipSounds>() != null)
        {
            GetComponent<ShipSounds>().PlayHitSound();
        }


        if (hitPoints > 0)
        {
            hitPoints = hitPoints - damage;

            if(hitPoints <= 0)
            {
                Death();

                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            Death();
            return true;
        }
    }

    public void Death()
    {
        if (doExplode)
        {
            //if(GetComponent<ShipSounds>() != null)
            //{
            //    GetComponent<ShipSounds>().PlayExplosionSound();
            //}

            if(useSpecificObjectTransform)
            {

                GameObject explode = Instantiate(explosion, GetComponent<Collider>().bounds.center, transform.rotation);

            }
            else
            {
                GameObject explode = Instantiate(explosion, transform.position, transform.rotation);

            }
        }

        if(spawnBrokenPartsOnDeath)
        {
            if(!isRacer)
            {
                if(dropPickupOnDeath)
                {
                    if(Random.Range(0f, 1f) < chanceToDrop)
                    {
                        GameObject drop = Instantiate(drops[Random.Range(0, drops.Length - 1)], transform.position, Quaternion.identity);
                    }
                }


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
                RacerDestroyed();

            }
        }
        else
        {
            //if not spawning broken parts
            //eventually I will add more broken parts to many objects


            if (carMove != null)
            {
                carMove.checkForObstacles = false;
                carMove.drive = false;
            }
            
            foreach (GameObject item in partsToDestroy)
            {
                //will this work ok? It could attempt to destroy itself before all parts are destroyed. Test on barriers
                //seems good
                Destroy(item);
            }

            if (isRacer)
            {
                RacerDestroyed();
            }
        }
    }

    public void RacerDestroyed()
    {
        //could add some effect here, or maybe a slight delay
        if (!shipController.careerMode)
        {
            shipController.RepositionAfterDelay();

            if (!shipController.inTraining)
            {
                //this will occur in Quick Race mode
                hitPoints = maxHitPoints;
            }
            else
            {
                //during ML training, agents get a random amount of hit points
                hitPoints = Random.Range(0f, maxHitPoints);
            }

        }
        else
        {
            //if the player is destroyed, it leads to an instant loss
            if (shipController.isHuman)
            {
                shipController.raceManager.OnRaceLoss();
            }
        }
    }
}
