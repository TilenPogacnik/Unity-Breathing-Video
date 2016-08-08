using UnityEngine;
using System.Collections;

public class LoadingVideoSceneManager : MonoBehaviour {
	private AsyncOperation async;

	IEnumerator Start(){
		//Wait until all videos are loaded
		while (!NewVideoController.instance.allVideosLoaded) {
			yield return new WaitForEndOfFrame();
		}

		//Play loading video
		NewVideoController.instance.PlayVideo ("loadingVideo.mp4", true, false);
		NewVideoController.instance.SetVideoOnEnd ("loadingVideo.mp4", LoadNextLevel);

		//Load next scene asynchronously and wait for permission to activate it
		async = Application.LoadLevelAsync (Application.loadedLevel + 1);
		async.allowSceneActivation = false;
		yield return async;
	}

	//Permits scene loading process to activate next scene
	void LoadNextLevel(){
		Debug.Log ("Video ended.");
		async.allowSceneActivation = true;
	}
}
