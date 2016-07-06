using UnityEngine;
using System.Collections;
using UnityEngine.UI;

	public class VideoController : MonoBehaviour {

		public GameObject[] videos;

		private float pausedVideoZ = 10;
		private float playingVideoZ = 0;

		public bool seekToStart = true;

		private bool firstVideo = true;
		private int currentVideo = -1;

		void OnEnable(){
			BreathingEvents.onExhale += handleOnExhale;
			BreathingEvents.onInhale += handleOnInhale;
		}

		void OnDisable(){
			BreathingEvents.onExhale -= handleOnExhale;
			BreathingEvents.onInhale -= handleOnInhale;
		}

		void handleOnInhale ()
		{
			setCurrentVideo (2);
		}
		
		void handleOnExhale ()
		{
			setCurrentVideo (1);
		}

	void Start(){
		setCurrentVideo (0);
	}
		
		void Update () {
			/*if (firstVideo) {
				setCurrentVideo(0);
			}
			if (isExhaling()) {
				setCurrentVideo (1);
				firstVideo = false;

			} else if (!firstVideo){
				setCurrentVideo (2);
			}*/

			if (Input.GetKeyDown (KeyCode.Alpha1)) {
				setCurrentVideo(0);
			} else if (Input.GetKeyDown (KeyCode.Alpha2)){
				setCurrentVideo(1);
			} else if (Input.GetKeyDown (KeyCode.Alpha3)) {
				setCurrentVideo(2);
			} 
		}

		void setCurrentVideo(int videoID){

			if (currentVideo != videoID) {
				for (int i = 0; i < videos.Length; i++) {
					sendVideoBack(i);
					pauseVideo(i);
				}
				sendVideoFront (videoID);
				playVideo (videoID);

				currentVideo = videoID;
			}

		}

		void playVideo(int videoID){
			Debug.Log ("Playing " + videos [videoID].gameObject.name);
			if (seekToStart) {
				videos[videoID].SendMessage("SeekToStart");
			}
			videos [videoID].SendMessage ("PlayVideo");

		}

		void pauseVideo(int videoID){
			Debug.Log ("Pausing " + videos [videoID].gameObject.name);
			videos [videoID].SendMessage ("PauseVideo");
		}


		void sendVideoBack(int videoID){
			videos[videoID].transform.position = new Vector3(videos[videoID].transform.position.x, videos[videoID].transform.position.y, pausedVideoZ); 
		}
		void sendVideoFront(int videoID){
			videos[videoID].transform.position = new Vector3(videos[videoID].transform.position.x, videos[videoID].transform.position.y, playingVideoZ); 
		}

		public void resetVideos(){
			setCurrentVideo (0);
			firstVideo = true;
			currentVideo = 0;
		}
}
