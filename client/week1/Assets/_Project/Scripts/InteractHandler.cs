using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
    float handThreshold = 0.2f;

    public float headSpeed = 2f;
    float handSpeed = 2f;

    Vector3 headOriPos;
    Vector3 handOriPosLeft;
    Vector3 handOriPosRight;

    public float mcActiveDistance = 0.25f;

    public EyeControl eyeContorl;

    public Animator animator;

    public float closeRange = 2f;
    public float mediumRange = 5f;
    public float farRange = 10f;

    public float walkSpeed = 1f;

    public AnimationHandler animHandler;

    public float turnSpeed = 200f;

    //public ShuoScripts.NetworkModule networkModule;

    public AldenNet.AldenNet aldenNet;

    // Use this for initialization
    void Start ()
    {
        headOriPos = aimTarget.position;
        handOriPosLeft = handTargetLeft.position;
        handOriPosRight = handTargetRight.position;

        aimTarget.GetComponent<MeshRenderer>().enabled = false;
        handTargetLeft.GetComponent<MeshRenderer>().enabled = false;
        handTargetRight.GetComponent<MeshRenderer>().enabled = false;

        Invoke("Greeeting", 5f);
    }

    void Greeeting()
    {
        string text = PredefinedTalkHandler.GetGreeting();
        MemoryStream sendms = new MemoryStream();
        sendms.Write(BitConverter.GetBytes(1002), 0, sizeof(Int32));
        sendms.Write(BitConverter.GetBytes(text.Length), 0, sizeof(Int32));
        sendms.Write(Encoding.UTF8.GetBytes(text), 0, text.Length);
        EmotionHandler.Emotion emotion = EmotionHandler.Emotion.Smile;
        sendms.Write(BitConverter.GetBytes((int)emotion), 0, sizeof(Int32));
        float confidence = 100;
        sendms.Write(BitConverter.GetBytes(confidence), 0, sizeof(float));

        byte[] sent = new byte[sendms.Length];
        sendms.Seek(0, SeekOrigin.Begin);
        sendms.Read(sent, 0, (int)sendms.Length);
        //networkModule.Send(sent);
        if(aldenNet.GetClient() != null)
            aldenNet.GetClient().Send(sent);
    }

    bool enterCloseRange = false;
    bool enterMediumRange = false;
    bool enterFarRange = false;
    void Enter_CloseRange()
    {
        if (!enterCloseRange)
        {
            //Debug.Log("Enter_CloseRange");
            //animator.SetBool("shakehand", true);
            //animator.SetBool("stand", false);
            //animator.SetBool("sit", false);

            animHandler.ChangeState(AnimationHandler.Anim.No);

            string text = PredefinedTalkHandler.GetTooClose();
            MemoryStream sendms = new MemoryStream();
            sendms.Write(BitConverter.GetBytes(1002), 0, sizeof(Int32));
            sendms.Write(BitConverter.GetBytes(text.Length), 0, sizeof(Int32));
            sendms.Write(Encoding.UTF8.GetBytes(text), 0, text.Length);
            EmotionHandler.Emotion emotion = EmotionHandler.Emotion.Sad;
            sendms.Write(BitConverter.GetBytes((int)emotion), 0, sizeof(Int32));
            float confidence = 100;
            sendms.Write(BitConverter.GetBytes(confidence), 0, sizeof(float));

            byte[] sent = new byte[sendms.Length];
            sendms.Seek(0, SeekOrigin.Begin);
            sendms.Read(sent, 0, (int)sendms.Length);
            //networkModule.Send(sent);
            if(aldenNet.GetClient() != null)
                aldenNet.GetClient().Send(sent);

            enterCloseRange = true;
        }
    }
    void Enter_MediumRange()
    {
        if (!enterMediumRange)
        {
            //Debug.Log("Enter_MediumRange");
            //animator.SetBool("stand", true);
            //animator.SetBool("shakehand", false);
            //animator.SetBool("sit", false);
            animHandler.ChangeState(AnimationHandler.Anim.Idle);
            enterMediumRange = true;
        }
    }

    void Enter_FarRange()
    {
        if (!enterFarRange)
        {
            //Debug.Log("Enter_FarRange");
            //animator.SetBool("sit", true); 
            //animator.SetBool("shakehand", false);
            //animator.SetBool("stand", false);
            animHandler.ChangeState(AnimationHandler.Anim.Walk);

            enterFarRange = true;
        }
    }


    Vector3 playerHeadLastPos;
    Vector3 girlHeadLastPos;
    void Update_Range()
    {
        float playerSpeed = Vector3.Distance(playerHeadLastPos, playerHead.position);
        float girlSpeed = Vector3.Distance(girlHeadLastPos, girlHead.position);
        //Debug.Log((playerSpeed - girlSpeed) > 0 ? "player faster, speed:"+playerSpeed:"girl faster speed:"+ girlSpeed);
        bool girlWalkToPlayer = true;
        if (playerSpeed > girlSpeed)
            girlWalkToPlayer = false;

        float range = Vector3.Distance(playerHead.position, girlHead.position);
        //Debug.Log("range:" + range);
        if (girlWalkToPlayer)
        {
            if (range < closeRange)
            {
                Enter_CloseRange();
                Update_Walkback();
                enterMediumRange = false;
                enterFarRange = false;
            }
            else if (range >= closeRange && range < mediumRange)
            {
                Enter_MediumRange();
                enterCloseRange = false;
                enterFarRange = false;
            }
            else if (range >= mediumRange && range < farRange)
            {
                Enter_FarRange();
                enterCloseRange = false;
                enterMediumRange = false;

                Update_Follow();
            }
            else { }
        }
        else
        {
            float tolerance = 0.1f;
            if (range < closeRange - tolerance)
            {
                Enter_CloseRange();
                Update_Walkback();
                enterMediumRange = false;
                enterFarRange = false;
            }
            else if (range >= closeRange- tolerance && range < mediumRange- tolerance)
            {
                Enter_MediumRange();
                enterCloseRange = false;
                enterFarRange = false;
            }
            else if (range >= mediumRange- tolerance && range < farRange- tolerance)
            {
                Enter_FarRange();
                enterCloseRange = false;
                enterMediumRange = false;

                Update_Follow();
            }
            else { }
        }


        playerHeadLastPos = playerHead.position;
        girlHeadLastPos = girlHead.position;
    }

    void Update_Walkback()
    {
        GameObject yifan = animator.gameObject;
        Vector3 from = new Vector3(yifan.transform.position.x, 0, yifan.transform.position.z);
        Vector3 to = new Vector3(playerHead.position.x, 0, playerHead.position.z);

        float dis = Vector3.Distance(from, to);
        if (dis > closeRange)
        {
            return;
        }

        yifan.transform.position = Vector3.Lerp(from, from - to + (from - to).normalized, Time.deltaTime * walkSpeed);

        Update_Turn();
    }

    void Update_Follow()
    {
        GameObject yifan = animator.gameObject;
        Vector3 from = new Vector3(yifan.transform.position.x, 0, yifan.transform.position.z);
        Vector3 to = new Vector3(playerHead.position.x, 0, playerHead.position.z);

        Vector3 dir = to - from;
        //Vector3 dir = yifan.transform.forward;
        //Debug.DrawRay(from, dir);
        yifan.transform.Translate(dir.normalized * Time.deltaTime * walkSpeed, Space.World);

        Update_Turn();
    }

    void Update_TurnThenFollow()
    {
        GameObject yifan = animator.gameObject;
        Vector3 from = new Vector3(yifan.transform.position.x, 0, yifan.transform.position.z);
        Vector3 to = new Vector3(playerHead.position.x, 0, playerHead.position.z);

        float angle = Vector3.Angle(yifan.transform.forward, to - from);
        Debug.Log("Update_TurnThenFollow angle:" + angle);
        Vector3 normal = Vector3.Cross(yifan.transform.forward, to - from);
        if (normal.y > 0 && angle > 5)
            yifan.transform.Rotate(0, Time.deltaTime * turnSpeed, 0);
        else if (normal.y < 0 && angle > 10)
            yifan.transform.Rotate(0, -Time.deltaTime * turnSpeed, 0);
        else
            Update_Follow();
    }

    void Update_Turn()
    {
        GameObject yifan = animator.gameObject;
        Vector3 from = new Vector3(yifan.transform.position.x, 0, yifan.transform.position.z);
        Vector3 to = new Vector3(playerHead.position.x, 0, playerHead.position.z);

        float angle = Vector3.Angle(yifan.transform.forward, to - from);
        Vector3 normal = Vector3.Cross(yifan.transform.forward, to - from);
        if (normal.y > 0 && angle > 5)
            yifan.transform.Rotate(0, Time.deltaTime * turnSpeed, 0);
        else if(normal.y < 0 && angle > 10)
            yifan.transform.Rotate(0, -Time.deltaTime * turnSpeed, 0);

    }

    // Update is called once per frame
    void Update ()
    {
        Update_Range();
        //Update_Follow();
        

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
