using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public Slider musicSlider, sfxSlider;


    public static AudioManager Instance;

    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;
    public bool isMusciPlaying;
    public int currentMusicIndex = 0;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        PlayMusic("Music01");
        musicSlider.value = .15f;
        sfxSlider.value = .5f;
    }

    public void PlayScoreMusicIfNotPlaying()
    {
        if (musicSource.isPlaying && musicSource.clip.name == "Tetris Theme (8-Bit Version)")
        {
            Debug.Log($"{musicSource.clip.name} is already playing");
        }
        else
        {            
            PlayMusic("Music01");
        }
    }
    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }
    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            sfxSource.PlayOneShot(s.clip);
        }
    }

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }
    public void ToggleSFX()
    {
        sfxSource.mute = !sfxSource.mute;
    }
    public void SetMusicVolume()
    {
        musicSource.volume = musicSlider.value;
    }
    public void SetSFXVolume()
    {
        sfxSource.volume = sfxSlider.value;
    }
}
