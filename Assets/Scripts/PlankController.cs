using UnityEngine;
using System.Collections;

public class PlankController : MonoBehaviour {

	public bool isActive = true;


	public void SetActive(bool active){

		gameObject.SetActive (active);

	}
}
