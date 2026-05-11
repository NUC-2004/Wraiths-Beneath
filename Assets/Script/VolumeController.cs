using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    private const string VolumeParameter = "MyExposedVolume";

    public AudioMixer myMixer;
    public Slider volumeSlider;

    void Start()
    {
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    public void SetVolume(float sliderValue)
    {
        if (myMixer == null)
        {
            return;
        }

        float dbValue = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;
        myMixer.SetFloat(VolumeParameter, dbValue);
    }
}
