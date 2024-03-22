using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.UI;
using TMPro;

//this keeps track of the order of the racers, so we know who is in the lead

//previously, it gathered all the racers in the scene
//now, I'm developing a system to use this script to instead add them to the scene

//for now this means I will manually add racers to the RacersList, later they'll be added from somewhere else... tournament manager? which comes from main menu stuff

//in Start, the racers are instantiated at the start zone positions

public class RaceManager : MonoBehaviour, IDataPersistence
{
    //this is a list of prefabs to be instantiated as races
    public List<GameObject> RacersList = new List<GameObject>();

    //this is a list of the racers which have been instantiated
    public List<GameObject> RacersInRace = new List<GameObject>();

    //this is a list of pilot object prefabs which will be attached to the ships
    public List<GameObject> Pilots = new List<GameObject>();

    //this is a list of stats for all racers partaking in the career
    public List<GameObject> characterSheets = new List<GameObject>();

    //this is a list of leaderboard positions for the racers
    public List<GameObject> Orders = new List<GameObject>();
    public bool updateOrders;

    //list of ships that have reached the goal
    public List<GameObject> GoalReached = new List<GameObject>();

    //the start zone is the positions the racers will take at the start of the race
    public Transform startZone;

    public GameObject characterSheet;

    public Transform startingLights;

    public DataPersistenceManager dataPersistenceManager;

    public Transform mainMenu;
    public Transform victoryScreen;
    public TextMeshProUGUI winList;
    public TextMeshProUGUI rewardText;
    public TextMeshProUGUI positionText;

    //career mode determines some other variables, such as if racers are repaired upon being destroyed
    public bool careerMode;

    public bool infiniteLevel;
    public bool randomLevelLength;
    public int maxLevelLength;



    private void Awake()
    {
        if(characterSheets.Count > 0)
        {

        }
        else
        {
            ReinitializeSheetList();

        }

    }

    // Start is called before the first frame update
    void Start()
    {
        //dataPersistenceManager = DataPersistenceManager.instance;

    }

    public void LoadData(GameData data)
    {
        for (int i = 0; i < characterSheets.Count; i++)
        {
            int j = i * 25;
            characterSheets[i].GetComponent<CharacterSheet>().wins = data.sheets[0 + j];
            characterSheets[i].GetComponent<CharacterSheet>().losses = data.sheets[1 + j];
            characterSheets[i].GetComponent<CharacterSheet>().money = data.sheets[2 + j];
            characterSheets[i].GetComponent<CharacterSheet>().health = data.sheets[3 + j];
            characterSheets[i].GetComponent<CharacterSheet>().maxHealth = data.sheets[4 + j];
            characterSheets[i].GetComponent<CharacterSheet>().ammo = data.sheets[5 + j];
            characterSheets[i].GetComponent<CharacterSheet>().fuel = data.sheets[6 + j];
            characterSheets[i].GetComponent<CharacterSheet>().acceleration = data.sheets[7 + j];
            characterSheets[i].GetComponent<CharacterSheet>().maxSpeed = data.sheets[8 + j];
            characterSheets[i].GetComponent<CharacterSheet>().maxFuel = data.sheets[9 + j];
            characterSheets[i].GetComponent<CharacterSheet>().fuelConsumption = data.sheets[10 + j];
            characterSheets[i].GetComponent<CharacterSheet>().boostValue = data.sheets[11 + j];
            characterSheets[i].GetComponent<CharacterSheet>().maxStrafe = data.sheets[12 + j];
            characterSheets[i].GetComponent<CharacterSheet>().brake = data.sheets[13 + j];
            characterSheets[i].GetComponent<CharacterSheet>().grip = data.sheets[14 + j];
            characterSheets[i].GetComponent<CharacterSheet>().weaponDamage = data.sheets[15 + j];
            characterSheets[i].GetComponent<CharacterSheet>().weaponSpeed = data.sheets[16 + j];
            characterSheets[i].GetComponent<CharacterSheet>().maxAmmo = data.sheets[17 + j];
            characterSheets[i].GetComponent<CharacterSheet>().ammoConsumption = data.sheets[18 + j];
            characterSheets[i].GetComponent<CharacterSheet>().weaponSlow = data.sheets[19 + j];
            characterSheets[i].GetComponent<CharacterSheet>().weaponAccuracy = data.sheets[20 + j];
            characterSheets[i].GetComponent<CharacterSheet>().pilot = data.sheets[21 + j];
            characterSheets[i].GetComponent<CharacterSheet>().pilotEngine = data.sheets[22 + j];
            characterSheets[i].GetComponent<CharacterSheet>().pilotWeapons = data.sheets[23 + j];
            characterSheets[i].GetComponent<CharacterSheet>().pilotShip = data.sheets[24 + j];

        }
    }

    public void SaveData(GameData data)
    {
        //if data already exists, it is overwritten, else new data created
        if(data.sheets.Count > 0)
        {
            for (int i = 0; i < characterSheets.Count; i++)
            {
                int j = i * 25;

                data.sheets[0 + j] = characterSheets[i].GetComponent<CharacterSheet>().wins;
                data.sheets[1 + j] = characterSheets[i].GetComponent<CharacterSheet>().losses;
                data.sheets[2 + j] = characterSheets[i].GetComponent<CharacterSheet>().money;
                data.sheets[3 + j] = characterSheets[i].GetComponent<CharacterSheet>().health;
                data.sheets[4 + j] = characterSheets[i].GetComponent<CharacterSheet>().maxHealth;
                data.sheets[5 + j] = characterSheets[i].GetComponent<CharacterSheet>().ammo;
                data.sheets[6 + j] = characterSheets[i].GetComponent<CharacterSheet>().fuel;
                data.sheets[7 + j] = characterSheets[i].GetComponent<CharacterSheet>().acceleration;
                data.sheets[8 + j] = characterSheets[i].GetComponent<CharacterSheet>().maxSpeed;
                data.sheets[9 + j] = characterSheets[i].GetComponent<CharacterSheet>().maxFuel;
                data.sheets[10 + j] = characterSheets[i].GetComponent<CharacterSheet>().fuelConsumption;
                data.sheets[11 + j] = characterSheets[i].GetComponent<CharacterSheet>().boostValue;
                data.sheets[12 + j] = characterSheets[i].GetComponent<CharacterSheet>().maxStrafe;
                data.sheets[13 + j] = characterSheets[i].GetComponent<CharacterSheet>().brake;
                data.sheets[14 + j] = characterSheets[i].GetComponent<CharacterSheet>().grip;
                data.sheets[15 + j] = characterSheets[i].GetComponent<CharacterSheet>().weaponDamage;
                data.sheets[16 + j] = characterSheets[i].GetComponent<CharacterSheet>().weaponSpeed;
                data.sheets[17 + j] = characterSheets[i].GetComponent<CharacterSheet>().maxAmmo;
                data.sheets[18 + j] = characterSheets[i].GetComponent<CharacterSheet>().ammoConsumption;
                data.sheets[19 + j] = characterSheets[i].GetComponent<CharacterSheet>().weaponSlow;
                data.sheets[20 + j] = characterSheets[i].GetComponent<CharacterSheet>().weaponAccuracy;
                data.sheets[21 + j] = characterSheets[i].GetComponent<CharacterSheet>().pilot;
                data.sheets[22 + j] = characterSheets[i].GetComponent<CharacterSheet>().pilotEngine;
                data.sheets[23 + j] = characterSheets[i].GetComponent<CharacterSheet>().pilotWeapons;
                data.sheets[24 + j] = characterSheets[i].GetComponent<CharacterSheet>().pilotShip;

            }
        }
        else
        {
            foreach (GameObject obj in characterSheets)
            {
                data.sheets.AddRange(
                    new List<float>() {
                    obj.GetComponent<CharacterSheet>().wins,
                    obj.GetComponent<CharacterSheet>().losses,
                    obj.GetComponent<CharacterSheet>().money,
                    obj.GetComponent<CharacterSheet>().health,
                    obj.GetComponent<CharacterSheet>().maxHealth,
                    obj.GetComponent<CharacterSheet>().ammo,
                    obj.GetComponent<CharacterSheet>().fuel,
                    obj.GetComponent<CharacterSheet>().acceleration,
                    obj.GetComponent<CharacterSheet>().maxSpeed,
                    obj.GetComponent<CharacterSheet>().maxFuel,
                    obj.GetComponent<CharacterSheet>().fuelConsumption,
                    obj.GetComponent<CharacterSheet>().boostValue,
                    obj.GetComponent<CharacterSheet>().maxStrafe,
                    obj.GetComponent<CharacterSheet>().brake,
                    obj.GetComponent<CharacterSheet>().grip,
                    obj.GetComponent<CharacterSheet>().weaponDamage,
                    obj.GetComponent<CharacterSheet>().weaponSpeed,
                    obj.GetComponent<CharacterSheet>().maxAmmo,
                    obj.GetComponent<CharacterSheet>().ammoConsumption,
                    obj.GetComponent<CharacterSheet>().weaponSlow,
                    obj.GetComponent<CharacterSheet>().weaponAccuracy,
                    obj.GetComponent<CharacterSheet>().pilot,
                    obj.GetComponent<CharacterSheet>().pilotEngine,
                    obj.GetComponent<CharacterSheet>().pilotWeapons,
                    obj.GetComponent<CharacterSheet>().pilotShip
                        }
                    );
            }
        }
    }

    public void NewCareer()
    {
        ReinitializeSheetList();

        dataPersistenceManager.SaveGame();
    }

    public void ReinitializeSheetList()
    {
        CleanUpExtraSheets();

        print("reinitializing sheets");
        for (int i = 0; i < characterSheets.Count; i++)
        {
            Destroy(characterSheets[i]);
        }

        RemoveOldCharacterSheets();

        characterSheets.Clear();

        for (int i = 0; i < RacersList.Count; i++)
        {
            GameObject charSheet = Instantiate(characterSheet, transform.position, Quaternion.identity, GameObject.FindObjectOfType<RaceManager>().transform);

            characterSheets.Add(charSheet);

            //characterSheets[i].GetComponent<CharacterSheet>().pilot = i;
        }
    }

    public void RemoveOldCharacterSheets()
    {
        for (int i = 0; i < characterSheets.Count; i++)
        {
            Destroy(characterSheets[i]);
        }
    }

    public void FindStartZone()
    {
        if (startZone == null)
        {
            //GameObject levelGenerator = FindObjectOfType<LevelGenerator>().gameObject;

            if (FindObjectOfType<LevelGenerator>() != null)
            {
                startZone = FindObjectOfType<LevelGenerator>().GetComponent<LevelGenerator>().levelPart_Start.Find("StartZone");

                SpawnRacers();
            }
        }
    }

    public void SpawnRacers()
    {
        for (int i = 0; i < RacersList.Count; i++)
        {
            GameObject racer = Instantiate(RacersList[i], startZone.GetChild(i).position, transform.rotation);

            if (careerMode)
            {
                GameObject charSheet = Instantiate(characterSheets[i], racer.transform.position, transform.rotation, racer.transform);
                racer.GetComponent<ShipController>().careerMode = true;
            }
            else
            {
                racer.GetComponent<ShipController>().careerMode = false;
            }

            racer.GetComponent<ShipController>().controlsEnabled = false;

            RacersInRace.Add(racer);
        }

        startingLights.gameObject.SetActive(true);
        GetComponentInChildren<Animator>().Play("startcount", -1, 0f);

        updateOrders = true;

        StartCoroutine(UpdateOrderedList());
    }

    public void CountdownStart()
    {
        for(int i = 0;i < RacersInRace.Count;i++)
        {
            RacersInRace[i].GetComponent<ShipController>().controlsEnabled = true;
        }

        GetComponentInChildren<Animator>().SetBool("startCount", false);
    }

    public IEnumerator UpdateOrderedList()
    {
        if(updateOrders)
        {
            Orders = OrderRacers();

            Orders.Reverse();

            for (int i = 0; i < Orders.Count; i++)
            {
                Orders[i].GetComponent<ShipController>().myPositionInRace = i;
            }

            yield return new WaitForSeconds(0.2f);


            StartCoroutine(UpdateOrderedList());
        }

    }

    public void EnableVictoryScreen()
    {
        victoryScreen.gameObject.SetActive(true);

        if (RacersInRace[9].GetComponent<ShipController>().myPositionInRace == 0)
        {
            positionText.text = "Congratulations! You win!";
        }
        else if (RacersInRace[9].GetComponent<ShipController>().myPositionInRace == 1)
        {
            positionText.text = "You came second! Pretty good!";
        }
        else if (RacersInRace[9].GetComponent<ShipController>().myPositionInRace == 2)
        {
            positionText.text = "Alright! Third place!";
        }
        else
        {
            positionText.text = "At least it's over!";
        }

        if (RacersInRace[0].GetComponentInChildren<CharacterSheet>() != null)
        {
            float cashReward = 3000f - (1000f * (float)RacersInRace[9].GetComponent<ShipController>().myPositionInRace);
            if (cashReward < 500f)
                cashReward = 500f;

            rewardText.text = "+" + cashReward + " cash earned.";
        }
        else
        {
            rewardText.text = "";
        }




        UpdateVictoryList();
    }

    public void UpdateVictoryList()
    {
        string victoryList = "";

        int count = 0;

        foreach (var item in GoalReached)
        {
            count++;

            string inGoal = Pilots[item.gameObject.GetComponent<CreateShipModel>().pilotIndex].GetComponent<PilotStats>().pilotName;
                //item.gameObject.GetComponentInChildren<CharacterSheet>().pilot
                //GetComponentInChildren<PilotStats>().pilotName;

            victoryList += count + ": " + inGoal + "\n";
        }

        winList.text = victoryList;
    }

    //this happens if the player's ship is destroyed during a race, causing instant loss
    //only difference to OnRaceFinished is that it skips player's position in race when calculating rewards, and gives a single hit point back for free 
    public void OnRaceLoss()
    {
        for (int i = 0; i < RacersInRace.Count; i++)
        {
            if(i != 9)
            {
                characterSheets[i].GetComponent<CharacterSheet>().health = RacersInRace[i].GetComponent<DamageManager>().hitPoints;
                characterSheets[i].GetComponent<CharacterSheet>().ammo = RacersInRace[i].GetComponent<ShipController>().ammo;
                characterSheets[i].GetComponent<CharacterSheet>().fuel = RacersInRace[i].GetComponent<ShipController>().fuel;
                characterSheets[i].GetComponent<CharacterSheet>().wins = RacersInRace[i].GetComponentInChildren<CharacterSheet>().wins;
                characterSheets[i].GetComponent<CharacterSheet>().money = RacersInRace[i].GetComponentInChildren<CharacterSheet>().money;
                //also money

                float cashReward = 3000f - (1000f * (float)RacersInRace[i].GetComponent<ShipController>().myPositionInRace);
                if (cashReward < 500f)
                    cashReward = 500f;

                characterSheets[i].GetComponent<CharacterSheet>().money += cashReward;
                //3000f - ((float)RacersInRace[i].GetComponent<ShipController>().myPositionInRace * 1000f);

            }
            else
            {
                characterSheets[i].GetComponent<CharacterSheet>().health = RacersInRace[i].GetComponent<DamageManager>().hitPoints + 1;
                characterSheets[i].GetComponent<CharacterSheet>().ammo = RacersInRace[i].GetComponent<ShipController>().ammo;
                characterSheets[i].GetComponent<CharacterSheet>().fuel = RacersInRace[i].GetComponent<ShipController>().fuel;
                characterSheets[i].GetComponent<CharacterSheet>().wins = RacersInRace[i].GetComponentInChildren<CharacterSheet>().wins;
                characterSheets[i].GetComponent<CharacterSheet>().money = RacersInRace[i].GetComponentInChildren<CharacterSheet>().money;

                characterSheets[i].GetComponent<CharacterSheet>().money += 500f;
            }


        }

        dataPersistenceManager.SaveGame();


        ResetRaceManager();

        //ReinitializeSheetList();

        SceneController.sceneControllerInstance.LoadScene("MainMenu");

        Invoke("GoToMainMenu", 2f);
    }

    //when the race is over, the character sheet is updated with some values
    public void OnRaceFinished()
    {

        if (RacersInRace[0].GetComponentInChildren<CharacterSheet>() != null)
        {
            for (int i = 0; i < RacersInRace.Count; i++)
            {
                characterSheets[i].GetComponent<CharacterSheet>().health = RacersInRace[i].GetComponent<DamageManager>().hitPoints;
                characterSheets[i].GetComponent<CharacterSheet>().ammo = RacersInRace[i].GetComponent<ShipController>().ammo;
                characterSheets[i].GetComponent<CharacterSheet>().fuel = RacersInRace[i].GetComponent<ShipController>().fuel;
                characterSheets[i].GetComponent<CharacterSheet>().wins = RacersInRace[i].GetComponentInChildren<CharacterSheet>().wins;
                characterSheets[i].GetComponent<CharacterSheet>().money = RacersInRace[i].GetComponentInChildren<CharacterSheet>().money;
                //also money

                float cashReward = 3000f - (1000f * (float)RacersInRace[i].GetComponent<ShipController>().myPositionInRace);
                if (cashReward < 500f)
                    cashReward = 500f;

                characterSheets[i].GetComponent<CharacterSheet>().money += cashReward;
                //3000f - ((float)RacersInRace[i].GetComponent<ShipController>().myPositionInRace * 1000f);




            }

            dataPersistenceManager.SaveGame();


            ResetRaceManager();

            //ReinitializeSheetList();

            SceneController.sceneControllerInstance.LoadScene("MainMenu");
            victoryScreen.gameObject.SetActive(false);


            Invoke("GoToCareerMenu", 2f);
        }
        else
        {
            ResetRaceManager();

            SceneController.sceneControllerInstance.LoadScene("MainMenu");
            victoryScreen.gameObject.SetActive(false);


            Invoke("GoToMainMenu", 2f);
        }



    }

    public void GoToMainMenu()
    {
        mainMenu.GetComponent<MainMenuScript>().ToggleMenuOn();
    }

    public void GoToCareerMenu()
    {
        //ReinitializeSheetList();

        mainMenu.GetComponent<MainMenuScript>().OpenCareerPage();

    }

    public void CleanUpExtraSheets()
    {
        CharacterSheet[] extras = GetComponentsInChildren<CharacterSheet>();

        print("cleaning sheets " + extras.Length);

        foreach (CharacterSheet characterSheetextra in extras)
        {
            Destroy(characterSheetextra.gameObject);
        }
    }

    //if race is cancelled and player returns to main menu, race manager's lists should be reset to avoid confusion
    public void ResetRaceManager()
    {
        updateOrders = false;
        RacersInRace.Clear();
        //transform.GetChild(0).GetComponentInChildren<Animation>().Stop("startcount");
        startingLights.gameObject.SetActive(false);
        Orders.Clear();
        GoalReached.Clear();

        foreach (var item in characterSheets)
        {
            Destroy(item.gameObject);
        }

        for (int i = 0; i < characterSheets.Count; i++)
        {
            Destroy(characterSheets[i]);
        }

        characterSheets.Clear();

        ReinitializeSheetList();
    }

    public List<GameObject> OrderRacers()
    {
        if(updateOrders)
        {
            if(RacersInRace != null)
            {
                //this keeps giving an error when changing scenes, but I'm not sure how to prevent it
                IOrderedEnumerable<GameObject> RacersListOrdered = RacersInRace.OrderBy(x => x.transform.position.z);

                return RacersListOrdered.ToList();
            }
            else
            {
                return null;
            }

        }
        else
        {
            return null;
        }

    }
}
