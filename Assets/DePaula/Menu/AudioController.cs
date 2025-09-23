using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    [SerializeField] Slider volumeSlider;

    private void Start()
    {
        LoadVolume();
    }

    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
        SaveVolume();
    }

    public void LoadVolume()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("GlobalVolume", 1f);
    }

    public void SaveVolume()
    {
        PlayerPrefs.SetFloat("GlobalVolume", volumeSlider.value);
    }
}
