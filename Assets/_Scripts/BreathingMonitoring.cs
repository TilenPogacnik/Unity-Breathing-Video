	using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BreathingMonitoring : MonoBehaviour {

	[Range(0.0f, 3.0f)]
	[SerializeField] private float MinimumAcceptableExhaleDuration;

	[SerializeField]private string CurrentExhaleSuffix;
	[SerializeField]private string CurrentExhalePrefix;
	[SerializeField] private string AverageTextPrefix;
	[SerializeField] private string MaximumTextPrefix;
	[SerializeField] private string LevelTextPrefix;
	[SerializeField] private string UsernameTextPrefix;


	[SerializeField] private Text AverageText;
	[SerializeField] private Text MaximumText;
	[SerializeField] private Text CurrentExhaleText;
	[SerializeField] private Text LevelText;
	[SerializeField] private Text UsernameText;

	public OpenGraphsUI openGraphsUI;


	public float exhaleTimer { get; private set;}
	private bool isPlayerExhaling = false;

	private float BreathDurationSum = 0.0f;
	private int BreathCount = 0;

	private float MaximumBreathDuration = 0.0f;

	void OnEnable(){
		BreathingEvents.onExhale += OnExhaleStarted;
		BreathingEvents.onInhale += OnExhaleEnded;
	}

	void OnDisable(){
		BreathingEvents.onExhale -= OnExhaleStarted;
		BreathingEvents.onInhale -= OnExhaleEnded;
	}

	void Start(){
		exhaleTimer = 0.0f;
		UpdateCurrentExhaleText ();
		UpdateMaximumBreathDuration ();
		UpdateAverageBreathDuration ();
	}

	void FixedUpdate () {
		if (isPlayerExhaling) {
			UpdateCurrentExhaleText ();
			UpdateLevelText();
		}

	}

	void UpdateCurrentExhaleText (){
			exhaleTimer += Time.deltaTime;
			CurrentExhaleText.text = CurrentExhalePrefix + exhaleTimer.ToString ("f1")/*.Replace (".", ":")*/ + CurrentExhaleSuffix;
	}

	void UpdateMaximumBreathDuration(){
		MaximumText.text = MaximumTextPrefix + MaximumBreathDuration.ToString ("f1") + CurrentExhaleSuffix; /*("00.00").Replace (".", ":");*/
	}

	void UpdateLevelText(){
		LevelText.text = LevelTextPrefix + (Mathf.Floor (exhaleTimer / 10) + 1).ToString ();
	}

	void UpdateUsernameText(){
		UsernameText.text = UsernameTextPrefix + openGraphsUI.currentUsername;
	}

	void UpdateAverageBreathDuration(){
		if (AverageText != null) {
			float averageBreathDuration = 0.0f;
			if (BreathCount > 0) {
				averageBreathDuration = BreathDurationSum / BreathCount;
			} else {
				averageBreathDuration = 0.0f;
			}
			AverageText.text = AverageTextPrefix + averageBreathDuration.ToString ("f1") + CurrentExhaleSuffix;
		}
	}

	void OnExhaleStarted(){
		//Start recording exhale
		isPlayerExhaling = true;
		exhaleTimer = 0.0f;
	}

	void OnExhaleEnded(){
		isPlayerExhaling = false;

		if (exhaleTimer > MaximumBreathDuration) {
				MaximumBreathDuration = exhaleTimer;
				UpdateMaximumBreathDuration();
		}

		if (exhaleTimer > MinimumAcceptableExhaleDuration) {
			BreathDurationSum += exhaleTimer;
			BreathCount++;
			UpdateAverageBreathDuration ();
		}
	}
}
