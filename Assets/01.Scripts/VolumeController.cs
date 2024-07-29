using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    public Slider bgmVolumeSlider;
    public Slider sfxVolumeSlider;

    private void Start()
    {
        bgmVolumeSlider.value = SoundManager.instance.bgmSource.volume;
        sfxVolumeSlider.value = SoundManager.instance.sfxSources[0].volume;
        
        bgmVolumeSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    private void SetBGMVolume(float volume)
    {
        SoundManager.instance.SetBGMVolume(volume);
    }

    private void SetSFXVolume(float volume)
    {
        SoundManager.instance.SetSFXVolume(volume);
    }
}