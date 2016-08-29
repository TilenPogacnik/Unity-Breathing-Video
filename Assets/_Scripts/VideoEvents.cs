using UnityEngine;
using System.Collections;

public class VideoEvents : MonoBehaviour {
	
	public static bool debugMessages = true;
	
	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
	}
	
	
	/************************/
	/* VIDEO EVENTS */
	/************************/
	
	public delegate void VideoAction();
	public static event VideoAction onVideoEnded;

	public static void TriggerOnVideoEnded(){
		if (debugMessages) {
			Debug.Log ("Event triggered: " + System.Reflection.MethodBase.GetCurrentMethod().Name);
		}
		
		if (onVideoEnded != null) {
			onVideoEnded();
		}
	}	
}
