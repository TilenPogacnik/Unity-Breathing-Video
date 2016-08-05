using UnityEngine;
using System.Collections;

public class PanelClosing : MonoBehaviour {

	[SerializeField] private GameObject panelToClose;
	[SerializeField] private GameObject reopenButton;
	[SerializeField] private bool openOnStart = true;

	void Start () {
		if (panelToClose == null) {
			Debug.LogError ("panelToClose cannot be null.");
		}

		if (reopenButton == null) {
			Debug.LogError ("reopenButton cannot be null.");
		}

		SetPanelOpen (openOnStart);
	}

	/*
	 * Handles opening and closing of panelToClose and reopenButton.
	 * SetPanelOpen(true) will open the panel and close the button
	 * SetPanelOpen(false) will close the panel and open the button
	 */
	public void SetPanelOpen(bool open){
		panelToClose.SetActive (open);
		reopenButton.SetActive (!open);
	}
}
