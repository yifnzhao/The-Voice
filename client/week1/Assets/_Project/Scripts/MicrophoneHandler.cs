using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class MicrophoneHandler : MonoBehaviour {

    public GameObject bgLight;
    public GameObject indicator;
    //public ShuoScripts.NetworkModule network;
    public AldenNet.AldenNet aldenNet;
    public MicSampler micSampler;
    public TalkIndicator talkIndicator;

    // Use this for initialization
    void Start () {
        SwitchOff();

    }
    bool enterSwitchOn = false;
    bool enterSwitchOff = false;
    public void SwitchOn()
    {
        if (enterSwitchOn)
        {
            Enter_SwitchOn();
            enterSwitchOn = false;
        }
        enterSwitchOff = true;

    }

    void Enter_SwitchOn()
    {
        // PlayerTalk
        //network.Send(BitConverter.GetBytes(1000));
        if(aldenNet.GetClient()!= null)
            aldenNet.GetClient().Send(BitConverter.GetBytes(1000));
        else
            Debug.LogError("No Server Connected");
        micSampler.StartSampling();

        talkIndicator.LightUp(1);

        // turn indicator green
        bgLight.GetComponent<Light>().color = Color.green;
        indicator.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.green);
        indicator.GetComponent<MeshRenderer>().material.SetColor("_MKGlowColor", Color.green);
        indicator.GetComponent<MeshRenderer>().material.SetColor("_MKGlowTexColor", Color.green);

        Invoke("SendThinkText", 0.1f); 
    }

    void Enter_SwitchOff()
    {
        bgLight.GetComponent<Light>().color = Color.red;
        indicator.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
        indicator.GetComponent<MeshRenderer>().material.SetColor("_MKGlowColor", Color.red);
        indicator.GetComponent<MeshRenderer>().material.SetColor("_MKGlowTexColor", Color.red);
    }

    public void SwitchOff()
    {
        if (enterSwitchOff)
        {
            Enter_SwitchOff();
            enterSwitchOff = false;
        }
        enterSwitchOn = true;
    }

    public void EndTalk()
    {
        bgLight.GetComponent<Light>().color = Color.red;
        indicator.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
        indicator.GetComponent<MeshRenderer>().material.SetColor("_MKGlowColor", Color.red);
        indicator.GetComponent<MeshRenderer>().material.SetColor("_MKGlowTexColor", Color.red);
    }

    // Update is called once per frame
    void Update () {
		
	}


    void SendThinkText()
    {
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
        //network.Send(senttt);
        if (aldenNet.GetClient() != null)
            aldenNet.GetClient().Send(senttt);
        else
            Debug.LogError("No Server Connected");
    }
}
