using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField] private string profileId = "";

    [Header("Content")]
    [SerializeField] private GameObject noDataContent;
    [SerializeField] private GameObject hasDataContent;

    [SerializeField] private TextMeshProUGUI pilotText;
    [SerializeField] private TextMeshProUGUI cashText;
    [SerializeField] private TextMeshProUGUI winsText;

    private Button saveSlotButton;

    public bool hasData { get; private set; } = false;

    private void Awake()
    {
        saveSlotButton = GetComponent<Button>();
    }

    public void SetData(GameData data)
    {
        if(data == null)
        {
            hasData = false;
            noDataContent.SetActive(true);
            hasDataContent.SetActive(false);
        }
        else
        {
            hasData = true;
            hasDataContent.SetActive(true);
            noDataContent.SetActive(false);

            pilotText.text = GameObject.FindObjectOfType<RaceManager>().Pilots[(int)data.sheets[data.sheets.Count - 4]].GetComponent<PilotStats>().pilotName;
                //data.sheets[data.sheets.Count-4].ToString();
            cashText.text = "Cash " + data.sheets[data.sheets.Count-23].ToString();
            winsText.text = "Wins " + data.sheets[data.sheets.Count-25].ToString();
        }
    }

    public string GetProfileId()
    { 
        return this.profileId; 
    }

    public void SetInteractable(bool interactable)
    {
        saveSlotButton.interactable = interactable;
    }

}
