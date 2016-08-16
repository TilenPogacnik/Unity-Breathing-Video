using UnityEngine;
using System.Collections;

public class LockingManager : MonoBehaviour {

	[SerializeField] private float trialTime;
	[SerializeField] private GameObject lockUI;

	private float timer;

	// Use this for initialization
	void Start () {
		lockUI.SetActive (false);
		timer = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (!PlayerPrefs.HasKey ("highsc")) {
			timer += Time.deltaTime;

			if (timer >= trialTime) {
				//TODO: Pause everything
				lockUI.SetActive(true);
			}
		}
	
	}
}
