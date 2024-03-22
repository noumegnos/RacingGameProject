using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilotStats : MonoBehaviour
{
    public string pilotName;
    public string description;

    //in quick races, these variables are added to the racer's stats
    //these variables are the pilot's preferences in career mode, and inform which upgrades to prioritize
    public float acceleration;
    public float maxSpeed;
    public float maxFuel;
    public float fuelConsumption;
    public float boostValue;
    public float maxStrafe;
    public float brake;
    public float grip;
    public float weaponDamage;
    public float weaponSlow;
    public float weaponSpeed;

    public int pilotEngine; //there are 3 engines, 0-2, and when ship is spawned if this value is not one of these a random one is picked
    public int pilotWeapons;
    public int pilotShip; //currently only one ship exists but same principle applies

    public Color[] colors;
    public float materialMetallic;
    public float materialSmoothness;

    public Sprite pilotFace;

    //during training, stats are always randomized
    public bool randomize;

    public void Awake()
    {
        if (randomize)
        {
            acceleration = Random.Range(-0.5f, 0.5f);
            maxSpeed = Random.Range(-5f, 5f);
            maxFuel = Random.Range(-5f, 5f);
            fuelConsumption = Random.Range(-5f, 10f);
            boostValue = Random.Range(-1f, 1f);
            maxStrafe = Random.Range(-1f, 2f);
            brake = Random.Range(-2f, 10f);
            grip = Random.Range(-1f, 10f);
        }
    }
}
