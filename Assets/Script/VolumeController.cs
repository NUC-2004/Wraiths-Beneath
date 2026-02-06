using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class VolumeController : MonoBehaviour
{
    public Slider volumeSlider;

    void Start()
    {
        // 游戏启动时，让滑块处于当前系统的实际音量位置
        volumeSlider.value = AudioListener.volume;
        
        // 监听滑动条数值变化
        volumeSlider.onValueChanged.AddListener(SetGlobalVolume);
    }

    public void SetGlobalVolume(float value)
    {
        // AudioListener.volume 控制的是 Unity 引擎排出的所有声音的总开关
        AudioListener.volume = value;
    }
}
