using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//career options include being able to repair and upgrade ship

//first, I'll make a system to display some relevant career data

public class CareerOptionsPage : MonoBehaviour, IDataPersistence
{
    public RaceManager raceManager;

    [SerializeField] private TextMeshProUGUI pilotText;
    [SerializeField] private TextMeshProUGUI cashText;
    [SerializeField] private TextMeshProUGUI winsText;

    [SerializeField] private TextMeshProUGUI enginesText;
    [SerializeField] private TextMeshProUGUI weaponsText;
    [SerializeField] private TextMeshProUGUI shipText;

    [SerializeField] private TextMeshProUGUI repairText;
    [SerializeField] private TextMeshProUGUI repairCostText;
    [SerializeField] private TextMeshProUGUI repairUpgradeCostText;
    [SerializeField] private Button repairButton;
    [SerializeField] private Button repairUpgradeButton;
    [SerializeField] private TextMeshProUGUI fuelText;
    [SerializeField] private TextMeshProUGUI fuelCostText;
    [SerializeField] private TextMeshProUGUI fuelUpgradeCostText;
    [SerializeField] private Button fuelButton;
    [SerializeField] private Button fuelUpgradeButton;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI ammoCostText;
    [SerializeField] private TextMeshProUGUI ammoUpgradeCostText;
    [SerializeField] private Button ammoButton;
    [SerializeField] private Button ammoUpgradeButton;

    [SerializeField] private TextMeshProUGUI accelerationText;
    [SerializeField] private TextMeshProUGUI accelerationCostText;
    [SerializeField] private Button accelerationButton;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI speedCostText;
    [SerializeField] private Button speedButton;
    [SerializeField] private TextMeshProUGUI gripText;
    [SerializeField] private TextMeshProUGUI gripCostText;
    [SerializeField] private Button gripButton;
    [SerializeField] private TextMeshProUGUI strafeText;
    [SerializeField] private TextMeshProUGUI strafeCostText;
    [SerializeField] private Button strafeButton;
    [SerializeField] private TextMeshProUGUI brakeText;
    [SerializeField] private TextMeshProUGUI brakeCostText;
    [SerializeField] private Button brakeButton;

    [SerializeField] private TextMeshProUGUI fuelEfficiencyText;
    [SerializeField] private TextMeshProUGUI fuelEfficiencyCostText;
    [SerializeField] private Button fuelEfficiencyButton;
    [SerializeField] private TextMeshProUGUI boostValueText;
    [SerializeField] private TextMeshProUGUI boostValueCostText;
    [SerializeField] private Button boostValueButton;

    [SerializeField] private TextMeshProUGUI weaponDamageText;
    [SerializeField] private TextMeshProUGUI weaponDamageCostText;
    [SerializeField] private Button weaponDamageButton;
    [SerializeField] private TextMeshProUGUI weaponSlowText;
    [SerializeField] private TextMeshProUGUI weaponSlowCostText;
    [SerializeField] private Button weaponSlowButton;
    [SerializeField] private TextMeshProUGUI weaponSpeedText;
    [SerializeField] private TextMeshProUGUI weaponSpeedCostText;
    [SerializeField] private Button weaponSpeedButton;

    float wins;
    float pilot;
    float ship;
    float engine;
    float weapons;

    float damage;
    float maxDamage;
    float cash;

    float fuel;
    float maxFuel;

    float ammo;
    float maxAmmo;

    float acceleration;
    float maxSpeed;
    float grip;
    float maxStrafe;
    float brake;

    float fuelConsumption;
    float boostValue;

    float weaponDamage;
    float weaponSlow;
    float ammoConsumption;
    float weaponSpeed;

    public DataPersistenceManager persistenceManager;

    public Image accelerationUpgradeLevels;
    public Image speedUpgradeLevels;
    public Image gripUpgradeLevels;
    public Image strafeUpgradeLevels;
    public Image brakeUpgradeLevels;
    public Image fuelUseUpgradeLevels;
    public Image boostUpgradeLevels;
    public Image weaponDamageUpgradeLevels;
    public Image weaponSlowUpgradeLevels;
    public Image weaponSpeedUpgradeLevels;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AgentRepairUntilFull(int i)
    {
        //repair 
        if (raceManager.characterSheets[i].GetComponent<CharacterSheet>().health < raceManager.characterSheets[i].GetComponent<CharacterSheet>().maxHealth)
        {
            raceManager.characterSheets[i].GetComponent<CharacterSheet>().money -= 100f;
            raceManager.characterSheets[i].GetComponent<CharacterSheet>().health += 10f;

            if (raceManager.characterSheets[i].GetComponent<CharacterSheet>().health > raceManager.characterSheets[i].GetComponent<CharacterSheet>().maxHealth)
            {
                raceManager.characterSheets[i].GetComponent<CharacterSheet>().health = raceManager.characterSheets[i].GetComponent<CharacterSheet>().maxHealth;
            }
        }

        if (raceManager.characterSheets[i].GetComponent<CharacterSheet>().health < raceManager.characterSheets[i].GetComponent<CharacterSheet>().maxHealth)
        {
            if (raceManager.characterSheets[i].GetComponent<CharacterSheet>().money >= 100f)
            {
                AgentRepairUntilFull(i);
            }
        }
    }

    public void AgentRefuelUntilEnough(int i, float enough)
    {
        //refuel 
        if (raceManager.characterSheets[i].GetComponent<CharacterSheet>().fuel < (raceManager.characterSheets[i].GetComponent<CharacterSheet>().maxFuel * enough))
        {
            raceManager.characterSheets[i].GetComponent<CharacterSheet>().money -= 100f;
            raceManager.characterSheets[i].GetComponent<CharacterSheet>().fuel += 10f;

            if (raceManager.characterSheets[i].GetComponent<CharacterSheet>().fuel > raceManager.characterSheets[i].GetComponent<CharacterSheet>().maxFuel)
            {
                raceManager.characterSheets[i].GetComponent<CharacterSheet>().fuel = raceManager.characterSheets[i].GetComponent<CharacterSheet>().maxFuel;
            }
        }

        if (raceManager.characterSheets[i].GetComponent<CharacterSheet>().fuel < (raceManager.characterSheets[i].GetComponent<CharacterSheet>().maxFuel * enough))
        {
            if (raceManager.characterSheets[i].GetComponent<CharacterSheet>().money >= 100f)
            {
                AgentRefuelUntilEnough(i, enough);
            }
        }
    }


    public void AgentResupplyAmmoUntilEnough(int i, float enough)
    {
        //reammo
        if (raceManager.characterSheets[i].GetComponent<CharacterSheet>().ammo < (raceManager.characterSheets[i].GetComponent<CharacterSheet>().maxAmmo * enough))
        {
            raceManager.characterSheets[i].GetComponent<CharacterSheet>().money -= 100f;
            raceManager.characterSheets[i].GetComponent<CharacterSheet>().ammo += 10f;

            if (raceManager.characterSheets[i].GetComponent<CharacterSheet>().ammo > raceManager.characterSheets[i].GetComponent<CharacterSheet>().maxAmmo)
            {
                raceManager.characterSheets[i].GetComponent<CharacterSheet>().ammo = raceManager.characterSheets[i].GetComponent<CharacterSheet>().maxAmmo;
            }
        }

        if (raceManager.characterSheets[i].GetComponent<CharacterSheet>().ammo < (raceManager.characterSheets[i].GetComponent<CharacterSheet>().maxAmmo * enough))
        {
            if (raceManager.characterSheets[i].GetComponent<CharacterSheet>().money >= 100f)
            {
                AgentResupplyAmmoUntilEnough(i, enough);
            }
        }
    }

    public void AgentShopDecisions()
    {
        for (int i = 0; i < raceManager.characterSheets.Count - 1; i++)
        {
            if (raceManager.characterSheets[i].GetComponent<CharacterSheet>().money >= 100f)
            {
                AgentRepairUntilFull(i);
            }
        }

        //ammo and fuel doesnt really need to be filled up every time, but how to decide what is enough? Maybe could use agent's stat priority thing
        //for now, lets make a random roll decide 

        for (int i = 0; i < raceManager.characterSheets.Count - 1; i++)
        {
            if (raceManager.characterSheets[i].GetComponent<CharacterSheet>().money >= 100f)
            {
                float enough = Random.Range(0f, 1f);

                AgentResupplyAmmoUntilEnough(i, enough);
            }
        }

        for (int i = 0; i < raceManager.characterSheets.Count - 1; i++)
        {
            if (raceManager.characterSheets[i].GetComponent<CharacterSheet>().money >= 100f)
            {
                float enough = Random.Range(0f, 1f);

                AgentRefuelUntilEnough(i, enough);
            }
        }

    }

    //this function is insane, needs condensing, don't even know if it works
    public void UpdateAgentRacersStats()
    {
        AgentShopDecisions();

        //for (int i = 0; i < raceManager.characterSheets.Count - 1; i++)
        //{
        //    //print("Updating agent stats " + i);

        //    //print(i + " " + raceManager.Pilots[(int)raceManager.characterSheets[i].GetComponent<CharacterSheet>().pilot].GetComponent<PilotStats>().pilotEngine);

        //    //if an agent has enough money to buy anything
        //    if (raceManager.characterSheets[i].GetComponent<CharacterSheet>().money >= 100f)
        //    {
        //        //repair 
        //        if (raceManager.characterSheets[i].GetComponent<CharacterSheet>().health < raceManager.characterSheets[i].GetComponent<CharacterSheet>().maxHealth)
        //        {
        //            while(raceManager.characterSheets[i].GetComponent<CharacterSheet>().money >= 100f && raceManager.characterSheets[i].GetComponent<CharacterSheet>().health < raceManager.characterSheets[i].GetComponent<CharacterSheet>().maxHealth)
        //            {
        //                raceManager.characterSheets[i].GetComponent<CharacterSheet>().money -= 100f;
        //                raceManager.characterSheets[i].GetComponent<CharacterSheet>().health += 10f;
        //            }
        //        }

        //        //choose randomly if agent should add more ammo or fuel
        //        //should also be able to choose not to add more, or to focus on one until it's filled first

        //        //if both ammo and fuel are less than half max
        //        if(raceManager.characterSheets[i].GetComponent<CharacterSheet>().ammo < (raceManager.characterSheets[i].GetComponent<CharacterSheet>().maxAmmo / 2) && raceManager.characterSheets[i].GetComponent<CharacterSheet>().fuel < (raceManager.characterSheets[i].GetComponent<CharacterSheet>().maxFuel / 2))
        //        {
        //            while (raceManager.characterSheets[i].GetComponent<CharacterSheet>().money >= 100f
        //                &&
        //                raceManager.characterSheets[i].GetComponent<CharacterSheet>().ammo < (raceManager.characterSheets[i].GetComponent<CharacterSheet>().maxAmmo / 2)
        //                && 
        //                raceManager.characterSheets[i].GetComponent<CharacterSheet>().fuel < (raceManager.characterSheets[i].GetComponent<CharacterSheet>().maxFuel / 2))
        //                {
        //                    int choice = Random.Range(0, 2);

        //                    if(choice == 0)
        //                    {
        //                        raceManager.characterSheets[i].GetComponent<CharacterSheet>().money -= 100f;
        //                        raceManager.characterSheets[i].GetComponent<CharacterSheet>().ammo += 10f;
        //                    }
        //                    else
        //                    {
        //                        raceManager.characterSheets[i].GetComponent<CharacterSheet>().money -= 100f;
        //                        raceManager.characterSheets[i].GetComponent<CharacterSheet>().fuel += 10f;
        //                    }
        //                }
        //        }
        //        else
        //        {

        //            //resupply
        //            if (raceManager.characterSheets[i].GetComponent<CharacterSheet>().ammo < (raceManager.characterSheets[i].GetComponent<CharacterSheet>().maxAmmo / 2))
        //            {
        //                while (raceManager.characterSheets[i].GetComponent<CharacterSheet>().money >= 100f && raceManager.characterSheets[i].GetComponent<CharacterSheet>().ammo < (raceManager.characterSheets[i].GetComponent<CharacterSheet>().maxAmmo / 2))
        //                {
        //                    raceManager.characterSheets[i].GetComponent<CharacterSheet>().money -= 100f;
        //                    raceManager.characterSheets[i].GetComponent<CharacterSheet>().ammo += 10f;
        //                }
        //            }

        //            //refuel
        //            if (raceManager.characterSheets[i].GetComponent<CharacterSheet>().fuel < (raceManager.characterSheets[i].GetComponent<CharacterSheet>().maxFuel / 2))
        //            {
        //                while (raceManager.characterSheets[i].GetComponent<CharacterSheet>().money >= 100f && raceManager.characterSheets[i].GetComponent<CharacterSheet>().fuel < (raceManager.characterSheets[i].GetComponent<CharacterSheet>().maxFuel / 2))
        //                {
        //                    raceManager.characterSheets[i].GetComponent<CharacterSheet>().money -= 100f;
        //                    raceManager.characterSheets[i].GetComponent<CharacterSheet>().fuel += 10f;
        //                }
        //            }
        //        }

        //        //print("try to upgrade agent");

        //        //int firstPriority = 0;
        //        //int secondPriority = 0;
        //        //int thirdPriority = 0;

        //        //float[] pilotPriority = { raceManager.Pilots[(int)raceManager.characterSheets[i].GetComponent<CharacterSheet>().pilot].GetComponent<PilotStats>().acceleration,
        //        //                              raceManager.Pilots[(int)raceManager.characterSheets[i].GetComponent<CharacterSheet>().pilot].GetComponent<PilotStats>().maxSpeed,
        //        //                              raceManager.Pilots[(int)raceManager.characterSheets[i].GetComponent<CharacterSheet>().pilot].GetComponent<PilotStats>().grip,
        //        //                              raceManager.Pilots[(int)raceManager.characterSheets[i].GetComponent<CharacterSheet>().pilot].GetComponent<PilotStats>().maxStrafe,
        //        //                              raceManager.Pilots[(int)raceManager.characterSheets[i].GetComponent<CharacterSheet>().pilot].GetComponent<PilotStats>().brake,
        //        //                              raceManager.Pilots[(int)raceManager.characterSheets[i].GetComponent<CharacterSheet>().pilot].GetComponent<PilotStats>().weaponDamage,
        //        //                              raceManager.Pilots[(int)raceManager.characterSheets[i].GetComponent<CharacterSheet>().pilot].GetComponent<PilotStats>().boostValue
        //        //    };


        //        //for (int j = 0; i < pilotPriority.Length; i++)
        //        //{
        //        //    if (pilotPriority[j] == 1)
        //        //    {
        //        //        thirdPriority = j;
        //        //    }

        //        //    if (pilotPriority[j] == 2)
        //        //    {
        //        //        secondPriority = j;
        //        //    }
        //        //    if (pilotPriority[j] == 3)
        //        //    {
        //        //        firstPriority = j;
        //        //    }
        //        //}

        //        //while(raceManager.characterSheets[i].GetComponent<CharacterSheet>().money >= 1000f)
        //        //{
        //        //    int choice = 0;

        //        //    int takePriority = Random.Range(0, 4);

        //        //    if(takePriority == 0)
        //        //    {
        //        //        choice = Random.Range(0, 4);
        //        //    }
        //        //    else
        //        //    {
        //        //        int whichPriority = Random.Range(0, 10);

        //        //        if (whichPriority >= 5)
        //        //        {
        //        //            choice = firstPriority;
        //        //        }
        //        //        else if(whichPriority >= 2 && whichPriority < 5)
        //        //        {
        //        //            choice = secondPriority;
        //        //        }
        //        //        else
        //        //        {
        //        //            choice = thirdPriority;
        //        //        }
        //        //    }

        //        //    if (choice == 0)
        //        //    {
        //        //        raceManager.characterSheets[i].GetComponent<CharacterSheet>().acceleration += 1f;
        //        //        raceManager.characterSheets[i].GetComponent<CharacterSheet>().money -= 1000f;
        //        //    }
        //        //    else if (choice == 1)
        //        //    {
        //        //        raceManager.characterSheets[i].GetComponent<CharacterSheet>().maxSpeed += 1f;
        //        //        raceManager.characterSheets[i].GetComponent<CharacterSheet>().money -= 1000f;
        //        //    }
        //        //    else if (choice == 2)
        //        //    {
        //        //        raceManager.characterSheets[i].GetComponent<CharacterSheet>().grip += 1f;
        //        //        raceManager.characterSheets[i].GetComponent<CharacterSheet>().money -= 1000f;
        //        //    }
        //        //    else if(choice == 3)
        //        //    {
        //        //        raceManager.characterSheets[i].GetComponent<CharacterSheet>().maxStrafe += 1f;
        //        //        raceManager.characterSheets[i].GetComponent<CharacterSheet>().money -= 1000f;
        //        //    }
        //        //    else if(choice == 4)
        //        //    {
        //        //        raceManager.characterSheets[i].GetComponent<CharacterSheet>().brake += 1f;
        //        //        raceManager.characterSheets[i].GetComponent<CharacterSheet>().money -= 1000f;
        //        //    }
        //        //    else if(choice == 5)
        //        //    {
        //        //        raceManager.characterSheets[i].GetComponent<CharacterSheet>().weaponDamage += 1f;
        //        //        raceManager.characterSheets[i].GetComponent<CharacterSheet>().money -= 1000f;
        //        //    }
        //        //    else if(choice == 6)
        //        //    {
        //        //        raceManager.characterSheets[i].GetComponent<CharacterSheet>().boostValue += 1f;
        //        //        raceManager.characterSheets[i].GetComponent<CharacterSheet>().money -= 1000f;
        //        //    }

        //        //////if agent can upgrade ship
        //        ////if (raceManager.characterSheets[i].GetComponent<CharacterSheet>().money >= 1000f)
        //        ////{





        //        ////    //randomly choose between the 3 current options
        //        ////    int choice = Random.Range(0, 4);

        //        ////    if(choice == 0)
        //        ////    {
        //        ////        raceManager.characterSheets[i].GetComponent<CharacterSheet>().acceleration += 1f;
        //        ////        raceManager.characterSheets[i].GetComponent<CharacterSheet>().money -= 1000f;
        //        ////    }
        //        ////    else if(choice == 1)
        //        ////    {
        //        ////        raceManager.characterSheets[i].GetComponent<CharacterSheet>().maxSpeed += 1f;
        //        ////        raceManager.characterSheets[i].GetComponent<CharacterSheet>().money -= 1000f;
        //        ////    }
        //        ////    else if(choice == 2)
        //        ////    {
        //        ////        raceManager.characterSheets[i].GetComponent<CharacterSheet>().grip += 1f;
        //        ////        raceManager.characterSheets[i].GetComponent<CharacterSheet>().money -= 1000f;
        //        ////    }
        //        //}
        //    }
        //}

        ApplyUpgrades();

        //DataPersistenceManager.instance.SaveGame();
        persistenceManager.SaveGame();
        //DataPersistenceManager.instance.LoadGame();
    }

    public void UpgradeAgentShip()
    {
        for (int i = 0; i < raceManager.characterSheets.Count - 1; i++)
        {
            //print("bloop " + i);

            UpgradeShip(i);

        }


        //for (int i = 0; i < raceManager.characterSheets.Count; i++)
        //{
        //    print("upgrading agent ship "+ i);

        //    int firstPriority = 0;
        //    int secondPriority = 0;
        //    int thirdPriority = 0;

        //    float[] pilotPriority = { raceManager.Pilots[(int)raceManager.characterSheets[i].GetComponent<CharacterSheet>().pilot].GetComponent<PilotStats>().acceleration,
        //                                      raceManager.Pilots[(int)raceManager.characterSheets[i].GetComponent<CharacterSheet>().pilot].GetComponent<PilotStats>().maxSpeed,
        //                                      raceManager.Pilots[(int)raceManager.characterSheets[i].GetComponent<CharacterSheet>().pilot].GetComponent<PilotStats>().grip,
        //                                      raceManager.Pilots[(int)raceManager.characterSheets[i].GetComponent<CharacterSheet>().pilot].GetComponent<PilotStats>().maxStrafe,
        //                                      raceManager.Pilots[(int)raceManager.characterSheets[i].GetComponent<CharacterSheet>().pilot].GetComponent<PilotStats>().brake,
        //                                      raceManager.Pilots[(int)raceManager.characterSheets[i].GetComponent<CharacterSheet>().pilot].GetComponent<PilotStats>().weaponDamage,
        //                                      raceManager.Pilots[(int)raceManager.characterSheets[i].GetComponent<CharacterSheet>().pilot].GetComponent<PilotStats>().boostValue
        //            };


        //    for (int j = 0; j < pilotPriority.Length; j++)
        //    {
        //        if (pilotPriority[j] == 1)
        //        {
        //            thirdPriority = j;
        //        }

        //        if (pilotPriority[j] == 2)
        //        {
        //            secondPriority = j;
        //        }
        //        if (pilotPriority[j] == 3)
        //        {
        //            firstPriority = j;
        //        }
        //    }

        //    while (raceManager.characterSheets[i].GetComponent<CharacterSheet>().money >= 1000f)
        //    {
        //        int choice = 0;

        //        int takePriority = Random.Range(0, 4);

        //        if (takePriority == 0)
        //        {
        //            choice = Random.Range(0, 4);
        //        }
        //        else
        //        {
        //            int whichPriority = Random.Range(0, 10);

        //            if (whichPriority >= 5)
        //            {
        //                choice = firstPriority;
        //            }
        //            else if (whichPriority >= 2 && whichPriority < 5)
        //            {
        //                choice = secondPriority;
        //            }
        //            else
        //            {
        //                choice = thirdPriority;
        //            }
        //        }

        //        if (choice == 0)
        //        {
        //            raceManager.characterSheets[i].GetComponent<CharacterSheet>().acceleration += 1f;
        //            raceManager.characterSheets[i].GetComponent<CharacterSheet>().money -= 1000f;
        //        }
        //        else if (choice == 1)
        //        {
        //            raceManager.characterSheets[i].GetComponent<CharacterSheet>().maxSpeed += 1f;
        //            raceManager.characterSheets[i].GetComponent<CharacterSheet>().money -= 1000f;
        //        }
        //        else if (choice == 2)
        //        {
        //            raceManager.characterSheets[i].GetComponent<CharacterSheet>().grip += 1f;
        //            raceManager.characterSheets[i].GetComponent<CharacterSheet>().money -= 1000f;
        //        }
        //        else if (choice == 3)
        //        {
        //            raceManager.characterSheets[i].GetComponent<CharacterSheet>().maxStrafe += 1f;
        //            raceManager.characterSheets[i].GetComponent<CharacterSheet>().money -= 1000f;
        //        }
        //        else if (choice == 4)
        //        {
        //            raceManager.characterSheets[i].GetComponent<CharacterSheet>().brake += 1f;
        //            raceManager.characterSheets[i].GetComponent<CharacterSheet>().money -= 1000f;
        //        }
        //        else if (choice == 5)
        //        {
        //            raceManager.characterSheets[i].GetComponent<CharacterSheet>().weaponDamage += 1f;
        //            raceManager.characterSheets[i].GetComponent<CharacterSheet>().money -= 1000f;
        //        }
        //        else if (choice == 6)
        //        {
        //            raceManager.characterSheets[i].GetComponent<CharacterSheet>().boostValue += 1f;
        //            raceManager.characterSheets[i].GetComponent<CharacterSheet>().money -= 1000f;
        //        }
        //    }
        //}

        persistenceManager.SaveGame();
        
    }

    //this function handles opponents upgrading their ships according to the pilot's preferences
    //each pilot has 3 stats they like, and will try to focus on
    //a random roll chooses whether to do so, with high odds of upgrading preferred stat, and low odds of a random stat
    public void UpgradeShip(int index)
    {
        //first, we need to get the stat index of the pilots preferred stat
        //preferred stats have values 1, 2, 3 for how high their preference is
        int firstPriority = 0;
        int secondPriority = 0;
        int thirdPriority = 0;

        //we create a temporary array of values using the pilot's stats
        float[] pilotPriority = { raceManager.Pilots[(int)raceManager.characterSheets[index].GetComponent<CharacterSheet>().pilot].GetComponent<PilotStats>().acceleration,
                                              raceManager.Pilots[(int)raceManager.characterSheets[index].GetComponent<CharacterSheet>().pilot].GetComponent<PilotStats>().maxSpeed,
                                              raceManager.Pilots[(int)raceManager.characterSheets[index].GetComponent<CharacterSheet>().pilot].GetComponent<PilotStats>().grip,
                                              raceManager.Pilots[(int)raceManager.characterSheets[index].GetComponent<CharacterSheet>().pilot].GetComponent<PilotStats>().maxStrafe,
                                              raceManager.Pilots[(int)raceManager.characterSheets[index].GetComponent<CharacterSheet>().pilot].GetComponent<PilotStats>().brake,
                                              raceManager.Pilots[(int)raceManager.characterSheets[index].GetComponent<CharacterSheet>().pilot].GetComponent<PilotStats>().weaponDamage,
                                              raceManager.Pilots[(int)raceManager.characterSheets[index].GetComponent<CharacterSheet>().pilot].GetComponent<PilotStats>().boostValue
                    };

        //then we loop through the array to find the stats which have values 1, 2, 3
        for (int j = 0; j < pilotPriority.Length; j++)
        {
            if (pilotPriority[j] == 1)
            {
                thirdPriority = j;
            }

            if (pilotPriority[j] == 2)
            {
                secondPriority = j;
            }
            if (pilotPriority[j] == 3)
            {
                firstPriority = j;
            }
        }

        //next, check if the pilot can afford any upgrades (should move this to the start of this function, no need to do the above loop if no money)
        //now I need to include a check here that determines what is the lowest possible amount to spend
        //since upgrades increase costs, a simple check for 1000 cash is not good enough
        //do I create an array of upgrade costs, and if money is higher than any of them, allow pass?
        //additionally, check if can afford a preferred stat, and if not, have a chance of choosing to save up

        float[] upgradeCosts = { 1000f + (1000f * (raceManager.characterSheets[index].GetComponent<CharacterSheet>().acceleration - raceManager.characterSheet.GetComponent<CharacterSheet>().acceleration)),
                                 1000f + (1000f * (raceManager.characterSheets[index].GetComponent<CharacterSheet>().maxSpeed - raceManager.characterSheet.GetComponent<CharacterSheet>().maxSpeed)),
                                 1000f + (1000f * (raceManager.characterSheets[index].GetComponent<CharacterSheet>().grip - raceManager.characterSheet.GetComponent<CharacterSheet>().grip)),
                                 1000f + (1000f * (raceManager.characterSheets[index].GetComponent<CharacterSheet>().maxStrafe - raceManager.characterSheet.GetComponent<CharacterSheet>().maxStrafe)),
                                 1000f + (1000f * (raceManager.characterSheets[index].GetComponent<CharacterSheet>().brake - raceManager.characterSheet.GetComponent<CharacterSheet>().brake)),
                                 1000f + (1000f * (raceManager.characterSheets[index].GetComponent<CharacterSheet>().weaponDamage - raceManager.characterSheet.GetComponent<CharacterSheet>().weaponDamage)),
                                 1000f + (1000f * (raceManager.characterSheets[index].GetComponent<CharacterSheet>().boostValue - raceManager.characterSheet.GetComponent<CharacterSheet>().boostValue))

                                    };

        bool canAffordAny = false;
        //bool canAffordPreferred = false;

        for (int i = 0; i < upgradeCosts.Length; i++)
        {
            //print("testing upgrade costs..." + upgradeCosts[i]);

            if (raceManager.characterSheets[index].GetComponent<CharacterSheet>().money >= upgradeCosts[i])
            {
                canAffordAny = true;

                //if(i == firstPriority || i == secondPriority || i == thirdPriority)
                //{
                //    canAffordPreferred = true;
                //}
            }
        }


        if (canAffordAny)
        {
            int choice = 0;

            int takePriority = Random.Range(0, 4);

            if (takePriority == 0)
            {
                choice = Random.Range(0, 4);
            }
            else
            {
                int whichPriority = Random.Range(0, 10);

                if (whichPriority >= 5)
                {
                    choice = firstPriority;
                }
                else if (whichPriority >= 2 && whichPriority < 5)
                {
                    choice = secondPriority;
                }
                else
                {
                    choice = thirdPriority;
                }
            }

            //count how many stats have been checked for upgrading
            //if count is equal to the number of stats(7 now, could make it dynamic upgradeCosts.Length), all have been checked and we should move on
            //what this is missing is the ability to choose to save cash... do a random roll at every choice, and if something, do break
            int count = 0;

            while(count < 7)
            {
                print("while looping..." + count);

                if (choice == 0 && count < 7)
                {
                    count++;

                    if (upgradeCosts[0] <= raceManager.characterSheets[index].GetComponent<CharacterSheet>().money)
                    {
                        raceManager.characterSheets[index].GetComponent<CharacterSheet>().acceleration += 1f;
                        raceManager.characterSheets[index].GetComponent<CharacterSheet>().money -= upgradeCosts[0];
                    }

                    choice = 1;
                }

                if (Random.Range(count, 7) == 7)
                {
                    break;
                }

                if (choice == 1 && count < 7)
                {
                    count++;

                    if (upgradeCosts[1] <= raceManager.characterSheets[index].GetComponent<CharacterSheet>().money)
                    {
                        raceManager.characterSheets[index].GetComponent<CharacterSheet>().maxSpeed += 1f;
                        raceManager.characterSheets[index].GetComponent<CharacterSheet>().money -= upgradeCosts[1];
                    }

                    choice = 2;

                }

                if (Random.Range(count, 7) == 7)
                {
                    break;
                }

                if (choice == 2 && count < 7)
                {
                    count++;

                    if (upgradeCosts[2] <= raceManager.characterSheets[index].GetComponent<CharacterSheet>().money)
                    {
                        raceManager.characterSheets[index].GetComponent<CharacterSheet>().grip += 1f;
                        raceManager.characterSheets[index].GetComponent<CharacterSheet>().money -= upgradeCosts[2];
                    }

                    choice = 3;

                }

                if (Random.Range(count, 7) == 7)
                {
                    break;
                }

                if (choice == 3 && count < 7)
                {
                    count++;

                    if (upgradeCosts[3] <= raceManager.characterSheets[index].GetComponent<CharacterSheet>().money)
                    {
                        raceManager.characterSheets[index].GetComponent<CharacterSheet>().maxStrafe += 1f;
                        raceManager.characterSheets[index].GetComponent<CharacterSheet>().money -= upgradeCosts[3];
                    }

                    choice = 4;

                }

                if (Random.Range(count, 7) == 7)
                {
                    break;
                }

                if (choice == 4 && count < 7)
                {
                    count++;

                    if (upgradeCosts[4] <= raceManager.characterSheets[index].GetComponent<CharacterSheet>().money)
                    {
                        raceManager.characterSheets[index].GetComponent<CharacterSheet>().brake += 1f;
                        raceManager.characterSheets[index].GetComponent<CharacterSheet>().money -= upgradeCosts[4];
                    }

                    choice = 5;

                }

                if (Random.Range(count, 7) == 7)
                {
                    break;
                }

                if (choice == 5 && count < 7)
                {
                    count++;

                    if (upgradeCosts[5] <= raceManager.characterSheets[index].GetComponent<CharacterSheet>().money)
                    {
                        raceManager.characterSheets[index].GetComponent<CharacterSheet>().weaponDamage += 1f;
                        raceManager.characterSheets[index].GetComponent<CharacterSheet>().money -= upgradeCosts[5];
                    }

                    choice = 6;

                }

                if (Random.Range(count, 7) == 7)
                {
                    break;
                }

                if (choice == 6 && count < 7)
                {
                    count++;

                    if (upgradeCosts[6] <= raceManager.characterSheets[index].GetComponent<CharacterSheet>().money)
                    {
                        raceManager.characterSheets[index].GetComponent<CharacterSheet>().boostValue += 1f;
                        raceManager.characterSheets[index].GetComponent<CharacterSheet>().money -= upgradeCosts[6];
                    }

                    choice = 0;
                }

                if (Random.Range(count, 7) == 7)
                {
                    break;
                }
            }

        }

        //if (raceManager.characterSheets[index].GetComponent<CharacterSheet>().money >= 1000f)
        //{
        //    UpgradeShip(index);
        //}
    }

    public void UpgradeHitPointsButtonClick()
    {
        float price = 1000f + (1000f * (maxDamage - raceManager.characterSheet.GetComponent<CharacterSheet>().maxHealth));

        if (cash >= price)
        {
            maxDamage += 1f;
            cash -= price;
        }

        UpdateValues();

    }

    public void UpgradeAmmoButtonClick()
    {
        float price = 1000f + (1000f * (maxAmmo - raceManager.characterSheet.GetComponent<CharacterSheet>().maxAmmo));

        if (cash >= price)
        {
            maxAmmo += 1f;
            cash -= price;
        }

        UpdateValues();

    }

    public void UpgradeFuelButtonClick()
    {
        float price = 1000f + (1000f * (maxFuel - raceManager.characterSheet.GetComponent<CharacterSheet>().maxFuel));

        if (cash >= price)
        {
            maxFuel += 1f;
            cash -= price;
        }

        UpdateValues();

    }

    public void UpgradeAccelerationButtonClick()
    {
        float price = 1000f + (1000f * (acceleration - raceManager.characterSheet.GetComponent<CharacterSheet>().acceleration));

        if(cash >= price)
        {
            acceleration += 1f;
            cash -= price;
        }

        UpdateValues();

    }

    public void UpgradeTopSpeedButtonClick()
    {
        float price = 1000f + (1000f * (maxSpeed - raceManager.characterSheet.GetComponent<CharacterSheet>().maxSpeed));


        if (cash >= price)
        {
            maxSpeed += 1f;
            cash -= price;
        }

        UpdateValues();

    }

    public void UpgradeGripButtonClick()
    {
        float price = 1000f + (1000f * (grip - raceManager.characterSheet.GetComponent<CharacterSheet>().grip));

        if (cash >= price)
        {
            grip += 1f;
            cash -= price;
        }

        UpdateValues();

    }

    public void UpgradeStrafeButtonClick()
    {
        float price = 1000f + (1000f * (maxStrafe - raceManager.characterSheet.GetComponent<CharacterSheet>().maxStrafe));

        if (cash >= price)
        {
            maxStrafe += 1f;
            cash -= price;
        }

        UpdateValues();

    }

    public void UpgradeBrakeButtonClick()
    {
        float price = 1000f + (1000f * (brake - raceManager.characterSheet.GetComponent<CharacterSheet>().brake));

        if (cash >= price)
        {
            brake += 1f;
            cash -= price;
        }

        UpdateValues();

    }

    public void UpgradeFuelEfficiencyButtonClick()
    {
        float price = 1000f + (1000f * (raceManager.characterSheet.GetComponent<CharacterSheet>().fuelConsumption - fuelConsumption));

        if (cash >= price)
        {
            fuelConsumption -= 1f;
            cash -= price;
        }

        UpdateValues();

    }

    public void UpgradeBoostButtonClick()
    {
        float price = 1000f + (1000f * (boostValue - raceManager.characterSheet.GetComponent<CharacterSheet>().boostValue));

        if (cash >= price)
        {
            boostValue += 1f;
            cash -= price;
        }

        UpdateValues();

    }

    public void UpgradeWeaponDamageButtonClick()
    {
        float price = 1000f + (1000f * (weaponDamage - raceManager.characterSheet.GetComponent<CharacterSheet>().weaponDamage));

        if (cash >= price)
        {
            weaponDamage += 1f;
            cash -= price;
        }

        UpdateValues();

    }

    public void UpgradeWeaponSpeedButtonClick()
    {
        float price = 1000f + (100000f * (raceManager.characterSheet.GetComponent<CharacterSheet>().weaponSpeed - weaponSpeed));

        if (cash >= price)
        {
            weaponSpeed -= 0.01f;
            cash -= price;
        }

        UpdateValues();

    }

    public void UpgradeWeaponSlowButtonClick()
    {
        float price = 1000f + (1000f * (weaponSlow - raceManager.characterSheet.GetComponent<CharacterSheet>().weaponSlow));

        if (cash >= price)
        {
            weaponSlow += 1f;
            cash -= price;
        }

        UpdateValues();

    }

    public void RepairButtonClick()
    {
        if(damage < maxDamage)
        {
            if(cash >= 100f)
            {
                damage = damage + 10f;
                cash = cash - 100f;

                if(damage > maxDamage)
                {
                    damage = maxDamage;
                }
            }
        }

        UpdateValues();

    }

    public void BuyAmmoButtonClick()
    {
        if(ammo < maxAmmo)
        {
            if(cash >= 100f)
            {
                ammo = ammo + 10f;
                cash = cash - 100f;

                if(ammo > maxAmmo)
                {
                    ammo = maxAmmo;
                }
            }
        }

        UpdateValues();

    }

    public void BuyFuelButtonClick()
    {
        if(fuel < maxFuel)
        {
            if (cash >= 100f)
            {
                fuel = fuel + 10f;
                cash = cash - 100f;

                if(fuel > maxFuel)
                {
                    fuel = maxFuel;
                }
            }
        }

        UpdateValues();

    }

    public void SwitchEnginesButtonClick()
    {
        if(engine < 2)
        {
            engine += 1;
        }
        else
        {
            engine = 0;
        }

        UpdateValues();

    }

    public void SwitchWeaponssButtonClick()
    {
        if (weapons < 2)
        {
            weapons += 1;
        }
        else
        {
            weapons = 0;
        }

        UpdateValues();

    }

    public void SwitchShipsButtonClick()
    {
        if (ship < 2)
        {
            ship += 1;
        }
        else
        {
            ship = 0;
        }

        UpdateValues();

    }

    private void OnEnable()
    {
        //raceManager.ReinitializeSheetList();


        Invoke("FetchData", 1f);



        //DataPersistenceManager.instance.LoadGame();

        persistenceManager.LoadGame();

    }

    public void FetchData()
    {
        print("getting data");
        GatherData();

        UpdateAgentRacersStats();

        Invoke("UpgradeAgentShip", 1f);
        //UpgradeAgentShip();
    }

    public void LoadData(GameData data)
    {

    }

    public void GatherData()
    {
        pilot = raceManager.characterSheets[9].GetComponent<CharacterSheet>().pilot;
        ship = raceManager.characterSheets[9].GetComponent<CharacterSheet>().pilotShip;
        engine = raceManager.characterSheets[9].GetComponent<CharacterSheet>().pilotEngine;
        weapons = raceManager.characterSheets[9].GetComponent<CharacterSheet>().pilotWeapons;

        wins = raceManager.characterSheets[9].GetComponent<CharacterSheet>().wins;
        cash = raceManager.characterSheets[9].GetComponent<CharacterSheet>().money;

        damage = raceManager.characterSheets[9].GetComponent<CharacterSheet>().health;
        maxDamage = raceManager.characterSheets[9].GetComponent<CharacterSheet>().maxHealth;
        fuel = raceManager.characterSheets[9].GetComponent<CharacterSheet>().fuel;
        maxFuel = raceManager.characterSheets[9].GetComponent<CharacterSheet>().maxFuel;
        ammo = raceManager.characterSheets[9].GetComponent<CharacterSheet>().ammo;
        maxAmmo = raceManager.characterSheets[9].GetComponent<CharacterSheet>().maxAmmo;

        acceleration = raceManager.characterSheets[9].GetComponent<CharacterSheet>().acceleration;
        maxSpeed = raceManager.characterSheets[9].GetComponent<CharacterSheet>().maxSpeed;
        grip = raceManager.characterSheets[9].GetComponent<CharacterSheet>().grip;
        maxStrafe = raceManager.characterSheets[9].GetComponent<CharacterSheet>().maxStrafe;
        brake = raceManager.characterSheets[9].GetComponent<CharacterSheet>().brake;

        fuelConsumption = raceManager.characterSheets[9].GetComponent<CharacterSheet>().fuelConsumption;
        boostValue = raceManager.characterSheets[9].GetComponent<CharacterSheet>().boostValue;

        weaponDamage = raceManager.characterSheets[9].GetComponent<CharacterSheet>().weaponDamage;
        weaponSlow = raceManager.characterSheets[9].GetComponent<CharacterSheet>().weaponSlow;
        weaponSpeed = raceManager.characterSheets[9].GetComponent<CharacterSheet>().weaponSpeed;

        UpdateValues();
    }

    //public void LoadData(GameData data)
    //{


    //    pilot = raceManager.characterSheets[9].GetComponent<CharacterSheet>().pilot;
    //    ship = raceManager.characterSheets[9].GetComponent<CharacterSheet>().pilotShip;
    //    engine = raceManager.characterSheets[9].GetComponent<CharacterSheet>().pilotEngine;
    //    weapons = raceManager.characterSheets[9].GetComponent<CharacterSheet>().pilotWeapons;

    //    wins = raceManager.characterSheets[9].GetComponent<CharacterSheet>().wins;
    //    cash = raceManager.characterSheets[9].GetComponent<CharacterSheet>().money;

    //    damage = raceManager.characterSheets[9].GetComponent<CharacterSheet>().health;
    //    maxDamage = raceManager.characterSheets[9].GetComponent<CharacterSheet>().maxHealth;
    //    fuel = raceManager.characterSheets[9].GetComponent<CharacterSheet>().fuel;
    //    maxFuel = raceManager.characterSheets[9].GetComponent<CharacterSheet>().maxFuel;
    //    ammo = raceManager.characterSheets[9].GetComponent<CharacterSheet>().ammo;
    //    maxAmmo = raceManager.characterSheets[9].GetComponent<CharacterSheet>().maxAmmo;

    //    acceleration = raceManager.characterSheets[9].GetComponent<CharacterSheet>().acceleration;
    //    maxSpeed = raceManager.characterSheets[9].GetComponent<CharacterSheet>().maxSpeed;
    //    grip = raceManager.characterSheets[9].GetComponent<CharacterSheet>().grip;
    //    maxStrafe = raceManager.characterSheets[9].GetComponent<CharacterSheet>().maxStrafe;
    //    brake = raceManager.characterSheets[9].GetComponent<CharacterSheet>().brake;

    //    fuelConsumption = raceManager.characterSheets[9].GetComponent<CharacterSheet>().fuelConsumption;
    //    boostValue = raceManager.characterSheets[9].GetComponent<CharacterSheet>().boostValue;

    //    weaponDamage = raceManager.characterSheets[9].GetComponent<CharacterSheet>().weaponDamage;
    //    weaponSlow = raceManager.characterSheets[9].GetComponent<CharacterSheet>().weaponSlow;
    //    weaponSpeed = raceManager.characterSheets[9].GetComponent<CharacterSheet>().weaponSpeed;

    //    UpdateValues();
    //}

    public void UpdateValues()
    {
        pilotText.text = "Pilot " + raceManager.Pilots[(int)pilot].GetComponent<PilotStats>().pilotName;
        cashText.text = "Cash " + cash;
        winsText.text = "Wins " + wins;



        if (cash < 100f)
        {
            repairButton.interactable = false;
            fuelButton.interactable = false;
            ammoButton.interactable = false;
        }
        else
        {
            repairButton.interactable = true;
            fuelButton.interactable = true;
            ammoButton.interactable = true;
        }

        repairText.text = "Damage " + damage + " / " + maxDamage;
        repairCostText.text = "$100";
        fuelText.text = "Fuel " + fuel + " / " + maxFuel;
        fuelCostText.text = "$100";
        ammoText.text = "Ammo " + ammo + " / " + maxAmmo;
        ammoCostText.text = "$100 ";

        float repairUpCost = 1000f + (1000f * (maxDamage - raceManager.characterSheet.GetComponent<CharacterSheet>().maxHealth));
        repairUpgradeCostText.text = "$" + repairUpCost;

        if(cash < repairUpCost)
        {
            repairUpgradeButton.interactable = false;
        }
        else
        {
            repairUpgradeButton.interactable = true;
        }

        float ammoUpCost = 1000f + (1000f * (maxAmmo - raceManager.characterSheet.GetComponent<CharacterSheet>().maxAmmo));
        ammoUpgradeCostText.text = "$" + ammoUpCost;

        if (cash < ammoUpCost)
        {
            ammoUpgradeButton.interactable = false;
        }
        else
        {
            ammoUpgradeButton.interactable = true;
        }

        float fuelUpCost = 1000f + (1000f * (maxFuel - raceManager.characterSheet.GetComponent<CharacterSheet>().maxFuel));
        fuelUpgradeCostText.text = "$" + fuelUpCost;

        if (cash < fuelUpCost)
        {
            fuelUpgradeButton.interactable = false;
        }
        else
        {
            fuelUpgradeButton.interactable = true;
        }

        //accelerationText.text = "Acceleration " + (acceleration - raceManager.characterSheet.GetComponent<CharacterSheet>().acceleration);
        float accCost = 1000f + (1000f * (acceleration - raceManager.characterSheet.GetComponent<CharacterSheet>().acceleration));
        accelerationCostText.text = "$" + accCost;

        if(cash < accCost)
        {
            accelerationButton.interactable = false;
        }
        else
        {
            accelerationButton.interactable = true;
        }

        accelerationUpgradeLevels.fillAmount = (acceleration - raceManager.characterSheet.GetComponent<CharacterSheet>().acceleration) / 10f;

        if (acceleration - raceManager.characterSheet.GetComponent<CharacterSheet>().acceleration >= 10f)
        {
            accelerationButton.interactable = false;
            accelerationButton.GetComponentInChildren<TextMeshProUGUI>().text = "MAX";
            accelerationCostText.text = "-";
            accelerationUpgradeLevels.color = Color.yellow;

        }

        //speedText.text = "Top speed " + (maxSpeed - raceManager.characterSheet.GetComponent<CharacterSheet>().maxSpeed);
        float speedCost = 1000f + (1000f * (maxSpeed - raceManager.characterSheet.GetComponent<CharacterSheet>().maxSpeed));
        speedCostText.text = "$" + speedCost;

        if(cash < speedCost)
        {
            speedButton.interactable = false;
        }
        else
        {
            speedButton.interactable = true;
        }

        speedUpgradeLevels.fillAmount = (maxSpeed - raceManager.characterSheet.GetComponent<CharacterSheet>().maxSpeed) / 50f;


        if (maxSpeed - raceManager.characterSheet.GetComponent<CharacterSheet>().maxSpeed >= 50f)
        {
            speedButton.interactable = false;
            speedButton.GetComponentInChildren<TextMeshProUGUI>().text = "MAX";
            speedCostText.text = "-";
            speedUpgradeLevels.color = Color.yellow;

        }

        //gripText.text = "Grip " + (grip - raceManager.characterSheet.GetComponent<CharacterSheet>().grip);
        float gripCost = 1000f + (1000f * (grip - raceManager.characterSheet.GetComponent<CharacterSheet>().grip));
        gripCostText.text = "$" + gripCost;

        if(cash < gripCost)
        {
            gripButton.interactable = false;
        }
        else
        {
            gripButton.interactable = true;
        }

        gripUpgradeLevels.fillAmount = (grip - raceManager.characterSheet.GetComponent<CharacterSheet>().grip) / 10f;


        if (grip - raceManager.characterSheet.GetComponent<CharacterSheet>().grip >= 10f)
        {
            gripButton.interactable = false;
            gripButton.GetComponentInChildren<TextMeshProUGUI>().text = "MAX";
            gripCostText.text = "-";
            gripUpgradeLevels.color = Color.yellow;

        }

        //strafeText.text = "Strafe " + (maxStrafe - raceManager.characterSheet.GetComponent<CharacterSheet>().maxStrafe);
        float strafeCost = 1000f + (1000f * (maxStrafe - raceManager.characterSheet.GetComponent<CharacterSheet>().maxStrafe));
        strafeCostText.text = "$" + strafeCost;

        if(cash < strafeCost)
        {
            strafeButton.interactable = false;
        }
        else
        {
            strafeButton.interactable = true;
        }

        strafeUpgradeLevels.fillAmount = (maxStrafe - raceManager.characterSheet.GetComponent<CharacterSheet>().maxStrafe) / 10f;


        if (maxStrafe - raceManager.characterSheet.GetComponent<CharacterSheet>().maxStrafe >= 10f)
        {
            strafeButton.interactable = false;
            strafeButton.GetComponentInChildren<TextMeshProUGUI>().text = "MAX";
            strafeCostText.text = "-";
            strafeUpgradeLevels.color = Color.yellow;

        }

        //brakeText.text = "Brake " + (brake - raceManager.characterSheet.GetComponent<CharacterSheet>().brake);
        float brakeCost = 1000f + (1000f * (brake - raceManager.characterSheet.GetComponent<CharacterSheet>().brake));
        brakeCostText.text = "$" + brakeCost;

        if(cash < brakeCost)
        {
            brakeButton.interactable = false;
        }
        else
        {
            brakeButton.interactable = true;
        }

        brakeUpgradeLevels.fillAmount = (brake - raceManager.characterSheet.GetComponent<CharacterSheet>().brake) / 20f;


        if (brake - raceManager.characterSheet.GetComponent<CharacterSheet>().brake >= 20f)
        {
            brakeButton.interactable = false;
            brakeButton.GetComponentInChildren<TextMeshProUGUI>().text = "MAX";
            brakeCostText.text = "-";
            brakeUpgradeLevels.color = Color.yellow;

        }

        //fuelEfficiencyText.text = "Fuel efficiency " + (raceManager.characterSheet.GetComponent<CharacterSheet>().fuelConsumption - fuelConsumption);
        float fuelConsumptionCost = 1000f + (1000f * (raceManager.characterSheet.GetComponent<CharacterSheet>().fuelConsumption - fuelConsumption));
        fuelEfficiencyCostText.text = "$" + fuelConsumptionCost;

        if(cash < fuelConsumptionCost)
        {
            fuelEfficiencyButton.interactable = false;
        }
        else
        {
            fuelEfficiencyButton.interactable = true;
        }

        fuelUseUpgradeLevels.fillAmount = (raceManager.characterSheet.GetComponent<CharacterSheet>().fuelConsumption - fuelConsumption) / 10f;


        if (raceManager.characterSheet.GetComponent<CharacterSheet>().fuelConsumption - fuelConsumption >= 10f)
        {
            fuelEfficiencyButton.interactable = false;
            fuelEfficiencyButton.GetComponentInChildren<TextMeshProUGUI>().text = "MAX";
            fuelEfficiencyCostText.text = "-";
            fuelUseUpgradeLevels.color = Color.yellow;

        }

        //boostValueText.text = "Fuel boost " + (boostValue - raceManager.characterSheet.GetComponent<CharacterSheet>().boostValue);
        float boostValueCost = 1000f + (1000f * (boostValue - raceManager.characterSheet.GetComponent<CharacterSheet>().boostValue));
        boostValueCostText.text = "$" + boostValueCost;

        if (cash < boostValueCost)
        {
            boostValueButton.interactable = false;
        }
        else
        {
            boostValueButton.interactable = true;
        }

        boostUpgradeLevels.fillAmount = (boostValue - raceManager.characterSheet.GetComponent<CharacterSheet>().boostValue) / 50f;


        if (boostValue - raceManager.characterSheet.GetComponent<CharacterSheet>().boostValue >= 50f)
        {
            boostValueButton.interactable = false;
            boostValueButton.GetComponentInChildren<TextMeshProUGUI>().text = "MAX";
            boostValueCostText.text = "-";
            boostUpgradeLevels.color = Color.yellow;

        }

        //weaponDamageText.text = "Weapon damage " + (weaponDamage - raceManager.characterSheet.GetComponent<CharacterSheet>().weaponDamage);
        float weaponDamageCost = 1000f + (1000f * (weaponDamage - raceManager.characterSheet.GetComponent<CharacterSheet>().weaponDamage));
        weaponDamageCostText.text = "$" + weaponDamageCost;

        if(cash < weaponDamageCost)
        {
            weaponDamageButton.interactable = false;
        }
        else
        {
            weaponDamageButton.interactable = true;
        }

        weaponDamageUpgradeLevels.fillAmount = (weaponDamage - raceManager.characterSheet.GetComponent<CharacterSheet>().weaponDamage) / 30f;


        if (weaponDamage - raceManager.characterSheet.GetComponent<CharacterSheet>().weaponDamage >= 30f)
        {
            weaponDamageButton.interactable = false;
            weaponDamageButton.GetComponentInChildren<TextMeshProUGUI>().text = "MAX";
            weaponDamageCostText.text = "-";
            weaponDamageUpgradeLevels.color = Color.yellow;

        }

        //weaponSlowText.text = "Weapon disruption " + (weaponSlow - raceManager.characterSheet.GetComponent<CharacterSheet>().weaponSlow);
        float weaponSlowCost = 1000f + (1000f * (weaponSlow - raceManager.characterSheet.GetComponent<CharacterSheet>().weaponSlow));
        weaponSlowCostText.text = "$" + weaponSlowCost;

        if (cash < weaponSlowCost)
        {
            weaponSlowButton.interactable = false;
        }
        else
        {
            weaponSlowButton.interactable = true;
        }

        weaponSlowUpgradeLevels.fillAmount = (weaponSlow - raceManager.characterSheet.GetComponent<CharacterSheet>().weaponSlow) / 10f;


        if (weaponSlow - raceManager.characterSheet.GetComponent<CharacterSheet>().weaponSlow >= 10f)
        {
            weaponSlowButton.interactable = false;
            weaponSlowButton.GetComponentInChildren<TextMeshProUGUI>().text = "MAX";
            weaponSlowCostText.text = "-";
            weaponSlowUpgradeLevels.color = Color.yellow;
        }

        //weaponSpeedText.text = "Weapon speed " + Mathf.RoundToInt((raceManager.characterSheet.GetComponent<CharacterSheet>().weaponSpeed - weaponSpeed) * 100f);
        float weaponSpeedCost = + 1000f + Mathf.RoundToInt(100000f * (raceManager.characterSheet.GetComponent<CharacterSheet>().weaponSpeed - weaponSpeed));
        weaponSpeedCostText.text = "$" + weaponSpeedCost;

        if (cash < weaponSpeedCost)
        {
            weaponSpeedButton.interactable = false;
        }
        else
        {
            weaponSpeedButton.interactable = true;
        }

        weaponSpeedUpgradeLevels.fillAmount = Mathf.RoundToInt((raceManager.characterSheet.GetComponent<CharacterSheet>().weaponSpeed - weaponSpeed) * 100f) / 10f;


        if (weaponSpeed - raceManager.characterSheet.GetComponent<CharacterSheet>().weaponSpeed >= 10f)
        {
            weaponSpeedButton.interactable = false;
            weaponSpeedButton.GetComponentInChildren<TextMeshProUGUI>().text = "MAX";
            weaponSpeedCostText.text = "-";
            weaponSpeedUpgradeLevels.color = Color.yellow;

        }

        if (ship == 0)
        {
            shipText.text = "Ship: Racer";
        }
        else if(ship == 1)
        {
            shipText.text = "Ship: Saucer";
        }
        else if(ship == 2)
        {
            shipText.text = "Ship: Fighter";
        }


        if (engine == 0)
        {
            enginesText.text = "Engine: Dual thrusters";
        }
        else if (engine == 1)
        {
            enginesText.text = "Engine: Zoomer";
        }
        else if (engine == 2)
        {
            enginesText.text = "Engine: Mega thruster";
        }

        if (weapons == 0)
        {
            weaponsText.text = "Weapon: Laser beam";
        }
        else if (weapons == 1)
        {
            weaponsText.text = "Weapon: Missiles";
        }
        else if (weapons == 2)
        {
            weaponsText.text = "Weapon: Machine gun";
        }
    }

    public void ApplyUpgrades()
    {
        raceManager.characterSheets[9].GetComponent<CharacterSheet>().pilot = pilot;
        raceManager.characterSheets[9].GetComponent<CharacterSheet>().pilotShip = ship;
        raceManager.characterSheets[9].GetComponent<CharacterSheet>().pilotEngine = engine;
        raceManager.characterSheets[9].GetComponent<CharacterSheet>().pilotWeapons = weapons;

        raceManager.characterSheets[9].GetComponent<CharacterSheet>().wins = wins;
        raceManager.characterSheets[9].GetComponent<CharacterSheet>().money = cash;

        raceManager.characterSheets[9].GetComponent<CharacterSheet>().health = damage;
        raceManager.characterSheets[9].GetComponent<CharacterSheet>().maxHealth = maxDamage;
        raceManager.characterSheets[9].GetComponent<CharacterSheet>().fuel = fuel;
        raceManager.characterSheets[9].GetComponent<CharacterSheet>().maxFuel = maxFuel;
        raceManager.characterSheets[9].GetComponent<CharacterSheet>().ammo = ammo;
        raceManager.characterSheets[9].GetComponent<CharacterSheet>().maxAmmo = maxAmmo;

        raceManager.characterSheets[9].GetComponent<CharacterSheet>().acceleration = acceleration;
        raceManager.characterSheets[9].GetComponent<CharacterSheet>().maxSpeed = maxSpeed;
        raceManager.characterSheets[9].GetComponent<CharacterSheet>().grip = grip;
        raceManager.characterSheets[9].GetComponent<CharacterSheet>().maxStrafe = maxStrafe;
        raceManager.characterSheets[9].GetComponent<CharacterSheet>().brake = brake;

        raceManager.characterSheets[9].GetComponent<CharacterSheet>().fuelConsumption = fuelConsumption;
        raceManager.characterSheets[9].GetComponent<CharacterSheet>().boostValue = boostValue;

        raceManager.characterSheets[9].GetComponent<CharacterSheet>().weaponDamage = weaponDamage;
        raceManager.characterSheets[9].GetComponent<CharacterSheet>().weaponSlow = weaponSlow;
        raceManager.characterSheets[9].GetComponent<CharacterSheet>().weaponSpeed = weaponSpeed;
    }

    public void SaveData(GameData data)
    {
        //if(data.sheets.Count > 0)
        //{
        //    data.sheets[data.sheets.Count - 23] = cash;

        //    print("save " + data.sheets[data.sheets.Count - 23]);
        //}
        //instead of this, the data must be pushed to the character sheet in race manager, then saved

    }

    //public void SetData(GameData data)
    //{
    //    pilotText.text = "Pilot " + data.sheets[data.sheets.Count - 4].ToString();
    //    cashText.text = "Cash " + data.sheets[data.sheets.Count - 23].ToString();
    //    winsText.text = "Wins " + data.sheets[data.sheets.Count - 25].ToString();
    //}
}
