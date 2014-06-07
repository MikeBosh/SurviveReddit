using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	private SpawnController mSpawnControl;
	public List <Vector3> Waves;
	public float ScavengeDuration;
	private float ScavengeTime;
	private int StageNumber;
	private GameObject mPlayer;
	private ItemSpawnController mItemSpawnController;
	private OnScreenMessages mOnScreenMessages;
	public bool SpawnActive;
	public bool ScavengeActive;
	public Transform TeleportLocation;


	// Use this for initialization
	void Start () {
	
		mSpawnControl = GameObject.FindGameObjectWithTag ("SpawnController").GetComponent<SpawnController>();
		mPlayer = GameObject.FindGameObjectWithTag ("Player");
		mItemSpawnController = GameObject.FindGameObjectWithTag ("ItemSpawnController").GetComponent<ItemSpawnController> ();
		mOnScreenMessages = gameObject.GetComponent<OnScreenMessages> ();
		SpawnActive = true;

	}
	
	// Update is called once per frame
	void Update () {

		if (SpawnActive) {

			mOnScreenMessages.DisplayMessage("Spawning wave " + StageNumber + ".");
			mSpawnControl.SetWaveInfo(Waves[StageNumber]);
			SpawnActive = false;
				}
	
		if (ScavengeActive) {

			if (Time.time > ScavengeTime){

				if (StageNumber % 5 == 0){
					Waves.Add (Waves[StageNumber] + new Vector3(1, 2, -1));
				}
				else if (StageNumber % 3 == 0){
					Waves.Add (Waves[StageNumber] + new Vector3(1, 2, 0));
				} else {
					Waves.Add (Waves[StageNumber] + new Vector3(1, 1, 0));
				}
				StageNumber++;
				ScavengeActive = false;
				mPlayer.transform.position = TeleportLocation.position;
				SpawnActive = true;

					}
				}
	}

	public void WaveEnd(){

		mOnScreenMessages.DisplayMessage("Wave " + StageNumber + " ended.");
		SpawnActive = false;
		ScavengeActive = true;
		ScavengeTime = Time.time + ScavengeDuration;
		mItemSpawnController.SpawnItems ();

	}
}
