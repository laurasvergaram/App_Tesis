using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneTimeSound : MonoBehaviour
{
    public AudioSource sound;

    public void playSound()
    {
        sound.Play();
    }
}
