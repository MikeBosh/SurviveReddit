using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    private PlayerController mPlayerController;
    private VitalsController mVitalsController;

    private void Start()
    {
        mVitalsController = GetComponent<VitalsController>();
        mPlayerController = GetComponent<PlayerController>();
    }

    private void OnGUI()
    {
        GUIText bloodGui = GameObject.Find("BloodGuiText").guiText;

        int blood = mVitalsController.CoreVitals.Blood;

        GUIText currentAmmoGui = GameObject.Find("CurrentAmmoText").guiText;
        GUIText maxAmmoGui = GameObject.Find("MaxAmmoText").guiText;
        GUIText slashGui = GameObject.Find("SlashText").guiText;

        if (mPlayerController.CurrentWeapon is ProjectileWeaponBase)
        {
            currentAmmoGui.enabled = maxAmmoGui.enabled = slashGui.enabled = true;
            var projectileWeapon = mPlayerController.CurrentWeapon as ProjectileWeaponBase;

            int currentAmmo = projectileWeapon.AmmoInClip;
            int maxAmmo = projectileWeapon.MaxClipAmmo;

            currentAmmoGui.text = currentAmmo.ToString();
            maxAmmoGui.text = maxAmmo.ToString();
        }
        else
        {
            currentAmmoGui.enabled = maxAmmoGui.enabled = slashGui.enabled = false;
        }

        // Blood
        if (blood <= 75) bloodGui.color = blood <= 25 ? Color.red : Color.yellow;
        else bloodGui.color = Color.white;

        bloodGui.text = string.Format("Blood       {0}", blood);
    }
}