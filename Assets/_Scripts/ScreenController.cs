using UnityEngine;
using System.Collections;

public class ScreenController : MonoBehaviour {
	[SerializeField] private bool UseScoreText;
	[SerializeField] private TextMesh ScoreText;
	[SerializeField] private string ScoreTextPrefix;
	[SerializeField] private string ScoreTextSuffix;
	[SerializeField] private float HiddenZPosition; 
	[SerializeField] private float VisibleZPosition;
	[SerializeField] private bool VisibleOnStart;

	private BreathingMonitoring breathingMonitoring;

	void OnEnable(){
		BreathingEvents.onInhale += UpdateText;
	}

	void OnDisable(){
		BreathingEvents.onInhale -= UpdateText;

	}

	void Start () {
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
		float newZPosition = 0.0f;
		if (visible) {
			newZPosition = VisibleZPosition;
		} else {
			newZPosition = HiddenZPosition;
		}
		this.gameObject.transform.position = new Vector3 (this.gameObject.transform.position.x, this.gameObject.transform.position.y, newZPosition); 
	}

	public void UpdateText (){
		if (UseScoreText) {
			ScoreText.text = ScoreTextPrefix + breathingMonitoring.exhaleTimer.ToString ("f2") + ScoreTextSuffix;
		}
	}
}
