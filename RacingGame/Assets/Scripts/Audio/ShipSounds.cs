using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script is attached to racer ships, and contains references to audio clips to be played at certain events:
//shooting sounds, getting hit sounds, jump sounds, destruction sounds

public class ShipSounds : MonoBehaviour
{
    AudioSource audioSource;
    //public AudioClip explosion;
    public AudioClip laser;
    public AudioClip gunshot;
    public AudioClip hit;
    public AudioClip launchMissile;

    public float volume;

    public SceneController sc;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        sc = SceneController.sceneControllerInstance;
    }

    //public void PlayExplosionSound()
    //{
    //    audioSource.PlayOneShot(explosion, 1f);
    //}

    public void PlayLaserSound()
    {
        if (sc.gameObject.GetComponentInChildren<MainMenuScript>().sfxOn)
        {
            audioSource.PlayOneShot(laser, volume);
        }
    }

    public void PlayGunSound()
    {
        if (sc.gameObject.GetComponentInChildren<MainMenuScript>().sfxOn)
        {
            audioSource.PlayOneShot(gunshot, volume);
        }
    }

    public void PlayHitSound()
    {
        if (sc.gameObject.GetComponentInChildren<MainMenuScript>().sfxOn)
        {
            audioSource.PlayOneShot(hit, volume);
        }

    }

    public void PlayMissileSound()
    {
        if (sc.gameObject.GetComponentInChildren<MainMenuScript>().sfxOn)
        {
            audioSource.PlayOneShot(launchMissile, volume /2f);
        }
    }


}
