using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShuoScripts;
using System;
using System.IO;

public class VoiceHandler : MonoBehaviour {

    public NetworkModule networkModule;
    public AudioSource audioSource;
	// Use this for initialization
	void Start ()
    {
        networkModule.Output += (b) => 
        {
            if (b == null || b.Length == 0)
                return;


            //float[] voice = wav.LeftChannel;
            //AudioClip clip = AudioClip.Create("voice", voice.Length, 1, 44100, false);
            //clip.SetData(voice, 0);
            audioSource.clip = WavUtility.ToAudioClip(b);
            audioSource.Play();

            //File.WriteAllBytes(Application.streamingAssetsPath+"/voice.wav", b);
        };
    }

    private float[] ConvertByteToFloat(byte[] array)
    {
        float[] floatArr = new float[array.Length / 4];
        for (int i = 0; i < floatArr.Length; i++)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(array, i * 4, 4);
            floatArr[i] = BitConverter.ToSingle(array, i * 4) / 0x80000000;
        }
        return floatArr;
    }

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            //audioSource.Play();
            AudioSource.PlayClipAtPoint(audioSource.clip, GetComponent<InteractHandler>().girlHead.position, 1.0f);
        }
	}
}
