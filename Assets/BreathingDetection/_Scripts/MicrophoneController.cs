﻿using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using System.Collections.Generic;



[RequireComponent (typeof (AudioSource))]
public class MicrophoneController : MonoBehaviour {

	private AudioSource aSource;
	public int samples = 1024;
	private int maxFrequency = 44100;
	private int minFrequency = 0;
	public bool mute = true;
	[HideInInspector]
	public float loudness;
	[SerializeField] private float loudnessMultiplier = 10.0f; //Multiply loudness with this number

	private float[] fftSpectrum;

	private bool isMicrophoneReady = false;
	private AudioMixer aMixer;

	public float highPassCutoff; //Ignores all frequencies above this value
	private float pitchValue;
	private List<float> pastPitches;
	[HideInInspector]
	public int pitchRecordTime = 5;
	private float averagePitch;

	private bool UseFFTCentroid;
	private float centroidValue;

	private bool EnableSavingOfRecordedAudio;

	private float maxPitch = 0.0f; //Delete this, its just for testing


	IEnumerator Start () {
		aSource = this.GetComponent<AudioSource> ();

		aMixer = Resources.Load ("MicrophoneMixer") as AudioMixer;		
		if(mute){
			aMixer.SetFloat("MicrophoneVolume",-80);
		}
		else{
			aMixer.SetFloat("MicrophoneVolume",0);
		}

		if (Microphone.devices.Length == 0) {
			Debug.LogWarning("No microphone detected.");
		}


		//if using Android or iOS -> request microphone permission
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
			yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);

			if (!Application.HasUserAuthorization(UserAuthorization.Microphone)){
				Debug.LogWarning ("Application does not have microphone permission.");
				yield break;
			}
		}

		prepareMicrophone ();

		fftSpectrum = new float[samples];
		pastPitches = new List<float> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (isMicrophoneReady) {
			loudness = calculateLoudness();

			if (UseFFTCentroid) {
				calculateFFTCentroid();

			} else {
				calculatePitch();
			}
		}
	}

	void OnGUI(){
		if (EnableSavingOfRecordedAudio) {
			if (GUI.Button (new Rect (10, 10, 50, 50), "Save")) {
				SaveRecordedAudio ();
			}
		}
	}

	void prepareMicrophone(){
		if (Microphone.devices.Length > 0) {
			//Gets the maxFrequency and minFrequency of the device
			Microphone.GetDeviceCaps (Microphone.devices [0], out minFrequency, out maxFrequency);
			if (maxFrequency == 0) {//These 2 lines of code are mainly for windows computers
				maxFrequency = 44100;
			}
			if (aSource.clip == null){
				aSource.clip = Microphone.Start (Microphone.devices [0], true, 1, maxFrequency);			
				aSource.loop = true;

				//Wait until microphone starts recording
				while (!(Microphone.GetPosition(Microphone.devices[0]) > 0)) {
				}
			}
			aSource.Play();
			isMicrophoneReady = true;

		} else {
				Debug.LogWarning("No microphone detected.");
		}

	}

	void calculatePitch(){
		// Gets the sound spectrum.
		aSource.GetSpectrumData(fftSpectrum, 0, FFTWindow.BlackmanHarris);
		float maxV = 0;
		int maxN = 0;
		
		// Find the highest sample.
		for (int i = 0; i < fftSpectrum.Length; i++){
			if (fftSpectrum[i] > maxV){
				maxV = fftSpectrum[i];
				maxN = i; // maxN is the index of max
			}
		}
		
		// Pass the index to a float variable
		float freqN = maxN;
		
		// Convert index to frequency
		pitchValue = HighPassFilter(freqN * 24000 / samples, highPassCutoff);
		updatePastPitches (pitchValue);

		if (pitchValue > maxPitch){
			maxPitch = pitchValue;
			//Debug.Log ("MaxPitch: " + maxPitch);

		}
	/*	if (pitchValue > 750 && pitchValue < 3000) {
			Debug.Log ("Pitch could be exhale");
		}*/
	}

	void calculateFFTCentroid(){
		aSource.GetSpectrumData (fftSpectrum, 0, FFTWindow.BlackmanHarris);

		float centroid = 0.0f;
		float fftSum = 0.0f;
		float weightedSum = 0.0f;

		for (int i = 0; i < fftSpectrum.Length/2; i++) {
			fftSum += fftSpectrum [i];
			weightedSum += fftSpectrum [i] * i * 24000/samples;
		}

		pitchValue =/*(24000 / samples) * */(weightedSum / fftSum);
		updatePastPitches (pitchValue);

		Debug.Log ("Centroid: " + pitchValue);
	}



	float HighPassFilter(float pitch, float cutOff){
		if (pitch > cutOff) {
			return 0;
		} else {
			return pitch;
		}
	}

	void updatePastPitches(float newPitch){
		pastPitches.Add(newPitch);

		if (pastPitches.Count > pitchRecordTime) {
			pastPitches.RemoveAt(0);
		}

		averagePitch = 0;
		foreach (float num in pastPitches) {
			averagePitch += num;
		}
		averagePitch /= pastPitches.Count;
		//Debug.Log ("Average pitch: " + averagePitch);
	}

	public float getPitch(){
		return pitchValue;
	}

	public float getAveragePitch(){
		return averagePitch;
	}

	public float getCentroid(){
		return centroidValue;
	}

	float calculateLoudness(){
		float[] microphoneData = new float[samples];
		float sum = 0;

		aSource.GetOutputData (microphoneData, 0);
		for (int i = 0; i < microphoneData.Length; i++) {
			sum += Mathf.Pow(microphoneData[i],2);//Mathf.Abs(microphoneData[i]);
		}

		return Mathf.Sqrt(sum/samples)*loudnessMultiplier;
	}

	void SaveRecordedAudio(){
		//EditorUtility.ExtractOggFile (GameObject.Find("TestAudioSource").GetComponent<AudioSource>().clip, Application.streamingAssetsPath);
	}
}
