using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSlotsMenu : MonoBehaviour
{

    private SaveSlot[] saveSlots;

    public bool isLoadingGame = false;

    [SerializeField] private ConfirmationPopupMenu confirmationPopup;

    public Transform mainMenu;

    public DataPersistenceManager dataPersistenceManager;

    private void Awake()
    {
        saveSlots = this.GetComponentsInChildren<SaveSlot>();
    }

    public void OnSaveSlotClicked(SaveSlot saveSlot)
    {
        //DataPersistenceManager.instance.ChangeSelectedProfileId(saveSlot.GetProfileId());

        //if(!isLoadingGame)
        //{
        //    DataPersistenceManager.instance.NewGame();

        //}

        //SceneController.sceneControllerInstance.LoadScene("TestSimple");

        if (isLoadingGame)
        {
            //DataPersistenceManager.instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
            dataPersistenceManager.ChangeSelectedProfileId(saveSlot.GetProfileId());
            //SaveGameAndLoadScene();
            GoToCareer();

        }
        else if(saveSlot.hasData)
        {
            confirmationPopup.ActivateMenu(
                "Overwrite save and start a new game? Are you sure?",
                () =>
                {
                    dataPersistenceManager.ChangeSelectedProfileId(saveSlot.GetProfileId());
                    //DataPersistenceManager.instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
                    //DataPersistenceManager.instance.NewGame();
                    dataPersistenceManager.NewGame();
                    //SaveGameAndLoadScene();
                    GoToPilotSelection();

                },
                () =>
                {
                    this.ActivateMenu(isLoadingGame);
                });
        }
        else
        {
            //DataPersistenceManager.instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
            dataPersistenceManager.ChangeSelectedProfileId(saveSlot.GetProfileId());
            //DataPersistenceManager.instance.NewGame();
            dataPersistenceManager.NewGame();
            //SaveGameAndLoadScene();
            GoToPilotSelection();
        }

        DeactivateMenu();
    }

    //private void SaveGameAndLoadScene()
    //{
    //    DataPersistenceManager.instance.SaveGame();

    //    SceneController.sceneControllerInstance.LoadScene("TestSimple");

    //}

    private void GoToCareer()
    {
        //DataPersistenceManager.instance.LoadGame();
        dataPersistenceManager.LoadGame();

        mainMenu.GetComponent<MainMenuScript>().OpenCareerPage();
    }

    private void GoToPilotSelection()
    {
        //DataPersistenceManager.instance.SaveGame();
        dataPersistenceManager.SaveGame();

        //mainMenu.GetComponent<MainMenuScript>().OpenCareerPage();
        mainMenu.GetComponent<MainMenuScript>().CareerPilotSelectionMenu();
    }

    public void ActivateMenu(bool isLoadingGame)
    {
        this.gameObject.SetActive(true);

        this.isLoadingGame = isLoadingGame;

        Dictionary<string, GameData> profilesGameData = dataPersistenceManager.GetAllProfilesGameData();
        //Dictionary<string, GameData> profilesGameData = DataPersistenceManager.instance.GetAllProfilesGameData();

        foreach (SaveSlot slot in saveSlots)
        {
            GameData profileData = null;

            profilesGameData.TryGetValue(slot.GetProfileId(), out profileData);
            slot.SetData(profileData);

            if(profileData == null && isLoadingGame)
            {
                slot.SetInteractable(false);
            }
            else
            {
                slot.SetInteractable(true);
            }
        }
    
    }

    public void DeactivateMenu()
    {
        this.gameObject.SetActive(false);
    }
}
