using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVisualization : MonoBehaviour
{
    public enum Mode
    {
        Mic,
        AudioClip
    }
    public AudioClip clip;
    public Mode mode = Mode.AudioClip;
    public AudioSource audioSource;

    public static float volume;
    private AudioClip micRecord;
    string device;
    const int CLIP_LENGTH = 128;
    float[] volumeData;

    void Start()
    {
        volumeData = new float[CLIP_LENGTH];
        if (mode == Mode.AudioClip)
        {
            if (audioSource != null)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
        }
        else if (mode == Mode.Mic)
        {
            device = Microphone.devices[0];     // get mic
            micRecord = Microphone.Start(device, true, 999, 44100);     // 44100 sample rate
        }
    }
    void Update()
    {
        volume = GetMaxVolume();
    }
    // handl received audio stream at every frame, range from 0 to 1
    float GetMaxVolume()
    {
        float maxVolume = 0f;
        if (audioSource == null || clip == null)
        {
            return 0;
        }
        if (mode == Mode.AudioClip)
        {
            int offset = (int)(audioSource.time * clip.frequency);
            if (offset < 0)
            {
                return 0;
            }
            if (clip != null)
            {
                clip.GetData(volumeData, offset);
            }
            else
                Debug.LogError("No clip");
        }
        else if (mode == Mode.Mic)
        {
            int offset = Microphone.GetPosition(device) - CLIP_LENGTH + 1;
            if (offset < 0)
            {
                return 0;
            }
            micRecord.GetData(volumeData, offset);
        }

        for (int i = 0; i < CLIP_LENGTH; i++)
        {
            float tempMax = volumeData[i]; // modify volume sensitive;
            if (maxVolume < tempMax)
            {
                maxVolume = tempMax;
            }
        }
        return maxVolume;
    }
}