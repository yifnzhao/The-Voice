using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneHandler : MonoBehaviour {

    public GameObject bgLight;
    public GameObject indicator;

	// Use this for initialization
	void Start () {
        SwitchOff();

    }

    public void SwitchOn()
    {
        bgLight.GetComponent<Light>().color = Color.green;
        indicator.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.green);
        indicator.GetComponent<MeshRenderer>().material.SetColor("_MKGlowColor", Color.green);
        indicator.GetComponent<MeshRenderer>().material.SetColor("_MKGlowTexColor", Color.green);

    }

    public void SwitchOff()
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
