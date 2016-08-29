	using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BreathingMonitoring : MonoBehaviour {

	[Range(0.0f, 3.0f)]
	[SerializeField] private float MinimumAcceptableExhaleDuration;

	[SerializeField]private string CurrentExhaleSuffix;
	[SerializeField] private string AverageTextPrefix;
	[SerializeField] private string MaximumTextPrefix;

	[SerializeField] private Text AverageText;
	[SerializeField] private Text MaximumText;
	[SerializeField] private Text CurrentExhaleText;

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
		}

	}

	void UpdateCurrentExhaleText (){
			exhaleTimer += Time.deltaTime;
			CurrentExhaleText.text = exhaleTimer.ToString ("f1")/*.Replace (".", ":")*/ + CurrentExhaleSuffix;
	}

	void UpdateMaximumBreathDuration(){
		MaximumText.text = MaximumTextPrefix + MaximumBreathDuration.ToString ("f1") + CurrentExhaleSuffix; /*("00.00").Replace (".", ":");*/
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
