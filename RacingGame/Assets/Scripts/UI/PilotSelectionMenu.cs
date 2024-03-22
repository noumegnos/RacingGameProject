using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PilotSelectionMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pilotName;
    [SerializeField] private TextMeshProUGUI pilotDescription;

    int pilotIndex;

    public RaceManager raceManager;
    public Transform mainMenu;

    public DataPersistenceManager dataPersistenceManager;

    public Image portrait;


    private void OnEnable()
    {
        pilotName.text = raceManager.Pilots[0].GetComponent<PilotStats>().pilotName;
        pilotDescription.text = raceManager.Pilots[0].GetComponent<PilotStats>().description;
        portrait.sprite = raceManager.Pilots[0].GetComponent<PilotStats>().pilotFace;

    }

    public void NextButton()
    {
        if(pilotIndex < raceManager.Pilots.Count -1)
        {
            pilotIndex++;
        }
        else
        {
            pilotIndex = 0;
        }

        pilotName.text = raceManager.Pilots[pilotIndex].GetComponent<PilotStats>().pilotName;
        pilotDescription.text = raceManager.Pilots[pilotIndex].GetComponent<PilotStats>().description;
        portrait.sprite = raceManager.Pilots[pilotIndex].GetComponent<PilotStats>().pilotFace;

    }

    public void PrevButton()
    {
        if (pilotIndex > 0)
        {
            pilotIndex--;
        }
        else
        {
            pilotIndex = raceManager.Pilots.Count -1;
        }
        pilotName.text = raceManager.Pilots[pilotIndex].GetComponent<PilotStats>().pilotName;
        pilotDescription.text = raceManager.Pilots[pilotIndex].GetComponent<PilotStats>().description;
        portrait.sprite = raceManager.Pilots[pilotIndex].GetComponent<PilotStats>().pilotFace;


    }

    public void SelectButton()
    {
        raceManager.characterSheets[9].GetComponent<CharacterSheet>().pilot = pilotIndex;

        int count = 0;

        for (int i = 0; i < raceManager.characterSheets.Count -1; i++)
        {
            if(i != pilotIndex)
            {
                raceManager.characterSheets[i].GetComponent<CharacterSheet>().pilot = i + count;

            }
            else
            {
                count = 1;
                raceManager.characterSheets[i].GetComponent<CharacterSheet>().pilot = i + count;

            }

            raceManager.characterSheets[i].GetComponent<CharacterSheet>().pilotEngine = raceManager.Pilots[(int)raceManager.characterSheets[i].GetComponent<CharacterSheet>().pilot].GetComponent<PilotStats>().pilotEngine;
            raceManager.characterSheets[i].GetComponent<CharacterSheet>().pilotWeapons = raceManager.Pilots[(int)raceManager.characterSheets[i].GetComponent<CharacterSheet>().pilot].GetComponent<PilotStats>().pilotWeapons;

        }

        dataPersistenceManager.SaveGame();
        dataPersistenceManager.LoadGame();

        this.gameObject.SetActive(false);

        mainMenu.GetComponent<MainMenuScript>().OpenCareerPage();
    }
}
