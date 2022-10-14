using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ButtonSounds : MonoBehaviour
{
    public AudioSource ButtonSource;

    // Start is called before the first frame update
    void Start()
    {
        ButtonSource = this.GetComponent<AudioSource>();  
    }

    public void PlaySound()
    {
        ButtonSource.PlayOneShot(ButtonSource.clip);
    }
}
