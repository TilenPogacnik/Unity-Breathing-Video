using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AndroidPluginInit : MonoBehaviour {
	
	//public Text mojText;
	
	AndroidJavaClass ajc;
	protected string pck_name = "com.developer.luka.mrak.headsetdetection.HeadsetDetector";
	
	private AndroidJavaObject testobj = null;
	private AndroidJavaObject playerActivityContext = null;
	
	//private int count = 0;

	private GameObject headsetPopUp;
	
	void Start () {


		#if UNITY_ANDROID
		
			if (testobj == null) {
			
				// First, obtain the current activity context
				AndroidJavaClass actClass = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
				playerActivityContext = actClass.GetStatic<AndroidJavaObject> ("currentActivity");
			
				// Pass the context to a newly instantiated UnityGetButtonsType object
				AndroidJavaClass pluginClass = new AndroidJavaClass (pck_name);
			
				if (pluginClass != null) {
				
					testobj = pluginClass.CallStatic<AndroidJavaObject> ("instance");
					testobj.Call ("setContext", playerActivityContext);
					testobj.Call ("initMethod");
					testobj.Call ("hideToastMsg");
				}
			
			}
		#endif

		//headsetPopUp = GameObject.Find ("HeadsetPopup");
		//headsetPopUp.SetActive (false);
	}

	public int getHeadsetState(){
			
		int sta = testobj.Call<int> ("getHeadsetState");
		/*if( sta == 0 ){
			// no headset
			headsetPopUp.SetActive (true);
		}else if ( sta == 1 ){
			headsetPopUp.SetActive (false);
		}*/
		return sta;
	}
	/*
	string msg = "";
	
	void Update(){
		
		if (Application.isMobilePlatform) {
			count++;// = count + 1;
			count = count % 2000;
			if (count == 10) {
				msg = "hide TOAST";
				// display Toast msgs
				testobj.Call ("hideToastMsg");
			} else if (count == 1010) {
				msg = "show TOAST";
				// hide Toast msgs
				testobj.Call ("showToastMsg");
			}
			int sta = testobj.Call<int> ("getHeadsetState");
			mojText.text = "State = " + sta + "\n\n count = " + count + "\n\n" + msg;

			if( sta == 0 ){
				// no headset
				headsetPopUp.SetActive (true);
			}else if ( sta == 1 ){
				headsetPopUp.SetActive (false);
			}
		}
	}
	*/
}
