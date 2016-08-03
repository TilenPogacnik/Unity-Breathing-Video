using UnityEngine;
using System.Collections;

public class SceneLoader : MonoBehaviour {
	

	public void toMenu(){
		Application.LoadLevel (0);
	}

	public void toVideoPlayer(){
		Application.LoadLevel (1);
	}

	public void toGraphView(){
		Application.LoadLevel (2);
	}

	public void loadSceneByNumber(int sceneNumber){
		Application.LoadLevel (sceneNumber);
	}
}
