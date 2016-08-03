using UnityEngine;
using System.Collections;

public class LoadingVideoSceneManager : MonoBehaviour {

	[SerializeField] private MediaPlayerCtrl loadingVideo;

	private AsyncOperation async;
	// Use this for initialization
	IEnumerator Start () {
		loadingVideo.OnEnd += LoadNextLevel;

		async = Application.LoadLevelAsync (Application.loadedLevel+1);
		async.allowSceneActivation = false;
		yield return async;
		Debug.Log ("Loading complete.");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void LoadNextLevel(){
		Debug.Log ("Ended video");
		async.allowSceneActivation = true;
	}
}
