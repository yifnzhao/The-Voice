using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalkContentUI : MonoBehaviour {

    public Text text;
    const int MAX = 10;
    Queue<string> content = new Queue<string>();

	// Use this for initialization
	void Start ()
    {
	}

    public void Add(string _str, string _color)
    {
        try
        {
            if (content.Count >= MAX)
                content.Dequeue();

            string str = string.Format("<color=#{0}>{1}</color>", _color, _str);
            content.Enqueue(str);
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }


        // display
        text.text = "";
        foreach (string s in content)
        {
            text.text += s;
            text.text += '\n';
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        //if (Input.GetKeyUp(KeyCode.A))
        //{
        //    Add(Time.time.ToString());
        //}
	}
}
