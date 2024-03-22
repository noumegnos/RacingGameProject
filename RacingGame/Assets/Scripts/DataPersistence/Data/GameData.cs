using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this is a data structure for saving the game



[System.Serializable]
public class GameData
{
    //public int wins;

    //public int losses;

    //public int money;

    //public int damage;

    //public int ammo;

    //public int fuel;

    //public int lastShip;

    ////TODO upgrades...

    //in Race Manager I'm giving the entire list of stats for each racer in the racer into this list to save
    public SerializableList<float> sheets = new SerializableList<float>();

    public long lastUpdated;

    public GameData()
    {
        this.sheets = new SerializableList<float>();
        //this.wins = 0;
        //this.losses = 0;
        //this.money = 1000;
        //this.damage = 0;
        //this.ammo = 0;
        //this.fuel = 0;
        //this.lastShip = 0;

    }


}
