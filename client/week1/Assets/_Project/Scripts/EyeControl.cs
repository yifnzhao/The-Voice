using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeControl : MonoBehaviour {

    public GameObject eyelidLeftUpper;
    public GameObject eyelidLeftLower;
    public GameObject eyelidRightUpper;
    public GameObject eyelidRightLower;

    public GameObject eyeballLeft;
    public GameObject eyeballRight;

    [HideInInspector]
    public GameObject eyeGazeTarget;

    public InteractHandler interactHandler;

    public enum EyeMove
    {
        Stop = 0,
        Moving,
        GazeOnPlayer,

        Total,
    }

    public enum EyelidMove
    {
        Stop = 0,
        Moving,

        Total,
    }

    public EyeMove eyeMove = EyeMove.Stop;
    public EyelidMove eyelidMove = EyelidMove.Stop;

    public float gazeTooCloseDistance = 0.2f;

    Quaternion curRotL, curRotR;
    Quaternion oriRotL, oriRotR;
    Quaternion targetRotL, targetRotR;

    // Use this for initialization
    void Start ()
    {
        curRotL = eyeballLeft.transform.localRotation;
        oriRotL = eyeballLeft.transform.localRotation;
        targetRotL = eyeballLeft.transform.localRotation;

        curRotR = eyeballRight.transform.localRotation;
        oriRotR = eyeballRight.transform.localRotation;
        targetRotR = eyeballRight.transform.localRotation;

        StartCoroutine(EyeState());
    }
    float randomEyeMove = 1f;
    int eyeballTempVar;
    IEnumerator EyeState()
    {
        while (true)
        {
            yield return new WaitForSeconds(randomEyeMove);

            if (eyeMove != EyeMove.GazeOnPlayer)
            {
                eyeballTempVar++;
                eyeMove = eyeballTempVar % 2 == 0 ? EyeMove.Moving : EyeMove.Stop;
            }

            eyelidMove = EyelidMove.Moving;
            randomEyeMove = UnityEngine.Random.Range(1f, 3f);
        }
    }

    // Update is called once per frame
    void Update ()
    {
        Update_Eyeball();
        Update_Eyelid();
    }
    bool enter = false;
    void Update_Eyeball()
    {
        switch (eyeMove)
        {
            case EyeMove.Moving:
                {
                    if (enter)
                    {
                        Enter_Eyeball();
                        enter = false;
                    }

                    eyeballLeft.transform.localRotation = Quaternion.Lerp(eyeballLeft.transform.localRotation, targetRotL, Time.deltaTime * 10f);
                    eyeballRight.transform.localRotation = Quaternion.Lerp(eyeballRight.transform.localRotation, targetRotR, Time.deltaTime * 10f);
                }
                break;
            case EyeMove.Stop:
                {
                    enter = true;
                }
                break;
            case EyeMove.GazeOnPlayer:
                {
                    if (Vector3.Distance(eyeGazeTarget.transform.position, interactHandler.girlHead.position) < gazeTooCloseDistance)
                    {
                        eyeMove = EyeMove.Moving;
                        break;
                    }
                    eyeballLeft.transform.localRotation = 
                        Quaternion.Lerp(eyeballLeft.transform.localRotation, oriRotL, Time.deltaTime * 10f);
                    eyeballRight.transform.localRotation = 
                        Quaternion.Lerp(eyeballRight.transform.localRotation, oriRotR, Time.deltaTime * 10f);

                    //Vector3 eyeDir = interactHandler.playerHead.position - eyeballLeft.transform.position;
                    //Quaternion leftEyeRot = Quaternion.LookRotation(eyeDir);
                    //Quaternion rightEyeRot = leftEyeRot * Quaternion.AngleAxis(10f, eyeballRight.transform.up);
                    //eyeballLeft.transform.rotation = leftEyeRot;
                    //eyeballRight.transform.rotation = rightEyeRot;


                    //Vector3 leftEyeGazeAt = headToHead - eyeballLeft.transform.position;
                    //eyeballLeft.transform.LookAt(leftEyeGazeAt);
                    //eyeballRight.transform.LookAt(eyeGazeTarget.transform);
                }
                break;
            default:
                break;
        }
    }

    float eyeballAngle = 15f;
    void Enter_Eyeball()
    {
        float xAngle = UnityEngine.Random.Range(-eyeballAngle, eyeballAngle);
        float yAngle = UnityEngine.Random.Range(-eyeballAngle, eyeballAngle);
        targetRotL = oriRotL * Quaternion.Euler(xAngle, yAngle, 0);
        targetRotR = oriRotR * Quaternion.Euler(xAngle, yAngle, 0);

        
    }

    float eyelidLeftUpperOpenZ = -0.03308579f;
    float eyelidLeftUpperCloseZ = -0.0256f;
    float eyelidLeftLowerOpenZ = -0.01538791f;
    float eyelidLeftLowerCloseZ = -0.0197f;

    float eyelidRightUpperOpenZ = -0.03267061f;
    float eyelidRightUpperCloseZ = -0.0251f;
    float eyelidRightLowerOpenZ = -0.01497275f;
    float eyelidRightLowerCloseZ = -0.0206f;

    float dir = 1f;
    void Update_Eyelid()
    {
        switch (eyelidMove)
        {
            case EyelidMove.Moving:
                {
                    float leftZ = eyelidLeftUpper.transform.localPosition.z;
                    float rightZ = eyelidRightUpper.transform.localPosition.z;
                    leftZ += Time.deltaTime * 0.1f * dir;
                    rightZ += Time.deltaTime * 0.1f * dir;
                    if (leftZ > eyelidLeftUpperCloseZ)
                    {
                        dir *= -1f;
                        leftZ = eyelidLeftUpperCloseZ;
                        rightZ = eyelidRightUpperCloseZ;
                    }
                    if (leftZ < eyelidLeftUpperOpenZ)
                    {
                        dir *= -1f;
                        eyelidMove = EyelidMove.Stop;
                        leftZ = eyelidLeftUpperOpenZ;
                        rightZ = eyelidRightUpperOpenZ;
                    }
                    //Debug.Log(leftZ);
                    eyelidLeftUpper.transform.localPosition = new Vector3(eyelidLeftUpper.transform.localPosition.x, eyelidLeftUpper.transform.localPosition.y, leftZ);
                    eyelidRightUpper.transform.localPosition = new Vector3(eyelidRightUpper.transform.localPosition.x, eyelidRightUpper.transform.localPosition.y, rightZ);

                }
                break;
            case EyelidMove.Stop:
                { }
                break;
        }
    }
}
