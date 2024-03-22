using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;

    [SerializeField] private bool useEncryption;

    public GameData gameData;
    //public static DataPersistenceManager instance { get; private set; }

    private List<IDataPersistence> dataPersistenceObjects = new List<IDataPersistence>();

    private FileDataHandler fileDataHandler;

    private string selectedProfileId = "";

    public RaceManager raceManager;

    private void Awake()
    {
        //if (instance != null)
        //{
        //    Debug.LogError("Found more than one Data Persistence Manager!");
        //}
        //instance = this;

        this.fileDataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);

        this.selectedProfileId = fileDataHandler.GetMostRecentlyUpdatedProfileId();
    }

    private void Start()
    {
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();



        //temp add test
        //LoadGame();
    }

    public void ChangeSelectedProfileId(string newProfileId)
    {
        this.selectedProfileId = newProfileId;
        LoadGame();
    }

    public void NewGame()
    {
        this.gameData = new GameData();

        raceManager.NewCareer();
    }

    public void LoadGame()
    {
        this.gameData = fileDataHandler.Load(selectedProfileId);

        //load game data from file data handler
        if(this.gameData == null)
        {
            Debug.Log("No data found. Please start a new game.");

            //NewGame();

            return;
        }

        //push data to where it goes

        foreach (IDataPersistence dataPersistence in dataPersistenceObjects)
        {
            dataPersistence.LoadData(gameData);

            print("pushing data to " + dataPersistence);
        }
    }

    public void SaveGame()
    {
        //push data to where it goes

        if(this.gameData == null)
        {
            Debug.LogWarning("No data was found. Please start a new game.");

            return;
        }

        foreach (IDataPersistence dataPersistence in dataPersistenceObjects)
        {
            dataPersistence.SaveData(gameData);
        }

        gameData.lastUpdated = System.DateTime.Now.ToBinary();

        //save data with file data handler
        fileDataHandler.Save(gameData, selectedProfileId);
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true)
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public bool HasGameData()
    {
        return gameData != null;
    }

    public Dictionary<string, GameData> GetAllProfilesGameData()
    {
        return fileDataHandler.LoadAllProfiles();
    }

}
