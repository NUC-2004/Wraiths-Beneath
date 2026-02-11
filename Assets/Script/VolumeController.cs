using UnityEngine;
using UnityEngine.Audio; // 必须引用这个命名空间
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    public AudioMixer myMixer; // 将刚才创建的 Mixer 拖进来
    public Slider volumeSlider; // 将 Slider 拖进来

    void Start()
    {
        // 初始化滑块的值（建议滑块 Min 设为 0.0001，Max 设为 1）
        // 如果你有保存过音量，可以在这里读取 PlayerPrefs
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float sliderValue)
    {
        // 关键点：音量通常是对数级别的，直接设置 0-1 效果不好
        // 我们需要将 0-1 的线性值转为 -80 到 20 的分贝值
        float dbValue = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20;
        
        myMixer.SetFloat("MyExposedVolume", dbValue);
    }
}
