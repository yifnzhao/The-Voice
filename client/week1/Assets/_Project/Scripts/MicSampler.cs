using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShuoScripts;

public class MicSampler : MonoBehaviour {

    public AudioVisualization audioVisualization;
    public NetworkModule networkModule;

    float highestPitch;

    bool start = false;

	// Use this for initialization
	void Start ()
    {
        if (audioVisualization.mode != AudioVisualization.Mode.Mic)
        {
            Debug.LogError("AudioVisualization need Mic mode");
            return;
        }

	}
	
	// Update is called once per frame
	void Update ()
    {
        if (start)
        {
            if (highestPitch < audioVisualization.volume)
            {
                highestPitch = audioVisualization.volume;
            }
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            StartSampling();
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            EndSampling();
        }
	}

    public void StartSampling()
    {
        highestPitch = 0;
        start = true;
    }

    public void EndSampling()
    {
        start = false;
    }

    public float GetHighestPitch()
    {
        return highestPitch;
    }
}
