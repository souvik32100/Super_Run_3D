using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager sharedInstance = null;

    public AudioSource sfxAuidoSource;
    [SerializeField] private AudioSource backgroundAudioSource;

    //=========================================
    public List<AudioClip> AttackSFX;
    public List<AudioClip> SharkBiteSFX;
    public List<AudioClip> PanicSFX;
    public List<AudioClip> femalePanicSFX;
    public AudioClip menubgm;
    public AudioClip bg;
    public AudioClip LevelUpSFX;
    public AudioClip DeathSFX;
    public AudioClip CoinFlyFX;
    public AudioClip SharkUpgrade;
    public AudioClip SharkEvolve;
    public AudioClip TapButton;
    public AudioClip PlayButton;
    public AudioClip ItemSelected;
    public AudioClip buyProduct;
    public AudioClip SpeedUpSound;
    public AudioClip ProductBuyButton;
    public AudioClip CountdownSFX;
    public AudioClip VictorySFX;
    public AudioClip DefeatSFX;
    public AudioClip rewardProgress;
    public AudioClip rewardProgressDone;
    public AudioClip iapChestOpen;
    public AudioClip btnTapAdd;
    public AudioClip btnVipTap;
    //=========================================

    [Header("Police Chase")]
    [Space]

    public GameObject sirenSFX;
    public GameObject engineSFX;

    public bool soundOn;
    public List<AudioClip> hitCarSFX;
    public List<AudioClip> CrushCarSFX;
    public List<AudioClip> enemyCarDownSFX;
    public List<AudioClip> brakePushSFX;
    public List<AudioClip> laneChangeSFX;
    public AudioClip tireBlastSFX;
    public AudioClip brakeSFX;
    public AudioClip hurtSFX;
    public AudioClip coinPickupSFX;
    public AudioClip healthPickupSFX;
    public AudioClip playBtnSFX;
    public AudioClip hitSFX;
    public AudioClip engineStartSFX;
    public AudioClip heliEngineSFX;
    public AudioClip scanSFX;
    public AudioClip missionCompleteSFX;
    public AudioClip missionFailedSFX;
    public List<AudioClip> thiefLaughing;


    [Header("Gun Assemble SFX")]
    public AudioClip briefcaseOpenSFX;
    public AudioClip pickupSFX; 
    public AudioClip dropWrongSFX;
    public AudioClip dropRightSFX;

    [Header("Scanning SFX")]
    public AudioClip searchSFX;
    public AudioClip fingerScanSFX;
    public AudioClip fingerScanLastSFX;
    public AudioClip matchFoundSFX;

    [HideInInspector] public Slider volumeSlider;
    [HideInInspector] public Slider soundEffectVolumeSlider;
    [Header("Car Shop SFX")]
    public List<AudioClip> carEngineShopSFX;

    public static SoundManager SharedManager()
    {
        return sharedInstance;
    }

    private void Awake()
    {
        if (sharedInstance == null)
        {
            sharedInstance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    void Start()
    {
        soundOn = true;
        sfxAuidoSource = GetComponent<AudioSource>();
        //backgroundAudioSource =  GetComponent<AudioSource>();

        if(backgroundAudioSource != null)
            PlayMainMenuAudio();

        if (sirenSFX !=null && engineSFX !=null)
        {
            sirenSFX.SetActive(false);
            engineSFX.SetActive(false);

        }
    }

    private void Update()
    {
        //if (Input.GetMouseButtonDown(1))
        //{
        //    sfxAuidoSource.PlayOneShot(LevelUpSFX);

        //}
    }

    public void PlaySFX(AudioClip audioClip)
    {
        sfxAuidoSource.PlayOneShot(audioClip);
    }
    public void PlayBiteSound(AudioClip audioClip)
    {
        if (!sfxAuidoSource.isPlaying)
        {
            sfxAuidoSource.clip = audioClip;
            sfxAuidoSource.Play();
        }
    }

    public AudioClip GetRandomHitCarSFX()
    {
        return hitCarSFX[Random.Range(0, hitCarSFX.Count)];
    }
    public AudioClip GetRandomCrushCarSFX()
    {
        return CrushCarSFX[Random.Range(0, CrushCarSFX.Count)];
    }
    public AudioClip GetRandomEnemyDownSFX()
    {
        return enemyCarDownSFX[Random.Range(0, enemyCarDownSFX.Count)];
    }
    public AudioClip GetRandomBrakeSFX()
    {
        return brakePushSFX[Random.Range(0, brakePushSFX.Count)];
    }
    public AudioClip GetRandomLaneTurnSFX()
    {
        return laneChangeSFX[Random.Range(0, laneChangeSFX.Count)];
    }
    public AudioClip GetRandomThiefLaughSFX()
    {
        return thiefLaughing[Random.Range(0, thiefLaughing.Count)];
    }


    //===============================================================
    public void PlayMainMenuAudio()
    {
        backgroundAudioSource.clip = menubgm;
        backgroundAudioSource.Play();
    }

    public void PlayBackGroundAudio()
    {
        backgroundAudioSource.clip = bg;
        backgroundAudioSource.Play();
    }

    public void StopBackGroundAudio()
    {
        backgroundAudioSource.clip = menubgm;
        backgroundAudioSource.Stop();
    }

    public AudioClip GetRandomAttackSFX()
    {
        return AttackSFX[Random.Range(0, AttackSFX.Count)];
    }

    public AudioClip GetRandomSharkBiteSFX()
    {
        return SharkBiteSFX[Random.Range(0, SharkBiteSFX.Count)];
    }

    public AudioClip GetRandomPanicSFX()
    {
        return PanicSFX[Random.Range(0, PanicSFX.Count)];
    }
    public AudioClip GetRandomFemalePanicSFX()
    {
        return femalePanicSFX[Random.Range(0, femalePanicSFX.Count)];
    }



    public void PlayPlayerDeathSFX()
    {
        sfxAuidoSource.PlayOneShot(DeathSFX);
    }

    public void PlayVictorySFX()
    {
        sfxAuidoSource.PlayOneShot(VictorySFX);
    }

    public void PlayDefeatSFX()
    {
        sfxAuidoSource.PlayOneShot(DefeatSFX);
    }
    public void PlayBriefcaseOpenSFX()
    {
        sfxAuidoSource.PlayOneShot(briefcaseOpenSFX);
    }

    public void PlayDragSFX()
    {
        sfxAuidoSource.PlayOneShot(pickupSFX);
    }

    public void PlayWrongDropSFX()
    {
        sfxAuidoSource.PlayOneShot(dropWrongSFX);
    }

    public void PlayRightDropSFX()
    {
        sfxAuidoSource.PlayOneShot(dropRightSFX);
    }

    //For Scanning
    public void PlaySearchSFX()
    {
        sfxAuidoSource.PlayOneShot(searchSFX);
    }

    public void PlayFingerScanSFX()
    {
        sfxAuidoSource.PlayOneShot(fingerScanSFX);
    }

    public void PlayFingerScanLastSFX()
    {
        sfxAuidoSource.PlayOneShot(fingerScanLastSFX);
    }

    public void PlayMatchFoundSFX()
    {
        sfxAuidoSource.PlayOneShot(matchFoundSFX);
    }

    //public void SetVolume()
    //{
    //    backgroundAudioSource.volume = volumeSlider.value;
    //    PlayerPrefs.SetFloat("volume", backgroundAudioSource.volume);
    //}

    //public void SetSoundEffectVolume()
    //{
    //    sfxAuidoSource.volume = soundEffectVolumeSlider.value;
    //    PlayerPrefs.SetFloat("soundEffectVolume", sfxAuidoSource.volume);
    //}
}
