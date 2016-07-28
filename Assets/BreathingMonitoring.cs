using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BreathingMonitoring : MonoBehaviour {

	[SerializeField] private string AverageTextPrefix;
	[SerializeField] private string MaximumTextPrefix;

	[SerializeField] private Text AverageText;
	[SerializeField] private Text MaximumText;
	[SerializeField] private Text CurrentExhaleText;

	private float exhaleTimer = 0.0f;
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
		UpdateCurrentExhaleText ();
		UpdateMaximumBreathDuration ();
		UpdateAverageBreathDuration ();
	}

	void FixedUpdate () {
		if (isPlayerExhaling) {
			UpdateCurrentExhaleText ();
		}

	}

	void UpdateCurrentExhaleText (){
			exhaleTimer += Time.deltaTime;
			CurrentExhaleText.text = exhaleTimer.ToString ("f1");
	}

	void UpdateMaximumBreathDuration(){
		MaximumText.text = MaximumTextPrefix + MaximumBreathDuration.ToString ("f1");
	}

	void UpdateAverageBreathDuration(){
		float averageBreathDuration = 0.0f;
		if (BreathCount > 0) {
			averageBreathDuration = BreathDurationSum / BreathCount;
		} else {
			averageBreathDuration = 0.0f;
		}
		AverageText.text = AverageTextPrefix + averageBreathDuration.ToString ("f1");
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

		BreathDurationSum += exhaleTimer;
		BreathCount++;
		UpdateAverageBreathDuration ();
	}
}
