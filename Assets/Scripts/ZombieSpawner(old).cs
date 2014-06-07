using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawnerOld : MonoBehaviour
{
    public int MaxAliveSpawnedZombies = 5;
    public float SpawnTimeInSeconds = 5.0f;
    public GameObject ZombiePrefab;
	public int MinSpawnDistance;
    private float mNextSpawnTime;
    private List<string> mSpawnedZombies;
	private GameObject mPlayer;

    private void Start()
    {
        mSpawnedZombies = new List<string>();
		mPlayer = GameObject.FindGameObjectWithTag ("Player");
    }

    private void Update()
    {
        if (Time.time >= mNextSpawnTime && mSpawnedZombies.Count < MaxAliveSpawnedZombies)
        {
            mNextSpawnTime = Time.time + SpawnTimeInSeconds;
            Vector3 positionToSpawn = Random.insideUnitSphere*30 + transform.position;
            positionToSpawn.y = 1f;

				if (Vector3.Distance (positionToSpawn, mPlayer.transform.position) > MinSpawnDistance){
            Object spawnedZombie = Instantiate(ZombiePrefab, positionToSpawn, transform.rotation);
            spawnedZombie.name = string.Format("{0}:{1}", spawnedZombie.GetInstanceID(), spawnedZombie.name);
            mSpawnedZombies.Add(spawnedZombie.name);
				} else {
				mNextSpawnTime = 0;
			}
        }
        else
        {
            var tempZombies = new List<string>();
            foreach (string zombieKey in mSpawnedZombies)
            {
                GameObject zombie = GameObject.Find(zombieKey);
                if (zombie != null)
                {
                    var zombieComponent = zombie.GetComponent<Zombie>();
                    if (zombieComponent != null && zombieComponent.IsAlive)
                    {
                        continue;
                    }
                }

                // If we get this far, then this zombie is either dead or can't be found... so kill it and spawn a new one.
                tempZombies.Add(zombieKey);
            }

            // Remove Zombie from temp zombie list.
            foreach (string tempZombie in tempZombies)
            {
                mSpawnedZombies.Remove(tempZombie);
            }
        }
    }
}