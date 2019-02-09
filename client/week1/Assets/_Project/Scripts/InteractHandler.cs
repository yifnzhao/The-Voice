using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractHandler : MonoBehaviour {

    /// <summary>
    /// to calculate distance from player
    /// </summary>
    public Transform girlHead;

    /// <summary>
    /// girl hand
    /// </summary>
    public Transform girlHand;

    /// <summary>
    /// girl look at
    /// </summary>
    public Transform aimTarget;

    /// <summary>
    /// girl hand target
    /// </summary>
    public Transform handTarget;

    public Transform microphone;

    public Transform playerHead;
    public Transform playerHand;

    public float headThreshold = 10f;
    public float handThreshold = 0.2f;

    public float headSpeed = 2f;
    public float handSpeed = 2f;

    Vector3 headOriPos;
    Vector3 handOriPos;

    public float mcActiveDistance = 0.25f;

    public EyeControl eyeContorl;

    // Use this for initialization
    void Start ()
    {
        headOriPos = aimTarget.position;
        handOriPos = handTarget.position;

        aimTarget.GetComponent<MeshRenderer>().enabled = false;
        handTarget.GetComponent<MeshRenderer>().enabled = false;

    }
	
	// Update is called once per frame
	void Update ()
    {
        // head
        float headDis = Vector3.Distance(playerHead.position, girlHead.position);
        if (headDis < headThreshold)
        {
            aimTarget.position = Vector3.Lerp(aimTarget.position, playerHead.position, Time.deltaTime * headSpeed);

            // girl gaze on player
            if (headDis > eyeContorl.gazeTooCloseDistance)
            {
                eyeContorl.eyeMove = EyeControl.EyeMove.GazeOnPlayer;
                eyeContorl.eyeGazeTarget = playerHead.gameObject;
            }
        }
        else
        {
            aimTarget.position = Vector3.Lerp(aimTarget.position, headOriPos, Time.deltaTime * headSpeed);
            eyeContorl.eyeMove = EyeControl.EyeMove.Moving;
        }


        // hand
        float handDis = Vector3.Distance(playerHand.position, girlHand.position);
        if (handDis < handThreshold)
        {
            handTarget.position = Vector3.Lerp(handTarget.position, playerHand.position, Time.deltaTime * handSpeed);
        }
        else
        {
            handTarget.position = Vector3.Lerp(handTarget.position, handOriPos, Time.deltaTime * handSpeed);
        }

        // microphone
        float mcDis = Vector3.Distance(playerHead.position, microphone.position);
        if (mcDis < mcActiveDistance)
        {
            microphone.GetComponent<MicrophoneHandler>().SwitchOn();
        }
        else
        {
            microphone.GetComponent<MicrophoneHandler>().SwitchOff();
        }
    }
}
