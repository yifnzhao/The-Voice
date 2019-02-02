using UnityEngine;
using System.Collections;

public class MapChanger : MonoBehaviour {

	public Texture2D[] texes = new Texture2D[5];
	private int selected = 0;
	private int oldselected = 0;
	public GUISkin customSkin;
	public Material targetmat;
	public Texture[] mattexes = new Texture[4];

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI () {

		GUI.skin = customSkin;
		//int sw = Screen.width;
		int sh = Screen.height;
		float bw = (float)sh / 7.5f;
		if (bw > 256f) bw = 256f;

		for (int i = 0; i < 4; i++) {
			if (GUI.Button(new Rect(bw*0.75f, bw + (bw*1.5f) * i, bw, bw), texes[i+1])) selected = i; ;
			if (i == selected) GUI.DrawTexture(new Rect(bw*0.75f - 0.125f*bw, bw + (bw*1.5f) * i - 0.125f*bw, bw*1.25f, bw*1.25f), texes[0]);
		}

		if (selected != oldselected) {
			targetmat.SetTexture ("_BoobMap", mattexes [selected]);
			oldselected = selected;
		}

	}


}
