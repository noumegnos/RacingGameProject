using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStats : MonoBehaviour
{
    public float weaponDamage;
    public float weaponSpeed;
    public float maxAmmo;
    public float ammoConsumption;
    public float weaponSlow;

    public float weaponAccuracy;

    //public Transform shotPoint;

    public bool randomize;

    public void Awake()
    {
        if (randomize)
        {
            weaponDamage = Random.Range(0, 3);
            weaponSpeed = Random.Range(-0.2f, 0.2f);
            maxAmmo = Random.Range(-40f, 40f);
            weaponSlow = Random.Range(-2f, 2f);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
