using System;
using UnityEngine;

public class VitalsController : MonoBehaviour
{
    public int BleedOutDecrementValue = 5;
    public int BleedRateInSeconds = 30;
    public CoreVitals CoreVitals;
    public bool IsBleeding = false;
    public bool IsDead;
    public int ResourceDecrementValue = 5;
    public int ResourceLossInSeconds = 30;

    private float mNextBleedOut;
    private float mNextResourceLoss;

    private void Start()
    {
        // Do nothing.
    }

    private void Update()
    {
        if (IsDead)
        {
            CoreVitals.Blood = 0;
            return;
        }

        if (IsBleeding && Time.time >= mNextBleedOut)
        {
            mNextBleedOut = Time.time + BleedRateInSeconds;
            CoreVitals.Blood -= BleedOutDecrementValue;
        }

        if (CoreVitals.Blood <= 0)
        {
            CoreVitals.Blood = 0;
            IsDead = true;
        }

        if (CoreVitals.Blood <= 0)
            CoreVitals.Blood = 0;
        else if (CoreVitals.Blood >= 100)
            CoreVitals.Blood = 100;
    }
}

[Serializable]
public class CoreVitals
{
    public int Blood = 100;
}