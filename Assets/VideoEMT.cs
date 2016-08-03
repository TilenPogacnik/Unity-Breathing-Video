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
	
	void Update () {

	}

	void LoadVideo(){

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
		videoPlayer.Stop ();
		videoPlayer.Play ();//SeekTo (0);
	}

	void OnVideoLoaded(){
		Debug.Log (gameObject.name + " loaded.");
		isLoaded = true;

		if (pendingPlay) {
			PlayVideo();
		}
	}
}