using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System;


public class handlingDataXml : MonoBehaviour {

	private UserDataContainer myContainer;
	private string PATH_SAVE_DATA;

	public void initXml(){
		// set path
		PATH_SAVE_DATA = Application.persistentDataPath + "/data.xml";

		// set container
		xmlInitFile ();
	}

	public void xmlInitFile( ){
		
		try {
			myContainer = UserDataContainer.loadUserData (PATH_SAVE_DATA);

		} catch (FileNotFoundException e) {
			// 
			myContainer = new UserDataContainer ();
			myContainer.userData = new List<UserData> ();

		} catch (System.IO.IsolatedStorage.IsolatedStorageException e) {	// android exception, when file doesn't exist yet			
			myContainer = new UserDataContainer ();
			myContainer.userData = new List<UserData> ();
		}
	}

	public UserDataContainer getXmlContainer(){
		return myContainer;
	}

	public void saveUserData( float currBreath, string usern){
		
		UserData ud = new UserData ();
		ud.BreathLength = currBreath;
		ud.TimeStamp = DateTime.Now;
		ud.Username = usern;
		xmlAddNewData (ud);
	}

	public List<string> getXmlUsernames(){
		//
		List<string> usernames = new List<string>();

		foreach (UserData user_data in myContainer.userData) {
			
			string curr_user = user_data.Username;

			if ( ! usernames.Contains (curr_user)) {
				usernames.Add (curr_user);
			}
		}
		return usernames;
	}

	public void xmlAddNewData(UserData ele ){
		if (myContainer == null) {
			initXml ();
		}
		myContainer.userData.Add (ele);
		myContainer.saveUserData (PATH_SAVE_DATA);
	}

	// SAVE XML FILE
	[XmlRoot("UserDataContainer")]
	public class UserDataContainer{

		public List<UserData> userData;
		//public UserData[] userData;

		public UserDataContainer(){

		}

		public void saveUserData( string path ){
			var serial = new XmlSerializer (typeof(UserDataContainer));
			using (var stream = new FileStream (path, FileMode.Create)) {
				serial.Serialize (stream, this);
			}
		}

		public static UserDataContainer loadUserData(string path){
			var serial = new XmlSerializer (typeof(UserDataContainer));
			using (var stream = new FileStream (path, FileMode.Open)) {
				return serial.Deserialize (stream) as UserDataContainer;
			}
		}

	}

	// data
	public class UserData{
		public DateTime TimeStamp;
		public float BreathLength = 0f;

		[XmlAttribute("username")]
		public string Username;

		public UserData(){
		}

	}


	// Goes over the xml file and returns all the avg's and max's for all the days for username usersUsername

	public List<UsersOneDayData> getUsersHistoryList( string usersUsername ){


		float tmp_num_of_data = 0;
		float tmp_curr_sum = 0;
		float tmp_curr_max = 0;

		DateTime tmp_this_date = DateTime.Now;
		DateTime tmp_date_to_save = tmp_this_date;

		List<UsersOneDayData> usersData = new List<UsersOneDayData>();

		// go over whole xml
		foreach (UserData data in myContainer.userData) {

			// check only this username
			if (data.Username == usersUsername) {
				
				if ( tmp_num_of_data == 0) {
					// first date
					tmp_this_date = data.TimeStamp;
					tmp_date_to_save = data.TimeStamp;
				}
				// check the date
				if (data.TimeStamp.Date.Equals (tmp_this_date.Date)) {
					// sum up data
					tmp_curr_sum += data.BreathLength;
					tmp_num_of_data += 1;
					// check for max
					if (tmp_curr_max < data.BreathLength) {
						tmp_curr_max = data.BreathLength;
					}
					tmp_date_to_save = data.TimeStamp;

				} else {
					// next date - calculate the avg and save the max value
					float avg_holder = tmp_curr_sum / tmp_num_of_data;
					UsersOneDayData tmp_obj_data = new UsersOneDayData ();
					tmp_obj_data.avgBreath = avg_holder;
					tmp_obj_data.maxBreath = tmp_curr_max;
					// date for saving :)
					tmp_obj_data.date = tmp_date_to_save;
					usersData.Add (tmp_obj_data);
					// next date
					tmp_this_date = data.TimeStamp;
					tmp_curr_sum = data.BreathLength;
					tmp_num_of_data = 1;
					// this date
					tmp_date_to_save = data.TimeStamp;
				}

			}
		}

		if (tmp_num_of_data > 0) {
			// save the last one aswell :D
			float avg_holder_2 = tmp_curr_sum / tmp_num_of_data;
			UsersOneDayData tmp_obj_data_2 = new UsersOneDayData ();
			tmp_obj_data_2.avgBreath = avg_holder_2;
			tmp_obj_data_2.maxBreath = tmp_curr_max;
			// save old date 
			tmp_obj_data_2.date = tmp_date_to_save;
			usersData.Add (tmp_obj_data_2);
		}

		return usersData;

	}

	public List<UsersOneDayData> getUsersData( string username, int days){

		//Text debugText = GameObject.Find("DEBUG_TEXT").GetComponent<Text> ();

		List<UsersOneDayData> allUsersData = getUsersHistoryList (username);
		DateTime today = DateTime.Now;
		DateTime daysAgo = DateTime.Now.AddDays ( - 1 * (days-1));

		List<UsersOneDayData> partOfData = new List<UsersOneDayData> ();

		foreach (UsersOneDayData userData in allUsersData) {
			if (userData.date.Date >= daysAgo.Date) {
				// add this date
				partOfData.Add(userData);
			}
		}
		// add missing values
		DateTime tmp_day = daysAgo;
		int count = 0;
		int counting = 0;
		while (tmp_day.Date <= today.Date) {
			// could be the last value
			try{
				if (partOfData [count].date.Date.Equals (tmp_day.Date)) {
					//got value
				} else {
					// value is missing
					UsersOneDayData one_day = new UsersOneDayData ();
					one_day.avgBreath = 0;
					one_day.maxBreath = 0;
					one_day.date = tmp_day;
					// add this data in the right place of list
					partOfData.Insert (count, one_day);
				}
				tmp_day = tmp_day.AddDays (1);
				count++;

			}catch(ArgumentOutOfRangeException aoure){
				// got no more values -> add new ones :)
				// value is missing
				UsersOneDayData one_day = new UsersOneDayData ();
				one_day.avgBreath = 0;
				one_day.maxBreath = 0;
				one_day.date = tmp_day;
				// add this data in the right place of list
				partOfData.Insert (count, one_day);
				
				tmp_day = tmp_day.AddDays (1);
				count++;
			}

			if (counting > 500 || count > days) {
				break;
			}
		}

		return partOfData;

	}

	// OLD CODE

	public List<float> getAvgMaxForUser( string currUsername, Text textTimeAvgMax ){

		float tmp_max = 0;

		float tmp_sumOfBrea = 0;
		int tmp_numberOfBreaths = 0;

		foreach (UserData user_data in myContainer.userData) {
			if (user_data.Username == currUsername) {
				tmp_sumOfBrea += user_data.BreathLength;
				tmp_numberOfBreaths++;
				if (user_data.BreathLength > tmp_max) {
					tmp_max = user_data.BreathLength;
				}
			}
		}
		float tmp_avgB = 0;
		if (tmp_numberOfBreaths > 0) {
			tmp_avgB = tmp_sumOfBrea / tmp_numberOfBreaths;
		}
		List<float> avg_max = new List<float> ();
		avg_max.Add (tmp_avgB);
		avg_max.Add (tmp_max);

		//added
		if (textTimeAvgMax != null) {
			textTimeAvgMax.text ="Average: " + tmp_avgB.ToString("00.00") + "s" + "\nLongest: " + tmp_max.ToString ("00.00") + "s" ;
		}

		return avg_max;
	}

	/*
	public List<float> getAvgForUser( string currUsername , int days){

		List<float> avgList = new List<float> ();

		float tmp_sum = 0;
		float tmp_items = 0;
		DateTime this_date = DateTime.Now;
		// go over whole xml
		foreach (UserData data in myContainer.userData) {

			// check only this username
			if (data.Username == currUsername) {
				if ( tmp_items == 0) {
					// first date
					this_date = data.TimeStamp;
				}
				if (data.TimeStamp.Date.Equals (this_date.Date)) {
					// the same date
					tmp_sum += data.BreathLength;
					tmp_items += 1;

				} else {
					float avg = tmp_sum / tmp_items;
					avgList.Add (avg);
					// next date
					this_date = data.TimeStamp;
					tmp_sum = data.BreathLength;
					tmp_items = 1;
				}

			}
		}
		// save the last one aswell :D
		float avg2 = tmp_sum / tmp_items;
		avgList.Add (avg2);

		return avgList;
	}
	*/
}

public class UsersOneDayData{

	public float avgBreath;
	public float maxBreath;
	public DateTime date;
}

