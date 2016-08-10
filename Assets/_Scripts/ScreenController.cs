using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScreenController : MonoBehaviour {
	[SerializeField] private bool UseScoreText;
	[SerializeField] private Text ScoreText;
	[SerializeField] private string ScoreTextPrefix;
	[SerializeField] private string ScoreTextSuffix;
	[SerializeField] private bool VisibleOnStart;

	private BreathingMonitoring breathingMonitoring;

	void Start () {
		BreathingEvents.onInhale += UpdateText;


		SetScreenVisible(VisibleOnStart);

		//If BreathingMonitoring is not defined try to find it in scene
		if (breathingMonitoring == null){

			breathingMonitoring = GameObject.Find("BreathingMonitoring").GetComponent<BreathingMonitoring>();

			//Throw an error if BreathingMonitoring wasn't found 
			if (breathingMonitoring == null){
				Debug.LogError ("There is no BreathingMonitoring present in scene.");
			}
		}
	}
	
	public void SetScreenVisible(bool visible){
		this.gameObject.SetActive (visible);
	}

	public void UpdateText (){
		if (UseScoreText) {
			ScoreText.text = ScoreTextPrefix + breathingMonitoring.exhaleTimer.ToString ("f2") + ScoreTextSuffix;
		}
	}
}
