using UnityEngine;
using System.Collections;

public class ItemRemover : MonoBehaviour {

	private GameController controller;
	private float mRemoveTime;

	// Use this for initialization
	void Start () {
	
		controller = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		mRemoveTime = Time.time + controller.ScavengeDuration;

	}

	void Update(){

		if (Time.time > mRemoveTime){

			Destroy (gameObject);

		}

	}

}
