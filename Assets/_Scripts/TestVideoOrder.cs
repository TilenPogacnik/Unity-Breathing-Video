using UnityEngine;
using System.Collections;

public class TestVideoOrder : MonoBehaviour {
	public string InhaleVideo;
	public string ExhaleVideo;
	public string StartVideo;

	IEnumerator Start () {
		//Wait until all videos are loaded
		while (!NewVideoController.instance.allVideosLoaded) {
			yield return new WaitForEndOfFrame();
		}

		PlayStartVideo ();
	}

	void OnEnable(){
		BreathingEvents.onExhale += PlayExhaleVideo;
		BreathingEvents.onInhale += PlayInhaleVideo;
	}

	void OnDisable(){
		BreathingEvents.onExhale -= PlayExhaleVideo;
		BreathingEvents.onInhale -= PlayInhaleVideo;
	}

	void PlayExhaleVideo(){
		NewVideoController.instance.PlayVideo (ExhaleVideo, true);
	}

	void PlayInhaleVideo(){
		NewVideoController.instance.PlayVideo (InhaleVideo, true);
	}

	void PlayStartVideo(){
		NewVideoController.instance.PlayVideo (StartVideo, true);
	}
}
