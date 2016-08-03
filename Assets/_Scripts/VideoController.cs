using UnityEngine;
using System.Collections;
using UnityEngine.UI;

	public class VideoController : MonoBehaviour {

		public static VideoController instance;
		[SerializeField] private GameObject[] videos;

		[SerializeField] private float pausedVideoZ = 10;
		[SerializeField] private float playingVideoZ = 0;

		[SerializeField] private bool seekToStart = true;

		private int loadedVideoCount = 0;

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

	void Awake(){
		instance = this;
	}

	void Start(){
	}
		
		void Update () {

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

		void startPlayingVideos(){
			setCurrentVideo (0);
		}

		public void OnVideoLoaded(){
			loadedVideoCount ++;
			if (loadedVideoCount >= videos.Length) {
				Debug.Log ("All videos loaded. We can start playing videos now.");
				startPlayingVideos();
			}
		}
}
