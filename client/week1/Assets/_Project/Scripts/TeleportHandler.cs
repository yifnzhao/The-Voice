using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class TeleportHandler : MonoBehaviour {

    public VRTeleporter teleporter;

    public SteamVR_Action_Boolean grabPinch;
    public SteamVR_Input_Sources inputSource;

    // Use this for initialization
    void Start () {
        grabPinch.AddOnChangeListener((fromAction, fromSource, newState) => 
        {
            //Debug.Log("VRControllerOutput:" + newState);
            if (newState)   // pressed
            {
                teleporter.ToggleDisplay(true);
            }
            else
            {
                teleporter.Teleport();
                teleporter.ToggleDisplay(false);
            }
        }
        , inputSource);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
