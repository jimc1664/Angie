using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RaycastFix : MonoBehaviour {


    GraphicRaycaster Gr;
    Camera Cam;
	// Use this for initialization
	void OnEnable () {
        Gr = GetComponent<GraphicRaycaster>();
        Cam = GetComponent<Canvas>().worldCamera;
	}
	
	// Update is called once per frame
	void Update () {
        Gr.enabled = Cam.enabled;
	}
}
