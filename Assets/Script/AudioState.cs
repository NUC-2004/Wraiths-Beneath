using UnityEngine;

public static class AudioState
{
    public static void ResumeGlobalAudio()
    {
        AudioListener.pause = false;
        AudioListener.volume = 1f;
    }
}
