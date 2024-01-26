using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script refills fuel, ammo, and/or health of the ship hitting the trigger

public class RefillScript : MonoBehaviour
{
    //whether or not to fill up on fuel, ammo, or health
    public bool refuel;
    public bool reammo;
    public bool repair;

    //amount of fuel, ammo, health to refill
    public float fuelAmount;
    public float ammoAmount;
    public int repairAmount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<ShipController>() != null)
        {
            if (refuel)
            {
                other.GetComponent<ShipController>().fuel = Mathf.Min(other.GetComponent<ShipController>().fuel + fuelAmount, other.GetComponent<ShipController>().maxFuel);
                //print("boop");
            }

            if(reammo)
            {
                other.GetComponent<ShipController>().ammo = Mathf.Min(other.GetComponent<ShipController>().ammo + ammoAmount, other.GetComponent<ShipController>().maxAmmo);

            }

            if (repair)
            {
                other.GetComponent<DamageManager>().hitPoints = Mathf.Min(other.GetComponent<DamageManager>(). hitPoints + repairAmount, other.GetComponent<DamageManager>().maxHitPoints);
            }

            if (!other.gameObject.GetComponent<ShipController>().isHuman)
            {
                other.gameObject.GetComponent<MLADrive2>().AddReward(0.5f);
            }
        }
    }
}
