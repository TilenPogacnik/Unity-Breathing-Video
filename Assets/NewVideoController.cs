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
				pendingPlay = false;
			} else {
				pendingPlay = true;
			}
		}

		public void Pause (){
			mediaPlayerControl.Pause ();
		}

		public void Stop(){
			mediaPlayerControl.Stop ();
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
			mediaPlayerControl.Stop (); //We don't want the video to start playing as soon as it loads

			ResizeVideoFullscreen ();

			isLoaded = true;
			Debug.Log ("VIDEO LOADED: " + gameObject.name);
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
	}

	[SerializeField] private string[] videoNames;
	[SerializeField] private GameObject VideoPrefab;
	[SerializeField] private List<Video> Videos;

	public static NewVideoController instance;

	private bool allVideosLoaded = false;

	void Awake(){
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

	public void PlayVideo(string name, bool seekToStart){
		Video video = Videos.First (v => v.videoName == name);
		if (video == null) {
			Debug.LogError ("You are trying to play video " + name + "which cannot be found.");
			return;
		}
		video.Play ();
	}
}
