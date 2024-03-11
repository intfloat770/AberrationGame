using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class SoundData
{
    public string soundID;
    [HideInInspector] public GameObject gameObject;
    [HideInInspector] public AudioSource audioSource;
    public float localVolume;
    public float basePitch;
    public float randomPitch;
    public AudioClip clip;

    //public bool useRandomClip;
    //public AudioClip[] clips;
}

public class AudioManager : MonoBehaviour
{
    static AudioManager instance;

    [SerializeField] float globalVolume;
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
                //if (sound.useRandomClip)
                //{
                //    sound.audioSource.clip = sound.clips[Random.Range(0, sound.clips.Length)];
                //}
                
                sound.audioSource.volume = sound.localVolume * instance.globalVolume;
                sound.audioSource.pitch = sound.basePitch + Random.Range(-sound.randomPitch, sound.randomPitch);
                sound.audioSource.Play();
                return;
            }
        }
        Debug.LogError($"Unable to player sound with ID '{soundID}'!");
    }
}
