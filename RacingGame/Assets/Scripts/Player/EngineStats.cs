using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this carries some data about engine performance, which is then added to the ship object

public class EngineStats : MonoBehaviour
{

    public float acceleration;
    public float maxSpeed;
    public float fuelConsumption;
    public float boostValue;
    public float maxStrafe;
    public float brake;
    public float grip;

    public bool randomize;

    public void Awake()
    {
        if (randomize)
        {
            acceleration = Random.Range(-2f, 5f);
            maxSpeed = Random.Range(-10f, 10f);
            fuelConsumption = Random.Range(-9f, 10f);
            boostValue = Random.Range(-4f, 10f);
            maxStrafe = Random.Range(-1f, 2f);
            brake = Random.Range(-2f, 10f);
            grip = Random.Range(-2f, 10f);
        }
    }


}


