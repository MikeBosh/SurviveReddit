using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const int mMenuToggleDelay = 1;
    public Transform AimedWeaponPosition;
    public ParticleSystem BloodSplatter;
    public WeaponBase CurrentWeapon;
    public Transform DefaultWeaponPosition;
    public float UseDistance = 4.0f;
    public List<Item> WeaponInventory;
    public GameObject WeaponSocket;
    public float mAimedFOV;
    private Inventory mInventory;
    private bool mIsMenuShowing;
    private bool mIsReloading;
    private ItemDatabase mItemDatabase;
    private CameraSmooth mMouseLook;
    private float mNextFire;
    private float mNextMenuToggle;
    public float mNormalFOV;
    private VitalsController mVitalsController;

    public bool IsMenuShowing

    {
        get { return mIsMenuShowing; }
        set { mIsMenuShowing = value; }
    }

    private void Start()
    {
        mItemDatabase = GameObject.FindGameObjectWithTag("Item Database").GetComponent<ItemDatabase>();
        mMouseLook = Camera.main.GetComponent<CameraSmooth>();
        mVitalsController = GetComponent<VitalsController>();
        mInventory = GetComponent<Inventory>();
        mInventory.SetPlayer(this);
        collider.material.dynamicFriction = 0f;
        collider.material.staticFriction = 0f;
        WeaponInventory = new List<Item>();
    }

    private IEnumerator PerformReload()
    {
        var projectileWeapon = CurrentWeapon as ProjectileWeaponBase;
        int totalAmmo = mInventory.inventory.Where(item => item.TheItemType == Item.ItemType.Ammo).Sum(item => item.ItemQuantity);
        if (totalAmmo <= 0)
        {
            yield break;
        }
        mIsReloading = true;
        audio.PlayOneShot(projectileWeapon.ReloadSoundEffect);
        yield return new WaitForSeconds(projectileWeapon.ReloadDuration);
        projectileWeapon.AmmoInClip = totalAmmo >= projectileWeapon.MaxClipAmmo ? projectileWeapon.MaxClipAmmo : totalAmmo;
        mIsReloading = false;
    }

    private void Update()
    {
        if (Input.GetButtonUp("Use"))
        {
            Vector3 fwd = Camera.main.transform.forward;
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, fwd, out hit, UseDistance))
            {
                var item = hit.collider.gameObject.GetComponent<ItemID>();
                if (item != null)
                {
                    // If it's a weapon, add it to weapon inventory instead of item inventory.
                    Item sourceItem = mItemDatabase.items.FirstOrDefault(i => i.itemID == item.itemID);
                    if (sourceItem != null)
                    {
                        if (sourceItem.TheItemType == Item.ItemType.Weapon)
                        {
                            if (WeaponInventory.All(w => w.itemID != sourceItem.itemID))
                            {
                                WeaponInventory.Add(sourceItem);
                            }
                        }
                    	else
                   		 {
                      	  mInventory.AddItem(item.itemID, item.itemQuantity);
                   		 }
					}

                    // Regardless whether it's a weapon or item, destroy the world object.
                    DestroyObject(hit.collider.gameObject);
                }

                if (hit.transform.tag == "Interactable")
                {
                    if (hit.transform.gameObject.GetComponent<DoorOpener>() != null)
                    {
                        GameObject interactable = hit.transform.gameObject;
                        interactable.GetComponent<DoorOpener>().doorOpen = !interactable.GetComponent<DoorOpener>().doorOpen;
                    }
                    else if (hit.transform.gameObject.GetComponent<ObjectInventory>() != null)
                    {
                        var interactable = hit.transform.gameObject.GetComponent<ObjectInventory>();

                        if (interactable.Locked)
                        {
                            if (mInventory.InventoryContains(interactable.KeyID))
                            {
                                interactable.Locked = false;
                                print("unlocked");
                            }
                            else
                            {
                                print("locked");
                            }
                        }
                        else
                        {
                            mInventory.IsInventoryShowing = true;
                            interactable.GetComponent<ObjectInventory>().IsInventoryShowing = !interactable.GetComponent<ObjectInventory>().IsInventoryShowing;
                        }
                    }
                    else
                    {
                        print("can't touch this! dun dun dun dun, dun dun, dun dun");
                    }
                }
                else if (hit.transform.tag == "Repair")
                {
                    GameObject repairObject = hit.transform.gameObject;

					if (repairObject.GetComponent<DestructibleController>() != null){
                    repairObject.GetComponent<DestructibleController>().Repair();
					} else if (repairObject.GetComponent<SpikeTrap>() != null){
						repairObject.GetComponent<SpikeTrap>().Repair();
					}
                }
            }
        }
        else if (Input.GetButton("Reload") && CurrentWeapon is ProjectileWeaponBase && !mIsMenuShowing && !mIsReloading)
        {
            StartCoroutine(PerformReload());
        }
        else if (Input.GetButton("Fire1") && CurrentWeapon != null && !mInventory.IsInventoryShowing && !mIsMenuShowing && Time.time > mNextFire && !mVitalsController.IsDead && !mIsReloading)
        {
            if (CurrentWeapon is ProjectileWeaponBase)
            {
                // If we have a projectile weapon, perform firing/ammo logic.
                var projectileWeapon = CurrentWeapon as ProjectileWeaponBase;

                // No ammo, play dry fire sound to notify user of a need to reload.
                if (projectileWeapon.AmmoInClip <= 0)
                {
                    projectileWeapon.AmmoInClip = 0;
                    audio.PlayOneShot(projectileWeapon.DryFire);
                }
                else
                {
                    // We have ammo, decrement ammo and perform firing logic.
                    projectileWeapon.AmmoInClip--;
                    mInventory.RemoveOneItemFromStack(mInventory.inventory.Where(item => item.TheItemType == Item.ItemType.Ammo).FirstOrDefault().itemID);
                    audio.PlayOneShot(CurrentWeapon.FireSound);

                    Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        var zombie = hit.collider.gameObject.GetComponent<Zombie>();
                        var zombieHead = hit.collider.gameObject.GetComponent<ZombieHeadHandler>();
                        if (zombieHead != null)
                        {
                            Instantiate(BloodSplatter, hit.point, zombieHead.gameObject.transform.rotation);
                            zombieHead.transform.parent.gameObject.GetComponent<Zombie>().SendMessage("BroadcastZombieHeadshot", this, SendMessageOptions.DontRequireReceiver);
                        }
                        else if (zombie != null)
                        {
                            Instantiate(BloodSplatter, hit.point, zombie.gameObject.transform.rotation);
                            zombie.SendMessage("BroadcastZombieHit", this, SendMessageOptions.DontRequireReceiver);
                        }
                    }

                    StartCoroutine(PerformShellDrop());
                    BroadcastSoundWave();
                }
            }
            else
            {
                // It's not a projectile weapon, just play the attack/fire sound effect for this weapon (could be melee).
                audio.PlayOneShot(CurrentWeapon.FireSound);
            }

            // Regardless of the weapon or whether we have ammo, always delay the next fire sequence.
            mNextFire = Time.time + CurrentWeapon.AttackSpeed;
        }
        else if ((Input.GetButton("Drop") || mVitalsController.IsDead) && CurrentWeapon != null && !mIsReloading)
        {
            DropItem(CurrentWeapon.gameObject, 0);
        }
        else if (Input.GetButton("Fire2") && CurrentWeapon != null && !mInventory.IsInventoryShowing && !mIsMenuShowing && !mVitalsController.IsDead && !mIsReloading)
        {
            if (Camera.main.fieldOfView > mAimedFOV)
            {
                Camera.main.fieldOfView -= 2;
            }
            else
            {
                Camera.main.fieldOfView = mAimedFOV;
            }

            WeaponSocket.transform.position = Vector3.Lerp(WeaponSocket.transform.position, AimedWeaponPosition.position, .25f);
        }
        else if (Input.GetButton("Weapon1"))
        {
            EquipWeapon(1);
        }
        else if (Input.GetButton("Weapon2"))
        {
            EquipWeapon(2);
        }
        else
        {
            if (Camera.main.fieldOfView < mNormalFOV)
            {
                Camera.main.fieldOfView += 2;
            }
            else
            {
                Camera.main.fieldOfView = mNormalFOV;
            }
            WeaponSocket.transform.position = Vector3.Lerp(WeaponSocket.transform.position, DefaultWeaponPosition.position, .25f);
        }
    }

    private void EquipWeapon(int weaponKeyId)
    {
        Item weaponFromWeaponInventory = WeaponInventory.FirstOrDefault(w => w.WeaponKeyBind == weaponKeyId);
        if (weaponFromWeaponInventory != null)
        {
            var weapon = (GameObject) Instantiate(weaponFromWeaponInventory.GamePrefab);

            if (CurrentWeapon != null)
            {
                // Remove current weapon from world.
                Destroy(CurrentWeapon.gameObject);
            }

            weapon.transform.parent = WeaponSocket.transform;
            weapon.transform.position = WeaponSocket.transform.position;
            weapon.transform.rotation = WeaponSocket.transform.rotation;
            weapon.collider.enabled = false;
            Destroy(weapon.rigidbody);
            CurrentWeapon = weapon.GetComponent<WeaponBase>();
        }
    }

    private void BroadcastSoundWave()
    {
        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");
        foreach (GameObject zombie in zombies)
        {
            zombie.SendMessage("BroadcastSoundWaveReceiver", this, SendMessageOptions.DontRequireReceiver);
        }
    }

    private IEnumerator PerformShellDrop()
    {
        yield return new WaitForSeconds(0.4f);
        audio.PlayOneShot((CurrentWeapon as ProjectileWeaponBase).ShellDrop);
    }

    private void OnGUI()
    {
        if (mVitalsController.IsDead)
        {
            mIsMenuShowing = true;
        }
        else if (Input.GetButtonDown("Esc") && Time.time > mNextMenuToggle)
        {
            mNextMenuToggle = Time.time + mMenuToggleDelay;
            mIsMenuShowing = !mIsMenuShowing;
        }
        if (mIsMenuShowing)
        {
            if (mInventory.IsInventoryShowing)
            {
                mInventory.IsInventoryShowing = false;
            }
            mMouseLook.enabled = false;
            Screen.lockCursor = false;
            if (mVitalsController.IsDead)
            {
                GetComponent<CharacterMotor>().enabled = false;
                GetComponent<FPSInputController>().enabled = false;
                if (mIsMenuShowing && GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2 - 50, 250, 250), "You have died... Quit"))
                {
                    ApplicationHelper.Quit();
                }
            }
            else
            {
                if (mIsMenuShowing && GUI.Button(new Rect(Screen.width/2 - 50, Screen.height/2 - 50, 250, 250), "Quit"))
                {
                    ApplicationHelper.Quit();
                }
            }
        }
        else
        {
            if (!mInventory.IsInventoryShowing)
            {
                mMouseLook.enabled = true;
                Screen.lockCursor = true;
            }
        }
    }

    public void DropItem(Object droppableObject, int quantity)
    {
        var worldItem = (GameObject) Instantiate(droppableObject, transform.position + transform.forward + transform.right, transform.rotation);
        worldItem.GetComponent<ItemID>().itemQuantity = quantity;
        var rigidBody = worldItem.GetComponent<Rigidbody>();
        if (rigidBody == null)
        {
            worldItem.AddComponent<Rigidbody>();
        }
        if (worldItem.collider.enabled == false)
        {
            worldItem.collider.enabled = true;
        }

        worldItem.rigidbody.AddForce(transform.forward*200);
        Destroy(droppableObject);
    }
}

public static class ApplicationHelper
{
    public static void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}