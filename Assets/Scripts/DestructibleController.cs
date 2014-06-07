using UnityEngine;
using System.Collections;

public class DestructibleController : MonoBehaviour {

	public bool IsActive = true;
	public int MaxHealth;
	public int CurrentHealth;
	public int mDamageInterval;
	public PlankController Plank1, Plank2, Plank3;
	private Inventory mInventory;


	public Transform AttackPosition;


	void Start(){

		mInventory = GameObject.FindGameObjectWithTag ("Player").GetComponent<Inventory> ();

		}

	public void Repair(){

		if (mInventory.InventoryContains(6) && CurrentHealth < MaxHealth){

			mInventory.RemoveItem(6);
			CurrentHealth += mDamageInterval;
			print ("Wall now has " + CurrentHealth + " health left.");

			if (!Plank3.isActive && CurrentHealth > mDamageInterval) {

				print ("Restoring Plank 3");
				Plank3.SetActive(true);
				Plank3.isActive = true;

			} else if (!Plank2.isActive && CurrentHealth > mDamageInterval * 2){

				print ("Restoring Plank 2");
				Plank2.SetActive(true);
				Plank2.isActive = true;

			} else if (!Plank1.isActive && CurrentHealth >= MaxHealth){

				print ("Restoring Plank 1");
				Plank1.SetActive(true);
				Plank1.isActive = true;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {


		if (CurrentHealth > MaxHealth) {

			CurrentHealth = MaxHealth;

				}

		if (CurrentHealth <= 0) {

			IsActive = false;

				}

		if (CurrentHealth <= (MaxHealth - mDamageInterval) ){

			if (Plank1.isActive){

				Plank1.isActive = false;
				Plank1.SetActive(false);

			} 

		} 

		if (CurrentHealth <= (MaxHealth - (mDamageInterval * 2)) ){

			if (Plank2.isActive){
				
				Plank2.isActive = false;
				Plank2.SetActive(false);
				
			} 
			
		}

		if (CurrentHealth <= mDamageInterval ){
			
			if (Plank3.isActive){
				
				Plank3.isActive = false;
				Plank3.SetActive(false);
				
			} 
			
		}

	}
}
