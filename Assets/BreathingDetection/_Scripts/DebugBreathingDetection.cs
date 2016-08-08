using UnityEngine;
using System.Collections;

public class DebugBreathingDetection : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.B)) {
			BreathingEvents.TriggerOnExhale ();
		} 
		if (Input.GetKeyUp (KeyCode.B)) {
			BreathingEvents.TriggerOnInhale ();
		}
	}
}
