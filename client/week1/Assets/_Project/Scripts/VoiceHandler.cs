using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShuoScripts;
using System;
using System.IO;
using System.Text;

public class VoiceHandler : MonoBehaviour {

    //public NetworkModule networkModule;
    public AldenNet.AldenNet aldenNet;
    public AudioSource audioSource;
    public AudioVisualization audioVis;
    public EmotionHandler emotionHandler;
    public MicrophoneHandler micHandler;
    public TextMesh subtitle;
    public MicSampler micSampler;
    public TalkIndicator talkIndicator;
    public TalkContentUI talkContent;
    public AnimationHandler animationHandler;
    string lastSentance;
    float lastSentenceTime;
    // Use this for initialization
    void Start ()
    {
        subtitle.text = "";

        if (audioVis == null)
        {
            Debug.LogError("No AudioVisualization");
            return;
        }

        audioVis.audioSource = audioSource;

        //networkModule.Output += (b) => 
        aldenNet.GetClient().Output+= (b) => 
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

                if (audioSource.isPlaying)
                {
                    //ms.Close();
                    //return;
                    audioSource.Stop();
                }

                talkIndicator.LightUp(5);

                byte[] respLen = new byte[sizeof(int)];
                ms.Read(respLen, 0, respLen.Length);
                byte[] respByte = new byte[BitConverter.ToInt32(respLen, 0)];
                ms.Read(respByte, 0, BitConverter.ToInt32(respLen, 0));
                byte[] emoByte = new byte[sizeof(int)];
                ms.Read(emoByte, 0, emoByte.Length);
                int emotion = BitConverter.ToInt32(emoByte, 0);
                byte[] confByte = new byte[sizeof(int)];
                ms.Read(confByte, 0, confByte.Length);
                float confidence = BitConverter.ToSingle(confByte, 0);

                // display subtitle
                string text = Encoding.UTF8.GetString(respByte);
                if (text == lastSentance && Time.time - lastSentenceTime < 5f)
                {
                    Debug.Log("Same response in 5s");
                    //return;
                }
                Debug.Log("Girl: " + text + " Emotion:" + emotion + " Confidence:" + confidence);
                talkContent.Add("Yifan Said: " + text, "EC00FF");
                subtitle.text = text;
                Invoke("CleanSubtitle", 5f);
                lastSentance = text;
                lastSentenceTime = Time.time;
                // perform emotion
                emotionHandler.ChangeState((EmotionHandler.Emotion)emotion, confidence);
                //emotionHandler.ChangeState((EmotionHandler.Emotion.Smile), 100);

                // play audio
                byte[] voiceByte = new byte[ms.Length - ms.Position];
                ms.Read(voiceByte, 0, voiceByte.Length);

                //string filePath = Application.streamingAssetsPath + "/voice.wav";
                //File.WriteAllBytes(filePath, voiceByte);
                //audioSource.clip = WavUtility.ToAudioClip(filePath);
                audioSource.clip = WavUtility.ToAudioClip(voiceByte);
                audioSource.Play();
                audioVis.clip = audioSource.clip;

                animationHandler.ChangeState(AnimationHandler.Anim.Talk);
                Invoke("ChangeToIdle", audioSource.clip.length);
            }
            else if (cmd == 2)  // request mic pitch
            {
                byte[] size = new byte[sizeof(int)];
                ms.Read(size, 0, sizeof(int));
                int ss = BitConverter.ToInt32(size, 0);
                byte[] str = new byte[ss];
                ms.Read(str, 0, ss);
                string recogStr = Encoding.UTF8.GetString(str);
                Debug.Log("Player:" + recogStr);
                talkContent.Add("You Said: " + recogStr, "008FFF");

                talkIndicator.LightUp(3);
                micSampler.EndSampling();
                float pitch = micSampler.GetHighestPitch();
                MemoryStream sendms = new MemoryStream();
                sendms.Write(BitConverter.GetBytes(1001), 0, sizeof(Int32));
                sendms.Write(BitConverter.GetBytes(pitch), 0, sizeof(float));

                byte[] sent = new byte[sendms.Length];
                sendms.Seek(0, SeekOrigin.Begin);
                sendms.Read(sent, 0, (int)sendms.Length);
                //networkModule.Send(sent);
                aldenNet.GetClient().Send(sent);

                //Invoke("SendThinkText", 0.1f);
            }
            else if (cmd == AldenNet.PredefinedMsg.HEARBEAT_ToClient || cmd == AldenNet.PredefinedMsg.HEARBEAT_ToServer)
            { }
            else
                Debug.LogError("Unknow Command:" + cmd);

            ms.Close();

        };
    }

    void ChangeToIdle()
    {
        animationHandler.ChangeState(AnimationHandler.Anim.Idle);
    }

    void SendThinkText()
    {
        return;
        // send thinking text
        string text = PredefinedTalkHandler.GetThinkingText();
        MemoryStream sendmss = new MemoryStream();
        sendmss.Write(BitConverter.GetBytes(1002), 0, sizeof(Int32));
        sendmss.Write(BitConverter.GetBytes(text.Length), 0, sizeof(Int32));
        sendmss.Write(Encoding.UTF8.GetBytes(text), 0, text.Length);
        EmotionHandler.Emotion emotion = EmotionHandler.Emotion.Smile;
        sendmss.Write(BitConverter.GetBytes((int)emotion), 0, sizeof(Int32));
        float confidence = 100;
        sendmss.Write(BitConverter.GetBytes(confidence), 0, sizeof(float));

        byte[] senttt = new byte[sendmss.Length];
        sendmss.Seek(0, SeekOrigin.Begin);
        sendmss.Read(senttt, 0, (int)sendmss.Length);
        //networkModule.Send(senttt);
        aldenNet.GetClient().Send(senttt);
    }

    void CleanSubtitle()
    {
        subtitle.text = "";
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
