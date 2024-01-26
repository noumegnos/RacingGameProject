using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script instantiates the model of the ship, if enabled

public class CreateShipModel : MonoBehaviour
{

    public bool enableModel;
    public GameObject shipModel;

    // Start is called before the first frame update
    void Start()
    {
        if(enableModel){

            GameObject shippy = Instantiate(shipModel, transform.position, transform.rotation);

            shippy.GetComponent<SmoothFollowObject>().target = this.gameObject.transform;

        }

    }
}
