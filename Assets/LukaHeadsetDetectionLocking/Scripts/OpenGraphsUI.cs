using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class OpenGraphsUI : MonoBehaviour {

	public GameObject graphsViewGO;
	public GameObject parent;
	public GameObject buttonPrefab;

	static string WHITE_SPACES = "    "; // 4 of them

	// handle user input
	public InputField inputFiled;
	// display current user
	public Text textCurrentUser;
	public Text textCurrentUserFirstPageDisplay;


	// set text for new
	public Text textAvgMaxUsers;

	// GRAPHS
	public WMG_Axis_Graph graph;
	public List<Vector2> avgData;
	public List<Vector2> maxData;
	public WMG_Series seriesAvg;
	public WMG_Series seriesMax;

	// username
	public string currentUsername;

	// colors

	public Color myGrayColor = new Color (0.725f,0.725f,0.725f,1);	//87,89,89

	public Color myGreenColor = new Color(0.651f, 0.808f, 0.224f);		// 166,206,57
	public Color myBlueColor = new Color(0, 0.682f, 0.937f);			// 0,174,239



	// users and views game objects
	public GameObject usersView;
	public GameObject viewButtons;


	void OnEnable(){
		BreathingEvents.onInhale += handleInhale;
	}

	void OnDisable(){
		BreathingEvents.onInhale -= handleInhale;
	}

	void handleInhale(){
		float currentBreathDuration = GameObject.Find ("BreathingMonitoring").GetComponent<BreathingMonitoring> ().exhaleTimer;
		GameObject.Find ("XmlScriptGO").GetComponent<handlingDataXml> ().saveUserData(currentBreathDuration, currentUsername);
	}

	public void onInteractiveImageClik(){

		// change orientation
		//Screen.orientation = ScreenOrientation.LandscapeLeft;

		// open graphs & share my data
		graphsViewGO.SetActive (true);


		closeViewButtons();
		closeUserView ();

		if (currentUsername.Equals ("")) {
			// get it from mp3?
			if (PlayerPrefs.HasKey ("lastUsername")) {
				currentUsername = PlayerPrefs.GetString ("lastUsername");
			} else {
				currentUsername = "Unknown user";
			}
		}
			
		if (parent.transform.childCount < 1) {
			Debug.Log ("Creating buttons");
			List<string> usernames = getUsernames ();

			foreach (string user in usernames) {
				createInstanceOfButton (user);
			}

			// instantiate the graph in here aswell

			if (graph == null) {
				
				graph = GameObject.Find ("LineGraph").GetComponent<WMG_Axis_Graph> ();

				updateCurrentUsername (currentUsername);
				drawingDataForUsername (currentUsername, 7);
			} else {
				updateCurrentUsername (currentUsername);
				drawingDataForUsername (currentUsername, 7);
			}

		} else {
			// need to update graph because of new data that could appear etc.
			updateCurrentUsername (currentUsername);
			drawingDataForUsername (currentUsername, 7);
		}

	}


	// user selects new username from list
	public void myOnClickHandler( string username ){

		updateCurrentUsername (username);
		drawingDataForUsername (username, 7);


		closeViewButtons ();
		closeUserView ();

	}

	public void createInstanceOfButton(string user){

		GameObject newButton = Instantiate (buttonPrefab) as GameObject;
		newButton.SetActive (true);
		newButton.name = user;

		foreach (Button go in newButton.GetComponentsInChildren<Button>()) {

			go.GetComponentInChildren<Text> ().text = WHITE_SPACES + user + WHITE_SPACES;

			string _username = user; // used cuz c# delegates otherwise clear it

			go.onClick.AddListener (() => {
				myOnClickHandler (_username);
			});
		}
		newButton.transform.SetParent (parent.transform, false);
	}


	public void toggleViewButtonsVisibility(){

		if (viewButtons.activeSelf) {
			closeViewButtons ();
		} else {
			openViewButtons ();
			closeUserView ();
		}
	}


	public void toggleUserViewVisibility(){

		if (usersView.activeSelf) {
			closeUserView ();
		} else {
			openUserView ();
			closeViewButtons ();
		}
	}


	// close View buttons
	public void closeViewButtons(){
		viewButtons.SetActive (false);
	}

	// open View buttons
	public void openViewButtons(){
		viewButtons.SetActive (true);
	}

	// close user buttons
	public void closeUserView(){
		usersView.SetActive (false);
	}

	// open user buttons
	public void openUserView(){
		usersView.SetActive (true);
	}


	// close
	public void closeGraphsUI(){
		// change orientation
		//Screen.orientation = ScreenOrientation.Portrait;

		graphsViewGO.SetActive (false);
	}

	// get all usernames from xml
	public List<string> getUsernames(){
		return GameObject.Find ("XmlScriptGO").GetComponent<handlingDataXml> ().getXmlUsernames ();
	}

	// user creates username
	public void setUsername(){
		// check for text
		if (inputFiled.text != "") {
			string newUsername = inputFiled.text;
			inputFiled.text = "";

			createInstanceOfButton (newUsername);
		}
	}

	public void onClickSevenDaysData(){
		drawingDataForUsername (currentUsername, 7);
		closeViewButtons ();
		closeUserView ();
	}

	public void onClickThirtyDaysData(){
		drawingDataForUsername (currentUsername, 30);
		closeViewButtons ();
		closeUserView ();
	}

	public void onClickNinetyDaysData(){
		drawingDataForUsername (currentUsername, 90);
		closeViewButtons ();
		closeUserView ();
	}

	// handling users data
	void drawingDataForUsername( string username, int number){
		
		if (avgData.Count > 0) {
			graph.deleteSeries ();
			avgData.Clear ();
		}
		if (maxData.Count > 0) {
			graph.deleteSeries ();
			maxData.Clear ();
		}

		handlingDataXml xmlHandler = GameObject.Find("XmlScriptGO").GetComponent<handlingDataXml> ();
		List<UsersOneDayData> gainedData = xmlHandler.getUsersData (username, number);

		int i = 0;
		float highscore = 0;
		foreach (UsersOneDayData da in gainedData) {
			if (da.maxBreath > highscore) {
				highscore = da.maxBreath;
			}
			//debugText.text += da.date + "|" + da.maxBreath + "|" + da.avgBreath + "*";
			maxData.Add(new Vector2(i, da.maxBreath) );
			avgData.Add(new Vector2(i, da.avgBreath) );
			i++;
		}

		seriesAvg = graph.addSeries ();
		seriesMax = graph.addSeries ();
		seriesAvg.seriesName = "Average";
		seriesMax.seriesName = "Maximum";

		seriesAvg.pointValues.SetList (avgData);
		seriesMax.pointValues.SetList (maxData);

		seriesAvg.lineColor = myBlueColor; // Color.blue;
		seriesMax.lineColor = myGreenColor; //Color.green;

		seriesAvg.pointColor = myGrayColor;//new Color (0.835,0.835,0.835,1); //Color.black;
		seriesMax.pointColor = myGrayColor;

		graph.xAxis.AxisMaxValue = number - 1 ;
		graph.xAxis.hideLabels = true;

		graph.yAxis.AxisMaxValue = highscore + 1;


		graph.xAxis.AxisTitleString = "Week view";
		if( number > 9 && number < 31){
			graph.xAxis.AxisTitleString = "Month view";
		}else if( number > 31){
			graph.xAxis.AxisTitleString = "Three months view";
		}

	}

	void updateCurrentUsername(string new_username){

		currentUsername = new_username;
		// update also texts

		textCurrentUser.text = "Results for " + new_username;

		if (textCurrentUserFirstPageDisplay != null) {
			textCurrentUserFirstPageDisplay.text = "Hello, " + new_username + ".";
		}

		// set public username in mp3Player script -> it'll set it for the xml
		//GameObject.Find("Mp3PlayGameObjHolder").GetComponent<Mp3Player>().setPublicUsername( new_username );


		PlayerPrefs.SetString ("lastUsername", new_username);
		PlayerPrefs.Save ();
	}

	// test
	public void testXmls( string usern ){

		//Text debugText = GameObject.Find("DEBUG_TEXT").GetComponent<Text> ();

		string tmpUsername = usern;//textCurrentUser.text;

		handlingDataXml xmlHandler = GameObject.Find("XmlScriptGO").GetComponent<handlingDataXml> ();

		List<UsersOneDayData> gainedData = xmlHandler.getUsersHistoryList (tmpUsername); //getUsersData (tmpUsername, 8);
		//debugText.text += "\n\nData without 0's: "+gainedData.Count+"\n\n";

		foreach (UsersOneDayData da in gainedData) {
			//debugText.text += da.date + "|" + da.maxBreath + "|" + da.avgBreath + "*";
		}

		List<UsersOneDayData> gainedData_2 = xmlHandler.getUsersData (tmpUsername, 30); //getUsersData (tmpUsername, 8);
		//debugText.text += "\n\nData with 0's "+"\n\n";

		foreach (UsersOneDayData da in gainedData_2) {
			//debugText.text += da.date + "|" + da.maxBreath + "|" + da.avgBreath + "*";
		}

	}

}
