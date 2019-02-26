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
        Debug.Log((playerSpeed - girlSpeed) > 0 ? "player faster, speed:"+playerSpeed:"girl faster speed:"+ girlSpeed);
        bool girlWalkToPlayer = true;
        if (playerSpeed > girlSpeed)
            girlWalkToPlayer = false;

        float range = Vector3.Distance(playerHead.position, girlHead.position);
        Debug.Log("range:" + range);
        if (girlWalkToPlayer)
        {
            if (range < closeRange)
            {
                Enter_CloseRange();
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

    void Update_Follow()
    {
        GameObject yifan = animator.gameObject;
        Vector3 from = new Vector3(yifan.transform.position.x, 0, yifan.transform.position.z);
        Vector3 to = new Vector3(playerHead.position.x, 0, playerHead.position.z);

        //float dis = Vector3.Distance(from, to);
        //if (dis > mediumRange)
        {
            yifan.transform.position = Vector3.Lerp(from, to, Time.deltaTime * walkSpeed);
            //animHandler.ChangeState(AnimationHandler.Anim.Walk);
        }
        //else
            //animHandler.ChangeState(AnimationHandler.Anim.Idle);

        float angle = Vector3.Angle(yifan.transform.forward, to - from);
        if (angle > 5)
        {
            // check normal direction to determine trun diretion
            Vector3 normal = Vector3.Cross(yifan.transform.forward, to - from);
            if(normal.y > 0)
                yifan.transform.Rotate(0, Time.deltaTime * turnSpeed, 0);
            else
                yifan.transform.Rotate(0, -Time.deltaTime * turnSpeed, 0);
        }
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
