using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class NewVideoController : MonoBehaviour {

	public class Video{
		public string videoName;
		public GameObject gameObject;
		public MediaPlayerCtrl mediaPlayerControl;
		public bool isLoaded { get; private set;}
		private bool pendingPlay = false;

		public Video(GameObject go, MediaPlayerCtrl mpc){
			videoName = mpc.m_strFileName;
			gameObject = go;
			mediaPlayerControl = mpc;
			isLoaded = false;
		}

		public void Play(){
			if (isLoaded) {
				mediaPlayerControl.Play ();
				UpdateVideoZPosition (true);
				pendingPlay = false;
			} else {
				pendingPlay = true;
			}
		}

		public void Pause (){
			mediaPlayerControl.Pause ();
			//UpdateVideoZPosition (false);
		}

		public void Stop(){
			mediaPlayerControl.Stop ();
			UpdateVideoZPosition (false);
		}

		public void SeekToStart (){
			if (isLoaded) {
				mediaPlayerControl.SeekTo(0);
				//mediaPlayerControl.Stop ();
				//mediaPlayerControl.Play ();
			} else {
				Debug.LogError("Can't seek to start if video is not loaded. Offending video: " + videoName);
			} 
		}

		public void OnVideoLoaded(){
			ResizeVideoFullscreen ();

			mediaPlayerControl.Stop (); //We don't want the video to start playing as soon as it loads
			UpdateVideoZPosition (false);
			isLoaded = true;

			//Debug.Log ("VIDEO LOADED: " + gameObject.name);

			NewVideoController.instance.CheckIfAllVideosAreLoaded ();

			if (pendingPlay) {
				this.Play();
			}
		}

		void ResizeVideoFullscreen(){
			float videoAspectRatio = GetVideoAspectRatio ();
			float cameraAspectRatio = Camera.main.aspect;

			float yScale;
			float xScale;

			//If video is wider than screen -> set video width to screen width and calculate correct height
			if (videoAspectRatio > cameraAspectRatio) {
				xScale = cameraAspectRatio * 2.0f * Camera.main.orthographicSize;
				yScale = xScale / videoAspectRatio;

			//If video is taller than screen -> set video height to screen height and calculate correct width
			} else {
				xScale = videoAspectRatio * 2.0f * Camera.main.orthographicSize;
				yScale = 2.0f*Camera.main.orthographicSize;
			}

			gameObject.transform.localScale = new Vector3 (xScale, yScale, 1.0f);
			gameObject.transform.position = Camera.main.transform.position;
		}

		float GetVideoAspectRatio(){
			return (float) mediaPlayerControl.GetVideoWidth() / (float) mediaPlayerControl.GetVideoHeight ();
		}

		void UpdateVideoZPosition (bool isPlaying){
			float newZPosition = 0.0f;

			if (isPlaying) {
				newZPosition = NewVideoController.instance.playingVideoZ;
			} else {
				newZPosition = NewVideoController.instance.pausedVideoZ;
			}

			gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, newZPosition);
		}
	}

	[SerializeField] private string[] videoNames;
	[SerializeField] private GameObject VideoPrefab;
	[SerializeField] private List<Video> Videos;

	[SerializeField] private float pausedVideoZ = 10;
	[SerializeField] private float playingVideoZ = 0;

	public static NewVideoController instance;

	private Video currentPlayingVideo;

	public bool allVideosLoaded { get; private set; }

	void Awake(){
		allVideosLoaded = false;
		currentPlayingVideo = null;
		instance = this;
	}

	void Start () { 
		Videos = CreateVideos ();
	}

	List<Video> CreateVideos(){

		List<Video> VideoList = new List<Video> ();
		foreach (string videoName in videoNames) {

			if (!File.Exists(Application.streamingAssetsPath + "/" + videoName)){
				Debug.LogError ("You are trying to load video " + videoName + " which does not exist.");
				return null;
			}

			GameObject videoObject = Instantiate(VideoPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			if (videoObject == null){
				Debug.LogError ("Failed to instantiate videoObject.");
				return null;
			}
			videoObject.transform.parent = this.transform;
			videoObject.name = videoName;

			MediaPlayerCtrl mediaPlayerControl = videoObject.GetComponent<MediaPlayerCtrl>();
			if (mediaPlayerControl == null){
				Debug.LogError ("Cannot find MediaPlayerCtrl script on the instantiated videoObject");
				return null;
			}
			mediaPlayerControl.m_strFileName = videoName;

			Video video = new Video(videoObject, mediaPlayerControl/*, videoEMT*/);
			VideoList.Add(video);

			mediaPlayerControl.OnVideoFirstFrameReady += video.OnVideoLoaded;
		}

		//Checks if all videos are different
		if (!(VideoList.Select (video => video.videoName).Distinct ().Count() == VideoList.Count)) {
			Debug.LogWarning ("You are trying to load multiple instances of the same video.");
		}

		return VideoList;
	}

	public bool CheckIfAllVideosAreLoaded(){

		foreach (Video video in Videos) {

			if (!video.isLoaded){
				allVideosLoaded = false;
				return false;
			}
		}
		
		allVideosLoaded = true;

		return true;
	}

	public bool PlayVideo(string name, bool seekToStart){
		Video video = Videos.First (v => v.videoName == name);
		if (video == null) {
			Debug.LogError ("You are trying to play video " + name + "which cannot be found.");
			return false;
		}
		if (!video.isLoaded) {
			Debug.LogError ("Video " + video.videoName + " can not be played before it is loaded."); 
			return false;
		}

		//Stop the video that is currently playing
		if (currentPlayingVideo != null) {
			currentPlayingVideo.Stop();
		}
		currentPlayingVideo = video;

		video.Play ();
		return true;
	}

	public bool PauseCurrentVideo(){
		if (currentPlayingVideo == null) {
			Debug.LogError ("Can not pause video - there is no video that is currently playing.");
			return false;
		}

		currentPlayingVideo.Pause ();
		return true;
	}

	public bool StopCurrentVideo(){
		if (currentPlayingVideo == null) {
			Debug.LogError ("Can not stop video - there is no video that is currently playing.");
			return false;
		}
		currentPlayingVideo.Stop ();
		return true;
	}
}
