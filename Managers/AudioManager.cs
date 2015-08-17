using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour 
{
    public AudioSource musicPlayer;
    public AudioSource effectPlayer;

    public AudioClip menuClickClip;
    public AudioClip missionCompleteClip;

    public AudioClip coinCollectedClip;
    public AudioClip explosionClip;

    public AudioClip powerupCollectedClip;
    public AudioClip powerupUsedClip;
    public AudioClip reviveClip;

    public bool audioEnabled;

    static AudioManager instance;
    public static AudioManager Instance { get { return instance; } }

	// Use this for initialization
	void Start () 
    {
        instance = this;
		
        if (SaveManager.audioEnabled == 1)
        {
            audioEnabled = true;

            if (musicPlayer)
                musicPlayer.Play();
        }
        else
        {
            audioEnabled = false;
        }
	}

    public void ChangeAudioState()
    {
        if (audioEnabled)
        {
            audioEnabled = false;
            SaveManager.audioEnabled = 0;

            if (musicPlayer)
                musicPlayer.Stop();

            if (effectPlayer)
                effectPlayer.Stop();
        }
        else
        {
            audioEnabled = true;
            SaveManager.audioEnabled = 1;

            if (musicPlayer)
                musicPlayer.Play();
        }

        SaveManager.SaveData();
    }

    public void PlayMenuClick()
    {
        if (menuClickClip && audioEnabled)
        {
            effectPlayer.clip = menuClickClip;
            effectPlayer.Play();
        }
    }
    public void PlayMissionComplete()
    {
        if (missionCompleteClip && audioEnabled)
        {
            effectPlayer.clip = missionCompleteClip;
            effectPlayer.Play();
        }
    }
    public void PlayCoinCollected()
    {
        if (coinCollectedClip && audioEnabled)
        {
            effectPlayer.clip = coinCollectedClip;
            effectPlayer.Play();
        }
    }
    public void PlayExplosion()
    {
        if (explosionClip && audioEnabled)
        {
            effectPlayer.clip = explosionClip;
            effectPlayer.Play();
        }
    }
    public void PlayPowerupCollected()
    {
        if (powerupCollectedClip && audioEnabled)
        {
            effectPlayer.clip = powerupCollectedClip;
            effectPlayer.Play();
        }
    }
    public void PlayPowerupUsed()
    {
        if (powerupUsedClip && audioEnabled)
        {
            effectPlayer.clip = powerupUsedClip;
            effectPlayer.Play();
        }
    }
    public void PlayRevive()
    {
        if (reviveClip && audioEnabled)
        {
            effectPlayer.clip = reviveClip;
            effectPlayer.Play();
        }
    }
}
