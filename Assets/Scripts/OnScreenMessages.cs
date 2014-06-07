using UnityEngine;
using System.Collections;

public class OnScreenMessages : MonoBehaviour {

	private bool mDisplayMessage;
	private float mDisplayDuration = 5;
	private float mDisplayTime;
	private string mMessage;

	public void DisplayMessage(string message){

		mMessage = message;
		mDisplayTime = Time.time + mDisplayDuration;
		mDisplayMessage = true;

		}

	void OnGUI(){

		if (mDisplayMessage) {

			GUI.Label(new Rect(Screen.width * .5f - 50f, Screen.height * .5f - 10f, 100f, 20f), mMessage);

			if (Time.time > mDisplayTime){

				mDisplayMessage = false;

			}
				}

	}
}
