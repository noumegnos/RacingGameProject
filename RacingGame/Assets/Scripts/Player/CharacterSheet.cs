using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSheet : MonoBehaviour //, IDataPersistence
{
    public float wins;
    public float losses;
    public float money;

    public float health;
    public float maxHealth;
    public float ammo;
    public float fuel;

    public float acceleration;
    public float maxSpeed;
    public float maxFuel;
    public float fuelConsumption;
    public float boostValue;
    public float maxStrafe;
    public float brake;
    public float grip;

    public float weaponDamage;
    public float weaponSpeed;
    public float maxAmmo;
    public float ammoConsumption;
    public float weaponSlow;

    public float weaponAccuracy;

    public float pilot; //somewhere there is a list of pilots, and the sheet will get an int representing the pilot - used when creating ship model
    //maybe pilot is only really relevant in the career page, to show a character portrait
    //I guess AI pilots will also get their data from this pilot list?

    public float pilotEngine; //there are 3 engines, 0-2, and when ship is spawned if this value is not one of these a random one is picked
    public float pilotWeapons;
    public float pilotShip; //currently only one ship exists but same principle applies

    //public List<float> sheetValues; 

    private void Start()
    {
        //sheetValues.AddRange(new List<float>() {wins, losses, money, health, maxHealth, ammo, maxAmmo, fuel, maxFuel,
        //acceleration, maxSpeed, fuelConsumption, boostValue, maxStrafe, brake, grip, weaponDamage, weaponSlow, weaponSpeed,
        //ammoConsumption,weaponAccuracy,pilot, pilotEngine, pilotWeapons, pilotShip});


    }

    //public void LoadData(GameData gameData)
    //{
    //    this.wins = gameData.wins;
    //    this.losses = gameData.losses;
    //    //this.money = gameData.money;
    //}

    //public void SaveData(ref GameData gameData)
    //{
    //    gameData.wins = this.wins;
    //}

}
