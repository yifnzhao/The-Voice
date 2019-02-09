using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JawHandler : MonoBehaviour {

    public Transform jawBone;
    public float speed = 10f;

    float naturalPos = 0.0536f;
    float openPos = 0.064f;
    Vector3 oriPos;
    Vector3 targetPos;
    
    // Use this for initialization
    void Start ()
    {
        oriPos = jawBone.localPosition;
        targetPos = oriPos;
    }
	
	// Update is called once per frame
	void Update ()
    {
        Update_Jaw(AudioVisualization.volume);
    }

    /// <summary>
    /// animate mouse open
    /// </summary>
    /// <param name="_precent">0-1</param>
    void Update_Jaw(float _precent)
    {
        if (jawBone == null)
            return;

        _precent = Mathf.Clamp(_precent, 0f, 1f);
        //Debug.Log("+++ Update_Jaw:_present:" + _precent);
        float curZ = naturalPos + (openPos - naturalPos) * _precent;
        targetPos.x = oriPos.x;
        targetPos.y = oriPos.y;
        targetPos.z = curZ;
        jawBone.localPosition = Vector3.Lerp(jawBone.localPosition, targetPos, Time.deltaTime * speed);

    }
}
