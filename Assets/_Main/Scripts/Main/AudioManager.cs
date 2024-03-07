using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SoundData
{
    public string soundID;
    [HideInInspector] public GameObject gameObject;
    [HideInInspector] public AudioSource audioSource;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    static AudioManager instance;

    [SerializeField] SoundData[] soundData;

    public void Init()
    {
        instance = this;

        foreach (SoundData sound in soundData)
        {
            sound.gameObject = new GameObject(sound.soundID);
            sound.gameObject.transform.parent = transform;
            sound.audioSource = sound.gameObject.AddComponent<AudioSource>();
            sound.audioSource.clip = sound.clip;
        }
    }

    public static void PlaySound(string soundID)
    {
        foreach (SoundData sound in instance.soundData)
        {
            if (sound.soundID.Equals(soundID))
            {
                sound.audioSource.Play();
            }
        }
    }
}
