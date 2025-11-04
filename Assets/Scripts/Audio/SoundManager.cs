using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource;

    [Header("UI Sounds")]
    public AudioClip buttonClickSound;
    [Range(0f, 1f)] public float buttonClickVolume = 1f;

    [Header("Gameplay Sounds")]
    public AudioClip shootSound;
    [Range(0f, 1f)] public float shootVolume = 1f;

    public AudioClip enemyHurtSound;
    [Range(0f, 1f)] public float enemyHurtVolume = 1f;

    public AudioClip playerHurtSound;
    [Range(0f, 1f)] public float playerHurtVolume = 1f;

    public AudioClip explosionSound;
    [Range(0f, 1f)] public float explosionVolume = 1f;

    public AudioClip levelClearSound;
    [Range(0f, 1f)] public float levelClearVolume = 1f;

    public AudioClip winSound;
    [Range(0f, 1f)] public float winVolume = 1f;

    public AudioClip loseSound;
    [Range(0f, 1f)] public float loseVolume = 1f;

    public AudioClip healthItemPickupSound;
    [Range(0f, 1f)] public float healthItemPickupVolume = 1f;

    public AudioClip boosterItemPickupSound;
    [Range(0f, 1f)] public float boosterItemPickupVolume = 1f;

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

    public void PlayButtonClick()
    {
        PlaySound(buttonClickSound, buttonClickVolume);
    }

    public void PlayButtonClickTrimmed(float startTime, float duration)
    {
        if (buttonClickSound == null || sfxSource == null) return;

        AudioSource tempSource = gameObject.AddComponent<AudioSource>();
        tempSource.clip = buttonClickSound;

        tempSource.volume = buttonClickVolume * sfxSource.volume;
        tempSource.outputAudioMixerGroup = sfxSource.outputAudioMixerGroup;
        tempSource.spatialBlend = sfxSource.spatialBlend;
        tempSource.pitch = sfxSource.pitch;
        tempSource.panStereo = sfxSource.panStereo;

        tempSource.time = Mathf.Clamp(startTime, 0, buttonClickSound.length - 0.01f);
        tempSource.Play();

        Destroy(tempSource, duration);
    }

    private void PlaySound(AudioClip clip, float baseVolume = 1f)
    {
        if (clip != null && sfxSource != null)
            sfxSource.PlayOneShot(clip, baseVolume * sfxSource.volume);
    }

    public void PlayShoot() => PlaySound(shootSound, shootVolume);
    public void PlayEnemyHurt() => PlaySound(enemyHurtSound, enemyHurtVolume);
    public void PlayPlayerHurt() => PlaySound(playerHurtSound, playerHurtVolume);
    public void PlayExplosion() => PlaySound(explosionSound, explosionVolume);
    public void PlayLevelClear() => PlaySound(levelClearSound, levelClearVolume);
    public void PlayWin() => PlaySound(winSound, winVolume);
    public void PlayLose() => PlaySound(loseSound, loseVolume);
    public void PlayHealthItemPickup() => PlaySound(healthItemPickupSound, healthItemPickupVolume);
    public void PlayBoosterItemPickup() => PlaySound(boosterItemPickupSound, boosterItemPickupVolume);

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
            sfxSource.volume = Mathf.Clamp01(volume);
    }
}
