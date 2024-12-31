using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
public class OptionsScript : MonoBehaviour
{
    public Slider masterVol, musicVol;
    public AudioMixer mainAudioMixer;

    public void ChangeMasterVol()
    {
        mainAudioMixer.SetFloat("MasterVol", masterVol.value);
    }  public void ChangeMusicVol()
    {
        mainAudioMixer.SetFloat("MusicVol", musicVol.value);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
