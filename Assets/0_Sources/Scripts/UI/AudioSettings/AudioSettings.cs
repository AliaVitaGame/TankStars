using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using YG;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [Space]
    [SerializeField] private string nameMusicVolumeExpose = "Music";
    [SerializeField] private string nameSfxVolumeExpose = "SFX";
    [SerializeField] private AudioMixer audioMixer;

    private void OnEnable() => YG2.onGetSDKData += LoadSettings;
    private void OnDisable() => YG2.onGetSDKData -= LoadSettings;

    private void Start()
    {
        if(YG2.isSDKEnabled)
         LoadSettings();
    }

    public void SetMusicVolume(float volume)
    {
        PlayerDataManager.Instance.SetMusicVolume(volume);
        SetMixerVolume(musicVolumeSlider, nameMusicVolumeExpose, volume);
    }

    public void SetSfxVolume(float volume)
    {
        PlayerDataManager.Instance.SetSfxVolume(volume);
        SetMixerVolume(sfxVolumeSlider, nameSfxVolumeExpose, volume);
    }

    private void LoadSettings()
    {
        var musicVolume = PlayerDataManager.Instance.GetMusicVolume();
        var sfxVolume = PlayerDataManager.Instance.GetSfxVolume();

        SetMusicVolume(musicVolume);
        SetSfxVolume(sfxVolume);
    }

    private void SetMixerVolume(Slider slider, string name, float volume)
    {
        audioMixer.SetFloat(name, volume);
        slider.value = volume;
        PlayerDataManager.Instance.Save();
    }
}
