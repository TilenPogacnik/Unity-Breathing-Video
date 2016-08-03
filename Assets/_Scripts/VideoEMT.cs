using UnityEngine;
using System.Collections;

[RequireComponent(typeof (MediaPlayerCtrl))]
public class VideoEMT : MonoBehaviour {

	private MediaPlayerCtrl videoPlayer;
	[HideInInspector]
	public bool isLoaded = false;
	
	private bool pendingPlay = false;
	
	void Awake () {
		videoPlayer = this.GetComponent<MediaPlayerCtrl> ();
		if (videoPlayer == null) {
			Debug.LogError("Could not find MPMP VideoPlayer attached to this GameObject");
		}
	}
	void Start(){
		videoPlayer.OnVideoFirstFrameReady += OnVideoLoaded;
	}

	[ContextMenu("Play Video")]
	public void PlayVideo(){
		Debug.Log ("Trying to play video.");
		if (isLoaded) {
			videoPlayer.Play ();
			pendingPlay = false;
		} else {
			pendingPlay = true;
		}
	}
	
	[ContextMenu("Pause Video")]
	public void PauseVideo(){
		videoPlayer.Pause ();
	}
	
	void SeekToStart(){
		if (isLoaded) {
			videoPlayer.SeekTo(0);
			//videoPlayer.Stop ();
			//videoPlayer.Play ();
		} else {
			Debug.LogError("Can't seek to start if video is not loaded. Offending object: " + gameObject.name);
		} 
	}

	void OnVideoLoaded(){
		videoPlayer.Stop ();
		Debug.Log (gameObject.name + " loaded.");
		isLoaded = true;
		VideoController.instance.OnVideoLoaded ();
		if (pendingPlay) {
			PlayVideo();
		}
	}
}