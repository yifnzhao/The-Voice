using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShuoScripts;
using System;
using System.IO;
using System.Text;

public class VoiceHandler : MonoBehaviour {

    public NetworkModule networkModule;
    public AudioSource audioSource;
    public AudioVisualization audioVis;
    public EmotionHandler emotionHandler;
    public MicrophoneHandler micHandler;

    // Use this for initialization
    void Start ()
    {
        if (audioVis == null)
        {
            Debug.LogError("No AudioVisualization");
            return;
        }

        audioVis.audioSource = audioSource;

        networkModule.Output += (b) => 
        {
            if (b == null || b.Length == 0)
                return;

            // read command
            MemoryStream ms = new MemoryStream(b);
            byte[] cmdB = new byte[sizeof(int)];
            ms.Read(cmdB, 0, cmdB.Length);
            int cmd = BitConverter.ToInt32(cmdB, 0);
            if (cmd == 1)
            {
                micHandler.EndTalk();

                byte[] respLen = new byte[sizeof(int)];
                ms.Read(respLen, 0, respLen.Length);
                byte[] respByte = new byte[BitConverter.ToInt32(respLen, 0)];
                ms.Read(respByte, 0, BitConverter.ToInt32(respLen, 0));
                byte[] emoByte = new byte[sizeof(int)];
                ms.Read(emoByte, 0, emoByte.Length);
                int emotion = BitConverter.ToInt32(emoByte, 0);
                byte[] confByte = new byte[sizeof(int)];
                ms.Read(confByte, 0, confByte.Length);
                int confidence = BitConverter.ToInt32(confByte, 0);

                // display subtitle
                Debug.Log("Girl: " + Encoding.UTF8.GetString(respByte) + " Emotion:" + emotion + " Confidence:" + confidence);

                // perform emotion
                emotionHandler.ChangeState((EmotionHandler.Emotion)emotion, confidence);
                //emotionHandler.ChangeState((EmotionHandler.Emotion.Smile), 100);

                // play audio
                byte[] voiceByte = new byte[ms.Length - ms.Position];
                ms.Read(voiceByte, 0, voiceByte.Length);
                audioSource.clip = WavUtility.ToAudioClip(voiceByte);
                audioSource.Play();
                audioVis.clip = audioSource.clip;
            }
            else
                Debug.LogError("Unknow Command:" + cmd);


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
