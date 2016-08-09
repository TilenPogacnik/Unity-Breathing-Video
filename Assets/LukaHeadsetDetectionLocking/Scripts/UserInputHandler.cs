using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System.Security.Cryptography;

public class UserInputHandler : MonoBehaviour {

	private string secretKey = "mySecretKey"; 
	private static string url = "https://www.breathinglabs.com/MobileValidation.php";// "http://localhost/xampp/BLabs_connect_sql.php"; 
	private static string emai;
	public string appName;

	private static long TOO_SHORT = 16; //1000000000000000;
	private static long TOO_LONG  = 16; //10000000000000000;


	public Text inputText;
	public Text errorText;

	public GameObject lockUI;

	// loading image
	public GameObject loadingImage;

	void Start(){
		// init
		emai = SystemInfo.deviceUniqueIdentifier; //The magic is that when using READ_PHONE_STATE permission IMEI returned, otherwise Android_ID
		appName = Application.productName;

	}

	public void onButtonClickEvent(){

		loadingImage.SetActive (true);
		// imamo stevilko uporabnika -> poglej stevilo mest?
		string enteredNumber = validateInput ( inputText.text );
		if (enteredNumber.Length > 3) {
			connectToDB (enteredNumber);
		} else {
			loadingImage.SetActive (false);
		}
	}

	private string validateInput( string input ){
		/*
		try{ // not needed - input type is int..
		*/
		long enteredNumber = long.Parse(inputText.text);
		// check input length
		if ( lengthValidator( inputText.text ) ){
			return enteredNumber.ToString("0000-0000-0000-0000");
		}else{
			return "1";
		}
		/*
		}catch( System.FormatException e ){
			return "Invalid number!";
		}*/

	}
	private bool lengthValidator( string enteredNum ){
		//Debug.Log ("len = " + enteredNum.Length);
		//Debug.Log (enteredNum);
		if (enteredNum.Length < TOO_SHORT ) {
			errorText.text = "Number too small";
			return false; 
		} else if (enteredNum.Length > TOO_LONG ) {
			errorText.text = "Number too big.";
			return false;
		} else {
			errorText.text = "";
			return true;
		}
	}

	public void setAppName( string name ){
		this.appName = name;
	}

	// remember to use StartCoroutine when calling this function!
	IEnumerator postData(string enteredNum) {

		loadingImage.SetActive (true);

		Debug.Log (" posting data...");

		string hash_1 = Sha256(appName + emai );
		string hash_2 = Sha256(  enteredNum + secretKey);
		string hash = Sha256( hash_1 + hash_2);

		WWWForm form = new WWWForm ();
		form.AddField ("name", appName );
		form.AddField ("emai", emai );
		form.AddField ("enteredNum", enteredNum );
		form.AddField ("hash", hash );

		//Hashtable headers = form.headers;
		byte[] rawData = form.data;

		WWW www = new WWW(url, rawData, form.headers);
		yield return www;

		string expected = "Successfully added: " + enteredNum;

		// check response
		if (www.error != null){
			Debug.Log("There was an error getting data: " + www.error);
			errorText.text = www.error;
		}else{
			// check if msg is Successfully added: ...

			errorText.text = www.text; 
			Debug.Log (www.text);
			if (www.text == expected ) {

				PlayerPrefs.SetInt ("highsc",1);
				PlayerPrefs.Save ();
				//SceneManager.LoadScene ("Mp3Scene");
				//SceneManager.UnloadScene("ValidationScene");
				Time.timeScale = 1.0f;
				lockUI.SetActive(false);
			}

		}

		loadingImage.SetActive (false);
	}


	private void connectToDB( string enteredNum ){
		
		StartCoroutine(postData(enteredNum));
	}

	/*
	IEnumerator getData(){

		WWWForm form = new WWWForm ();
		form.AddField ("name", "get" );
		byte[] rawData = form.data;

		WWW www = new WWW( url, rawData, form.headers);

		yield return www;

		if (www.error != null){
			Debug.Log("There was an error getting data: " + www.error);
		}else{
			errorText.text = www.text;
			Debug.Log (www.text);
		}

	}*/


	public  string Md5Sum(string strToEncrypt){
		
		byte[] asciiBytes = ASCIIEncoding.ASCII.GetBytes (strToEncrypt);
		byte[] hashedBytes = MD5CryptoServiceProvider.Create ().ComputeHash (asciiBytes);
		string hashedString = System.BitConverter.ToString (hashedBytes).Replace ("-", "").ToLower ();

		return hashedString;
	}

	public string Sha256( string strToEncrypt){

		System.Security.Cryptography.SHA256Managed hm = new System.Security.Cryptography.SHA256Managed();
		byte[] hashedBytes = hm.ComputeHash(System.Text.Encoding.ASCII.GetBytes( strToEncrypt ));
		//Console.WriteLine(System.BitConverter.ToString(hashValue).Replace("-", "").ToLower());
		string hashedString = System.BitConverter.ToString (hashedBytes).Replace ("-", "").ToLower ();

		return hashedString;
	}
}
