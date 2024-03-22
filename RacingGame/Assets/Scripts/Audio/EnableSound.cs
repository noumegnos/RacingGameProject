using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableSound : MonoBehaviour
{
    public SceneController sc;

    public AudioSource audioSource;

    // Start is called before the first frame update
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        sc = SceneController.sceneControllerInstance;

        audioSource.enabled = sc.GetComponentInChildren<MainMenuScript>().sfxOn;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
