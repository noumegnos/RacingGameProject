using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//starting lights contains an animation event, which triggers the race manager, allowing the racers to start racing

public class StartingLights : MonoBehaviour
{
    //public RaceManager raceManager;

    public void RaceBegin()
    {
        GetComponentInParent<RaceManager>().CountdownStart();
    }

}
