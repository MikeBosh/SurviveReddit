using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    public List<ZombieSpawner> Spawners;
    public float mMinSpawn;
    private float mNextSpawn;
    private int mSpawnChoice;
    public float mSpawnIncrease;
    private float mSpawnRate;
	private float mSpawnAmount;
	private float mNumberOfWaves;
	private float mWaveCount;
	private float mLastWaveTime = 30;
	private GameController mController;
	private bool mLastWave;
	private float mLastWaveDuration;

	public bool Spawning;


	void Start(){

		mController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();

	}


	public void SetWaveInfo(Vector3 waveInfo){

		mNumberOfWaves = waveInfo.x;
		mSpawnAmount = waveInfo.y;
		mSpawnRate = waveInfo.z;

		Spawning = true;
		mWaveCount = 0;

		}


    private void Update()
    {
		if (Spawning) {
						if (Time.time > mNextSpawn) {
								mNextSpawn = Time.time + mSpawnRate;

									for (int i = 0; i < mSpawnAmount; i ++){
										mSpawnChoice = Random.Range (0, Spawners.Count);
										Spawners [mSpawnChoice].SpawnZombie ();
									}
								
								mWaveCount++;
						}

						if (Time.time > mSpawnIncrease) {
								mSpawnIncrease += Time.time;

								if (mSpawnRate > mMinSpawn) {
										mSpawnRate--;
								}
						}
					
					if (mWaveCount >= mNumberOfWaves){

						mLastWave = true;
						mLastWaveDuration = Time.time + mLastWaveTime; 		
						Spawning = false;
						}
					
				}

		if (mLastWave && Time.time > mLastWaveDuration) {

			mController.WaveEnd();
			mLastWave = false;
				}

    }
}