using UnityEngine;

public class ProjectileWeaponBase : WeaponBase
{
    public int AmmoInClip;
    public AudioClip DryFire;
    public float Force;
    public Transform InitialShotPosition;
    public int MaxClipAmmo;
    public float ReloadDuration = 0.5f;
    public AudioClip ReloadSoundEffect;
    public AudioClip ShellDrop;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }
}