using UnityEngine;
using System.Collections;

public class SceneLoader : MonoBehaviour {
	

	public void toMenu(){
		Application.LoadLevel (1);
	}

	public void loadSceneByNumber(int sceneNumber){
		Application.LoadLevel (sceneNumber);
	}
}
