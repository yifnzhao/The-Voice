using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MicrophoneHandler : MonoBehaviour {

    public GameObject bgLight;
    public GameObject indicator;
    public ShuoScripts.NetworkModule network;

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
        string str = "PlayerTalking";
        network.Send(Encoding.ASCII.GetBytes(str));

        // turn indicator green
        bgLight.GetComponent<Light>().color = Color.green;
        indicator.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.green);
        indicator.GetComponent<MeshRenderer>().material.SetColor("_MKGlowColor", Color.green);
        indicator.GetComponent<MeshRenderer>().material.SetColor("_MKGlowTexColor", Color.green);
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
}
