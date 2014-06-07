using UnityEngine;
using System.Collections;

public class SpikeTrap : MonoBehaviour {

	private float mNextDamage;
	private float mDamageRate = 2;
	private Inventory mInventory;
	public float CurrentHealth;
	public float MaxHealth;
	private float mRepairAmount = 25;

	void Start(){

		mInventory = GameObject.FindGameObjectWithTag ("Player").GetComponent<Inventory> ();

		}

	void OnTriggerStay(Collider other){

		if (other.tag == "Zombie") {

			if (Time.time > mNextDamage){

			other.GetComponent<Zombie>().Health -= 15;
			CurrentHealth -= 1;
			mNextDamage = Time.time + mDamageRate;
				print ("Zombie takes 5 damage!");
			}
		}

	}

	void Update(){

		if (CurrentHealth <= 0) {

			Destroy (gameObject);

				}
	}

	public void Repair(){
		
		if (mInventory.InventoryContains (6) && CurrentHealth < MaxHealth) {
			
						mInventory.RemoveItem (6);
						CurrentHealth += mRepairAmount;
						print ("Spike Trap now has " + CurrentHealth + " health left.");
				}
		}
}
