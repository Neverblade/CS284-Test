using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Recorder : MonoBehaviour {

    public Text text;
    public float maxTime;
    public int frameRate;
    public string folder;

	// Use this for initialization
	void Start () {
        Time.captureFramerate = frameRate;
    }
	
	// Update is called once per frame
	void Update () {
        if (Time.time >= maxTime) {
            Application.Quit();
            UnityEditor.EditorApplication.isPlaying = false;
        }


        text.text = "" + Time.time;
        string name = string.Format("{0}/{1:D04} shot.png", folder, Time.frameCount);
        Application.CaptureScreenshot(name);
    }
}
