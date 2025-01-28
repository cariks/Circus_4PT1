using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectsScript : MonoBehaviour
{
    public AudioClip[] soundEffects;
    public AudioSource audioSource;

   public void Hover()
    {
        audioSource.PlayOneShot(soundEffects[0]);
    }

    public void Clicked()
    {
        audioSource.PlayOneShot(soundEffects[1]);
    }

    public void OnDice()
    {
        audioSource.loop = true;
        audioSource.clip = soundEffects[2];
        audioSource.Play();
    }
}
