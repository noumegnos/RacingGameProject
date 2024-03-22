using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//this script contains functions that determine behaviour of UI buttons on the main menu

public class MainMenuScript : MonoBehaviour 
{
    public SceneController sceneController;

    //public CanvasGroup menuButtons;

    public GameObject menuButtonHolder;

    public GameObject pauseMenuButtonHolder;

    public GameObject careerMenuHolder;

    public GameObject careerOptionsHolder;

    public GameObject savesMenuHolder;

    public GameObject pilotSelectionMenu;

    public GameObject optionsPageHolder;
    public bool musicOn;
    public bool sfxOn;
    public GameObject musicPlayer;

    public GameObject helpPage;

    public SaveSlotsMenu saveSlotsMenu;

    public DataPersistenceManager dataPersistenceManager;

    //public GameObject careerSheet;

    public RaceManager raceManager;

    public Button continueCareerButton;
    public Button loadCareerButton;

    private void Start()
    {
        sceneController = SceneController.sceneControllerInstance;
        //dataPersistenceManager = DataPersistenceManager.instance;

        //if (!dataPersistenceManager.HasGameData())
        //{
        //    continueCareerButton.interactable = false;
        //}
    }

    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Escape))
        //{
        //    if(pauseMenuButtonHolder.activeSelf)
        //    {
        //        //ToggleMenuOff();
        //        UnpauseTime();

        //    }
        //    else
        //    { 
        //        //ToggleMenuOn();
        //        PauseTime();
        //    }
        //}
    }

    public void PauseMenuOn()
    {
        pauseMenuButtonHolder.SetActive(true);
        PauseTime();
    }
    public void PauseMenuOff()
    {
        pauseMenuButtonHolder.SetActive(false);
        OptionsToggleOff();
        HelpMenuOff();
        UnpauseTime();

    }

    public void HelpMenuOn()
    {
        if(helpPage != null)
        {
            if(helpPage.activeSelf == true)
            {
                helpPage.SetActive(false);
            }
            else
            {
                helpPage.SetActive(true);
            }
        }
    }

    public void HelpMenuOff()
    {
        if(helpPage != null)
        {
            helpPage.SetActive(false);
        }
    }

    public void ToggleMenuOn()
    {
        if(menuButtonHolder != null)
        {
            menuButtonHolder.SetActive(true);
        }
    }

    public void ToggleMenuOff()
    {
        if (menuButtonHolder != null)
        {
            menuButtonHolder.SetActive(false);
            //HelpMenuOff();

        }
    }

    public void OptionsToggleOn()
    {
        if (optionsPageHolder != null)
        {
            optionsPageHolder.SetActive(true);
            HelpMenuOff();


        }
    }
    public void OptionsToggleOff()
    {
        if (optionsPageHolder != null)
        {
            optionsPageHolder.SetActive(false);
        }
    }

    public void OptionsMusicToggle()
    {
        musicOn = !musicOn;
        musicPlayer.SetActive(musicOn);
    }

    public void OptionsSFXToggle()
    {
        sfxOn = !sfxOn;
    }

    public void PauseTime()
    {
        Time.timeScale = 0f;
    }

    public void UnpauseTime()
    {
        Time.timeScale = 1f;
    }

    public void QuickRaceButton()
    {
        ToggleMenuOff();
        OptionsToggleOff();
        HelpMenuOff();
        UnpauseTime();

        raceManager.careerMode = false;

        sceneController.LoadScene("TestSimple");

    }

    public void CareerModeButton()
    {
        ToggleMenuOff();
        OptionsToggleOff();
        HelpMenuOff();



        raceManager.careerMode = true;

        raceManager.ReinitializeSheetList();

        dataPersistenceManager.LoadGame();


        careerMenuHolder.SetActive(true);


        if (!dataPersistenceManager.HasGameData())
        {
            continueCareerButton.interactable = false;
        }




        //raceManager.characterSheet = careerSheet;

    }

    public void NewCareerButton()
    {
        ToggleMenuOff();
        UnpauseTime();

        careerMenuHolder.SetActive(false);

        //dataPersistenceManager.NewGame();

        //sceneController.LoadScene("TestSimple");

        saveSlotsMenu.ActivateMenu(false);
        //this.DeactivateMenu();

        //ToggleMenuOff

    }

    public void OnLoadGameClicked()
    {
        careerMenuHolder.SetActive(false);
        saveSlotsMenu.ActivateMenu(true);

    }

    public void ContinueCareerButton()
    {
        ToggleMenuOff();
        UnpauseTime();

        dataPersistenceManager.LoadGame();

        OpenCareerPage();

    }

    public void CareerRaceButton()
    {
        ToggleMenuOff();
        HelpMenuOff();

        UnpauseTime();

        careerMenuHolder.SetActive(false);
        careerOptionsHolder.SetActive(false);

        sceneController.LoadScene("TestSimple");

    }

    public void BackToMainMenuButton()
    {
        ToggleMenuOn();
        savesMenuHolder.SetActive(false);
        careerMenuHolder.SetActive(false);
        careerOptionsHolder.SetActive(false);

    }

    public void BackToCareerMenu()
    {
        savesMenuHolder.SetActive(false);
        careerMenuHolder.SetActive(true);
    }

    public void OpenCareerPage()
    {
        //dataPersistenceManager.LoadGame();

        careerOptionsHolder.SetActive(true);
        careerMenuHolder.SetActive(false);

        //careerOptionsHolder.GetComponent<CareerOptionsPage>().SetData();
    }

    public void CareerPilotSelectionMenu()
    {
        careerMenuHolder.SetActive(false);

        pilotSelectionMenu.SetActive(true);


    }

    public void MainMenuButton()
    {
        ToggleMenuOn();
        PauseMenuOff();
        UnpauseTime();

        raceManager.ResetRaceManager();

        sceneController.LoadScene("MainMenu");
    }

    public void QuitGameButton()
    {
        UnpauseTime();

        Application.Quit();
    }
}
