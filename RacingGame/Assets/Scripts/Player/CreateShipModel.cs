using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;

//this script instantiates the model of the ship, if enabled

public class CreateShipModel : MonoBehaviour
{

    public bool enableModel;

    //the holder is a mostly-empty object to which parts are added
    public GameObject shipModelHolder;

    //ship base is the base model of the ship
    public GameObject[] shipBase;

    //weapon object has no model for now but contain data on weapon stats
    public GameObject[] weaponObject;

    //pilot object has no model but contains some stats
    public GameObject[] pilotObject;

    //engines 
    public bool enableEngines;
    public GameObject[] shipEngines;

    public bool addCamera;
    public GameObject cam;

    public CharacterSheet charSheet;

    //easy way of remembering the pilot, which is used by the victory screen - previously used char sheet which quick races dont use
    public int pilotIndex;

    // Start is called before the first frame update
    void Start()
    {
        if (GetComponentInChildren<CharacterSheet>() != null)
        {
            charSheet = GetComponentInChildren<CharacterSheet>();

            ApplyCareerStats();
        }
        else
        {
            //CharacterSheet randomCharSheet = Instantiate(charSheet, transform.position, transform.rotation, transform);
            ApplyRandomStats();
        }
    }

    public void ApplyCareerStats()
    {
        GameObject shippy = Instantiate(shipModelHolder, transform.position, transform.rotation);
        shippy.GetComponent<SmoothFollowObject>().target = this.gameObject.transform;

        GameObject pilot = Instantiate(pilotObject[(int)charSheet.pilot], shippy.transform.position, shippy.transform.rotation, shippy.transform);
        pilotIndex = (int)charSheet.pilot;


        int shipChoice;

        if (!GetComponent<ShipController>().isHuman)
        {
            //some pilots have a specific preference, others will pick a random ship
            //player will always use the ship selected on the options screen

            if (pilot.GetComponent<PilotStats>().pilotShip > shipBase.Length - 1)
            {
                shipChoice = Random.Range(0, shipBase.Length);
                //print("random ship");
            }
            else
            {
                shipChoice = pilot.GetComponent<PilotStats>().pilotShip;
            }
        }
        else
        {
            shipChoice = (int)charSheet.pilotShip;
        }


        GameObject shipModel = Instantiate(shipBase[shipChoice], shippy.transform.GetChild(0).transform.position, transform.rotation, shippy.transform.GetChild(0).transform);

        Color[] pilotColors = pilot.GetComponent<PilotStats>().colors;

        for (int i = 0; i < shipModel.GetComponent<PilotColors>().shipParts.Length; i++)
        {
            //if (shipModel.GetComponent<PilotColors>().shipParts[i].gameObject.GetComponent<Renderer>().materials.Length <= )

            if(i == 0)
            {
                for (int j = 0; j < shipModel.GetComponent<PilotColors>().emissionColorIndexes.Length; j++)
                {
                    shipModel.GetComponent<PilotColors>().shipParts[i].gameObject.GetComponent<Renderer>().materials[shipModel.GetComponent<PilotColors>().emissionColorIndexes[j]].SetColor("_EmissionColor", pilotColors[j] * 2f);
                    //shipModel.GetComponent<PilotColors>().shipParts[i].gameObject.GetComponent<Renderer>().materials[shipModel.GetComponent<PilotColors>().colorIndexes[j]].SetColor("_EmissionColor", pilotColors[j] * 2f);

                }

                for (int j = 0; j < shipModel.GetComponent<PilotColors>().normalColorIndexes.Length; j++)
                {
                    shipModel.GetComponent<PilotColors>().shipParts[i].gameObject.GetComponent<Renderer>().materials[shipModel.GetComponent<PilotColors>().normalColorIndexes[j]].color = pilotColors[j];
                    shipModel.GetComponent<PilotColors>().shipParts[i].gameObject.GetComponent<Renderer>().materials[shipModel.GetComponent<PilotColors>().normalColorIndexes[j]].SetFloat("_Metallic", pilot.GetComponent<PilotStats>().materialMetallic);
                    shipModel.GetComponent<PilotColors>().shipParts[i].gameObject.GetComponent<Renderer>().materials[shipModel.GetComponent<PilotColors>().normalColorIndexes[j]].SetFloat("_Glossiness", pilot.GetComponent<PilotStats>().materialSmoothness);
                    //color = pilotColors[j];

                }
            }
            else
            {
                shipModel.GetComponent<PilotColors>().shipParts[i].gameObject.GetComponent<Renderer>().materials[0].SetColor("_EmissionColor", pilotColors[0] * 2f);
                shipModel.GetComponent<PilotColors>().shipParts[i].gameObject.GetComponent<Renderer>().materials[1].color = pilotColors[1];
                shipModel.GetComponent<PilotColors>().shipParts[i].gameObject.GetComponent<Renderer>().materials[1].SetFloat("_Metallic", pilot.GetComponent<PilotStats>().materialMetallic);
                shipModel.GetComponent<PilotColors>().shipParts[i].gameObject.GetComponent<Renderer>().materials[1].SetFloat("_Glossiness", pilot.GetComponent<PilotStats>().materialSmoothness);
                //SetColor("_EmissionColor", pilotColors[0]);
            }


        }

        GameObject engines = Instantiate(shipEngines[(int)charSheet.pilotEngine], shipModel.transform.GetChild(0).transform.position, transform.rotation, shipModel.transform.GetChild(0));
        
        GameObject weapons = Instantiate(weaponObject[(int)charSheet.pilotWeapons], shipModel.transform.position, transform.rotation, shipModel.transform);



        foreach (EngineBehaviour engine in engines.GetComponentsInChildren<EngineBehaviour>())
        {
            engine.parentShip = this.gameObject;

            for (int j = 0; j < engine.colors.Length; j++)
            {
                engine.colors[j] = pilotColors[j];
            }

        }

        //engine + ship stats
        float a = charSheet.acceleration + engines.GetComponent<EngineStats>().acceleration + shipModel.GetComponent<ShipStats>().acceleration + pilot.GetComponent<PilotStats>().acceleration;
        float s = charSheet.maxSpeed + engines.GetComponent<EngineStats>().maxSpeed + shipModel.GetComponent<ShipStats>().maxSpeed + pilot.GetComponent<PilotStats>().maxSpeed;
        float f = charSheet.fuelConsumption + engines.GetComponent<EngineStats>().fuelConsumption + shipModel.GetComponent<ShipStats>().fuelConsumption + pilot.GetComponent<PilotStats>().fuelConsumption;
        float b = charSheet.boostValue + engines.GetComponent<EngineStats>().boostValue + shipModel.GetComponent<ShipStats>().boostValue + pilot.GetComponent<PilotStats>().boostValue;
        float t = charSheet.maxStrafe + engines.GetComponent<EngineStats>().maxStrafe + shipModel.GetComponent<ShipStats>().maxStrafe + pilot.GetComponent<PilotStats>().maxStrafe;
        float r = charSheet.brake + engines.GetComponent<EngineStats>().brake + shipModel.GetComponent<ShipStats>().brake + pilot.GetComponent<PilotStats>().brake;
        float g = charSheet.grip + engines.GetComponent<EngineStats>().grip + shipModel.GetComponent<ShipStats>().grip + pilot.GetComponent<PilotStats>().grip;

        //ships stats
        float f2 = charSheet.maxFuel + shipModel.GetComponent<ShipStats>().maxFuel;
        float f3 = charSheet.fuel;
        float hp = charSheet.health;
        float hp2 = charSheet.maxHealth + shipModel.GetComponent<ShipStats>().maxHits;

        //weapon stats
        float w = charSheet.weaponDamage + weapons.GetComponent<WeaponStats>().weaponDamage + pilot.GetComponent<PilotStats>().weaponDamage;
        float w2 = charSheet.weaponSpeed + weapons.GetComponent<WeaponStats>().weaponSpeed;
        float w3 = charSheet.maxAmmo + weapons.GetComponent<WeaponStats>().maxAmmo + shipModel.GetComponent<ShipStats>().maxAmmo;
        float w4 = charSheet.ammoConsumption + weapons.GetComponent<WeaponStats>().ammoConsumption;
        float w5 = charSheet.weaponSlow + weapons.GetComponent<WeaponStats>().weaponSlow;
        float w6 = charSheet.ammo;


        //apply stats to the ship object
        GetComponent<DamageManager>().hitPoints = hp;
        GetComponent<DamageManager>().maxHitPoints = hp2;

        GetComponent<ShipController>().weaponsSystems = weapons.GetComponent<WeaponsSystems>();
        weapons.GetComponent<WeaponsSystems>().ship = GetComponent<ShipController>();

        GetComponent<ShipController>().careerMode = true;

        GetComponent<ShipController>().waitForTarget = weapons.GetComponent<WeaponsSystems>().missiles;

        GetComponent<ShipController>().accelerationStat = a;
        GetComponent<ShipController>().maxSpeedStat = s;
        GetComponent<ShipController>().fuelConsumption = f;
        GetComponent<ShipController>().nitroBoostStat = b;
        GetComponent<ShipController>().maxStrafe = t;
        GetComponent<ShipController>().braking = r;
        GetComponent<ShipController>().grip = g;

        GetComponent<ShipController>().maxFuel = f2;
        GetComponent<ShipController>().fuel = f3;

        GetComponent<ShipController>().weaponDamage = w;
        GetComponent<ShipController>().shootCooldown = w2;
        GetComponent<ShipController>().aimShootTime = w2;
        GetComponent<ShipController>().defaultShootCooldown = w2;
        GetComponent<ShipController>().defAimShootTime = w2;
        GetComponent<ShipController>().maxAmmo = w3;
        GetComponent<ShipController>().ammoConsumption = w4;
        GetComponent<ShipController>().weaponSlowMod = w5;
        GetComponent<ShipController>().ammo = w6;

        if (addCamera)
        {
            GameObject camera = Instantiate(cam, transform.position, transform.rotation);

            camera.GetComponent<CameraFollow>().target = shipModel.transform.GetChild(0).transform.transform;
        }
    }

    public void ApplyRandomStats()
    {
        if (enableModel)
        {

            GameObject shippy = Instantiate(shipModelHolder, transform.position, transform.rotation);

            shippy.GetComponent<SmoothFollowObject>().target = this.gameObject.transform;

            int pilotChoice = Random.Range(0, pilotObject.Length);
            GameObject pilot = Instantiate(pilotObject[pilotChoice], shippy.transform.position, transform.rotation, shippy.transform);
            pilotIndex = pilotChoice;

            pilot.GetComponent<PilotStats>().randomize = true;

            int shipChoice;

            if (pilot.GetComponent<PilotStats>().pilotShip > shipBase.Length -1)
            {
                shipChoice = Random.Range(0, shipBase.Length);
            }
            else
            {
                shipChoice = pilot.GetComponent<PilotStats>().pilotShip;
            }

            GameObject shipModel = Instantiate(shipBase[shipChoice], shippy.transform.GetChild(0).transform.position, transform.rotation, shippy.transform.GetChild(0).transform);
            shipModel.GetComponent<ShipStats>().randomize = true;

            if (enableEngines)
            {

                int engineChoice;
                int weaponChoice;

                if (pilot.GetComponent<PilotStats>().pilotEngine > shipEngines.Length)
                {
                    engineChoice = Random.Range(0, shipEngines.Length);
                }
                else
                {
                    engineChoice = pilot.GetComponent<PilotStats>().pilotEngine;
                }

                if (pilot.GetComponent<PilotStats>().pilotWeapons > weaponObject.Length)
                {
                    weaponChoice = Random.Range(0, weaponObject.Length);
                }
                else
                {
                    weaponChoice = pilot.GetComponent<PilotStats>().pilotWeapons;
                }

                GameObject engines = Instantiate(shipEngines[engineChoice], shipModel.transform.GetChild(0).transform.position, transform.rotation, shipModel.transform.GetChild(0).transform);
                engines.GetComponent<EngineStats>().randomize = true;

                foreach (EngineBehaviour engine in engines.GetComponentsInChildren<EngineBehaviour>())
                {
                    engine.parentShip = this.gameObject;

                }

                GameObject weapons = Instantiate(weaponObject[weaponChoice], shipModel.transform.position, transform.rotation, shipModel.transform);


                //apply engine stats
                float a = engines.GetComponent<EngineStats>().acceleration + shipModel.GetComponent<ShipStats>().acceleration + pilot.GetComponent<PilotStats>().acceleration;
                float s = engines.GetComponent<EngineStats>().maxSpeed + shipModel.GetComponent<ShipStats>().maxSpeed + pilot.GetComponent<PilotStats>().maxSpeed;
                float f = engines.GetComponent<EngineStats>().fuelConsumption + shipModel.GetComponent<ShipStats>().fuelConsumption + pilot.GetComponent<PilotStats>().fuelConsumption;
                float b = engines.GetComponent<EngineStats>().boostValue + shipModel.GetComponent<ShipStats>().boostValue + pilot.GetComponent<PilotStats>().boostValue;
                float t = engines.GetComponent<EngineStats>().maxStrafe + shipModel.GetComponent<ShipStats>().maxStrafe + pilot.GetComponent<PilotStats>().maxStrafe;
                float r = engines.GetComponent<EngineStats>().brake + shipModel.GetComponent<ShipStats>().brake + pilot.GetComponent<PilotStats>().brake;
                float g = engines.GetComponent<EngineStats>().grip + shipModel.GetComponent<ShipStats>().grip + pilot.GetComponent<PilotStats>().grip;

                //ships stats
                float f2 = shipModel.GetComponent<ShipStats>().maxFuel + pilot.GetComponent<PilotStats>().maxFuel;

                //weapon stats
                float w = weapons.GetComponent<WeaponStats>().weaponDamage;
                float w2 = weapons.GetComponent<WeaponStats>().weaponSpeed;
                float w3 = weapons.GetComponent<WeaponStats>().maxAmmo;
                float w4 = weapons.GetComponent<WeaponStats>().ammoConsumption;
                float w5 = weapons.GetComponent<WeaponStats>().weaponSlow;

                GetComponent<ShipController>().weaponsSystems = weapons.GetComponent<WeaponsSystems>();
                weapons.GetComponent<WeaponsSystems>().ship = GetComponent<ShipController>();

                GetComponent<ShipController>().waitForTarget = weapons.GetComponent<WeaponsSystems>().missiles;

                if(a < 1f)
                {
                    a = 1f;
                }

                if(s < 1f)
                {
                    s = 1f;
                }

                GetComponent<ShipController>().accelerationStat = GetComponent<ShipController>().accelerationStat + a;
                GetComponent<ShipController>().maxSpeedStat = GetComponent<ShipController>().maxSpeedStat + s;
                GetComponent<ShipController>().fuelConsumption = GetComponent<ShipController>().fuelConsumption + f;
                GetComponent<ShipController>().nitroBoostStat = GetComponent<ShipController>().nitroBoostStat + b;
                GetComponent<ShipController>().maxStrafe = GetComponent<ShipController>().maxStrafe + t;
                GetComponent<ShipController>().braking = GetComponent<ShipController>().braking + r;
                GetComponent<ShipController>().grip = GetComponent<ShipController>().grip + g;

                GetComponent<ShipController>().maxFuel = GetComponent<ShipController>().maxFuel + f2;

                GetComponent<ShipController>().weaponDamage = GetComponent<ShipController>().weaponDamage + w;
                GetComponent<ShipController>().shootCooldown = GetComponent<ShipController>().shootCooldown + w2;
                GetComponent<ShipController>().aimShootTime = GetComponent<ShipController>().aimShootTime + w2;
                GetComponent<ShipController>().defaultShootCooldown = GetComponent<ShipController>().defaultShootCooldown + w2;
                GetComponent<ShipController>().defAimShootTime = GetComponent<ShipController>().defAimShootTime + w2;
                GetComponent<ShipController>().maxAmmo = GetComponent<ShipController>().maxAmmo + w3;
                GetComponent<ShipController>().ammoConsumption = GetComponent<ShipController>().ammoConsumption + w4;
                GetComponent<ShipController>().weaponSlowMod = GetComponent<ShipController>().weaponSlowMod + w5;
                //GetComponent<ShipController>().shotPoint = weapons.GetComponent<WeaponStats>().shotPoint;

                if (GetComponent<ShipController>().inTraining)
                {
                    GetComponent<DamageManager>().hitPoints = Random.Range(1f, 100f);
                    GetComponent<DamageManager>().maxHitPoints = GetComponent<DamageManager>().hitPoints;
                    GetComponent<ShipController>().ammo = Random.Range(0f, 100f);
                    GetComponent<ShipController>().fuel = Random.Range(0f, 100f);
                }

            }

            if (addCamera)
            {
                GameObject camera = Instantiate(cam, transform.position, transform.rotation);

                camera.GetComponent<CameraFollow>().target = shipModel.transform.GetChild(0).transform;
            }

        }
    }
}
