using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//speed modifier script
//when a ship touches the attached trigger object, its speed is either increased or slowed

public class SpeedModifier : MonoBehaviour
{
    public bool speedUp = true;

    public float speedValue = 1f; //for speeding, a value of 1 or more works (10 is good, adds 10 to top speed and acceleration), and for slowing a value of 2 should be good, halving speed

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<ShipController>() != null)
        {
            other.gameObject.GetComponent<ShipController>().isBoosting = true; //boosting tells the ship controller to start a cooldown

            if(speedUp)
            {
                other.gameObject.GetComponent<ShipController>().speedMod = speedValue;

                if(!other.gameObject.GetComponent<ShipController>().isHuman)
                {
                    other.gameObject.GetComponent<MLADrive2>().AddReward(0.05f);
                }


            }
            else
            {
                other.gameObject.GetComponent<ShipController>().slowMod = speedValue;

                if (!other.gameObject.GetComponent<ShipController>().isHuman)
                {
                    other.gameObject.GetComponent<MLADrive2>().AddReward(-0.01f);
                }

            }
        }
    }
}
