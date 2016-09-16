using UnityEngine;
using System.Collections;

[RequireComponent (typeof(AndroidPluginInit))]
public class HeadsetDetection : MonoBehaviour {

	private AndroidPluginInit androidPlugin;

	[SerializeField] private GameObject headsetPanel;
	public bool IsHeadsetConnected = false;

	// Use this for initialization
	void Start () {
		androidPlugin = GetComponent<AndroidPluginInit> ();
		headsetPanel.SetActive (false);

		#if UNITY_ANDROID && !UNITY_EDITOR
			if (androidPlugin.getHeadsetState () < 1) {
				IsHeadsetConnected = false;
				headsetPanel.SetActive(true);
				StartCoroutine (WaitForHeadset ());
			} else {
				IsHeadsetConnected = true;
				headsetPanel.SetActive(false);
			}	
		
		#else
			IsHeadsetConnected = true;
			headsetPanel.SetActive(false);
		#endif
	}
	
	IEnumerator WaitForHeadset(){
		while (androidPlugin.getHeadsetState() < 1) {
			yield return new WaitForEndOfFrame();
		}
		IsHeadsetConnected = true;
		headsetPanel.SetActive (false);
	}
}
