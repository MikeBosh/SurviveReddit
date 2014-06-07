using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
	private ItemDatabase database;


	void Start(){

		database = GameObject.FindGameObjectWithTag ("Item Database").GetComponent<ItemDatabase> ();
	}

  	public void SpawnItem(int id){

		for (int i = 0; i < database.items.Count; i++) {

			print (database.items[i].itemID);

			if (database.items[i].itemID == id){

				Item toSpawn = database.items[i];
				print (toSpawn.ItemName);
				Instantiate (toSpawn.GamePrefab, transform.position, transform.rotation);
				break;
			}

		}

	}

    private void Update()
    {
    }
}