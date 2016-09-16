using UnityEngine;
using System.Collections;

public class LevelPopupManager : MonoBehaviour {

	public ScreenController sCont;
	public float visibleTime = 2.0f;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void LevelButtonClick(){
		sCont.SetScreenVisible (true);
		Invoke ("HideLevelScreen", visibleTime);
	}

	void HideLevelScreen(){
		sCont.SetScreenVisible (false);
	}
}
