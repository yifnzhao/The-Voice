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
    public Transform girlHandLeft;
    public Transform girlHandRight;

    /// <summary>
    /// girl look at
    /// </summary>
    public Transform aimTarget;

    /// <summary>
    /// girl hand target
    /// </summary>
    public Transform handTargetLeft;
    public Transform handTargetRight;

    public Transform microphone;

    public Transform playerHead;
    public Transform playerHandLeft;
    public Transform playerHandRight;

    public float headThreshold = 10f;
    public float handThreshold = 0.2f;

    public float headSpeed = 2f;
    public float handSpeed = 2f;

    Vector3 headOriPos;
    Vector3 handOriPosLeft;
    Vector3 handOriPosRight;

    public float mcActiveDistance = 0.25f;

    public EyeControl eyeContorl;

    // Use this for initialization
    void Start ()
    {
        headOriPos = aimTarget.position;
        handOriPosLeft = handTargetLeft.position;
        handOriPosRight = handTargetRight.position;

        aimTarget.GetComponent<MeshRenderer>().enabled = false;
        handTargetLeft.GetComponent<MeshRenderer>().enabled = false;
        handTargetRight.GetComponent<MeshRenderer>().enabled = false;

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
        float handDis = Vector3.Distance(playerHandLeft.position, girlHandLeft.position);
        if (handDis < handThreshold)
        {
            handTargetLeft.position = Vector3.Lerp(handTargetLeft.position, playerHandLeft.position, Time.deltaTime * handSpeed);
        }
        else
        {
            handTargetLeft.position = Vector3.Lerp(handTargetLeft.position, handOriPosLeft, Time.deltaTime * handSpeed);
        }

        handDis = Vector3.Distance(playerHandLeft.position, girlHandRight.position);
        if (handDis < handThreshold)
        {
            handTargetRight.position = Vector3.Lerp(handTargetRight.position, playerHandLeft.position, Time.deltaTime * handSpeed);
        }
        else
        {
            handTargetRight.position = Vector3.Lerp(handTargetRight.position, handOriPosRight, Time.deltaTime * handSpeed);
        }

        handDis = Vector3.Distance(playerHandRight.position, girlHandLeft.position);
        if (handDis < handThreshold)
        {
            handTargetLeft.position = Vector3.Lerp(handTargetLeft.position, playerHandRight.position, Time.deltaTime * handSpeed);
        }
        else
        {
            handTargetLeft.position = Vector3.Lerp(handTargetLeft.position, handOriPosLeft, Time.deltaTime * handSpeed);
        }

        handDis = Vector3.Distance(playerHandRight.position, girlHandRight.position);
        if (handDis < handThreshold)
        {
            handTargetRight.position = Vector3.Lerp(handTargetRight.position, playerHandRight.position, Time.deltaTime * handSpeed);
        }
        else
        {
            handTargetRight.position = Vector3.Lerp(handTargetRight.position, handOriPosRight, Time.deltaTime * handSpeed);
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
