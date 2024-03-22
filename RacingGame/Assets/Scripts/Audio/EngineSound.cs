using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://www.gamedeveloper.com/audio/procedural-audio-in-unity

public class EngineSound: MonoBehaviour
{

    [Range(-1f, 1f)]
    public float offset;

    System.Random rand = new System.Random();
    AudioLowPassFilter lowPassFilter;

    public float engineIntensity;
    public float multiplier;

    public SceneController sc;


    void Awake()
    {
        sc = SceneController.sceneControllerInstance;

        lowPassFilter = GetComponent<AudioLowPassFilter>();
        Update();

        
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = (float)(rand.NextDouble() * 2.0 - 1.0 + offset);
        }
    }

    void Update()
    {
        if (sc.gameObject.GetComponentInChildren<MainMenuScript>().sfxOn)
        {
            GetComponent<AudioSource>().mute = false;

            engineIntensity = Mathf.Lerp(engineIntensity, GetComponentInParent<ShipController>().currentSpeed * multiplier, Time.deltaTime * 10f);

            lowPassFilter.cutoffFrequency = Mathf.Clamp(engineIntensity, 200f, 2000f);
        }
        else
        {
            GetComponent<AudioSource>().mute = true;

        }


    }
}
