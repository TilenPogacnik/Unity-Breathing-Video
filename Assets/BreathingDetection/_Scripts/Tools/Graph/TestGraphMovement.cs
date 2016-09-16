using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class TestGraphMovement : MonoBehaviour {

	public enum GraphType{Loudness, Variance, TrueLoudness, Pitch};

	public GraphType gType;

	public float maxY;
	public float minY;

	public float minX;
	public float maxX;
	public float xSpeed;

	public float thresholdVisualizationY = 0.5f;

	private float prevLoudness = 0f;
	private float varianceBuffer = 0f;

	public MicrophoneController micControl;
	public BreathingDetection bDetection;

	public Text loudnessText;

	//Spremenljivke za vizualizacijo
	public GameObject stateChangeIndicator;

	private TrailRenderer tRend;

	void OnEnable(){
		BreathingEvents.onExhale += exhaleStarted;
		BreathingEvents.onInhale += inhaleStarted;
	}

	void OnDisable(){
		BreathingEvents.onExhale -= exhaleStarted;
		BreathingEvents.onInhale -= inhaleStarted;
	}


	void Start () {
		tRend = this.GetComponent<TrailRenderer> ();
		StartCoroutine (DrawGraph ());
	}

	IEnumerator DrawGraph(){
		while (true){

			if (loudnessText != null) {
				loudnessText.text = "Loudness: " + Mathf.Round (micControl.loudness * 100.0f) / 100.0f;
			}
			
			switch (gType) {
			case (GraphType.Loudness):
				this.transform.position = new Vector3 (this.transform.position.x + xSpeed * Time.deltaTime, minY + (maxY - minY) * bDetection.minimizedLoudness, this.transform.position.z);
				
				if (this.transform.position.x >= maxX) {
					float zPoz = this.transform.position.z;
					this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, thresholdVisualizationY);
					yield return new WaitForEndOfFrame();
					this.transform.position = new Vector3(minX, this.transform.position.y, thresholdVisualizationY);
					yield return new WaitForEndOfFrame();
					this.transform.position = new Vector3(minX, this.transform.position.y, zPoz);	
					yield return new WaitForEndOfFrame();

				}
				break;
				
			case (GraphType.Variance):
				this.transform.position = new Vector3 (this.transform.position.x + xSpeed * Time.deltaTime, minY + (maxY - minY) * thresholdVisualizationY, this.transform.position.z);
				
				if (this.transform.position.x >= maxX) {
					float zPoz = this.transform.position.z;
					this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, thresholdVisualizationY);
					yield return new WaitForEndOfFrame();
					this.transform.position = new Vector3(minX, this.transform.position.y, thresholdVisualizationY);
					yield return new WaitForEndOfFrame();
					this.transform.position = new Vector3(minX, this.transform.position.y, zPoz);	
					yield return new WaitForEndOfFrame();

				}
				break;
				
				
			case (GraphType.TrueLoudness):
				this.transform.position = new Vector3 (this.transform.position.x + xSpeed * Time.deltaTime, minY + (maxY - minY) * micControl.loudness, this.transform.position.z);
				
				if (this.transform.position.x >= maxX) {
					float zPoz = this.transform.position.z;
					this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, thresholdVisualizationY);
					yield return new WaitForEndOfFrame();
					this.transform.position = new Vector3(minX, this.transform.position.y, thresholdVisualizationY);
					yield return new WaitForEndOfFrame();
					this.transform.position = new Vector3(minX, this.transform.position.y, zPoz);	
					yield return new WaitForEndOfFrame();
				}
				break;
				
			case (GraphType.Pitch):
				this.transform.position = new Vector3 (this.transform.position.x + xSpeed * Time.deltaTime, minY + (maxY - minY) * micControl.getAveragePitch(), this.transform.position.z);
				
				if (this.transform.position.x >= maxX) {
					float zPoz = this.transform.position.z;
					this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, thresholdVisualizationY);
					yield return new WaitForEndOfFrame();
					this.transform.position = new Vector3(minX, this.transform.position.y, thresholdVisualizationY);
					yield return new WaitForEndOfFrame();
					this.transform.position = new Vector3(minX, this.transform.position.y, zPoz);	
					yield return new WaitForEndOfFrame();
				}
				break;
				
			default:
				Debug.Log ("This shouldn't happen" + gType.ToString());
				break;
			}
			yield return new WaitForEndOfFrame();
		}
	}

	// Update is called once per frame
	void Update () {

	}

	float getVariance(){
		varianceBuffer = micControl.loudness - prevLoudness;
		prevLoudness = micControl.loudness;
		return varianceBuffer;
	}

	void exhaleStarted(){
		Instantiate (stateChangeIndicator, this.transform.position, Quaternion.identity);
	}

	void inhaleStarted(){
		Instantiate (stateChangeIndicator, this.transform.position, Quaternion.identity);

	}
}
