using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotionHandler : MonoBehaviour {

    public Transform mouthCornerLeft;
    public Transform mouthCornerRight;

    float natualZ = 0.05540104f;
    float smileZ = 0.04539991f;
    float sadZ = 0.05909988f;

    public enum Emotion
    {
        Natual=0,
        Smile,
        Sad,

        Total
    }
    public Emotion emotion = Emotion.Natual;

    Vector3 oriLeft;
    Vector3 oriRight;
    // Use this for initialization
    void Start ()
    {
        oriLeft = mouthCornerLeft.localPosition;
        oriRight = mouthCornerRight.localPosition;

        ChangeState(Emotion.Natual);
        //InvokeRepeating("PeriodlyChange", 1f, 1f);
	}

    void PeriodlyChange()
    {
        int next = (int)emotion + 1;
        if (next >= (int)Emotion.Total)
            next = 0;
        ChangeState((Emotion)next);
    }

    public void ChangeState(Emotion _emo, float _confidence=100)
    {
        emotion = _emo;
        switch (emotion)
        {
            case Emotion.Natual:
                {
                    targetLeft = new Vector3(oriLeft.x, oriLeft.y, natualZ);
                    targetRight = new Vector3(oriRight.x, oriRight.y, natualZ);
                }
                break;
            case Emotion.Smile:
                {
                    targetLeft = new Vector3(oriLeft.x, oriLeft.y, natualZ - (natualZ - smileZ) * (_confidence / 100f));
                    targetRight = new Vector3(oriRight.x, oriRight.y, natualZ - (natualZ - smileZ) * (_confidence / 100f));
                }
                break;
            case Emotion.Sad:
                {
                    targetLeft = new Vector3(oriLeft.x, oriLeft.y, natualZ + (natualZ - sadZ) * (_confidence / 100f));
                    targetRight = new Vector3(oriRight.x, oriRight.y, natualZ + (natualZ - sadZ) * (_confidence / 100f));
                }
                break;
        }
        
    }
    Vector3 targetLeft;
    Vector3 targetRight;

    // Update is called once per frame
    void Update ()
    {
        mouthCornerLeft.localPosition = Vector3.Lerp(mouthCornerLeft.localPosition, targetLeft, Time.deltaTime* 2f);
        mouthCornerRight.localPosition = Vector3.Lerp(mouthCornerRight.localPosition, targetRight, Time.deltaTime * 2f);


    }


}
