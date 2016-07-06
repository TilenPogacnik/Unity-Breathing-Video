using UnityEngine;
using System.Collections;

namespace monoflow {
	[RequireComponent(typeof (MPMP))]
	public class Video : MonoBehaviour {

		private MPMP videoPlayer;
		[HideInInspector]
		public bool isLoaded = false;

		private bool pendingPlay = false;

		 void Awake () {
			videoPlayer = this.GetComponent<MPMP> ();
			if (videoPlayer == null) {
				Debug.LogError("Could not find MPMP VideoPlayer attached to this GameObject");
			}
			videoPlayer.OnLoaded += _OnLoaded;
			videoPlayer.OnError += _OnError;
			videoPlayer.Load ();
		}
		
		void Update () {
		
		}

		[ContextMenu("Play Video")]
		public void PlayVideo(){
			if (isLoaded && !videoPlayer.IsPlaying()) {
				videoPlayer.Play ();
			} else {
				pendingPlay = true;	
				//videoPlayer.Load();
					
			}
		}

		[ContextMenu("Pause Video")]
		public void PauseVideo(){
			Debug.Log ("Pause called");
			if (videoPlayer.IsPlaying()) {
				videoPlayer.Pause();
			}
		}

		void SeekToStart(){
			videoPlayer.SeekTo (0f );
		}

		void _OnError(MPMP mpmp){
			Debug.Log ("Error while loading " + this.gameObject.name + "! " + mpmp.ToString());
		}

		void _OnLoaded(MPMP mpmp){
			Debug.Log ("Video loaded");
			isLoaded = true;
			if (videoPlayer.IsPlaying ()) {
				videoPlayer.Pause ();
			}
			if (pendingPlay) {
				pendingPlay = false;
				PlayVideo ();
			}
		}
	}
}