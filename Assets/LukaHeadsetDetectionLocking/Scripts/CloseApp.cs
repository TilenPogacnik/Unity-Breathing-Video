using UnityEngine;
using System.Collections;

public class CloseApp : MonoBehaviour {

    public float lastClickedTime = -50;
    public float CONST_timePeriodForDoubleClick = 2f;
    public float CONST_timeHideBackButton = 2f;
    public GameObject quitAppPopup;


    void Update(){

        if (Input.GetKeyDown(KeyCode.Escape) ) {
            if (Time.time - lastClickedTime < CONST_timePeriodForDoubleClick){
                // close our app
                Application.Quit();
            }
            else{
                quitAppPopup.SetActive(true);
                lastClickedTime = Time.time;
            }
        }
        if (Time.time - lastClickedTime > CONST_timeHideBackButton)
        {
            quitAppPopup.SetActive(false);
        }

    }

}
