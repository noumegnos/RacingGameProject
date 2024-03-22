using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICanvas : MonoBehaviour
{

    public TextMeshProUGUI speedText;
    public TextMeshProUGUI fuelText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI positionText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int speed = (int)GetComponentInParent<ShipController>().currentSpeed;
        int fuel = (int)GetComponentInParent<ShipController>().fuel;
        speedText.text = "Speed: " + speed.ToString();
        fuelText.text = "Fuel: " + fuel.ToString();

        healthText.text = GetComponentInParent<DamageManager>().hitPoints.ToString() + "/" + GetComponentInParent<DamageManager>().maxHitPoints;

        float shotsLeft = GetComponentInParent<ShipController>().ammo / GetComponentInParent<ShipController>().ammoConsumption;
        float totalShots = GetComponentInParent<ShipController>().maxAmmo / GetComponentInParent<ShipController>().ammoConsumption;

        ammoText.text = "Ammo: " + shotsLeft.ToString() + "/" + totalShots.ToString();

        int position = GetComponentInParent<ShipController>().myPositionInRace + 1;

        positionText.text = "Position: " + position;
    }
}
