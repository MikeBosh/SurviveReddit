using UnityEngine;
using System.Collections;

public class DoorOpener : MonoBehaviour {

	public float smoothing;
	private float mDoorOpenAngle = 90;
	public bool doorOpen;

	private Vector3 mDefaultRot;
	private Vector3 mOpenRot;


	void Start(){

		mDefaultRot = transform.eulerAngles;
		mOpenRot = new Vector3 (mDefaultRot.x, mDefaultRot.y + mDoorOpenAngle, mDefaultRot.z);

	}

	void Update(){

		if (doorOpen) {

						transform.eulerAngles = Vector3.Slerp (transform.eulerAngles, mOpenRot, Time.deltaTime * smoothing);
				} else {
				
			transform.eulerAngles = Vector3.Slerp (transform.eulerAngles, mDefaultRot, Time.deltaTime * smoothing);
				}

	}

}
