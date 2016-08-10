using UnityEngine;
using System.Collections;
using System.IO;

public class BreathingDetectionTestingManager : MonoBehaviour {

	public string FileName;
	public string SavePath;
	public AudioSource Audio;

	private StreamWriter streamWriter;

	void OnEnable(){
		BreathingEvents.onExhale += HandleExhale;
		BreathingEvents.onInhale += HandleInhale;
	}

	void OnDisable(){
		BreathingEvents.onExhale -= HandleExhale;
		BreathingEvents.onInhale -= HandleInhale;
	}


	void Start () {
		if (SavePath == "") {
			SavePath = Application.streamingAssetsPath;
		}
		streamWriter = new StreamWriter (SavePath + "/" + FileName);
		streamWriter.WriteLine ("DATA: " + Audio.clip.name);
	}

	void HandleExhale(){
		streamWriter.WriteLine ("EXHALE " + Audio.time);
	}

	void HandleInhale(){
		streamWriter.WriteLine ("INHALE " + Audio.time);
	}

	[ContextMenu ("Close Stream")]
	public void CloseStream(){
		streamWriter.Flush ();
		streamWriter.Close ();
	}

}
