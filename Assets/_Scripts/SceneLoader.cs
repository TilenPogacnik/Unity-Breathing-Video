using UnityEngine;
using System.Collections;

public class SceneLoader : MonoBehaviour {

	private bool paused = false;

	void Update(){
		if (Input.GetKeyUp (KeyCode.N)) {
			LoadNextLevel ();
		} else if (Input.GetKeyUp (KeyCode.V)) {
			LoadPreviousLevel ();
		} else if (Input.GetKeyUp (KeyCode.P)) {
			Pause ();
		}
	}

	public void toMenu(){
		Application.LoadLevel (1);
	}

	public void loadSceneByNumber(int sceneNumber){
		Application.LoadLevel (sceneNumber);
	}

	void LoadNextLevel(){
		Application.LoadLevel (Application.loadedLevel + 1);
	}
	void LoadPreviousLevel(){
		Application.LoadLevel (Application.loadedLevel - 1);
	}

	void Pause(){
		if (!paused) {
			paused = true;
			Time.timeScale = 0;
		} else {
			paused = false;
			Time.timeScale = 1;
		}
	}
}
