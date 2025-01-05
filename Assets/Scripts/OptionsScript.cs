using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
public class OptionsScript : MonoBehaviour
{
    public Slider masterVol, musicVol;
    public AudioMixer mainAudioMixer;

    // Adjusts the master volume using the value from the master volume slider
    public void ChangeMasterVol()
    {
        mainAudioMixer.SetFloat("MasterVol", masterVol.value);
    }

    // Adjusts the music volume using the value from the music volume slider
    public void ChangeMusicVol()
    {
        mainAudioMixer.SetFloat("MusicVol", musicVol.value);
    }
}
