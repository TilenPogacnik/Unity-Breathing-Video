using UnityEngine;
using System.Collections;

public class DebugBreathingDetection : MonoBehaviour {

#if UNITY_EDITOR
	void Update () {
		if (Input.GetKeyDown (KeyCode.B)) {
			BreathingEvents.TriggerOnExhale ();
		} 
		if (Input.GetKeyUp (KeyCode.B)) {
			BreathingEvents.TriggerOnInhale ();
		}
	}
#endif
}
