using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class BillboardHandler : MonoBehaviour {

    public Text text;
    public Image image;

	// Use this for initialization
	void Start ()
    {
        // set default state
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);

        // start
        InvokeRepeating("IntervalChange", 0, 3f);
    }

    void IntervalChange()
    {
        //Debug.Log("IntervalChange, text.color.a=" + text.color.a + " image.color.a=" + image.color.a);

        if (text.color.a < 0.5f)
            StartCoroutine(ChangeAlpha_Text(false));
        else
            StartCoroutine(ChangeAlpha_Text(true));

        if (image.color.a < 0.5f)
            StartCoroutine(ChangeAlpha_Image(false));
        else
            StartCoroutine(ChangeAlpha_Image(true));
    }
    const float speed = 0.1f;
    IEnumerator ChangeAlpha_Text(bool _down)
    {
        while (true)
        {
            yield return null;

            float a = text.color.a;
            if (_down)
                a -= speed;
            else
                a += speed;
            a = Mathf.Clamp01(a);
            if (_down)
            {
                if (a < 0.1f)
                {
                    text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
                    break;
                }
            }
            else
            {
                if (a > 0.9f)
                {
                    text.color = new Color(text.color.r, text.color.g, text.color.b, 1f);
                    break;
                }
            }
            text.color = new Color(text.color.r, text.color.g, text.color.b, a);

        }
    }

    IEnumerator ChangeAlpha_Image(bool _down)
    {
        while (true)
        {
            yield return null;

            float a = image.color.a;
            if (_down)
                a -= speed;
            else
                a += speed;
            a = Mathf.Clamp01(a);
            if (_down)
            {
                if (a < 0.1f)
                {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
                    break;
                }
            }
            else
            {
                if (a > 0.9f)
                {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
                    break;
                }
            }
            image.color = new Color(image.color.r, image.color.g, image.color.b, a);
        }
    }

    // Update is called once per frame
    void Update ()
    {
		
	}
}
