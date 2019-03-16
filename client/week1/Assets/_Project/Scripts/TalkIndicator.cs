using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkIndicator : MonoBehaviour {

    public GameObject[] indicators;

	// Use this for initialization
	void Start ()
    {
        Reset();
    }

    public void Reset()
    {
        for (int i = 0; i < indicators.Length; i++)
        {
            GetMat(indicators[i]).color = Color.red;
        }
    }

    Material GetMat(GameObject _go)
    {
        return _go.transform.Find("Mesh").GetComponent<Renderer>().material;
    }

    public void LightUp(int _amount)
    {
        int amount = Mathf.Clamp(_amount, 1, indicators.Length);

        Reset();

        StartCoroutine(LightUpDelay(_amount));
    }

    IEnumerator LightUpDelay(int _amount)
    {
        for (int i = 0; i < _amount; i++)
        {
            GetMat(indicators[i]).color = Color.green;
            yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 0.3f));
            if (i == indicators.Length - 1)
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 2f));
                Reset();
            }
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        //if (Input.GetKeyUp(KeyCode.Alpha1))
        //    LightUp(1);
        //if (Input.GetKeyUp(KeyCode.Alpha2))
        //    LightUp(2);
        //if (Input.GetKeyUp(KeyCode.Alpha3))
        //    LightUp(3);
        //if (Input.GetKeyUp(KeyCode.Alpha4))
        //    LightUp(4);
        //if (Input.GetKeyUp(KeyCode.Alpha5))
        //    LightUp(5);
        //if (Input.GetKeyUp(KeyCode.Alpha6))
        //    LightUp(6);
    }
}
