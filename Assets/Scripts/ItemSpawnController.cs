using UnityEngine;
using System.Collections;

public class ItemSpawnController : MonoBehaviour {

	private GameObject[] mItemSpawners;


	void Start(){


		mItemSpawners = GameObject.FindGameObjectsWithTag ("ItemSpawner");

	}

	public void SpawnItems(){


		for (int i = 0; i < mItemSpawners.Length; i++) {

			mItemSpawners[i].GetComponent<ItemSpawner>().SpawnItem(Random.Range(4, 7));

				}


		}
}
