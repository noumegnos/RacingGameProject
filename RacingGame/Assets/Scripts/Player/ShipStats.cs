using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this contains the ship's stats, which is added to the ship objects base stats

public class ShipStats : MonoBehaviour
{
    public float acceleration;
    public float maxSpeed;
    public float maxFuel;
    public float maxHits;
    public float maxAmmo;
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
            acceleration = Random.Range(-1f, 5f);
            maxSpeed = Random.Range(-10f, 10f);
            maxFuel = Random.Range(-40f, 40f);
            fuelConsumption = Random.Range(-9f, 10f);
            boostValue = Random.Range(-4f, 10f);
            maxStrafe = Random.Range(-1f, 2f);
            brake = Random.Range(-2f, 10f);
            grip = Random.Range(-1f, 10f);
        }
    }
}
